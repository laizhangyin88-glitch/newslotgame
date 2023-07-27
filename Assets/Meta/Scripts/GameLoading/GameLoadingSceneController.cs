using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections.Generic;

namespace BagelCode
{
    public class GameLoadingSceneController : MonoBehaviour
    {
        private Blackboard mBlackboard => GetComponent<Blackboard>();

        private int enterGameId;

        public void AddMetaGameDownloadList()
        {
            var downloadList = BlackboardUtils.GetOrCreateVariable<List<string>>(mBlackboard, "downloadList")?.value;

            // Hog
            if (BlackboardQueryUtils.IsHiddenObjectsActive())
                downloadList.Add("mghiddenobjectscommon");

            // Vip
            if(BlackboardQueryUtils.IsVipLoungeActive())
                downloadList.Add("mgviploungecommon");
        }

        public Orientation GetOrientation()
        {
            return BlackboardUtils.FindValue<Orientation>("/enterGameInfo/orientation");
        }

        /// <summary>
        /// Create "In Game Scene" in Main Canvas/Area.
        /// </summary>
        /// <param name="gameType">About Loading Scene</param>
        /// <param name="orientation">Game Orientation Info</param>
        /// <param name="saveAs">Save As Blackboard Variable</param>

        public IEnumerator LoadTutorialSceneAsync(Orientation orientation)
        {
            bool isMobile = Common.CheckPlatform(TargetPlatform.Android | TargetPlatform.IOS);
            bool isLandscape = orientation == Orientation.LANDSCAPE || !isMobile;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isLandscape ? "In Game Tutorial Scene" : "In Game Tutorial Portrait Scene";
            Transform parent = GameObject.Find("Anchor/Right Event").transform;

            GameObject tutorialObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject obj) => tutorialObj = obj));

            var bb = GetComponent<Blackboard>();
            var gameSceneObj = bb.GetVariable<GameObject>("_gameScene")?.value;
            MetaObjectUtils.SetCalleeCaller(tutorialObj, gameSceneObj);

            tutorialObj.SetActive(true);
        }
        public IEnumerator LoadInGameSceneAsync(GameType gameType, Orientation orientation, string saveAs)
        {
            // Game Type
            switch (gameType)
            {
                case GameType.SLOT_MACHINE:
                    {
                        if (orientation == Orientation.LANDSCAPE)
                        {
                            yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                        else if (orientation == Orientation.PORTRAIT)
                        {
                            if(OrientationUtils.Instance.PossibleChangeOrientation())
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Vertical Mode Scene Slot", "Main Canvas/Area", saveAs);
                            else
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME,"In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                    }
                    break;
                case GameType.VIDEO_POKER:
                    {
                        yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Video Poker", "Main Canvas/Area", saveAs);
                    }
                    break;
                case GameType.KENO:
                    {
                        yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                    }
                    break;
                default:
                    {
                        // throw exception.

                        if (orientation == Orientation.LANDSCAPE)
                        {
                            yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                        else if (orientation == Orientation.PORTRAIT)
                        {
                            if(OrientationUtils.Instance.PossibleChangeOrientation())
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Vertical Mode Scene Slot", "Main Canvas/Area", saveAs);
                            else
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME,"In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Create "Game Contents Scene" in Game Canvas.
        /// </summary>
        /// <param name="orientation">Game Orientation Info</param>
        /// <param name="saveAs">Save As Blackboard Variable</param>
        public IEnumerator LoadContentSceneAsync(Orientation orientation, string saveAs)
        {
            string gameBundleName = mBlackboard.GetValue<string>("_gameTitle");
            if (orientation == Orientation.LANDSCAPE)
            {
                yield return LoadSceneAsync(gameBundleName, "Game Contents Scene", "Game Canvas", saveAs);
            }
            else if (orientation == Orientation.PORTRAIT)
            {
                // #if UNITY_WEBGL || UNITY_WSA// || UNITY_EDITOR
                //                 MetaObjectUtils.MakePrefab(BUNDLE_NAME, "Portrait to Landscape Anchor Content", null, "Game Canvas", "Game Contents");
                //                 yield return LoadSceneAsync(gameBundleName,"Game Contents Scene", "Game Canvas/Game Contents", saveAs);
                // #else
                yield return LoadSceneAsync(gameBundleName, "Game Contents Scene", "Game Canvas", saveAs);
                // #endif
            }
            else
            {
                //Orientation.Unknwon Fail
            }
        }

        private IEnumerator LoadSceneAsync(string bundleName, string assetName, string root, string saveAs)
        {
            var loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(bundleName, assetName);
            yield return new WaitUntil(() => loadSceneInfoOperation.IsDone());
            var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();

            var sceneLoadOperation = SceneManager.LoadSceneAsync(GameObject.Find(root).transform, sceneInfo, true);
            yield return new WaitUntil(() => sceneLoadOperation.IsDone());
            if (!string.IsNullOrEmpty(saveAs))
            {
                mBlackboard.AddVariable(saveAs, sceneLoadOperation.GetScene());
            }
        }

        private IEnumerator CreateVecticalObjAsync(string assetName, string _name, string parentName)
        {
            var loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName);
            yield return new WaitUntil(() => loadSceneInfoOperation.IsDone());
            var verticalPrefab = loadSceneInfoOperation.GetAsset<GameObject>();

            var mainCanvas = GameObject.Find(parentName);
            var verticalObj = Instantiate(verticalPrefab, mainCanvas.transform);
            verticalObj.name = _name;
        }
    }
}
