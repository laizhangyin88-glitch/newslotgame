#if DEV
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using SlotMaker;
using SlotMaker.Json;
using SlotMaker.TestSuite;

using static UnityEngine.GUILayout;

namespace BagelCode
{
    public class AssetDependencyChecker : EditorWindowBase<AssetDependencyChecker>
    {
        private bool initialized = false;
        private Vector2 contentsScrollPosition;
        private Vector2 ignorePathScrollPosition;
        private string query = "";
        private List<ContentInfo> queryResult;
        private List<ContentInfo> contentInfos;
        private TextAsset ignorePathData;
        private List<string> ignorePathList;

        private const string GAME_PATH = "Assets/Contents/Contents Group {0}/{1}";
        private const string SAVE_DATA_PATH = "Assets/SlotMaker/Editor/Tools/Bundle/";

        public override string GetEditorName()
        {
            return "Asset Dependency";
        }

        [MenuItem ("BagelCode/AssetDependencyChecker")]
        private static void Init()
        {
            CreateWindow();
        }

        private void Reset()
        {
            initialized = true;
            contentInfos = ContentsManifest.GetContentInfos();

            ignorePathData = (TextAsset)AssetDatabase.LoadAssetAtPath(SAVE_DATA_PATH + "AssetDependencyChecker_IgnorePathData.json", typeof(TextAsset));
            ignorePathList = SlotSimpleJson.DeserializeObject<List<string>>(ignorePathData.text);

            Query();
        }

        private void SaveData(string result)
        {
            // Save DependencyResult
            string resultPath = AssetDatabase.GenerateUniqueAssetPath(SAVE_DATA_PATH + "DependencyResult.json");
            File.WriteAllText(resultPath, result);

            // Save IgnorePathData
            File.WriteAllText(SAVE_DATA_PATH + "AssetDependencyChecker_IgnorePathData.json", SlotSimpleJson.SerializeObject(ignorePathList));

            AssetDatabase.Refresh();
            var resultAsset = AssetDatabase.LoadAssetAtPath(resultPath, typeof(TextAsset));
            Selection.objects = new UnityEngine.Object[] { resultAsset };
        }

        private void CheckDependencyGUI()
        {
            BeginHorizontal();
            Label("Slot Name : <color=yellow>" + (m_selectedObject == null ? "None" : m_selectedObject.name) + "</color>");
            isFixed = GUILayout.Toggle(isFixed, "   Is Locked ");
            EndHorizontal();
            BeginHorizontal();
            Label("BundleName ");
            assetBundleNameText = TextField(assetBundleNameText);
            EndHorizontal();
            if (GUILayout.Button("Check Asset Bundle"))
            {
                if(m_selectedObject == null || isFixed == false)
                {
                    m_selectedObject = Selection.activeObject;
                    isFixed = true;
                }

                if(m_selectedObject != null)
                {
                    SearchObject(AssetDatabase.GetAssetPath(m_selectedObject));
                }
            }
        }

        private void CheckDependencyWithOutOfBoundsGUI()
        {
            BeginCheck();
            query = TextField(query);
            if (EndCheck())
            {
                Query();
            }

            BeginHorizontal();
                if (Button("Reset"))
                {
                    Reset();
                }
                if (Button("Check All"))
                {
                    var result = new Dictionary<string, object>();

                    foreach (ContentInfo info in contentInfos)
                    {
                        string path = string.Format(GAME_PATH, 1, info.gameTitleName);
                        result[info.gameTitleName] = CheckAssetBundle(path, info.gameTitle, 1, info.gameTitleName);
                    }

                    SaveData(SlotSimpleJson.SerializeObject(result));
                }
            EndHorizontal();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(contentsScrollPosition, GUILayout.Height(100f)))
            {
                contentsScrollPosition = scrollView.scrollPosition;

                for (int i = 0; i < queryResult.Count; ++i)
                {
                    BeginHorizontal();
                        if (Button(string.Format("{0} - {1}", queryResult[i].gameTitle, queryResult[i].gameTitleName), TextAnchor.MiddleLeft))
                        {
                            var result = new Dictionary<string, object>();

                            string path = string.Format(GAME_PATH, 1, queryResult[i].gameTitleName);
                            result[queryResult[i].gameTitleName] = CheckAssetBundle(path, queryResult[i].gameTitle, 1, queryResult[i].gameTitleName);

                            SaveData(SlotSimpleJson.SerializeObject(result));
                        }
                    EndHorizontal();
                }
            }

            Space();

            BeginHorizontal();
                if (Button("Add Ignore Path"))
                {
                    ignorePathList.Add("");
                }
            EndHorizontal();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(ignorePathScrollPosition))
            {
                ignorePathScrollPosition = scrollView.scrollPosition;
                for (int i = 0; i < ignorePathList.Count; ++i)
                {
                    BeginHorizontal();
                        ignorePathList[i] = TextField(ignorePathList[i]);
                        if (Button("X", GUILayout.Width(20f)))
                        {
                            ignorePathList.RemoveAt(i);
                        }
                    EndHorizontal();
                }
            }
        }

        private int tabIndex = 0;
        private string[] tabNames = {"Type 1", "Type 2"};

        private void OnGUI()
        {
            if (!initialized)
                Reset();

            tabIndex = GUILayout.Toolbar(tabIndex, tabNames);
            switch(tabIndex)
            {
                case 0:
                    CheckDependencyGUI();
                    break;
                case 1:
                    CheckDependencyWithOutOfBoundsGUI();
                    break;
            }
        }

        private UnityEngine.Object m_selectedObject = null;
        private string assetBundleNameText = "";
        private bool isFixed = false;

        private void ShowDependencies(string path)
        {
            string []dependencies = AssetDatabase.GetDependencies(path, true);
            for (int i = 0; i < dependencies.Length; ++i)
            {
                string bundleName = AssetImporter.GetAtPath(dependencies[i]).assetBundleName;
                if (!string.IsNullOrEmpty(bundleName) && assetBundleNameText != bundleName)
                {
                    Debug.Log("<color=red>["+bundleName+"]</color> "+dependencies[i]);
                    Debug.Log("<color=yellow> -> </color>" + path );
                }
            }
        }

        private void SearchObject(string sAssetFolderPath)
        {
            if (Directory.Exists(sAssetFolderPath))
            {
                string sDataPath = Application.dataPath;
                string sFolderPath  = sDataPath.Substring(0 ,sDataPath.Length-6)+sAssetFolderPath; // "/Users/mchoi/Documents/VCS/BagelCode/VPS3 Client/Assets", remove "Assets" path
                string[] aFilePaths = Directory.GetFiles(sFolderPath);
                foreach (string sFilePath in aFilePaths)
                {
                    string sAssetPath  = sFilePath.Substring(sDataPath.Length-6);
                    string []extension = sAssetPath.Split('.');

                    if (extension[extension.Length - 1].ToLower() == "meta" ||
                        extension[extension.Length - 1].ToLower() == "cs")
                        continue;

                    // to check UnityEngine.Object, if not, ignore.
                    UnityEngine.Object objAsset =  AssetDatabase.LoadAssetAtPath(sAssetPath,typeof(UnityEngine.Object));
                    if (objAsset != null)
                    {
                        ShowDependencies(AssetDatabase.GetAssetPath(objAsset));
                    }
                }

                string[] directories = Directory.GetDirectories(sFolderPath);
                for (int i = 0; i < directories.Length; ++i)
                {
                    string[] splits = directories[i].Split('/');
                    SearchObject(sAssetFolderPath+"/"+splits[splits.Length-1]);
                }
            }
            else
            {
                ShowDependencies(sAssetFolderPath);
            }
        }

        private Dictionary<string, object> CheckAssetBundle(string currentPath, string sourceBundleName, int groupId, string gameTitle)
        {
            var dic = new Dictionary<string, object>();

            string[] searchFolders = new string[]{ currentPath };
            var guidArray = AssetDatabase.FindAssets("t: Object", searchFolders);
            for (int i = 0; i < guidArray.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guidArray[i]);
                var dependencyList = CheckAssetDependency(assetPath, sourceBundleName, groupId, gameTitle);
                if (dependencyList != null)
                    dic[assetPath] = dependencyList;
            }

            return dic;
        }

        private List<string> CheckAssetDependency(string assetPath, string sourceBundleName, int groupId, string gameTitle)
        {
            List<string> dependencyList = new List<string>();

            var objectDependencies = AssetDatabase.GetDependencies(assetPath, true);
            for (int j = 0; j < objectDependencies.Length; ++j)
            {
                string objectDependencyPath = objectDependencies[j];
                string dependencyDetail = ShowDependencyDetail(objectDependencyPath, sourceBundleName, groupId, gameTitle);
                if (!string.IsNullOrEmpty(dependencyDetail))
                {
                    dependencyList.Add(dependencyDetail);
                }
            }

            if (dependencyList.Count > 0)
                return dependencyList;

            return null;
        }

        private string ShowDependencyDetail(string targetPath, string sourceBundleName, int groupId, string gameTitle)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(targetPath);
            if (assetImporter == null)
                return "";

            string targetBundleName = assetImporter.assetBundleName;
            if (string.IsNullOrEmpty(targetBundleName))
            {
                if (!NeedToCheckDependency(targetPath))
                    return "";

                string gamePath = string.Format(GAME_PATH, groupId, gameTitle);
                if (AssetOutOfBounds(targetPath, gamePath))
                    return targetPath;
            }
            else if (sourceBundleName != targetBundleName)
            {
                // Exception for xxxlang
                if (targetBundleName.Contains(sourceBundleName))
                    return "";

                // Assets in other games
                return targetPath;
            }
            return "";
        }

        private bool NeedToCheckDependency(string path)
        {
            string fileExtension = path.Split('.').Last();
            if (fileExtension.Equals("cs")     ||
                fileExtension.Equals("shader") ||
                fileExtension.Equals("json")   ||
                fileExtension.Equals("dll")    ||
                fileExtension.Equals("mat"))
                return false;

            return true;
        }

        private bool AssetOutOfBounds(string targetPath, string sourcePath)
        {
            if (targetPath.StartsWith(sourcePath))
                return false;

            for (int i = 0; i < ignorePathList.Count; ++i)
            {
                if (targetPath.StartsWith(ignorePathList[i]))
                    return false;
            }

            return true;
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
    }
}
#endif
