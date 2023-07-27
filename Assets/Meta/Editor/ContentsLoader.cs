#if DEV
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.TestSuite;
using NodeCanvas.Framework;

namespace BagelCode
{

public class ContentsLoader : EditorWindowBase<ContentsLoader>
{
    private bool initialized = false;
    private List<ContentInfo> contentInfos;
    private string query = "";
    private List<ContentInfo> queryResult;
    private Vector2 scrollPosition;
    private int toolbarIndex;
    private string[] toolbarStrings;
    private bool paytableWindow = false;

    public override string GetEditorName()
    {
        return "Contents Loader";
    }

    [MenuItem("BagelCode/Contents Loader", false, 100)]
    private static void Initialize()
    {
        CreateWindow();
    }

    private void OnGUI()
    {
        if (!initialized)
            Refresh();

        if (paytableWindow)
        {
            BeginCheck();
            toolbarIndex = Toolbar(toolbarIndex, toolbarStrings);
            if (EndCheck())
            {
                GameObject pages = GameObject.Find("Popup Manager/Paytable/Anchor/Pages");
                pages.GetComponent<PageScrollRect>().pageIndex = toolbarIndex;
                Selection.activeGameObject = pages;
                EditorUtility.SetDirty(Selection.activeGameObject);
            }
        }

        BeginCheck();
        query = TextField(query);
        if (EndCheck())
        {
            Query();
        }

        BeginHorizontal();
            if (Button("Splash", TextAnchor.MiddleLeft))
            {
                LoadSplash();
            }
            if (Button("BB", GUILayout.Width(25f)))
            {
                Selection.activeGameObject = GameObject.Find("Global Blackboard/content");
            }
            if (Button("FSM", GUILayout.Width(35f)))
            {
                Selection.activeGameObject = GameObject.Find("Content FSM");
            }
            if (Button("Slot", GUILayout.Width(30f)))
            {
                Selection.activeGameObject = GameObject.Find("Slot Machine");
            }
        EndHorizontal();

        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

			for (int i = 0; i < queryResult.Count; ++i)
			{
                BeginHorizontal();
                    if (IconButton("", "Assets", TextAnchor.MiddleCenter, GUILayout.Width(30f)))
                    {
                        ContentInfo info = queryResult[i];
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(GetAssetDirectoryPath(info.gameTitle, "Game Contents"));
                    }
    				if (Button(string.Format("{0} - {1}", queryResult[i].gameTitle, queryResult[i].gameTitleName), TextAnchor.MiddleLeft))
                    {
                        LoadSplash();
                        ClearMainCanvas();
                        LoadContent(i);
                        EditorPrefs.SetString("gameId", queryResult[i].gameTitle);
                    }
                    if (Button("S", TextAnchor.MiddleCenter, GUILayout.Width(25f)))
                    {
                        LoadSplash();
                        ClearMainCanvas();
                        LoadContent(i);
                        CreateSlotMachine();
                    }
                    if (Button("P", TextAnchor.MiddleCenter, GUILayout.Width(25f)))
                    {
                        LoadSplash();
                        ClearMainCanvas();
                        LoadPaytable(i);
                        paytableWindow = true;
                    }
                EndHorizontal();
			}
        }
    }

    private void Refresh()
    {
        var json = AssetBundleManager.LoadAsset<TextAsset>("testsuite", "Contents");
        contentInfos = SlotSimpleJson.DeserializeObject<List<ContentInfo>>(json.text);

        Query();
    }

    private void Query()
    {
        int version = -1;

        string[] tokens = query.Split(' ');
        List<Regex> regs = new List<Regex>();
        foreach (var token in tokens)
        {
            string[] options = token.Split(':');
            if (options.Length > 1)
            {
                if (string.Equals(options[0], "v"))
                {
                    int v;
                    if (int.TryParse(options[1], out v))
                        version = v;
                }
            }
            else
            {
                regs.Add(new Regex(token, RegexOptions.IgnoreCase | RegexOptions.Compiled));
            }
        }

        queryResult = new List<ContentInfo>();
        foreach (var contentInfo in contentInfos)
        {
            if (version > 0 && version != contentInfo.version)
                continue;

            if (regs.Count == 0)
            {
                queryResult.Add(contentInfo);
                continue;
            }

            foreach (var reg in regs)
            {
                if (reg.IsMatch(contentInfo.gameTitle) || reg.IsMatch(contentInfo.gameTitleName))
                {
                    queryResult.Add(contentInfo);
                    break;
                }
            }
        }

        queryResult.Reverse();
    }

    private void LoadSplash()
    {
        EditorSceneManager.OpenScene("Assets/Meta/Scenes/Splash.unity");
        paytableWindow = false;
        toolbarIndex = 0;
    }

    private void ClearMainCanvas()
    {
        GameObject.Find("Main Canvas").DestroyImmediateChildren();
    }

    private void LoadContent(int resultIndex)
    {
        ContentInfo info = queryResult[resultIndex];

        var root = GameObject.Find("Main Canvas").transform;
        {
            string assetName = info.needCustomInGameScene ? "In Game Scene Video Poker" : "In Game Scene Slot";

            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName).GetSceneInfo();
            SceneManager.LoadSceneInEditor(root, sceneInfo);
        }
        root = GameObject.Find("Game Canvas").transform;
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(info.gameTitle, "Game Contents Scene").GetSceneInfo();
            SceneManager.LoadSceneInEditor(root, sceneInfo);
        }

        root = GameObject.Find("Meta System").transform;
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(info.gameTitle, "Content FSM");
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.name = "Content FSM";
            go.transform.SetParent(root, false);
        }

        root = GameObject.Find("Global Blackboard/content").transform;
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(info.gameTitle, "customData");
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.name = "customData";
            go.transform.SetParent(root, false);
        }

        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(
            string.Format("Assets/Contents/Contents Group {0}/{1}/Prefabs", 1, info.gameTitleName)
        );
    }

    private void CreateSlotMachine()
    {
        var slotData = ContentCustomData.GetSlotData(0);
        int column = slotData.column;
        int row = slotData.row;

        CreateReelStripsManager(column, row);

        var slotMachine = GameObject.Find("Slot Machine").GetComponent<SlotMachine>();
        List<int> visibleCounts = slotData.visibleCounts;
        for (int i = 0; i < column; ++i)
        {
            int beginRow = row - visibleCounts[i];
            int endRow = beginRow + visibleCounts[i];
            slotMachine.CreateReel(i, beginRow, i + 1, endRow, 0);
        }
        slotMachine.Shuffle();

        // symbol controller version
        slotMachine.Visit((sb) =>
        {
            var image = sb.gameObject.GetComponent<Blackboard>().GetValue<SpriteRenderer>("baseImage");
            if (image != null)
                ContentCustomData.Instance.symbolSprite.Apply(sb, image, 0);
        });
    }

    private void CreateReelStripsManager(int column, int row)
    {
        var parent = ContentCustomData.Instance.transform;
        var go = new GameObject();
        go.name = "ReelStrips Manager";
        go.transform.SetParent(parent, false);

        var mgr = go.AddComponent<GlobalReelStrips>();
        mgr.stripsList = new List<ReelStrips>();

        var symbolMask = ContentCustomData.GetSlotData(0).symbolMask;
        int symbolCount = ContentCustomData.Instance.symbolCount;

        List<List<int>> reelSequenceList = new List<List<int>>();
        for (int colIndex = 0; colIndex < column; ++colIndex)
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < 20; ++i)
                indexList.Add(UnityEngine.Random.Range(0, symbolCount));
            reelSequenceList.Add(indexList);
        }

        go = new GameObject();
        go.name = "ReelStrips";
        go.transform.SetParent(mgr.transform, false);

        var reelStrips = go.AddComponent<ReelStrips>();
        reelStrips.reelStrips = new List<BaseReelStrip>();

        for (int i = 0; i < reelSequenceList.Count; ++i)
        {
            go = new GameObject();
            go.name = "ReelStrip";
            go.transform.SetParent(reelStrips.transform, false);

            var reelStrip = go.AddComponent<ReelStrip>();
            reelStrip.stripIndex = i;
            reelStrip.strip = new List<SymbolInfo>();

            var indexList = reelSequenceList[i];
            for (int j = 0; j < indexList.Count; ++j)
            {
                reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(indexList[j], symbolMask));
            }
            reelStrips.reelStrips.Add(reelStrip);
        }
        mgr.stripsList.Add(reelStrips);
    }

    private void LoadPaytable(int resultIndex)
    {
        ContentInfo info = queryResult[resultIndex];

        var root = GameObject.Find("Global Blackboard/content").transform;
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(info.gameTitle, "customData");
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.name = "customData";
            go.transform.SetParent(root, false);
        }

        CreateContentLang(resultIndex);
        CreateSymbolPay();

        root = GameObject.Find("Popup Manager").transform;
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(info.gameTitle, "Paytable");
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.name = "Paytable";
            go.transform.SetParent(root, false);
            int pageCount = go.GetComponent<Blackboard>().GetValue<int>("pageCount");
            toolbarStrings = new string[pageCount];
            for (int i = 0; i < pageCount; ++i)
            {
                toolbarStrings[i] = "Page " + (i + 1);
            }
        }

        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(
            string.Format("Assets/Contents/Contents Group {0}/{1}/Prefabs", 1, info.gameTitleName)
        );
    }

    private void CreateContentLang(int resultIndex)
    {
        var contentBB = StringTable.Get(StringTable.StringTableType.Content);
		contentBB.variables.Clear();
        var stringTable = AssetBundleManager.LoadAsset<StringTableObject>(queryResult[resultIndex].gameTitle + "lang", "EN_Content").stringTable;
        foreach (var pair in stringTable)
        {
            var variable = contentBB.AddVariable(pair.key, typeof(string));
            variable.value = pair.value;
        }
    }

    private void CreateSymbolPay()
    {
        var cb = ContentBlackboard.Get();

        var customData = cb.AddVariable("customData", typeof(Blackboard));
        customData.value = GameObject.Find("Global Blackboard/content/customData").GetComponent<Blackboard>();

        var game = BlackboardUtils.GetOrCreateBlackboard(cb, "game");
        var paytables = BlackboardUtils.GetOrCreateBlackboardList(game, "paytables");
        var child = BlackboardUtils.CreateBlackboard("List`1");
        BlackboardUtils.AddToBlackboardList(game, "paytables", child);
        BlackboardUtils.GetOrCreateBlackboardList(paytables[0], "value");

        // set dummy symbol pay
        int symbolCount = ContentCustomData.Instance.symbolCount;
        List<long> symbolPay = new List<long>();
        for (long j = 0; j < 10; ++j)
            symbolPay.Add((j + 1) * 30);

        for (int i = 0; i < symbolCount + 40; ++i)
        {
            var symbolPayBB = BlackboardUtils.CreateBlackboard("List`1");
            BlackboardUtils.SetOrCreateValue(symbolPayBB, "value", symbolPay);
            BlackboardUtils.AddToBlackboardList(paytables[0], "value", symbolPayBB);
        }

        // set normal path of scatter pay
        BlackboardUtils.SetOrCreateValue(game, "scatterPay", symbolPay);
        BlackboardUtils.SetOrCreateValue(game, "scatterPays", symbolPay);
    }

    string GetAssetPath(string bundleName, string assetName)
    {
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
        if (assetPaths.Length > 0)
            return assetPaths[0];

        return null;
    }

    string GetAssetDirectoryPath(string bundleName, string assetName)
    {
        string assetPath = GetAssetPath(bundleName, assetName);
        if (assetPath != null)
        {
            return Path.GetDirectoryName(assetPath);
        }

        return null;
    }
}

}
#endif
