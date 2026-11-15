using Editions;
using Ship;
using System.IO;
using Upgrade;

/// <summary>
/// Builds default local card image paths (relative to Resources/Sprites/Cards/, without extension).
/// Cards that set ImageUrl explicitly use that value instead.
/// </summary>
public static class ImageUrls
{
    private const string DamageDeckPath = "DamageCards/";

    public static string GetImageUrl(GenericUpgrade upgrade, string filename = null)
    {
        return Edition.Current.GetUpgradeImageUrl(upgrade, filename);
    }

    public static string GetImageUrl(GenericDamageCard crit, string filename = null)
    {
        return $"{DamageDeckPath}{(string.IsNullOrWhiteSpace(filename) ? FormatName(crit.Name) : Path.GetFileNameWithoutExtension(filename))}";
    }

    public static string GetImageUrl(GenericShip ship, string filename = null)
    {
        return Edition.Current.GetPilotImageUrl(ship, filename);
    }

    public static string FormatName(string name)
    {
        return name
            .ToLower()
            .Replace("(", "")
            .Replace(")", "")
            .Replace(' ', '-')
            .Replace('/', '-')
            .Replace('\'', '-')
            .Replace("\"", "");
    }
}