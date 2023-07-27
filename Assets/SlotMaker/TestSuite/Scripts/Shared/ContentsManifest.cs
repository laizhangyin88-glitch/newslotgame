using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using SlotMaker.Json;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker.TestSuite
{
    [CreateAssetMenu(fileName="contents-manifest", menuName="SlotMaker2/TestSuite/Contents Manifest")]
    public class ContentsManifest : ScriptableObject
    {
        public string version;
        public string slotMakerVersion;
        public List<TextAsset> contents;

        public List<GameManifest> GameInfos { get; protected set; }

        private void OnEnable()
        {
            UpdateGameInfos();
        }

        private static ContentsManifest instance;

        public static ContentsManifest Instance
        {
            get
            {
                if (instance == null)
                {
                    // TODO: Will be support multi contents repositories
                    instance = (ContentsManifest)AssetBundleManager.LoadAsset<ContentsManifest>("testsuite", "contents-manifest 1");
                }

                return instance;
            }
        }

        public static void UpdateGameInfos()
        {
            Instance.GameInfos = new List<GameManifest>();
            foreach (var textAsset in Instance.contents)
            {
                var json = textAsset.text;
                var gameInfo = SlotSimpleJson.DeserializeObject<GameManifest>(json);
                gameInfo.ParseVersion();
                Instance.GameInfos.Add(gameInfo);
            }
            Instance.GameInfos.Sort(GameManifest.Sort);
        }

        // Utils for legacy systems
        public static List<ContentInfo> GetContentInfos()
        {
            var contentInfos = new List<ContentInfo>();
            foreach (var gameInfo in Instance.GameInfos)
            {
                var contentInfo = new ContentInfo()
                {
                    gameId = gameInfo.gameId,
                    gameTitle = gameInfo.name,
                    gameTitleName = gameInfo.displayName,
                    version = gameInfo.GetPublishMajorVersion()
                };
                contentInfos.Add(contentInfo);
            }

            return contentInfos;
        }

#if UNITY_EDITOR
        public static Manifest GetSlotMakerManifest()
        {
            var json = ((TextAsset) AssetDatabase.LoadAssetAtPath("Assets/SlotMaker/version.json", typeof(TextAsset))).text;
            return SlotSimpleJson.DeserializeObject<Manifest>(json);
        }

        public static string GetContentsVersion()
        {
            var paths = AssetDatabase.FindAssets("l:testsuite contents-manifest").Select(x => AssetDatabase.GUIDToAssetPath(x))
                .ToArray();
            string lastVersion = "0.1.0";
            foreach (var path in paths)
            {
                var manifest = (ContentsManifest) AssetDatabase.LoadAssetAtPath(path, typeof(ContentsManifest));
                if (Manifest.CompareSementicVersion(manifest.version, lastVersion) > 0)
                    lastVersion = manifest.version;
            }

            return lastVersion;
        }

        public static void RefreshAll()
        {
            var paths = AssetDatabase.FindAssets("l:testsuite contents-manifest").Select(x => AssetDatabase.GUIDToAssetPath(x))
                .ToArray();
            foreach (var path in paths)
            {
                var manifest = (ContentsManifest) AssetDatabase.LoadAssetAtPath(path, typeof(ContentsManifest));
                manifest.Refresh();
            }
        }

        private static List<TextAsset> FindGameInfoAssets(string rootPath)
        {
            var paths = AssetDatabase.FindAssets(
                "l:testsuite gameInfo", new string[]{ rootPath }).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            var ret = new List<TextAsset>();
            foreach (var path in paths)
            {
                ret.Add((TextAsset)AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)));
            }
            return ret;
        }

        [Button]
        [PropertyOrder(-1)]
        public void Refresh()
        {
            slotMakerVersion = GetSlotMakerManifest().version;

            var rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            contents = FindGameInfoAssets(rootPath);
            GameManifest lastManifest = null;
            foreach (var content in contents)
            {
                var manifest = SlotSimpleJson.DeserializeObject<GameManifest>(content.text);
                manifest.ParseVersion();
                if (lastManifest == null || (GameManifest.Sort(manifest, lastManifest) > 0))
                    lastManifest = manifest;
            }

            if (lastManifest != null)
                version = lastManifest.publishVersion;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateGameInfos();
        }
#endif
    }
}
