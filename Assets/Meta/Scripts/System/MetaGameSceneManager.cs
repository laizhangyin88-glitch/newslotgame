using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using UnityEngine.CrashReportHandler;

namespace BagelCode
{
    public class MetaGameSceneManager : MonoSingleton<MetaGameSceneManager>
    {
        public List<MetaGameType> sceneStack = new List<MetaGameType>();
        public string currentScene = "";

        private static string sceneStackString = "";

        private const string LOADING = "LOADING_";
        private const string UNLOADING = "UNLOADING_";

        private const string META_DATA_NAME_META_CURRENT_SCENE = "Meta.currentScene";
        private const string META_DATA_NAME_META_SCENE_STACK = "Meta.sceneStack";

        public static void OnEnterScene(MetaGameType type)
        {
            Instance.currentScene = type.ToString();
            Instance.sceneStack.Add(type);
            OnUpdateScene();

            if (ApplicationSettings.LogTest())
            {
                Debug.Log("Enter " + Instance.currentScene);
                Debug.Log("Scene Stack: " + sceneStackString);
            }
        }

        public static void OnLoadingScene(MetaGameType type, bool isLoading = true, int progress = 0, int progressMax = 0)
        {
            string progressString = progressMax > 0 ? string.Format(" ({0}/{1})", progress, progressMax) : "";
            string loadingUnloadingString = isLoading ? LOADING : UNLOADING;
            Instance.currentScene = loadingUnloadingString + type.ToString() + progressString;
            OnUpdateScene();

            if (ApplicationSettings.LogTest())
            {
                Debug.Log("Loading " + Instance.currentScene);
                Debug.Log("Scene Stack: " + sceneStackString);
            }
        }

        public static void OnLeaveScene(MetaGameType type)
        {
            bool exist = Instance.sceneStack.Remove(type);
            if (!exist)
            {
                Debug.LogWarning("MetaGameSceneManager.OnLeaveScene failure. " + type + " is not stacked.");
                return;
            }

            OnUpdateScene();

            if (ApplicationSettings.LogTest())
            {
                Debug.Log("Leave " + type);
                Debug.Log("Scene Stack: " + sceneStackString);
            }
        }

        private static void OnUpdateScene()
        {
            sceneStackString = string.Join("/", Instance.sceneStack);

            CrashReportHandler.SetUserMetadata(META_DATA_NAME_META_CURRENT_SCENE, Instance.currentScene);
            CrashReportHandler.SetUserMetadata(META_DATA_NAME_META_SCENE_STACK, sceneStackString);
        }
    }
}
