﻿using ExtraOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Upgrade;

public class OptionsUI : MonoBehaviour {

    public GameObject PlaymatSelector { get; private set; }
    public static OptionsUI Instance { get; private set; }
    public GameObject Selector { get; private set; }

    private const float FREE_SPACE_EXTRA_OPTIONS = 25f;

    private static readonly List<string> QualityNames = new List<string>()
    {
        "Fast",
        "Good",
        "Beautiful"
    };

    private void Start()
    {
        Instance = this;
    }

    public void OnClickPlaymatChange(GameObject playmatImage)
    {
        PlayerPrefs.SetString("PlaymatName", playmatImage.name);
        PlayerPrefs.Save();

        Options.Playmat = playmatImage.name;

        PlaymatSelector.transform.position = playmatImage.transform.position;
    }

    public void RestoreDefaults()
    {
        Options.Playmat = "3DSceneHoth";
        Options.ChangeParameterValue("Music Volume", 0.25f);
        Options.ChangeParameterValue("SFX Volume", 0.25f);
        Options.ChangeParameterValue("Animation Speed", 0.25f);
        Options.ChangeParameterValue("Maneuver Speed", 0.25f);

        Options.InitializePanel();
    }

    public void InitializeOptionsPanel()
    {
        CategorySelected(GameObject.Find("UI/Panels/OptionsPanel/Content/CategoriesPanel/PlayerButton"));
    }

    public void CategorySelected(GameObject categoryGO)
    {
        ClearOptionsView();
        categoryGO.GetComponent<Image>().color = new Color(0, 0.5f, 1, 100f/256f);

        switch (categoryGO.GetComponentInChildren<Text>().text)
        {
            case "Player":
                ShowPlayerView();
                break;
            case "Animations":
                ShowViewSimple("Animations");
                break;
            case "Video":
                ShowVideoView();
                break;
            case "Sounds":
                ShowViewSimple("Sounds");
                break;
            case "Playmat":
                ShowPlaymatSelection();
                break;
            case "Background":
                ShowBackgroundSelection();
                break;
            case "Extra":
                ShowExtraView();
                break;
            default:
                break;
        }
    }

    private void ShowPlayerView()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/PlayerViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject panel = Instantiate(prefab, parentTransform);

        InputField nameText = panel.transform.Find("NameInputPanel/InputField").GetComponent<InputField>();
        nameText.text = Options.NickName;
        nameText.onEndEdit.AddListener(delegate { MainMenu.CurrentMainMenu.ChangeNickName(nameText.text); });

        Button avatarButton = panel.transform.Find("AvatarChangePanel/AvatarButton").GetComponent<Button>();
        AvatarFromUpgrade avatar = avatarButton.transform.GetComponent<AvatarFromUpgrade>();
        avatar.Initialize(Options.Avatar.ToString(), delegate { MainMenu.CurrentMainMenu.ChangePanel("BrowseAvatarsPanel"); });

        InputField titleText = panel.transform.Find("TitleInputPanel/InputField").GetComponent<InputField>();
        titleText.text = Options.Title;
        titleText.onEndEdit.AddListener(delegate { MainMenu.CurrentMainMenu.ChangeTitle(titleText.text); });
    }

    private void ShowBackgroundSelection()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/BackgroundSelectionViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject imageListParent = Instantiate(prefab, parentTransform);
        imageListParent.name = "BackgroundSelectionViewPanel";

        foreach (Sprite backgroundImage in Resources.LoadAll<Sprite>("Sprites/Backgrounds/MainMenu"))
        {
            GameObject backgroundGO = new GameObject(backgroundImage.name);
            backgroundGO.AddComponent<Image>().sprite = backgroundImage;
            backgroundGO.transform.SetParent(imageListParent.transform);
            backgroundGO.transform.localScale = Vector3.one;

            Button button = backgroundGO.AddComponent<Button>();
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = new Color(1, 1, 1, 200f / 256f);
            button.colors = buttonColors;

            button.onClick.AddListener(() =>
            {
                SetBackgroundImageSelected(backgroundImage.name);
            });
        }

        ChangeBackground();
    }

    private void SetBackgroundImageSelected(string name)
    {
        PlayerPrefs.SetString("BackgroundImage", name);
        PlayerPrefs.Save();
        Options.BackgroundImage = name;

        ChangeBackground();

        MainMenu.SetBackground();
    }

    private void ChangeBackground()
    {
        GameObject.Destroy(Selector);

        string prefabPath = "Prefabs/MainMenu/Options/Selector";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        Selector = Instantiate(prefab, GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel/BackgroundSelectionViewPanel/" + Options.BackgroundImage).transform);
    }

    private void ShowPlaymatSelection()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/PlaymatSelectionViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject imageListParent = Instantiate(prefab, parentTransform);
        imageListParent.name = "PlaymatSelectionViewPanel";

        foreach (Sprite playmatSprite in Resources.LoadAll<Sprite>("Playmats/Thumbnails"))
        {
            GameObject playmatPreviewGO = new GameObject(playmatSprite.name);
            playmatPreviewGO.AddComponent<Image>().sprite = playmatSprite;
            playmatPreviewGO.transform.SetParent(imageListParent.transform);
            playmatPreviewGO.transform.localScale = Vector3.one;
            playmatPreviewGO.name = playmatSprite.name.Replace("Playmat", "");

            Button button = playmatPreviewGO.AddComponent<Button>();
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = new Color(1, 1, 1, 200f / 256f);
            button.colors = buttonColors;

            string playmatName = playmatSprite.name.Replace("Thumbnail", "").Replace("Playmat", "");
            if (playmatName == Options.Playmat)
            {
                SetPlaymatSelected();
            }

            button.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("PlaymatName", playmatName);
                PlayerPrefs.Save();

                Options.Playmat = playmatName;
                SetPlaymatSelected();
            });
        }
    }

    private void ShowViewSimple(string name)
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/" + name + "ViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        //GameObject panel = 
        Instantiate(prefab, parentTransform);
    }

    private void ClearOptionsView()
    {
        Transform categoryTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/CategoriesPanel").transform;
        foreach (Transform transform in categoryTransform.transform)
        {
            transform.GetComponent<Image>().color = new Color(0, 0.5f, 1, 0);
        }

        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        foreach (Transform transform in parentTransform.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    private void SetPlaymatSelected()
    {
        GameObject.Destroy(Selector);

        string prefabPath = "Prefabs/MainMenu/Options/Selector";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        Selector = Instantiate(prefab, GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel/PlaymatSelectionViewPanel/" + Options.Playmat + "Thumbnail").transform);
    }

    private void ShowExtraView()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/ExtraOptionsViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject panel = Instantiate(prefab, parentTransform);
        panel.name = "ExtraOptionsViewPanel";

        GameObject itemPrefab = (GameObject)Resources.Load("Prefabs/UI/ExtraOptionPanel", typeof(GameObject));
        GameObject ExtraOptionsPanel = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel/ExtraOptionsViewPanel/Viewport/Content").gameObject;

        RectTransform modsPanelRectTransform = ExtraOptionsPanel.GetComponent<RectTransform>();
        Vector3 currentPosition = new Vector3(modsPanelRectTransform.sizeDelta.x / 2, -FREE_SPACE_EXTRA_OPTIONS, ExtraOptionsPanel.transform.localPosition.z);

        foreach (Transform transform in ExtraOptionsPanel.transform)
        {
            Destroy(transform.gameObject);
        }

        modsPanelRectTransform.sizeDelta = new Vector2(modsPanelRectTransform.sizeDelta.x, 0);
        GameObject.Find("UI/Panels").transform.Find("ModsPanel").Find("Scroll View").GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;

        foreach (var mod in ExtraOptionsManager.ExtraOptions)
        {
            GameObject ModRecord;

            ModRecord = MonoBehaviour.Instantiate(itemPrefab, ExtraOptionsPanel.transform);
            ModRecord.transform.localPosition = currentPosition;
            ModRecord.name = mod.Key.ToString();

            ModRecord.transform.Find("Label").GetComponent<Text>().text = mod.Value.Name;

            Text description = ModRecord.transform.Find("Text").GetComponent<Text>();
            description.text = mod.Value.Description;
            RectTransform descriptionRectTransform = description.GetComponent<RectTransform>();
            descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, description.preferredHeight);

            RectTransform modRecordRectTransform = ModRecord.GetComponent<RectTransform>();
            modRecordRectTransform.sizeDelta = new Vector2(modRecordRectTransform.sizeDelta.x, modRecordRectTransform.sizeDelta.y + description.preferredHeight);

            currentPosition = new Vector3(currentPosition.x, currentPosition.y - modRecordRectTransform.sizeDelta.y - FREE_SPACE_EXTRA_OPTIONS, currentPosition.z);
            modsPanelRectTransform.sizeDelta = new Vector2(modsPanelRectTransform.sizeDelta.x, modsPanelRectTransform.sizeDelta.y + modRecordRectTransform.sizeDelta.y + FREE_SPACE_EXTRA_OPTIONS);

            ModRecord.transform.Find("Toggle").GetComponent<Toggle>().isOn = ExtraOptionsManager.ExtraOptions[mod.Key].IsOn;
        }
    }

    private void ShowVideoView()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/VideoViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject panel = Instantiate(prefab, parentTransform);

        Toggle fullscreen = panel.transform.Find("FullscreenCheckboxPanel/ToggleHolder/Toggle").GetComponent<Toggle>();
        fullscreen.isOn = Options.FullScreen;
        fullscreen.onValueChanged.AddListener(ChangeFullscreen);

        Toggle fps = panel.transform.Find("FpsCheckboxPanel/ToggleHolder/Toggle").GetComponent<Toggle>();
        fps.isOn = Options.ShowFps;
        fps.onValueChanged.AddListener(ChangeFps);

        int qualityLevel = Options.Quality;
        Text qualityText = panel.transform.Find("QualityComboboxPanel/ComboboxHolder/InputBox/Text").GetComponent<Text>();
        qualityText.text = QualityNames[qualityLevel];
        Button buttonQualityLess = panel.transform.Find("QualityComboboxPanel/ComboboxHolder/ButtonLess").GetComponent<Button>();
        buttonQualityLess.onClick.AddListener(delegate { ChangeQuality(qualityText, -1); });
        Button buttonQualityMore = panel.transform.Find("QualityComboboxPanel/ComboboxHolder/ButtonMore").GetComponent<Button>();
        buttonQualityMore.onClick.AddListener(delegate { ChangeQuality(qualityText, +1); });

        try
        {
            string resolution = Options.Resolution;
            Text resolutionText = panel.transform.Find("ResolutionComboboxPanel/ComboboxHolder/InputBox/Text").GetComponent<Text>();
            if (!HasSupportOfResolution(resolution)) resolution = GetAllResolutions().Last().ToString();
            resolutionText.text = resolution;
            Button buttonResolutionLess = panel.transform.Find("ResolutionComboboxPanel/ComboboxHolder/ButtonLess").GetComponent<Button>();
            buttonResolutionLess.onClick.AddListener(delegate { ChangeResolution(resolutionText, -1); });
            Button buttonResolutionMore = panel.transform.Find("ResolutionComboboxPanel/ComboboxHolder/ButtonMore").GetComponent<Button>();
            buttonResolutionMore.onClick.AddListener(delegate { ChangeResolution(resolutionText, +1); });
        }
        catch (Exception)
        {
            Text resolutionText = panel.transform.Find("ResolutionComboboxPanel/ComboboxHolder/InputBox/Text").GetComponent<Text>();
            resolutionText.text = "Default";
        }

        try
        {
            int displayId = Options.DisplayId;
            Text displayIdText = panel.transform.Find("DisplayComboboxPanel/ComboboxHolder/InputBox/Text").GetComponent<Text>();
            displayIdText.text = "Monitor " + (displayId + 1);
            Button buttonDisplayLess = panel.transform.Find("DisplayComboboxPanel/ComboboxHolder/ButtonLess").GetComponent<Button>();
            buttonDisplayLess.onClick.AddListener(delegate { ChangeDisplay(ref displayId, -1, displayIdText); });
            Button buttonDisplayMore = panel.transform.Find("DisplayComboboxPanel/ComboboxHolder/ButtonMore").GetComponent<Button>();
            buttonDisplayMore.onClick.AddListener(delegate { ChangeDisplay(ref displayId, +1, displayIdText); });
        }
        catch (Exception)
        {
            Messages.ShowError("Error during creation of list of available monitors");
        }
    }

    private void ChangeDisplay(ref int displayId, int change, Text displayIdText)
    {
        try
        {
            displayId += change;

            if (displayId < 0 || displayId >= Display.displays.Count())
            {
                displayId -= change;
            }
            else
            {
                Options.DisplayId = displayId;
                Options.ChangeParameterValue("DisplayId", displayId);

                displayIdText.text = "Monitor " + (displayId + 1);

                PlayerPrefs.SetInt("UnitySelectMonitor", displayId);
            }

            Messages.ShowInfo("Restart to apply change of monitor");
        }
        catch (Exception)
        {
            Messages.ShowError("Error during changing of monitor");
        }
    }

    private void ChangeResolution(Text resolutionText, int change)
    {
        try
        {
            Resolution[] availableResolutions = GetAllResolutions();
            if (HasSupportOfResolution(resolutionText.text))
            {
                Resolution currentResolution = availableResolutions.FirstOrDefault(n => n.ToString() == resolutionText.text);
                int currentIndex = Array.IndexOf(availableResolutions, currentResolution);
                int newIndex = Mathf.Clamp(currentIndex + change, 0, availableResolutions.Length - 1);

                Resolution newResolution = availableResolutions[newIndex];
                resolutionText.text = newResolution.ToString();
                Options.ChangeParameterValue("Resolution", newResolution.ToString());
                FullScreenMode newFullScreenMode = Options.FullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
                Screen.SetResolution(newResolution.width, newResolution.height, newFullScreenMode, newResolution.refreshRateRatio);
            }
        }
        catch (Exception)
        {
            Messages.ShowError("Error during changing of resolution");
        }
    }

    private Resolution[] GetAllResolutions()
    {
        return Screen.resolutions.Where(n => n.width >= 1280).ToArray();
    }

    private bool HasSupportOfResolution(string resolutionText)
    {
        return GetAllResolutions().Any(n => n.ToString() == resolutionText);
    }

    private void ChangeQuality(Text text, int change)
    {
        int qualityLevel = Options.Quality;

        qualityLevel = Mathf.Clamp(qualityLevel + change, 0, 2);

        Options.Quality = qualityLevel;
        Options.ChangeParameterValue("Quality", qualityLevel);
        text.text = QualityNames[qualityLevel];
    }

    private void ChangeFullscreen(bool isFullscreen)
    {
        Options.FullScreen = isFullscreen;
        Options.ChangeParameterValue("FullScreen" , isFullscreen);
        Screen.fullScreen = isFullscreen;
    }

    private void ChangeFps(bool isShowFps)
    {
        Options.ShowFps = true;
        Options.ChangeParameterValue("ShowFps", isShowFps);
    }

}
