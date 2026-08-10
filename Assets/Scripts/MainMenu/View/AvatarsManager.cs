using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

public static class AvatarsManager
{
    public static GameObject Selector { get; private set; }
    public static Faction AvatarFaction { get; private set; }
    public static int WaitingToDownload { get; set; }

    private static readonly Dictionary<Faction, int> FactionCellSizes = new()
    {
        { Faction.None, 85 },
        { Faction.Rebel, 150 },
        { Faction.Imperial, 150 },
        { Faction.Scum, 150 },
        { Faction.Resistance, 150 },
        { Faction.FirstOrder, 150 },
        { Faction.Republic, 150 },
        { Faction.Separatists, 150 },
    };

    public static void LoadAvatars(Faction avatarFaction)
    {
        ShowLoadingStub();

        AvatarFaction = avatarFaction;

        Transform galleryTransform = GameObject.Find("UI/Panels/BrowseAvatarsPanel/Content").transform;

        foreach (Transform transform in galleryTransform)
        {
            GameObject.Destroy(transform.gameObject);
        }

        galleryTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(FactionCellSizes[AvatarFaction], FactionCellSizes[AvatarFaction]);

        List<string> avatarsAdded = new();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList.FirstEdition", StringComparison.Ordinal) || String.Equals(t.Namespace, "UpgradesList.SecondEdition", StringComparison.Ordinal))
            .ToList();

        WaitingToDownload = 0;

        foreach (Type type in typelist)
        {
            if (type.MemberType == MemberTypes.NestedType) continue;

            GenericUpgrade newUpgradeContainer = (GenericUpgrade)Activator.CreateInstance(type);

            if (newUpgradeContainer.UpgradeInfo.Name != null
                && newUpgradeContainer.Avatar != null)
            {
                if (newUpgradeContainer.Avatar.AvatarFaction == AvatarFaction || AvatarFaction == Faction.None)
                {
                    string avatarName = $"{AvatarFaction} - {newUpgradeContainer.UpgradeInfo.Name}";

                    if (!avatarsAdded.Contains(avatarName))
                    {
                        // Prevents duplicate avatars due to SecondEdition / XWA Edition sharing a base edition
                        avatarsAdded.Add(avatarName);
                        AddAvailableAvatar(newUpgradeContainer);
                    }
                }
            }
        }
    }

    private static void AddAvailableAvatar(GenericUpgrade avatarUpgrade)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/MainMenu/AvatarImage", typeof(GameObject));
        GameObject avatarPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/BrowseAvatarsPanel/Content").transform);

        avatarPanel.name = avatarUpgrade.GetType().ToString();

        WaitingToDownload++;
        AvatarFromUpgrade avatar = avatarPanel.GetComponent<AvatarFromUpgrade>();
        avatar.Initialize(avatarUpgrade.GetType().ToString(), ChangeAvatar, WhenDownloaded);

        if (avatarUpgrade.GetType().ToString() == Options.Avatar)
        {
            SetAvatarSelected();
        }
    }

    private static void WhenDownloaded()
    {
        WaitingToDownload--;
        if (WaitingToDownload == 0) HideLoadingStub();
    }

    private static void ChangeAvatar(string avatarName)
    {
        Options.Avatar = avatarName;
        Options.ChangeParameterValue("AvatarV2", avatarName);

        SetAvatarSelected();
    }

    private static void SetAvatarSelected()
    {
        GameObject.Destroy(Selector);

        string prefabPath = "Prefabs/MainMenu/Options/Selector";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        Selector = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/BrowseAvatarsPanel/Content/" + Options.Avatar).transform);
    }

    public static void LoadAvatars(string factionChar)
    {
        LoadAvatars(CharToFaction(factionChar));
    }

    private static Faction CharToFaction(string faction)
    {
        return faction switch
        {
            "!" => Faction.Rebel,
            "@" => Faction.Imperial,
            "#" => Faction.Scum,
            "-" => Faction.Resistance,
            "+" => Faction.FirstOrder,
            "/" => Faction.Republic,
            "." => Faction.Separatists,
            _ => Faction.None,
        };
    }

    private static void ShowLoadingStub()
    {
        GameObject.Find("UI/Panels/BrowseAvatarsPanel/Loading").SetActive(true);
    }

    private static void HideLoadingStub()
    {
        GameObject.Find("UI/Panels/BrowseAvatarsPanel/Loading").SetActive(false);
    }
}
