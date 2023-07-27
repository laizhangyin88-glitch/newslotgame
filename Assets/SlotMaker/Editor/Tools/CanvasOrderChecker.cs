#if DEV
using System.Diagnostics;
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
using System.Linq;
using UnityEngine.UI;
using System.Reflection.Emit;

namespace SlotMaker.Tools
{
    public class CanvasOrderChecker : EditorWindowBase<CanvasOrderChecker>
    {
        private bool initialized = false;
        private List<ContentInfo> contentInfos;
        private string query = "";
        private List<ContentInfo> queryResult;
        private Vector2 scrollPosition;
        private int toolbarIndex;
        private int checkMask;

        int tabIndex;
        private string[] toolbarStrings;
        private string[] functionToolBarStrings = { "Execute", "Setting" };

        int currentMask;
        public override string GetEditorName()
        {
            return "Canvas Order Checker";
        }

        [MenuItem("SlotMaker/Tools/Canvas Order Checker", false, 1000)]
        private static void Initialize()
        {
            CreateWindow();
        }

        private void OnGUI()
        {
            if (!initialized)
                Refresh();

            tabIndex = GUILayout.Toolbar(tabIndex, functionToolBarStrings);
            switch (tabIndex)
            {
                case 0:
                    ExecuteGUI();
                    break;
                case 1:
                    SettingGUI();
                    break;
            }
        }

        private void ExecuteGUI()
        {

            if (Button("Check Canvas All Game", TextAnchor.MiddleCenter))
            {

            }
            if (Button("Check Order All Game", TextAnchor.MiddleCenter))
            {

            }
            EditorGUILayout.Space(5);
            BeginCheck();
            query = TextField(query);
            if (EndCheck())
            {
                Query();
            }
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;

                for (int i = 0; i < queryResult.Count; ++i)
                {
                    BeginHorizontal();
                    EditorGUILayout.LabelField($"{queryResult[i].gameTitle} - {queryResult[i].gameTitleName}");
                    if (Button("Order", TextAnchor.MiddleCenter))
                    {
                        ContentInfo info = queryResult[i];
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(GetAssetDirectoryPath(info.gameTitle, "Game Contents"));
                        UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

                        foreach (var each in selectedAsset)
                        {
                            if (each as GameObject != null)
                                CheckCanvasOrder(each as GameObject);
                        }

                    }
                    if (Button("Canvas", TextAnchor.MiddleCenter))
                    {
                        ContentInfo info = queryResult[i];
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(GetAssetDirectoryPath(info.gameTitle, "Game Contents") + "/Game Contents", typeof(GameObject));

                    }
                    EndHorizontal();
                }
            }
        }
        private void SettingGUI()
        {
            string[] displayString = SortingLayer.layers.ToList().Select((sortingLayer) => sortingLayer.name).ToArray();
            checkMask = EditorGUILayout.MaskField("Checking Order Mask", checkMask, displayString);
        }

        private void CheckCanvasOrder(GameObject prefab)
        {
            List<Canvas> listCanvas = prefab.GetComponentsInChildren<Canvas>(true).ToList();
            List<SpriteRenderer> listSpriteRenderer = prefab.GetComponentsInChildren<SpriteRenderer>(true).ToList();
            List<ParticleSystemRenderer> listParticleRenderer = prefab.GetComponentsInChildren<ParticleSystemRenderer>(true).ToList();
            List<SortingLayer> listSortingLayer = SortingLayer.layers.ToList();

            foreach (var each in listCanvas)
            {
                int index = listSortingLayer.FindIndex(0, listSortingLayer.Count, (sortingLayer) => sortingLayer.id == each.sortingLayerID);
                if ((checkMask & (1 << index)) == (1 << index))
                    UnityEngine.Debug.LogError($"Unproper Layer Dectected : {each.sortingLayerName}\nFolder Path : {AssetDatabase.GetAssetPath(prefab)}\nObject Path : {GetPathFromParent(prefab, each.gameObject)}");
            }
            foreach (var each in listSpriteRenderer)
            {
                int index = listSortingLayer.FindIndex(0, listSortingLayer.Count, (sortingLayer) => sortingLayer.id == each.sortingLayerID);
                if ((checkMask & (1 << index)) == (1 << index))
                    UnityEngine.Debug.LogError($"Unproper Layer Dectected : {each.sortingLayerName}\nFolder Path : {AssetDatabase.GetAssetPath(prefab)}\nObject Path : {GetPathFromParent(prefab, each.gameObject)}");
            }
            foreach (var each in listParticleRenderer)
            {
                int index = listSortingLayer.FindIndex(0, listSortingLayer.Count, (sortingLayer) => sortingLayer.id == each.sortingLayerID);
                if ((checkMask & (1 << index)) == (1 << index))
                    UnityEngine.Debug.LogError($"Unproper Layer Dectected : {each.sortingLayerName}\nFolder Path : {AssetDatabase.GetAssetPath(prefab)}\nObject Path : {GetPathFromParent(prefab, each.gameObject)}");
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


        string GetPathFromParent(GameObject Parent, GameObject Child)
        {
            string strPath = "";

            Transform iterator = Child.transform;
            while (true)
            {
                strPath = strPath.Insert(0, iterator == Child.transform ? iterator.gameObject.name : iterator.gameObject.name + "/");

                if (iterator == Parent.transform)
                    break;
                else
                    iterator = iterator.parent;
            }


            return strPath;
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
