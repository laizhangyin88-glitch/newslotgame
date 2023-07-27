using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NodeCanvas.Editor;
using NodeCanvas.Framework;
using SlotMaker.Json;
using SlotMaker.TestSuite;
using SlotMaker.Slots.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using StyleSheet = UnityEngine.UIElements.StyleSheet;

namespace SlotMaker
{
    public class ContentsLoader : EditorWindow
    {
        private static GUIContent _titleContent = new GUIContent("Contents Loader");

        [MenuItem("SlotMaker/Contents Loader", false, 10)]
        public static void ShowEditor()
        {
            var window = GetWindow<ContentsLoader>();
            window.minSize = new Vector2(240, 120);
            window.titleContent = _titleContent;
        }

        private bool gameSearchTitleOn;
        private bool gameSearchTitleNameOn;
        private bool gameSearchDevelopOn;
        private bool gameSearchAlphaOn;
        private bool gameSearchBetaOn;
        private bool gameSearchVerifiedOn;

        private bool optionShowGameInfo;
        private bool optionAutoOpenGraph;

        private readonly string EDITOR_PREFS_PREFIX = "SlotMaker.Editor.";

        private Toolbar paytableSelector;

        private GameManifest selectedGame = new GameManifest();

        private List<GameManifest> gameInfos;
        private List<GameManifest> queryResults = new List<GameManifest>();
        private Dictionary<string, string> gameInfoPaths = new Dictionary<string, string>();

        private ListView listViewGames;
        private VisualElement gameInfoBox;

        private IntegerField gameInfoGameId;
        private TextField gameInfoGameTitle;
        private TextField gameInfoGameTitleName;
        private TextField gameInfoVersion;
        private TextField gameInfoPublishVersion;
        private EnumField gameInfoStage;

        private StyleSheet styleSheet;

        public void OnEnable()
        {
            LoadGameInfos();

            gameSearchTitleOn = GetBool("gameSearchTitleOn", true);
            gameSearchTitleNameOn = GetBool("gameSearchTitleNameOn", true);
            gameSearchDevelopOn = GetBool("gameSearchDevelopOn", true);
            gameSearchAlphaOn = GetBool("gameSearchAlphaOn", true);
            gameSearchBetaOn = GetBool("gameSearchBetaOn", true);
            gameSearchVerifiedOn = GetBool("gameSearchVerifiedOn", true);
            optionShowGameInfo = GetBool("optionShowGameInfo", false);
            optionAutoOpenGraph = GetBool("optionAutoOpenGraph");
            selectedGame.name = GetString("selectedGame");

            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SlotMaker/Editor/ContentsLoader/styles.uss");

            var root = this.rootVisualElement;
            root.styleSheets.Add(styleSheet);

            var toolbar = new Toolbar();
            root.Add(toolbar);

            var gameSearch = new ToolbarPopupSearchField();
            var actualSearchValue = GetString("Search");

            gameSearch.value = actualSearchValue;
            gameSearch.RegisterValueChangedCallback(OnGameSearchTextChanged);
            gameSearch.menu.AppendAction("Title", a =>
            {
                 gameSearchTitleOn = Toggle("gameSearchTitleOn", gameSearchTitleOn);
                 RefreshGameList(actualSearchValue);
            }, a => gameSearchTitleOn ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            gameSearch.menu.AppendAction("TitleName", a =>
            {
                gameSearchTitleNameOn = Toggle("gameSearchTitleNameOn", gameSearchTitleNameOn);
                RefreshGameList(actualSearchValue);
            }, a => gameSearchTitleNameOn ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            gameSearch.menu.AppendAction("Develop", a =>
            {
                gameSearchDevelopOn = Toggle("gameSearchDevelopOn", gameSearchDevelopOn);
                RefreshGameList(actualSearchValue);
            }, a => gameSearchDevelopOn ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            gameSearch.menu.AppendAction("Alpha", a =>
            {
                gameSearchAlphaOn = Toggle("gameSearchAlphaOn", gameSearchAlphaOn);
                RefreshGameList(actualSearchValue);
            }, a => gameSearchAlphaOn ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            gameSearch.menu.AppendAction("Beta", a =>
            {
                gameSearchBetaOn = Toggle("gameSearchBetaOn", gameSearchBetaOn);
                RefreshGameList(actualSearchValue);
            }, a => gameSearchBetaOn ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            gameSearch.menu.AppendAction("Verified", a =>
            {
                gameSearchVerifiedOn = Toggle("gameSearchVerifiedOn", gameSearchVerifiedOn);
                RefreshGameList(actualSearchValue);
            }, a => gameSearchVerifiedOn ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            toolbar.Add(gameSearch);

            toolbar.Add(new ToolbarButton(() => LoadSplashScene()) { name = "labelSplash", text = "Q", tooltip = "Open Splash" });

            var menuOptions = new ToolbarMenu { text = "..." };
            menuOptions.menu.AppendAction("Refresh", a => RefreshAll());
            menuOptions.menu.AppendAction("Show GameInfo", a =>
            {
                optionShowGameInfo = Toggle("optionShowGameInfo", optionShowGameInfo);
                gameInfoBox.style.display = optionShowGameInfo ? DisplayStyle.Flex : DisplayStyle.None;
            }, a => optionShowGameInfo ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            menuOptions.menu.AppendAction("Auto Open Graph", a => optionAutoOpenGraph = Toggle("optionAutoOpenGraph", optionAutoOpenGraph), a => optionAutoOpenGraph ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            toolbar.Add(menuOptions);

            //
            var toolbar2 = new Toolbar();
            root.Add(toolbar2);

            toolbar2.Add(new ToolbarButton(() => ReOpenGame()){ text = "R", tooltip = "ReOpen" });
            toolbar2.Add(new ToolbarSpacer());
            toolbar2.Add(new ToolbarButton(() => SelectFSM()){ name = "labelFSM", text = "F", tooltip = "Select Contents FSM" });
            toolbar2.Add(new ToolbarButton(() => SelectBlackboard()){ name = "labelBlackboard", text = "B", tooltip = "Select Contents Blackboard" });
            toolbar2.Add(new ToolbarButton(() => SelectBase()){ name = "labelBase", text = "C", tooltip = "Select Contents Base" });
            toolbar2.Add(new ToolbarButton(() => SelectSlotMachine()){ name = "labelSlotMachine", text = "S", tooltip = "Select Slot Machine" });
            toolbar2.Add(new ToolbarSpacer());
            toolbar2.Add(new ToolbarButton(() => LegacyContentsLoader.CreateSlotMachine()){ text = "^S", tooltip = "Load Slot Machine" });
            toolbar2.Add(new ToolbarButton(() => LoadPaytable(GetString("gameTitle"))){ text = "P", tooltip = "Load Paytable" });

            //
            paytableSelector = new Toolbar();
            root.Add(paytableSelector);

            var gameListBox = new VisualElement();
            gameListBox.style.flexDirection = FlexDirection.Column;
            gameListBox.style.flexGrow = 1f;
            gameListBox.style.flexShrink = 0f;
            gameListBox.style.flexBasis = 0f;
            root.Add(gameListBox);

            listViewGames = new ListView();
            listViewGames.viewDataKey = "ListViewGames";
            listViewGames.styleSheets.Add(styleSheet);
            listViewGames.selectionType = SelectionType.Single;
            listViewGames.onItemChosen += obj => OpenGame(obj as GameManifest);
            listViewGames.onSelectionChanged += objects => objects.ForEach((obj) => SelectGame(obj as GameManifest));
            listViewGames.style.flexGrow = 1f;
            listViewGames.style.flexShrink = 0f;
            listViewGames.style.flexBasis = 0f;
            listViewGames.itemHeight = 45;
            listViewGames.itemsSource = queryResults;
            listViewGames.makeItem = () =>
            {
                var box = new VisualElement();
                box.style.flexDirection = FlexDirection.Row;
                box.style.flexGrow = 1f;
                box.style.flexShrink = 0f;
                box.style.flexBasis = 0f;

                box.Add(new Image() { style = { width = 43, height = 43 } });

                var infoBox = new VisualElement();
                infoBox.style.flexDirection = FlexDirection.Column;
                infoBox.style.flexGrow = 1f;
                infoBox.style.flexShrink = 0f;
                infoBox.style.flexBasis = 0f;
                box.Add(infoBox);

                infoBox.Add(new Label());

                var tagBox = new VisualElement();
                tagBox.style.flexDirection = FlexDirection.Row;
                tagBox.style.flexGrow = 1f;
                tagBox.style.flexShrink = 0f;
                tagBox.style.flexBasis = 0f;
                tagBox.Add(new Label());
                tagBox.Add(new Label() { name = "gameTitle" });

                infoBox.Add(tagBox);

                return box;
            };
            listViewGames.bindItem = (e, i) =>
            {
                var gameInfo = queryResults[i];
                e.userData = gameInfo;
                (e.ElementAt(0) as Image).image = AssetBundleManager.LoadAsset<Texture2D>("slotthumb1", "Slot Thumbnail Image " + gameInfo.name.ToUpper());

                var infoBox = e.ElementAt(1) as VisualElement;
                (infoBox.ElementAt(0) as Label).text = gameInfo.displayName;

                var tagBox = infoBox.ElementAt(1) as VisualElement;

                var versionLabel = tagBox.ElementAt(0) as Label;
                versionLabel.name = gameInfo.stage.ToString();
                versionLabel.text = string.Format("{0} {1}", gameInfo.version, gameInfo.stage.ToString().ToLower());
                (tagBox.ElementAt(1) as Label).text = gameInfo.name;
            };
            gameListBox.Add(listViewGames);

            gameInfoBox = new VisualElement();
            gameInfoBox.style.flexDirection = FlexDirection.Column;
            gameInfoBox.style.flexGrow = 0f;
            gameInfoBox.style.flexShrink = 0f;
            gameInfoBox.style.flexBasis = 150f;
            gameInfoBox.style.display = optionShowGameInfo ? DisplayStyle.Flex : DisplayStyle.None;
            gameListBox.Add(gameInfoBox);

            var gameInfoToolbar = new Toolbar();
            gameInfoToolbar.style.flexShrink = 0f;
            gameInfoBox.Add(gameInfoToolbar);
            gameInfoBox.Add(gameInfoGameId = new IntegerField() { label = "Game Id" });
            gameInfoBox.Add(gameInfoGameTitle = new TextField() { label = "Game Title" });
            gameInfoBox.Add(gameInfoGameTitleName = new TextField() { label = "Game Title Name" });
            gameInfoBox.Add(gameInfoVersion = new TextField() { label = "Version" });
            gameInfoBox.Add(gameInfoPublishVersion = new TextField() { label = "Publish Version" });
            gameInfoBox.Add(gameInfoStage = new EnumField(GameManifest.Stage.Develop) { label = "Stage" });
            gameInfoBox.Add(new Button(() => ApplyGameInfo()) { text = "Apply" });

            // Shortcut
            root.RegisterCallback<KeyDownEvent>(OnShortcut);
            RefreshGameList(actualSearchValue);
        }

        private void OnShortcut(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Q:
                    LoadSplashScene();
                    break;
                case KeyCode.R:
                    ReOpenGame();
                    break;
                case KeyCode.F:
                    SelectFSM();
                    break;
                case KeyCode.B:
                    SelectBlackboard();
                    break;
                case KeyCode.C:
                    SelectBase();
                    break;
                case KeyCode.S:
                    if (evt.ctrlKey)
                        LegacyContentsLoader.CreateSlotMachine();
                    else
                        SelectSlotMachine();
                    break;
                case KeyCode.P:
                    LoadPaytable(GetString("gameTitle"));
                    break;
            }
        }

        private void LoadGameInfos()
        {
            var paths = AssetDatabase.FindAssets("l:testsuite gameInfo").Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            gameInfos = new List<GameManifest>();
            foreach (var path in paths)
            {
                var textAsset = (TextAsset) AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
                var json = textAsset.text;
                var gameInfo = SlotSimpleJson.DeserializeObject<GameManifest>(json);
                gameInfo.ParseVersion();
                gameInfos.Add(gameInfo);
                gameInfoPaths[gameInfo.name] = path;
            }

            gameInfos.Sort(GameManifest.Sort);
            gameInfos.Reverse();

            queryResults.Clear();
            foreach (var gameInfo in gameInfos)
            {
                queryResults.Add(gameInfo);
            }
        }

        private void ApplyGameInfo()
        {
            selectedGame.gameId = gameInfoGameId.value;
            selectedGame.name = gameInfoGameTitle.value;
            selectedGame.displayName = gameInfoGameTitleName.value;
            selectedGame.version = gameInfoVersion.value;
            selectedGame.publishVersion = gameInfoPublishVersion.value;
            selectedGame.stage = (GameManifest.Stage)gameInfoStage.value;
            selectedGame.dependencies["slotmaker"] = ContentsManifest.GetSlotMakerManifest().version;

            SlotsEditorUtils.SaveJson(gameInfoPaths[selectedGame.name], selectedGame);
            ContentsManifest.RefreshAll();

            listViewGames.Refresh();
        }

        private void LoadSplashScene()
        {
            EditorSceneManager.OpenScene( EditorBuildSettings.scenes[0].path );
        }

        private void RefreshAll()
        {
            LoadGameInfos();
            RefreshGameList(GetString("Search"));
        }

        private void LoadContentsScene()
        {
            EditorSceneManager.OpenScene( "Assets/SlotMaker/Packages/Presets/Scenes/Contents Landscape Scene.unity" );

            paytableSelector.Clear();
        }

        private void LoadContent(string bundleName)
        {
            var root = GetGameCanvas().transform;
            LoadSceneInfo(bundleName, "Game Contents Scene", root);

            root = GameObject.Find("Meta System").transform;
            {
                var go = LoadGameObject(bundleName, "Content FSM", root);
                go.GetComponent<GraphOwner>().enabled = false;
            }

            root = GameObject.Find("Global Blackboard/content").transform;
            LoadGameObject(bundleName, "customData", root);

            SelectFolder(bundleName, "Game Contents");
        }

        private void LoadPaytable(string bundleName)
        {
            if (Application.isPlaying)
            {
                var paytable = LoadGameObject(bundleName, "Paytable", PopupManager.Instance.contents);
                PopupManager.Instance.Open(paytable);
            }
            else
            {
                int pageCount = LegacyContentsLoader.LoadPaytable(bundleName);
                paytableSelector.Clear();
                for (int i = 0; i < pageCount; ++i)
                {
                    int pageIndex = i;
                    paytableSelector.Add(new Button(() => LegacyContentsLoader.SelectPaytable(pageIndex)){ text = (i + 1).ToString() });
                }
            }
        }

        private void LoadTestBridge()
        {
            var root = GameObject.Find("Meta System").transform;
            LoadGameObject(GetAppBundle("login"), "GS Manager", root);
            LoadGameObject("testsuite", "Bridge", root);
        }

        private void LoadTestSuite()
        {
            LoadGameObject("testsuite", "TestSuite Manager", null);
        }

        private void LoadScene(string bundleName, string assetName, Transform parent)
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundleName, assetName).GetSceneInfo();
            SceneManager.LoadSceneInEditor(parent, sceneInfo);
        }

        private GameObject LoadGameObject(string bundleName, string assetName, Transform parent)
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, assetName);
            GameObject go = null;
            if (Application.isPlaying)
            {
                go = GameObject.Instantiate(prefab) as GameObject;
            }
            else
            {
                go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            go.name = assetName;
            go.transform.SetParent(parent, false);
            return go;
        }

        private void LoadSceneInfo(string bundleName, string assetName, Transform parent)
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundleName, assetName).GetSceneInfo();
            SceneManager.LoadSceneInEditor(parent, sceneInfo);
        }

        private string GetAssetPath(string bundleName, string assetName)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            if (assetPaths.Length > 0)
                return assetPaths[0];
            return null;
        }

        private string GetAssetDirectoryPath(string bundleName, string assetName)
        {
            string assetPath = GetAssetPath(bundleName, assetName);
            if (assetPath != null)
                return Path.GetDirectoryName(assetPath);
            return null;
        }

        private string GetRootPath(string bundleName)
        {
            var path = GetAssetDirectoryPath(bundleName, "Game Contents");
            return Path.GetDirectoryName(path);
        }

        private void SelectFolder(string bundleName, string assetName)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(GetAssetDirectoryPath(bundleName, assetName));
        }

        private void SelectFolder(string bundleName)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(GetRootPath(bundleName));
        }

        private void SelectRelativePath(string bundleName, string path)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(GetRootPath(bundleName) + path);
        }

        private string GetAppBundle(string bundleName)
        {
            return ApplicationSettings.MakeApplicationBundleName(bundleName);
        }

        private GameObject GetGameCanvas()
        {
            var go = GameObject.Find("Game Canvas");
            if (go == null)
                go = GameObject.Find("Main Canvas");
            return go;
        }

        private GameObject SelectGameObject(string gameObjectName)
        {
            var go = GameObject.Find(gameObjectName);
            Selection.activeGameObject = go;
            return go;
        }

        private void SelectFSM()
        {
            var go = SelectGameObject("Content FSM");
            if (optionAutoOpenGraph)
                GraphEditor.OpenWindow(go.GetComponent<GraphOwner>());
        }

        private void SelectBlackboard()
        {
            SelectGameObject("Global Blackboard/content");
        }

        private void SelectBase()
        {
            SelectGameObject("Game Contents/Animator/Anchor/Base");
        }

        private void SelectSlotMachine()
        {
            SelectGameObject("Slot Machine");
        }

        private void OnGameSearchTextChanged(ChangeEvent<string> evt)
        {
            RefreshGameList(evt.newValue);
            SetString("Search", evt.newValue);
        }

        private void RefreshGameList(string filter)
        {
            var reg = new Regex(filter, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            queryResults.Clear();
            foreach (var gameInfo in gameInfos)
            {
                switch (gameInfo.stage)
                {
                    case GameManifest.Stage.Develop:
                        if (!gameSearchDevelopOn) continue;
                        break;
                    case GameManifest.Stage.Alpha:
                        if (!gameSearchAlphaOn) continue;
                        break;
                    case GameManifest.Stage.Beta:
                        if (!gameSearchBetaOn) continue;
                        break;
                    case GameManifest.Stage.Verified:
                        if (!gameSearchVerifiedOn) continue;
                        break;
                }
                if ((gameSearchTitleOn && reg.IsMatch(gameInfo.name)) || (gameSearchTitleNameOn && reg.IsMatch(gameInfo.displayName)))
                    queryResults.Add(gameInfo);
            }
            listViewGames.Refresh();
        }

        private void OpenGame(GameManifest gameInfo)
        {
            SetInt("gameId", gameInfo.gameId);
            SetString("gameTitle", gameInfo.name);
            SetString("gameTitleName", gameInfo.displayName);
            SelectGame(gameInfo);

            if (Application.isPlaying)
            {
                MetaSystem.SelectGame(gameInfo.gameId);
                MetaSystem.EnterGame();
            }
            else
            {
                LoadContentsScene();
                LoadContent(gameInfo.name);
            }

            _titleContent.text = string.Format("Contents Loader - {0}", gameInfo.name);
            _titleContent.tooltip = gameInfo.displayName;
        }

        private void ReOpenGame()
        {
            LoadContentsScene();
            LoadContent(GetString("gameTitle"));
        }

        private void SelectGame(GameManifest gameInfo)
        {
            selectedGame = gameInfo;
            SelectFolder(selectedGame.name);

            gameInfoGameId.value = gameInfo.gameId;
            gameInfoGameTitle.value = gameInfo.name;
            gameInfoGameTitleName.value = gameInfo.displayName;
            gameInfoVersion.value = gameInfo.version;
            gameInfoPublishVersion.value = gameInfo.publishVersion;
            gameInfoStage.value = gameInfo.stage;
        }

        private bool Toggle(string key, bool value)
        {
            value = !value;
            SetBool(key, value);
            return value;
        }

        private bool GetBool(string key, bool defaultValue = false)
        {
            return EditorPrefs.GetBool(EDITOR_PREFS_PREFIX + key, defaultValue);
        }

        private void SetBool(string key, bool value)
        {
            EditorPrefs.SetBool(EDITOR_PREFS_PREFIX + key, value);
        }

        private int GetInt(string key, int defaultValue = 0)
        {
            return EditorPrefs.GetInt(EDITOR_PREFS_PREFIX + key, defaultValue);
        }

        private void SetInt(string key, int value)
        {
            EditorPrefs.SetInt(EDITOR_PREFS_PREFIX + key, value);
        }

        private string GetString(string key)
        {
            return EditorPrefs.GetString(EDITOR_PREFS_PREFIX + key);
        }

        private void SetString(string key, string value)
        {
            EditorPrefs.SetString(EDITOR_PREFS_PREFIX + key, value);
        }
    }
}
