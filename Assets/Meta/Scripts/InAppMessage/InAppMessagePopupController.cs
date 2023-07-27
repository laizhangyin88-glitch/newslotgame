using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using System.Collections;

namespace BagelCode.InAppMessage
{
    public class InAppMessagePopupController : InAppMessageBase
    {
        private const string IAM_BAB_CLOSE_LAST_TRIGGER_TIMESTAMP = "IAM_BAB_CLOSE_LAST_TRIGGER_TIMESTAMP";

        protected override void OnDisable()
        {
            base.OnDisable();
            if (IAMUtils.onVideoAdsCallback != null)
            {
                IAMUtils.onVideoAdsCallback();
                IAMUtils.onVideoAdsCallback = null;
            }
        }

        public override void LoadIAM(string bundleName, Blackboard loadIamInfo, long iamEndTimestamp)
        {
            isPreview = false;

            base.LoadIAM(bundleName, loadIamInfo, iamEndTimestamp);

            bool isInLobby = !BlackboardQueryUtils.IsIngame();

            var lobbyAnchor = GameObject.Find("Main Canvas/Area/Lobby/Anchor");
            if (lobbyAnchor != null && lobbyAnchor.activeSelf == false)
                isInLobby = false;

            var behaviorKey = isInLobby ?
                BlackboardUtils.GetOrCreateVariable<string>(iamInfo, "behaviorKeyViewFromLobby") :
                BlackboardUtils.GetOrCreateVariable<string>(iamInfo, "behaviorKeyViewFromAll");

            BagelCodeClientAPI.RequestBehaviorEventTrigger(behaviorKey.value,
            (response) =>
            {
                if (response.needToReloadCampaign)
                {
                    ReloadCampaign();
                }
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }

        public void OpenSureClosePopup() // IAM_Main_FSM
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Sure Close Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public IEnumerator OpenPopupCoroutine(ActionOpenPopupType type)
        {
            yield return StartCoroutine(IAMUtils.OpenPopupCoroutine(this, type));
        }

        public void OnCloseTriggerSalePopup() // IAM_Main_FSM
        {
            var iamType = BlackboardUtils.FindVariable<InAppMessageType>(bb, "_iamInfo/type");

            // TODO : Expend trigger type modules. for now use only BAB type.
            switch (iamType.value)
            {
                case InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP:
                case InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP:
                case InAppMessageType.SUPER_BONUS_PURCHASE_POPUP:
                    {
                        var shopBB = BlackboardQueryUtils.GetShopBB(ShopType.GEM_BAB_PROMOTION);
                        if (shopBB != null)
                        {
                            var lastTimestamp = PlayerPrefsUtils.GetOrCreateInt64(IAM_BAB_CLOSE_LAST_TRIGGER_TIMESTAMP);
                            if (lastTimestamp == 0L)
                            {
                                OpenItemSalePopup();
                            }
                            else
                            {
                                long currentTimestamp = TimeUtils.GetTimeStamp();
                                long targetTimestamp = GetCoolTimestamp(lastTimestamp);
                                if (currentTimestamp >= targetTimestamp)
                                {
                                    OpenItemSalePopup();
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public void TriggerCloseIAM() // IAMCloseTriggerBT
        {
            var triggerType = BlackboardUtils.FindVariable<InAppMessageTriggerType>(bb, "triggerType");
            if (triggerType != null)
            {
                switch (triggerType.value)
                {
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_COLLECTING_GAME:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_GEM_JACKPOT:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_SEASON_PASS:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_BOSS_RAIDERS:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_CLUB_ARENA:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_TIME_COLLECT:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_SHOP:
                    case InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF:
                    case InAppMessageTriggerType.ENTER_LOBBY_FROM_LOGIN:
                        {
                            var caller = bb.GetValue<GameObject>("iamCaller");
                            var contextID = bb.GetValue<string>("_biContextID");
                            if (IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.CLOSE_VIDEO_ADS, caller, contextID))
                                BlackboardUtils.SetOrCreateValue<bool>(bb, "silenceClose", true);
                        }
                        break;
                    case InAppMessageTriggerType.ENTER_CLUB_FROM_LOBBY_WITH_LEADER_PUSH_UNLOCKED:
                        {
                            bool isActionSuccess = bb.GetVariable<bool>("_isActionSuccess")?.value ?? false;
                            if (isActionSuccess)
                            {
                                BlackboardUtils.SetOrCreateValue<bool>(bb, "silenceClose", true);
                            }
                        }
                        break;
                }
            }

            // Is purchased?
            bool isPurchaseSuccess = bb.GetVariable<bool>("_isPurchaseSuccess")?.value ?? false;
            if (isPurchaseSuccess)
            {
                var caller = IAMUtils.GetIamCaller();
                if(caller != null)
                {
                    Blackboard callerBB = caller.GetComponent<Blackboard>();
                    if (callerBB != null)
                        BlackboardUtils.SetOrCreateValue(callerBB, "isPurchased", true);
                }
            }
        }

        private void ReloadCampaign()
        {
            BagelCodeClientAPI.RequestCampaignList(
            (response) =>
            {
                BlackboardQueryUtils.UpdateInAppMessageList(response.inAppMessageList);
                BlackboardQueryUtils.LoadInAppMessageWebImages(CacheType.FileCache, true);
                BlackboardQueryUtils.UpdateSlotBannerList(response.slotBannerGroupList);
                IAMRouter.Instance.UpdateIAMInfo();

                EventSender.SendGlobalEvent("RefreshDeal");
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new ParadoxNotion.EventData("OnRefreshSlotList"));
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }

        private void OpenItemSalePopup()
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Item Sale Scene").GetSceneInfo();
            var parent = GameObject.Find("Popup Manager/Area");
            var sceneObj = SceneManager.LoadScene(parent.transform, sceneInfo);
            var sceneBB = sceneObj.GetComponent<Blackboard>();

            var biContextID = BlackboardUtils.FindVariable<string>(bb, "_biContextID");
            BlackboardUtils.SetOrCreateValue<string>(sceneBB, "_biContextID", biContextID.value);
            BlackboardUtils.SetOrCreateValue<GameObject>(sceneBB, "caller", gameObject);

            PopupManager.Instance.Open(sceneObj);
            sceneObj.SetActive(true);

            PlayerPrefsUtils.SetInt64(IAM_BAB_CLOSE_LAST_TRIGGER_TIMESTAMP, TimeUtils.GetTimeStamp());
        }

        private long GetCoolTimestamp(long timestamp)
        {
#if DEV
            return timestamp + TimeUtils.ONE_MIN_MS;
#else
            return timestamp + TimeUtils.ONE_DAY_MS;
#endif
        }
    }
}
