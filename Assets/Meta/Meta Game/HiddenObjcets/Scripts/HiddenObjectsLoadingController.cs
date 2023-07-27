using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsLoadingController : MonoBehaviour
    {
        // Settings
        private const float MAIN_LOADING_SPEED = 1.7f;
        private const float IN_GAME_LOADING_SPEED = 3f;
        //

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private string contextId;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetBool("Active", true);

            StartCoroutine(PlayLoadingSoundCoroutine());
        }

        private IEnumerator PlayLoadingSoundCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            bool isMetaInGame = bb.GetVariable<bool>("isMetaInGame")?.value ?? false;
            if (!isMetaInGame)
            {
                // Play Sound
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_START).Play();
            }
        }

        public IEnumerator LoadContentsBundleCoroutine()
        {
            // Update Scene
            // MetaGameSceneManager.OnLoadingScene(MetaGameType.HIDDEN_OBJECTS_MAIN);

            SetLogoTrigger("MetaGameLogo");

            contextId = bb.GetValue<string>("_biContextID");
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_start");

            // Enter Meta
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_META_GAME);

            // Bundle Load Progress
            bool isSuccess = false;
            bool isFail = false;
            var bundleList = new List<string>() { HiddenObjects.Defines.CONTENTS_BUNDLE };
            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "Hidden Objects Progress Bar Icon", CHILDREN);
            yield return StartCoroutine(MetaAssetBundleUtils.UpdateAssetBundleLoadingProgressCoroutine(
                bundleList, MAIN_LOADING_SPEED, contextId,
                root, progressBarElement, progressBarIconElement,
                () => isSuccess = true,
                () => isFail = true
                ));

            if (isSuccess)
            {
                // Send BI
                MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_end");

                EventSender.SendEvent(gameObject, "OnSuccess");
            }
            else if(isFail)
            {
                EventSender.SendEvent(gameObject, "OnFail");
            }
        }

        public IEnumerator UnloadContentsBundleCoroutine()
        {
            // Update Scene
            // MetaGameSceneManager.OnLoadingScene(MetaGameType.HIDDEN_OBJECTS_MAIN, false);

            SetLogoTrigger("LobbyLogo");

            // Unload Bundles
            AssetBundleManager.UnloadAssetBundle(HiddenObjects.Defines.CONTENTS_BUNDLE, false);
            var operation = Resources.UnloadUnusedAssets();
            yield return new WaitUntil(() => operation.isDone);

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator LoadStageSceneBundleCoroutine()
        {
            // Update Scene
            // MetaGameSceneManager.OnLoadingScene(MetaGameType.HIDDEN_OBJECTS_IN_GAME);

            SetLogoTrigger("InGameLogo");

            int chapter = bb.GetValue<int>("targetChapter");
            int stage = bb.GetValue<int>("targetStage");

            string bundle = HiddenObjects.Utils.GetStageBundleName(chapter, stage);

            // Bundle Load Progress
            bool isSuccess = false;
            bool isFail = false;
            var bundleList = new List<string>() { bundle };
            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "Hidden Objects Progress Bar Icon", CHILDREN);
            yield return StartCoroutine(MetaAssetBundleUtils.UpdateAssetBundleLoadingProgressCoroutine(
                bundleList, IN_GAME_LOADING_SPEED, contextId,
                root, progressBarElement, progressBarIconElement,
                () => isSuccess = true,
                () => isFail = true
                ));

            if (isSuccess)
            {
                EventSender.SendEvent(gameObject, "OnSuccess");
            }
            else if (isFail)
            {
                EventSender.SendEvent(gameObject, "OnFail");
            }
        }

        public IEnumerator UnloadStageSceneBundleCoroutine()
        {
            // Update Scene
            // MetaGameSceneManager.OnLoadingScene(MetaGameType.HIDDEN_OBJECTS_IN_GAME, false);

            SetLogoTrigger("MetaGameLogo");

            // Unload Bundles
            int chapter = bb.GetValue<int>("currentChapter");
            int stage = bb.GetValue<int>("currentStage");
            string bundle = HiddenObjects.Utils.GetStageBundleName(chapter, stage);

            AssetBundleManager.UnloadAssetBundle(bundle, false);
            var operation = Resources.UnloadUnusedAssets();
            yield return new WaitUntil(() => operation.isDone);

            // Update Progress Bar
            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "Hidden Objects Progress Bar Icon", CHILDREN);
            progressBarElement.GetComponent<Slider>().handleRect = progressBarIconElement.GetComponent<RectTransform>();
            var progressProperty = progressBarElement as IContextFloatProperty;

            float DURATION = 0.7f;
            yield return root.StartCoroutine(AsyncActionUtils.ProgressiveActionCoroutine(DURATION, null,
                (float t) => progressProperty.SetFloatProperty(t), null));

            yield return new WaitForSeconds(0.3f);
        }

        public IEnumerator LeaveCoroutine()
        {
            // Update Scene
            // MetaGameSceneManager.OnLeaveScene(MetaGameType.HIDDEN_OBJECTS_MAIN);
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "OnLeaveMetaGame");

            if (BlackboardQueryUtils.IsIngame())
            {
                var prevOrientation = BlackboardUtils.GetOrCreateVariable<Orientation>(MainBlackboard.Get(), "prevOrientation")?.value ?? Orientation.LANDSCAPE;
                var orientation = BlackboardQueryUtils.GetOrientation();
                if (prevOrientation != orientation)
                {
                    MetaGameUtils.SetOrientation(prevOrientation, gameObject);

                    var callbackTrigger = new EventTrigger(gameObject, "OnFinishedChangeOrientation");
                    yield return new WaitUntilTrigger(callbackTrigger);
                }

                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "ReturnToInGame");
                BlackboardQueryUtils.SetMetaGameSceneState(SceneState.INGAME);
            }
            else
            {
                BlackboardQueryUtils.SetMetaGameSceneState(SceneState.LOBBY);
            }

            Close();
        }

        public void LeaveToMainScene()
        {
            // Update Scene
            // MetaGameSceneManager.OnLeaveScene(MetaGameType.HIDDEN_OBJECTS_IN_GAME);
            EventSender.SendCalleeCallback(gameObject);
            Close();
        }

        public IEnumerator MakeMainSceneCoroutine()
        {
            // Make HOG Scene
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Hidden Objects Main Scene";
            Transform parent = MetaObjectUtils.MainCanvasAreaTransform;

            GameObject metaGameScene = null;
            StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject sceneObj) => metaGameScene = sceneObj));

            // Enter Request
            bool success = false;
            bool fail = false;
            Debug.Log("start [RequestHiddenObjectsEnter]");
            BagelCodeClientAPI.RequestHiddenObjectsEnter(
                (response) =>
                {
                    success = true;
                    Debug.Log("succeed [RequestHiddenObjectsEnter]");

                    var bb = HiddenObjects.Utils.HiddenObjectsInfo;
                    ClientAPI2Blackboard.Serialize(bb, response);

                    HiddenObjects.Utils.UpdateFinderCount(response.finder, response.maxFinder);
                },
                (error) =>
                {
                    fail = true;
                    Debug.LogError(error.errorCode);
                    // HIDDEN_UNIVERSE_IS_NOT_ACTIVE_ERROR
                    GlobalErrorHandler.GlobalError(error);
                });

            yield return new WaitUntil(() => (success || fail) && metaGameScene != null);

            if (fail)
            {
                Close();
                yield break;
            }

            var mainSceneBB = metaGameScene.GetComponent<Blackboard>();
            string metaGroupContextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "metaGroupContextId")?.value;
            if(!string.IsNullOrEmpty(metaGroupContextId))
            {
                BlackboardUtils.SetOrCreateValue(mainSceneBB, "metaGroupContextId", metaGroupContextId);
            }

            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "finish");

            metaGameScene.SetActive(true);

            // Update Scene
            BlackboardQueryUtils.MetaGameCrashReport(HiddenObjects.Utils.EVENT_NAME);
            BlackboardQueryUtils.SetMetaGameSceneState(SceneState.EVENT_META_GAME);
            // MetaGameSceneManager.OnEnterScene(MetaGameType.HIDDEN_OBJECTS_MAIN);

            // Finish
            Close();
        }

        public IEnumerator MakeStageSceneCoroutine()
        {
            int chapter = bb.GetValue<int>("targetChapter");
            int stage = bb.GetValue<int>("targetStage");

            bool playAgain = bb.GetValue<bool>("playAgain");

            // Make Stage
            string bundle = HiddenObjects.Utils.GetStageBundleName(chapter, stage);
            string asset = HiddenObjects.Utils.GetStageAssetName(chapter, stage);

            if(string.IsNullOrEmpty(bundle) || string.IsNullOrEmpty(asset))
            {
                Debug.LogError(string.Format("HiddenObjectsLoadingController.MakeStageSceneCoroutine failure.\n" +
                    "Bundle loading failure. chapter: {0}, stage: {1}\n" +
                    "Check \"HiddenObjectsChapterData\" asset.", chapter, stage));

                Close();
                yield break;
            }

            Transform mainArea = MetaObjectUtils.MainCanvasAreaTransform;

            GameObject stageObj = null;
            StartCoroutine(MetaObjectUtils.MakePrefabCoroutine(bundle, asset, mainArea,
                (GameObject sceneObj) =>
                {
                    stageObj = sceneObj;
                    stageObj.SetActive(false);
                }));

            // Request Play
            bool success = false;
            bool fail = false;
            string type = playAgain ? "replay" : "default";
            HiddenUniversePlayEnterResponse response = null;
            BagelCodeClientAPI.RequestHiddenObjectsPlay(chapter, stage, type,
                (_response) =>
                {
                    success = true;
                    response = _response;
                },
                (error) =>
                {
                    fail = true;
                    Debug.LogError("HiddenObjectsLoadingController.MakeStageSceneCoroutine failure.");
                    // HIDDEN_UNIVERSE_IS_NOT_ACTIVE_ERROR
                    // HIDDEN_UNIVERSE_CHAPTER_NOT_GlOBAL_UNLOCK_ERROR
                    // HIDDEN_UNIVERSE_CHAPTER_NOT_UNLOCK_ERROR
                    // HIDDEN_UNIVERSE_STAGE_NOT_UNLOCK_ERROR
                    Debug.LogError(error.errorCode);
                    GlobalErrorHandler.GlobalError(error);
                });

            // Make In Game Scene
            bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            asset = "Hidden Objects In Game Scene";

            GameObject inGameObj = null;
            StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, mainArea,
                (GameObject sceneObj) => inGameObj = sceneObj));

            // Wait
            yield return new WaitUntil(() => (success || fail) && stageObj != null && inGameObj != null);

            if (fail)
            {
                Destroy(stageObj);
                Destroy(inGameObj);
                Close();
                yield break;
            }

            // Set Stage Parent
            var inGameElement = inGameObj.GetComponent<ContextElement>();
            inGameElement.UpdateContext(true);
            var stageAreaElement = ContextUtils.FindElement(inGameElement, "Stage Area", CHILDREN);
            stageObj.transform.parent = stageAreaElement.transform;

            // Set Stage Value
            var stageInfo = bb.GetValue<HiddenUniverseStageInfo>("stageInfo");
            var inGameBB = inGameObj.GetComponent<Blackboard>();
            int chapterTotalStarCount = bb.GetValue<int>("chapterTotalStarCount");
            int stageStarCount = bb.GetValue<int>("stageStarCount");
            BlackboardUtils.SetOrCreateValue(inGameBB, "chapter", chapter);
            BlackboardUtils.SetOrCreateValue(inGameBB, "stage", stage);
            BlackboardUtils.SetOrCreateValue(inGameBB, "needFinderCount", stageInfo.needFinderCount);
            BlackboardUtils.SetOrCreateValue(inGameBB, "prevStageInfo", stageInfo);
            BlackboardUtils.SetOrCreateValue(inGameBB, "playAgain", playAgain);
            BlackboardUtils.SetOrCreateValue(inGameBB, "chapterTotalStarCount", chapterTotalStarCount);
            BlackboardUtils.SetOrCreateValue(inGameBB, "stageStarCount", stageStarCount);
            BlackboardUtils.SetOrCreateValue(inGameBB, "stageObject", stageObj);
            ClientAPI2Blackboard.Serialize(inGameBB, response);

            // Set Caller
            var caller = bb.GetValue<GameObject>("caller"); // main scene
            MetaObjectUtils.SetCalleeCaller(stageObj, caller);
            MetaObjectUtils.SetCalleeCaller(inGameObj, caller);

            stageObj.SetActive(true);
            inGameObj.SetActive(true);

            // Update Scene
            // MetaGameSceneManager.OnEnterScene(MetaGameType.HIDDEN_OBJECTS_IN_GAME);

            // Finish
            Close();
        }

        private void SetLogoTrigger(string key)
        {
            anim.SetBool("LobbyLogo", key == "LobbyLogo");
            anim.SetBool("InGameLogo", key == "InGameLogo");
            anim.SetBool("MetaGameLogo", key == "MetaGameLogo");
        }

        private void Close()
        {
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
