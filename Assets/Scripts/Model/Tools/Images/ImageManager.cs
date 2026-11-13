using System;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// ImageUrl is kept as the property name for compatibility, but its value is now a local card image path:
/// relative to Resources/Sprites/Cards/, without file extension (for example "Pilots/lukeskywalker").
/// </summary>
public interface IImageHolder
{
    string ImageUrl { get; set; }
}

public static class ImageManager
{
    public const string CardsRootPath = "Sprites/Cards/";

    private const string LegacyCacheDeletedPrefKey = "LegacyImageCacheDeleted";

    /// <summary>
    /// Loads a card image from Resources/Sprites/Cards/{imagePath}.
    /// The callback gets a readable copy that may freely resized or modified or null if the image does not exist
    /// </summary>
    static public IEnumerator GetTexture(Action<Texture2D> callback, string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            callback(null);
            yield break;
        }

        ResourceRequest request = Resources.LoadAsync<Texture2D>(CardsRootPath + imagePath);
        yield return request;

        Texture2D texture = null;
        Texture2D source = request.asset as Texture2D;

        if (source != null)
        {
            try
            {
                texture = CreateReadableCopy(source);
            }
            catch (Exception e)
            {
                Debug.Log("Cannot read card image: " + imagePath + Environment.NewLine + e.Message);
            }
        }
        else
        {
            Debug.Log("Card image not found: " + CardsRootPath + imagePath);
        }

        callback(texture);
    }

    // The loaded asset is shared by everyone who loads this path, but callers scale and modify the texture
    // they get (TextureScale.Bilinear), so they must receive their own copy. Going through a RenderTexture
    // works whether or not the imported asset has Read/Write enabled or uses a compressed format.
    private static Texture2D CreateReadableCopy(Texture2D source)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        RenderTexture previous = RenderTexture.active;

        try
        {
            Graphics.Blit(source, renderTexture);
            RenderTexture.active = renderTexture;

            Texture2D copy = new(source.width, source.height, TextureFormat.RGBA32, false);
            copy.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            copy.Apply();
            return copy;
        }
        finally
        {
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }

    /// <summary>
    /// One-time cleanup of the image cache that older versions downloaded into
    /// {persistentDataPath}/{Edition}/ImageCache. Images are local now, so the cache is not needed.
    /// Only folders named ImageCache directly inside an edition folder are removed
    /// (replays, saved squadrons and other data next to them are not touched).
    /// </summary>
    static public void DeleteLegacyImageCache()
    {
        if (PlayerPrefs.GetInt(LegacyCacheDeletedPrefKey, 0) == 1) return;

        try
        {
            string root = Application.persistentDataPath;

            if (Directory.Exists(root))
            {
                foreach (string editionFolder in Directory.GetDirectories(root))
                {
                    string cacheFolder = Path.Combine(editionFolder, "ImageCache");

                    if (Directory.Exists(cacheFolder))
                    {
                        Directory.Delete(cacheFolder, true);
                    }
                }
            }

            PlayerPrefs.SetInt(LegacyCacheDeletedPrefKey, 1);
        }
        catch (Exception e)
        {
            // Not critical: try again on next launch
            Debug.Log("Cannot delete legacy image cache: " + e.Message);
        }
    }
}
