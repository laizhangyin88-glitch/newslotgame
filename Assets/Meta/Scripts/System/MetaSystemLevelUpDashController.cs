using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;
using BagelCode.ClientModels;
using System.Collections.Generic;
using ParadoxNotion;

namespace BagelCode.LevelUpDash
{
    public class MetaSystemLevelUpDashController : EventMonoSingleton<MetaSystemLevelUpDashController>
    {
        private Blackboard bb;

        private List<Blackboard> rewardResultList;

        public void Init()
        {
            bb = GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(bb, "isReservedLevelDashClosedNoti", false);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_PASSIVE_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.Events.CHECK_LEVEL_UP_DASH_REWARD, CheckReward);
            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.Events.OPEN_LEVEL_BOOST_IAM, OpenBoostIam);

            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.START_PASSIVE_EVENT, OnStartPassive);
            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.REFRESH_PASSIVE_EVENT, OnRefreshPassive);

            Register(LevelUpDash.Events.ON_DISPLAY_LEVEL_DASH_CLOSED, OnDisplayLevelDashClosed);
        }

        private void OnDisplayLevelDashClosed()
        {
            BlackboardUtils.SetOrCreateValue(bb, "isReservedLevelDashClosedNoti", false);
        }

        private void OnStartPassive(EventData eventData)
        {
            if ((EventInfoType)eventData.value == EventInfoType.LEVEL_UP_DASH_MISSION)
            {
                BlackboardUtils.SetOrCreateValue(bb, "isReservedLevelDashClosedNoti", false);
            }
        }

        private void OnRefreshPassive(EventData eventData)
        {
            var eventInfo = PassiveEventManager.Instance.GetEventInfoFromID((int)eventData.value, true);
            if (eventInfo != null)
            {
                if (eventInfo.type == EventInfoType.LEVEL_UP_DASH_MISSION &&
                    !LevelUpDash.Utils.IsActiveLevelUpDash())
                {
                    BlackboardUtils.SetOrCreateValue(bb, "isReservedLevelDashClosedNoti", true);

                    if (bb.GetValue<bool>("isLevelUpDashOpened") == false)
                        EventSender.SendGlobalMetaEvent(LevelUpDash.Events.NOTIFY_LEVEL_UP_DASH_ENDED);
                }
            }
        }

        private void CheckReward()
        {
            rewardResultList = LevelUpDash.Utils.GetCurrentMissionRewardResultList();
            if (rewardResultList != null && rewardResultList.Count > 0)
            {
                bool prevAutoSpinState = BlackboardQueryUtils.IsAutoSpin();
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);

                BlackboardUtils.SetOrCreateValue(bb, "prevAutoSpinState", prevAutoSpinState);
                BlackboardUtils.SetOrCreateValue(bb, "enterType", "level_up");

                EventSender.SendGlobalMetaEvent(LevelUpDash.Events.OPEN_LEVEL_UP_DASH);
            }
            else
            {
                StartCoroutine(FinishCheckRewardCoroutine());
            }
        }

        private IEnumerator FinishCheckRewardCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            EventSender.SendGlobalMetaEvent(LevelUpDash.Events.FINISH_CHECK_REWARD);
        }

        public bool IsBoostEnabled() => LevelUpDash.Utils.IsEnabledPurchaseBooster();

        public void OpenCloseIam()
        {
            bool triggered = IAMRouter.Instance.TriggerIAM(
                InAppMessageTriggerType.CLOSE_LEVEL_UP_DASH_MAIN,
                gameObject,
                "");

            if (!triggered)
                EventSender.SendEvent(gameObject, "OnCancel");
        }

        public void PrintMetaInterruptingCountException()
        {
#if DEV
            Debug.LogError("LevelUpDash popup open failure. The \"metaInterruptingCount\" greater than 0.");
#endif
        }

        private void OpenBoostIam()
        {
            StartCoroutine(OpenBoostIamCoroutine());
        }

        private IEnumerator OpenBoostIamCoroutine()
        {
            bool triggered = MetaPopupUtils.OpenDealIam(
                gameObject, InAppMessageTriggerType.ANY_PURCHASE_CLICK_GET_BUTTON, false);

            if (triggered)
            {
                var callbackTrigger = new EventTrigger(gameObject, IAMUtils.ON_IAM_CALLBACK_EVENT);
                yield return new WaitUntilTrigger(callbackTrigger);
                EventSender.SendGlobalMetaEvent(LevelUpDash.Events.ON_CLOSE_LEVEL_BOOST_IAM);
            }
            else
            {
                // Open Shop
                MetaPopupUtils.OpenShop(gameObject);

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
                EventSender.SendGlobalMetaEvent(LevelUpDash.Events.ON_CLOSE_LEVEL_BOOST_IAM);
            }
        }

        public void SetAutoSpinState(bool forceStop)
        {
            bool prevAutoSpinState = BlackboardUtils.FindVariable<bool>(bb, "prevAutoSpinState")?.value ?? false;
            if (forceStop)
            {
                EventSender.SendGlobalMetaEvent(MetaEventDefine.ACTIVE_META_UI);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "UpdateButtonState");
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);
            }
            else
            {
                if (prevAutoSpinState)
                {
                    EventSender.SendGlobalMetaEvent(MetaEventDefine.INACTIVE_META_UI);
                    BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", true);
                }
                else
                {
                    EventSender.SendGlobalMetaEvent(MetaEventDefine.ACTIVE_META_UI);
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "UpdateButtonState");
                    BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);
                }
            }

            BlackboardUtils.SetOrCreateValue(bb, "prevAutoSpinState", false);
        }

        public void OpenLevelUpDashMain()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Level Up Dash Main Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            var popupBB = popupObj.GetComponent<Blackboard>();
            if (rewardResultList != null)
                BlackboardUtils.SetOrCreateValue(popupBB, "rewardResultList", rewardResultList);

            string enterType;
            var enterTypeVariable = BlackboardUtils.FindVariable<string>(bb, "enterType");
            if(enterTypeVariable != null && !string.IsNullOrEmpty(enterTypeVariable.value))
            {
                enterType = enterTypeVariable.value;
                enterTypeVariable.value = "";
            }
            else
            {
                bool isInGame = BlackboardQueryUtils.IsIngame();
                enterType = isInGame ? "in_game" : "lobby_event_button";
            }

            string contextId = BlackboardUtils.FindVariable<string>(bb, "metaGroupContextId")?.value;

            if(!string.IsNullOrEmpty(contextId))
            {
                BlackboardUtils.SetOrCreateValue(popupBB, "metaGroupContextId", contextId);
                BlackboardUtils.SetOrCreateValue(bb, "metaGroupContextId", "");
            }

            bool prevAutoSpinState = BlackboardUtils.FindVariable<bool>(bb, "prevAutoSpinState")?.value ?? false;
            BlackboardUtils.SetOrCreateValue(popupBB, "prevAutoSpinState", prevAutoSpinState);
            BlackboardUtils.SetOrCreateValue(popupBB, "enterType", enterType);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            BlackboardUtils.SetOrCreateValue(bb, "isLevelUpDashOpened", true);
        }

        public void OnLevelUpDashFinalize()
        {
            BlackboardUtils.SetOrCreateValue(bb, "isLevelUpDashOpened", false);

            if (LevelUpDash.Utils.IsActiveLevelUpDash() == false)
                EventSender.SendGlobalMetaEvent(LevelUpDash.Events.NOTIFY_LEVEL_UP_DASH_ENDED);
        }
    }
}
