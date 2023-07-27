using System;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.Json;
using SlotMaker.TestSuite;
using Sirenix.OdinInspector;


namespace SlotMaker.Contents
{
    public class ContentsVersionManager : MonoSingleton<ContentsVersionManager>
    {
        private Dictionary<string, int> versionCache = new Dictionary<string, int>();
        private int currentVersion = -1;

        [ShowInInspector, ReadOnly]
        private string currentGameTitle = "";

        public int GetGameVersion(string gameTitle)
        {
            currentGameTitle = gameTitle;
            return GetCurrentGameVersion();
        }

        public int GetCurrentGameVersion()
        {
            if (string.IsNullOrEmpty(currentGameTitle))
            {
                Debug.LogError($"[ContentsVersionManager] current game title is invalid: '{currentGameTitle}'");
                return -1;
            }

            int version;
            if (versionCache.TryGetValue(currentGameTitle, out version))
                return version;

            var gameInfo = LoadGameInfo(currentGameTitle);

            // if there is no valid GameInfo, then return the lowest version ("0.0.1") as default
            version = (gameInfo != null) ? GetNumericVersion(gameInfo.version) : 1;

            // Cache the version so that the gameInfo for this game would not be loaded twice
            versionCache[currentGameTitle] = version;
            return version;
        }

        // TODO: Extract this to something like GameManifestUtils, if there is another code that should load gameInfo.json
        private GameManifest LoadGameInfo(string gameTitle)
        {
            var rawGameInfo = AssetBundleManager.LoadAsset<TextAsset>(gameTitle, "gameInfo");
            if (rawGameInfo == null)
            {
                Debug.LogWarning($"[GameManifest] gameInfo.json in '{gameTitle}' assetbundle is not found.");
                return null;
            }

            GameManifest gameInfo;
            try
            {
                gameInfo = SlotSimpleJson.DeserializeObject<GameManifest>(rawGameInfo.text);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameManifest] gameInfo.json of '{gameTitle}' is unable to be deserialized: ${e.Message}");
                return null;
            }
            return gameInfo;
        }

        public int GetContentsVersion()
        {
            if (currentVersion > 0)
                return currentVersion;

            var contentsInfo = LoadContentsInfo();
            currentVersion = (contentsInfo != null) ? GetNumericVersion(contentsInfo.version) : 1;

            return currentVersion;
        }

        private Manifest LoadContentsInfo()
        {
            var rawContentsInfo = AssetBundleManager.LoadAsset<TextAsset>("contents", "contentsInfo");
            if (rawContentsInfo == null)
            {
                Debug.LogWarning($"[Manifest] contentsInfo.json in 'contents' assetbundle is not found.");
                return null;
            }

            Manifest contentsInfo;
            try
            {
                contentsInfo = SlotSimpleJson.DeserializeObject<Manifest>(rawContentsInfo.text);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Manifest] contentsInfo.json of 'contents' is unable to be deserialized: ${e.Message}");
                return null;
            }
            return contentsInfo;
        }

        private static readonly char[] versionSeparator = new char[] { '.', '+', '-' };
        private int GetNumericVersion(string versionString)
        {
            // If there are 'postfix'es in versionString, it would be stored in versionParts[3] and it would be ignored.
            var versionParts = versionString.Split(versionSeparator, 4);

            int majorVersion = Int32.Parse(versionParts[0]);
            int minorVersion = Int32.Parse(versionParts[1]);
            int patchVersion = Int32.Parse(versionParts[2]);

            return majorVersion * 10000 + minorVersion * 100 + patchVersion;
        }
    }
}
