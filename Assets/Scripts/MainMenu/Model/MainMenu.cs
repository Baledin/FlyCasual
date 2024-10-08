using Editions;
using ExtraOptions;
using Migrations;
using Mods;
using Obstacles;
using Players;
using SquadBuilderNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

public class MainMenu : MonoBehaviour {

    const string PatreonUrl = "https://www.patreon.com/Sandrem";
    private Faction CurrentAvatarsFaction = Faction.Rebel;
    public GameObject CurrentPanel;
    public GameObject RosterBuilderPrefab;
    public GameObject UpgradeLinePrefab;
    public static MainMenu CurrentMainMenu;
    public string PreviousPanelName;
    string NewVersionUrl;

    void Start ()
    {
        MigrationsManager.PerformMigrations();
        InitializeMenu();
    }

    private void AddAvailableAvatar(GenericUpgrade avatarUpgrade, int count)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/MainMenu/AvatarImage", typeof(GameObject));
        GameObject avatarPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/AvatarsPanel/ContentPanel").transform);

        int row = count / 8;
        int column = count - row * 8;

        avatarPanel.transform.localPosition = new Vector2(20 + column * 120, -20 - row * 110);
        avatarPanel.name = avatarUpgrade.GetType().ToString();

        AvatarFromUpgrade avatar = avatarPanel.GetComponent<AvatarFromUpgrade>();
        avatar.Initialize(avatarUpgrade.GetType().ToString(), ChangeAvatar);

        if (avatarUpgrade.GetType().ToString() == Options.Avatar)
        {
            SetAvatarSelected(avatarPanel.transform.position);
        }
    }

    public void BrowseMatches()
    {
        StartCoroutine(BrowseMatchesAsync());
    }

    private IEnumerator BrowseMatchesAsync()
    {
        BrowseNetworkRoomsUI.Instance.ShowLoading();
        Network.BrowseMatches();

        yield return new WaitForSeconds(3);

        BrowseNetworkRoomsUI.Instance.ShowRooms();
    }

    public void CancelWaitingForOpponent()
    {
        Network.CancelWaitingForOpponent();
        ChangePanel("MultiplayerDecisionPanel");
    }

    private void ChangeAvatar(string avatarName)
    {
        Options.Avatar = avatarName;
        Options.ChangeParameterValue("Avatar", avatarName);

        SetAvatarSelected(GameObject.Find("UI/Panels/AvatarsPanel/ContentPanel/" + avatarName).transform.position);
    }

    public void ChangeEditionIsClicked(GameObject editionGO)
    {
        ShowActiveEdition(editionGO.name);
        SetEdition(editionGO.name);
    }

    public void ChangeNickName(string text)
    {
        Options.NickName = text;
        Options.ChangeParameterValue("NickName", text);
    }

    public void ChangePanel(string panelName)
    {
        PreviousPanelName = CurrentPanel.name;

        if (Edition.Current.IsSquadBuilderLocked)
        {
            if (panelName == "SquadronOptionsPanel")
            {
                if (CurrentPanel.name == "SquadBuilderPanel")
                {
                    Messages.ShowError("This part of squad builder is disabled");
                    return;
                }
                else
                {
                    panelName = "SelectFactionPanel";
                }
            }
        }

        CurrentPanel.SetActive(false);

        GameObject panel = GameObject.Find("UI/Panels").transform.Find(panelName).gameObject;
        InitializePanelContent(panelName, CurrentPanel.name);
        panel.SetActive(true);
        CurrentPanel = panel;
    }

    public void ChangePanel(GameObject panel)
    {
        ChangePanel(panel.name);
    }

    public void ChangeTitle(string text)
    {
        Options.Title = text;
        Options.ChangeParameterValue("Title", text);
    }

    private void CheckPatreonSupportNotification(bool arg1, bool arg2, int arg3)
    {
        int support = RemoteSettings.GetInt("SupportOnPatreon", -1);
        if (support != -1) ShowSupportOnPatreon(support);

        RemoteSettings.Completed -= CheckPatreonSupportNotification;
    }

    private void CheckSupportUkraineNotification(bool arg1, bool arg2, int arg3)
    {
        bool support = RemoteSettings.GetBool("SupportUkraine", false);
        if (support != false) ShowSupportUkraine();

        RemoteSettings.Completed -= CheckSupportUkraineNotification;
    }

    private void CheckUpdateNotification(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse)
    {
        Global.LatestVersionInt = RemoteSettings.GetInt("UpdateLatestVersionInt", Global.CurrentVersionInt);
        if (Global.LatestVersionInt > Global.CurrentVersionInt)
        {
            string latestVersion = RemoteSettings.GetString("UpdateLatestVersion", Global.CurrentVersion);
            string updateLink = RemoteSettings.GetString("UpdateLink");
            ShowNewVersionIsAvailable(latestVersion, updateLink);
        }

        RemoteSettings.Completed -= CheckUpdateNotification;
    }

    private void ClearAvatarsPanel()
    {
        GameObject avatarsPanel = GameObject.Find("UI/Panels/AvatarsPanel/ContentPanel").gameObject;
        foreach (Transform transform in avatarsPanel.transform)
        {
            GameObject.Destroy(transform.gameObject);
        }

        GameObject.Find("UI/Panels/AvatarsPanel/AvatarSelector").GetComponent<Image>().enabled = false;

        Resources.UnloadUnusedAssets();
    }

    private void ClearBatchAiSquadsTestingMode()
    {
        ExtraOptions.ExtraOptionsList.BatchAiSquadsTestingModeExtraOption.ClearResults();
    }

    public void CreateMatch()
    {
        string roomName = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Name").GetComponentInChildren<InputField>().text;
        string password = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Password").GetComponentInChildren<InputField>().text;

        GameController.Initialize();
        ReplaysManager.TryInitialize(ReplaysMode.Write);

        Network.CreateMatch(roomName, password);

        ChangePanel("WaitingForOpponentsPanel");
    }

    public static Sprite GetRandomMenuBackground()
    {
        List<Sprite> spritesList = Resources.LoadAll<Sprite>("Sprites/Backgrounds/MainMenu/")
            .Where(n => n.name != "_RANDOM")
            .ToList();
        return spritesList[UnityEngine.Random.Range(0, spritesList.Count)];
    }

    public void InitializeAvatars(string factionString)
    {
        CurrentAvatarsFaction = (Faction)Enum.Parse(typeof(Faction), factionString);

        ClearAvatarsPanel();

        int count = 0;

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList.FirstEdition", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            if (type.MemberType == MemberTypes.NestedType) continue;

            GenericUpgrade newUpgradeContainer = (GenericUpgrade)System.Activator.CreateInstance(type);
            if (newUpgradeContainer.UpgradeInfo.Name != null)
            {
                if (newUpgradeContainer.Avatar != null && newUpgradeContainer.Avatar.AvatarFaction == CurrentAvatarsFaction) AddAvailableAvatar(newUpgradeContainer, count++);
            }
        }
    }

    private void InitializeMenu()
    {
        CurrentMainMenu = this;
        SetCurrentPanel();

        DontDestroyOnLoad(GameObject.Find("GlobalUI").gameObject);

        ModsManager.Initialize();
        Options.ReadOptions();
        Options.UpdateVolume();
        ExtraOptionsManager.Initialize();
        SetBackground();
        UpdateVersionInfo();
        ClearBatchAiSquadsTestingMode();

        PrepareUpdateChecker();

        new ObstaclesManager();
    }

    private void InitializeNickName()
    {
        GameObject.Find("UI/Panels/AvatarsPanel/NickName/InputField").gameObject.GetComponent<InputField>().text = Options.NickName;
    }

    private void InitializePanelContent(string panelName, string previousPanelName)
    {
        switch (panelName)
        {
            case "MainMenuPanel":
                ClearBatchAiSquadsTestingMode();
                break;
            case "OptionsPanel":
                OptionsUI.Instance.InitializeOptionsPanel();
                break;
            case "StatsPanel":
                StatsUI.Instance.InitializeStatsPanel();
                break;
            case "ModsPanel":
                ModsManager.InitializePanel();
                break;
            case "CreditsPanel":
                CreditsUI.InitializePanel();
                break;
            case "BrowseRoomsPanel":
                BrowseMatches();
                break;
            case "SelectFactionPanel":
                Global.SquadBuilder.CurrentSquad.ClearAll();
                Global.SquadBuilder.View.ShowFactionsImages();
                Global.SquadBuilder.View.ShowCurrentFormat();
                break;
            case "SquadBuilderPanel":
                Global.SquadBuilder.View.ShowShipsAndUpgrades();
                Global.SquadBuilder.View.UpdateNextButton();
                break;
            case "SelectShipPanel":
                Global.SquadBuilder.View.ShowShipsFilteredByFaction();
                break;
            case "SelectPilotPanel":
                Global.SquadBuilder.View.ShowPilotsFilteredByShipAndFaction();
                break;
            case "ShipSlotsPanel":
                Global.SquadBuilder.View.ShowPilotWithSlots();
                break;
            case "SelectUpgradePanel":
                Global.SquadBuilder.View.ShowUpgradesList();
                break;
            case "BrowseSavedSquadsPanel":
                Global.SquadBuilder.View.BrowseSavedSquads();
                break;
            case "SaveSquadronPanel":
                Global.SquadBuilder.View.PrepareSaveSquadronPanel();
                break;
            case "AvatarsPanel":
                InitializePlayerCustomization();
                break;
            case "EditionPanel":
                ShowActiveEdition(Options.Edition);
                break;
            case "ShipInfoPanel":
                Global.SquadBuilder.View.ShowShipInformation();
                break;
            case "SkinsPanel":
                Global.SquadBuilder.View.ShowSkinsPanel();
                break;
            case "ChosenObstaclesPanel":
                Global.SquadBuilder.View.ShowChosenObstaclesPanel();
                break;
            case "BrowseObstaclesPanel":
                Global.SquadBuilder.View.ShowBrowseObstaclesPanel();
                break;
            case "BrowsePopularSquadsPanel":
                if (previousPanelName == "SquadOptionsPanel") PopularSquads.LastChosenFaction = "All";
                PopularSquads.LoadPopularSquads();
                break;
            case "BrowsePopularSquadsVariantsPanel":
                PopularSquads.LoadPopularSquadsVariants();
                break;
            case "BrowseAvatarsPanel":
                AvatarsManager.LoadAvatars(Faction.None);
                break;
        }
    }

    public void InitializePlayerCustomization()
    {
        InitializeAvatars("Rebel");
        InitializeNickName();
        InitializeTitle();
    }

    public void InitializeSquadBuilder(string modeName)
    {
        Global.SquadBuilder = new SquadBuilder();
        new SquadBuilderView(Global.SquadBuilder);
        Global.SquadBuilder.SetPlayers(modeName);
    }

    private IEnumerator InitializeSquadBuilderCoroutine(string modeName)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        InitializeSquadBuilder(modeName);
        ChangePanel("SelectFactionPanel");
    }

    private void InitializeTitle()
    {
        GameObject.Find("UI/Panels/AvatarsPanel/Title/InputField").gameObject.GetComponent<InputField>().text = Options.Title;
    }

    public void JoinMatch(GameObject panel)
    {
        //Messages.ShowInfo("Joining room...");
        string password = panel.transform.Find("Password").GetComponentInChildren<InputField>().text;
        Network.JoinRoom(password);
    }

    public void JoinRoomByIp(Text ipText)
    {
        Network.ServerUri = "tcp4://" + ipText.text;
        Network.JoinRoom(null);
    }

    public void OnSupportOnPatreonClick()
    {
        Application.OpenURL(PatreonUrl);
    }

    public void OnSupportUkraineClick()
    {
        Application.OpenURL("https://ukraine.ua/news/stand-with-ukraine/");
    }

    public void OnUpdateAvailableClick()
    {
        Application.OpenURL(NewVersionUrl);
    }

    public void OpenPatreon()
    {
        Application.OpenURL("https://www.patreon.com/Baledin");
    }

    private void PrepareUpdateChecker()
    {
        RemoteSettings.Completed += CheckUpdateNotification;
        RemoteSettings.Completed += CheckPatreonSupportNotification;
        RemoteSettings.Completed += CheckSupportUkraineNotification;
        RemoteSettings.ForceUpdate();
    }

    public void PreviousPanel()
    {
        CurrentMainMenu.ChangePanel(CurrentMainMenu.PreviousPanelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static void ScalePanel(Transform panelTransform, float maxScale = float.MaxValue, bool twoBorders = false)
    {
        float bordersSize = (twoBorders) ? 250f : 125f;
        float globalUiScale = GameObject.Find("UI").GetComponent<RectTransform>().localScale.y;

        float scaleX = Screen.width / panelTransform.GetComponent<RectTransform>().sizeDelta.x / globalUiScale;
        float scaleY = (Screen.height - bordersSize * globalUiScale) / panelTransform.GetComponent<RectTransform>().sizeDelta.y / globalUiScale;
        float scale = Mathf.Min(scaleX, scaleY);
        scale = Mathf.Min(scale, maxScale);
        panelTransform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetAvatarSelected(Vector3 position)
    {
        GameObject selector = GameObject.Find("UI/Panels/AvatarsPanel/AvatarSelector");
        selector.GetComponent<Image>().enabled = true;
        selector.transform.position = position;
    }

    public static void SetBackground()
    {
        Sprite background = null;

        if (Options.BackgroundImage != "_RANDOM")
        {
            background = Resources.Load<Sprite>("Sprites/Backgrounds/MainMenu/" + Options.BackgroundImage);
            if (background == null)
            {
                //Background is not present anymore, switch to Random
                background = GetRandomMenuBackground();
                Options.BackgroundImage = "_RANDOM";
                PlayerPrefs.SetString("BackgroundImage", "_RANDOM");
            }
        }
        else
        {
            background = GetRandomMenuBackground();
        }

        GameObject.Find("UI/BackgroundImage").GetComponent<Image>().sprite = background;

        //Fix of bug in Unity 2020.2
        GameObject.Find("UI/BackgroundImage").GetComponent<AspectRatioFitter>().enabled = false;
        GameObject.Find("UI/BackgroundImage").GetComponent<AspectRatioFitter>().enabled = true;
    }

    private void SetCurrentPanel()
    {
        CurrentPanel = GameObject.Find("UI/Panels/MainMenuPanel");
    }

    public static void SetEdition(string editionName)
    {
        Options.Edition = editionName;
        Options.ChangeParameterValue("Edition", editionName);

        switch (editionName)
        {
            /*case "FirstEdition":
                new FirstEdition();
                break;*/
            case "SecondEdition":
                new SecondEdition();
                break;
            default:
                break;
        }
    }

    public void SetFaction(string factionChar)
    {
        PopularSquads.SetFaction(factionChar);
    }

    public void SetFactionForAvatars(string factionChar)
    {
        AvatarsManager.LoadAvatars(factionChar);
    }

    private void ShowActiveEdition(string editionName)
    {
        foreach (Transform panelTransform in GameObject.Find("UI/Panels/EditionPanel/Content").gameObject.transform)
        {
            Image backgroundImage = panelTransform.GetComponent<Image>();
            if (backgroundImage != null) backgroundImage.enabled = false;
        }

        GameObject.Find("UI/Panels/EditionPanel/Content/" + editionName).GetComponent<Image>().enabled = true;
    }

    public static void ShowLoadingScreenNetwork()
    {
        if ((Global.SquadBuilder.SquadLists[PlayerNo.Player2].PlayerType == typeof(AggressorAiPlayer)) && (!Options.DontShowAiInfo))
        {
            GameObject.Find("GlobalUI/LoadingScreen").transform.Find("AiInformation").gameObject.SetActive(true);
            GameObject.Find("GlobalUI/LoadingScreen/AiInformation").transform.Find("ToggleBlock").gameObject.SetActive(false);
        }
    }

    public static void ShowLoadingScreenNetworkContinue()
    {
        GameObject.Find("GlobalUI/LoadingScreen/LoadingInfoPanel").gameObject.SetActive(false);

        GameObject.Find("GlobalUI/LoadingScreen/AiInformation").transform.Find("ToggleBlock").gameObject.SetActive(true);
        GameObject.Find("GlobalUI/LoadingScreen").transform.Find("StartPanel").gameObject.SetActive(true);

        Button startButton = GameObject.Find("GlobalUI/LoadingScreen/StartPanel/StartButton").GetComponent<Button>();
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate
        {
            Options.DontShowAiInfo = true;
            Options.ChangeParameterValue("DontShowAiInfo", GameObject.Find("GlobalUI/LoadingScreen/AiInformation/ToggleBlock/DontShowAgain").GetComponent<Toggle>().isOn);
            Global.StartBattle();
        });
    }

    private void ShowNewVersionIsAvailable(string newVersion, string downloadUrl)
    {
        GameObject mainMenuPanel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").gameObject;
        if (!mainMenuPanel.activeSelf) return;

        GameObject panel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").Find("NewVersionIsAvailable").gameObject;

        panel.transform.Find("Text").GetComponent<Text>().text = "New version\nis available!\n\n" + newVersion;
        panel.transform.position = new Vector2(Screen.width - 20, 20);
        NewVersionUrl = downloadUrl;

        panel.SetActive(true);
    }

    private void ShowSupportOnPatreon(int support)
    {
        //Don't show if new version is available
        if (Global.LatestVersionInt <= Global.CurrentVersionInt)
        {
            GameObject mainMenuPanel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").gameObject;
            if (!mainMenuPanel.activeSelf) return;

            GameObject panel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").Find("SupportOnPatreon").gameObject;

            //panel.transform.Find("Text").GetComponent<Text>().text = $"\"Rules 2.5\" and new expansions are coming! If you want\nto see them in Fly Casual -\nsupport me on patreon\n{support} / 200";
            panel.transform.position = new Vector2(Screen.width - 20, 20);

            panel.SetActive(true);
        }
    }

    private void ShowSupportUkraine()
    {
        GameObject mainMenuPanel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").gameObject;
        if (!mainMenuPanel.activeSelf) return;

        GameObject panel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").Find("SupportUkraine").gameObject;
        panel.SetActive(true);
    }

    public void StartReplay()
    {
        GameController.StartBattle(ReplaysMode.Read);
    }

    public void StartSquadBuilerMode(string modeName)
    {
        ChangePanel("LoadingCardsStubPanel");
        Global.Instance.StartCoroutine(InitializeSquadBuilderCoroutine(modeName));
    }

    private void UpdateVersionInfo()
    {
        GameObject.Find("UI/Panels/MainMenuPanel/Background/Version/Version Text").GetComponent<Text>().text = Global.CurrentVersion;
    }

}
