using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode.VipLounge
{
    public class VipLoungeLoadingController : MonoBehaviour
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

        // TODO : Setting Sound
        private IEnumerator PlayLoadingSoundCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            bool isMetaInGame = bb.GetValue<bool>("isMetaInGame");
            if (!isMetaInGame)
            {
                // Play Sound
                GSManager.Instance.GetHandler(VipLounge.Defines.VIP_LOUNGE_LOADING).Play();
            }
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
            var bundleList = new List<string>() { VipLounge.Defines.CONTENTS_BUNDLE };
            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "VIP Epic Lounge Progress Bar Icon", CHILDREN);
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

            SetLogoTrigger("LobbyLogo");
            SetAnimtionCloseCurtain();

            // Unload Bundles
            AssetBundleManager.UnloadAssetBundle(VipLounge.Defines.CONTENTS_BUNDLE, false);
            var operation = Resources.UnloadUnusedAssets();
            yield return new WaitUntil(() => operation.isDone);

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator CheckLeaveToOrientation()
        {
            var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
            if (prevOrientation != null)
            {
                MetaGameUtils.SetOrientation(prevOrientation.value, gameObject);

                var callbackTrigger = new EventTrigger(gameObject, "OnFinishedChangeOrientation");
                yield return new WaitUntilTrigger(callbackTrigger);
            }
        }

        public void LeaveToLobby()
        {
            // Update Scene
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

            GSManager.Instance.GetHandler(VipLounge.Defines.VIP_LOUNGE_MAIN_BGM).Clear();
            Close();
        }

        public IEnumerator MakeMainSceneCoroutine()
        {
            // Make VIP Lounge Scene
            GameObject metaGameScene = VipLounge.Utils.MainScene;
            if (metaGameScene == null)
            {
                string bundle = VipLounge.Defines.CONTENTS_BUNDLE;
                string asset = "VIP Epic Lounge Main Scene";
                Transform parent = MetaObjectUtils.MainCanvasAreaTransform;

                StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                    (GameObject sceneObj) => metaGameScene = sceneObj));
            }
            // // Enter Request
            bool success = false;
            bool fail = false;
            VipLoungeEnterResponse enterRes = null; 
            Debug.Log("start [RequestVipLoungeEnter]");
            BagelCodeClientAPI.RequestVipLoungeEnter(
                (response) =>
                {
                    success = true;
                    Debug.Log("succeed [RequestVipLoungeEnter]");

                    enterRes = response;
                    BlackboardQueryUtils.UpdateResponseVIPLounge(response.error, response.common, response.serverTime, response.vipLoungeInfo);
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
            string contextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "_biContextID")?.value;
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "contextID", contextID);
            var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
            if (prevOrientation != null)
                BlackboardUtils.SetOrCreateValue(mainSceneBB, "prevOrientation", prevOrientation.value);

            string enterType = BlackboardUtils.GetOrCreateVariable<string>(bb, "enter_type")?.value;
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "enterType", enterType);
            ClientAPI2Blackboard.Serialize(mainSceneBB, enterRes);
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "finish");

            metaGameScene.SetActive(true);

            // Update Scene
            BlackboardQueryUtils.MetaGameCrashReport(VipLounge.Utils.EVENT_NAME);
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

        private void SetAnimtionCloseCurtain()
        {
            ContextElement curtainLeftElement = ContextUtils.FindElement(root, "Curtain Left", CHILDREN);
            ContextElement curtainRightElement = ContextUtils.FindElement(root, "Curtain Right", CHILDREN);

            curtainLeftElement.GetComponent<Animator>()?.SetBool("Active", true);
            curtainRightElement.GetComponent<Animator>()?.SetBool("Active", true);
        }
    }
}
