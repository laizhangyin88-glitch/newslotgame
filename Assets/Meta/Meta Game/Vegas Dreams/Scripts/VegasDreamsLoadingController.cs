using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsLoadingController : MonoBehaviour
    {
        // Settings
        private const float MAIN_LOADING_SPEED = 1.7f;
        private const float IN_GAME_LOADING_SPEED = 3f;

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

        // TODO : Setting Sound
        private IEnumerator PlayLoadingSoundCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            GSManager.Instance.GetHandler(VegasDreams.Defines.VEGAS_DREAMS_LOADING).Play();
        }

        public IEnumerator LoadContentsBundleCoroutine()
        {
            // Update Scene

            SetLogoTrigger("MetaGameLogo");

            contextId = bb.GetValue<string>("_biContextID");
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_start");

            // Enter Meta
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "OnEnterMetaGame");

            // Bundle Load Progress
            bool isSuccess = false;
            bool isFail = false;
            
            var bundles = new List<string>() { VegasDreams.Defines.CONTENTS_BUNDLE, VegasDreams.Utils.GetContentsSeasonBundle(), VegasDreams.Utils.GetObjectSeasonBundle() };

            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar UV", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "Vegas Dreams Progress Bar Icon", CHILDREN);
            yield return StartCoroutine(MetaAssetBundleUtils.UpdateAssetBundleLoadingProgressCoroutine(
                bundles, MAIN_LOADING_SPEED, contextId,
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

            SetLogoTrigger("LobbyLogo");

            // Unload Bundles
            var bundles = new List<string>() { VegasDreams.Defines.CONTENTS_BUNDLE, VegasDreams.Utils.GetContentsSeasonBundle(), VegasDreams.Utils.GetObjectSeasonBundle() };
            foreach (var bundle in bundles)
            {
                AssetBundleManager.UnloadAssetBundle(bundle, false);
                var operation = Resources.UnloadUnusedAssets();
                yield return new WaitUntil(() => operation.isDone);
            }

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator CheckLeaveToOrientation()
        {
            if (!(BlackboardUtils.FindVariable<bool>(bb, "isMetaInGame")?.value ?? false))
            {
                var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
                if (prevOrientation != null)
                {
                    MetaGameUtils.SetOrientation(prevOrientation.value, gameObject);

                    var callbackTrigger = new EventTrigger(gameObject, "OnFinishedChangeOrientation");
                    yield return new WaitUntilTrigger(callbackTrigger);
                }
            }
        }

        public void LeaveToLobby()
        {
            // Update Scene
            if (BlackboardUtils.FindVariable<bool>(bb, "isMetaInGame")?.value ?? false)
            {
                EventSender.SendGlobalEvent("OnReturnToMetaGame");
            }
            else
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "OnLeaveMetaGame");

                if (BlackboardQueryUtils.IsIngame())
                {
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "ReturnToInGame");
                    BlackboardQueryUtils.SetMetaGameSceneState(SceneState.INGAME);
                }
                else
                {
                    BlackboardQueryUtils.SetMetaGameSceneState(SceneState.LOBBY);
                }

                if (BlackboardQueryUtils.IsVipLoungeEnabled())
                {
                    var eventData = new ParadoxNotion.EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, null);
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                }
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
            string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "_biContextID")?.value;
            var isMetaInGame = bb.GetVariable<bool>("isMetaInGame")?.value ?? false;

            BiEventUtils.SendBiEventEnter(isMetaInGame ? "vip_lounge" : "meta_button", "build_dream", contextId);

            string bundle = VegasDreams.Utils.GetContentsSeasonBundle();
            string asset = $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Scene";
            Transform parent = MetaObjectUtils.MainCanvasAreaTransform;

            GameObject metaGameScene = null;
            BuildDreamEnterResponse enterResponse = null;
            StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject sceneObj) => metaGameScene = sceneObj));

            // // Enter Request
            bool success = false;
            bool fail = false;

            Debug.Log("start [RequestVegasDreamsEnter]");
            BagelCodeClientAPI.RequestVegasDreamEnter(
                (response) =>
                {
                    success = true;
                    Debug.Log("succeed [RequestVegasDreamsEnter]");
                    enterResponse = response;
                },
                (error) =>
                {
                    fail = true;
                    Debug.LogError(error.errorCode);
                    GlobalErrorHandler.GlobalError(error);
                });

            yield return new WaitUntil(() => (success || fail) && metaGameScene != null);

            if (fail)
            {
                Close();
                yield break;
            }

            
            var mainSceneBB = metaGameScene.GetComponent<Blackboard>();
            ClientAPI2Blackboard.Serialize(mainSceneBB, enterResponse);
            BlackboardQueryUtils.UpdateVegasDreamsInfo(enterResponse.buildDreamInfo);
            var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
            if (prevOrientation != null)
                BlackboardUtils.SetOrCreateValue(mainSceneBB, "prevOrientation", prevOrientation.value);

            BlackboardUtils.SetOrCreateValue(mainSceneBB, "contextID", contextID);
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "isMetaInGame", isMetaInGame);

            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "finish");

            metaGameScene.SetActive(true);

            // Update Scene
            BlackboardQueryUtils.MetaGameCrashReport(VegasDreams.Utils.EVENT_NAME);
            BlackboardQueryUtils.SetMetaGameSceneState(SceneState.EVENT_META_GAME);
            // Finish
            Close();
        }

        private void SetLogoTrigger(string key)
        {
            // anim.SetBool("LobbyLogo", key == "LobbyLogo");
            // anim.SetBool("InGameLogo", key == "InGameLogo");
            anim.SetBool("MetaGameLogo", key == "MetaGameLogo");
        }

        private void Close()
        {
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
