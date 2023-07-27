using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class VipBonusButtonController : MonoBehaviour
    {
        private ContextElement rootElement;

        public Blackboard rootBlackboard;
        public Animator rootAnimator;

        private ContextElement collctButtonElement;
        private ContextElement showADButtonElement;
        private ContextElement collectParticleElement;
        private ContextElement textValueElement;

        private RemainingTimerController timerController;

        private Variable<int> meTier;
        private Variable<long> vipDailyBonusTimestamp;
        private Variable<long> videoAdTimestamp;
        private Variable<bool> inhouseAdsEnabled;
        private Variable<bool> videoAdsEnabled;
        private Variable<long> vipDailyBonusCooltime;

        private Variable<string> videoAdsType;
        private Variable<string> videoPlacementKey;
        private Variable<InAppMessageTriggerType> iamTriggerType;

        private long NextCollectTimestamp
        {
            get
            {
                return vipDailyBonusTimestamp.value + vipDailyBonusCooltime.value;
            }
        }

        private bool isInit = false;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private void OnDisable()
        {
            CancelInvoke();
        }

        public void OnInit()
        {
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator   = gameObject.GetComponent<Animator>();

            textValueElement = ContextUtils.FindElement(rootElement, "VIP Collect Button/Text 2", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SimpleSetText(rootElement, "VIP Collect Button/Text 1", StringTableUtils.GetString(tableType, "BUTTON_SHOP_COLLECT"), ContextSearchingType.FullNameSearch);

            collctButtonElement = ContextUtils.FindElement(rootElement, "VIP Collect Button", ContextSearchingType.ChildrenSearch);
            showADButtonElement = ContextUtils.FindElement(rootElement, "Bonus AD Button", ContextSearchingType.ChildrenSearch);
            collectParticleElement = ContextUtils.FindElement(rootElement, "Particle Collect", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                collctButtonElement,
                "OnCollect",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                showADButtonElement,
                "OnShowVideo",
                rootElement,
                null
            );

            timerController = gameObject.AddComponent<RemainingTimerController>();

            meTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/tier");
            vipDailyBonusTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/vipDailybonusTimestamp");
            vipDailyBonusCooltime = BlackboardUtils.FindVariable<long>( MainBlackboard.Get(), "vipDailyBonusCooltime");
            videoAdTimestamp = BlackboardUtils.FindVariable<long>( MainBlackboard.Get(), "lastShopBonusVideoAdsClaimTimestamp");

            inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

            videoAdsType = BlackboardUtils.GetOrCreateVariable<string>(rootBlackboard, "videoAdsType");
            iamTriggerType = BlackboardUtils.GetOrCreateVariable<InAppMessageTriggerType>(rootBlackboard, "iamTriggerType");
            videoPlacementKey = BlackboardUtils.GetOrCreateVariable<string>(rootBlackboard, "placementKey");

            isInit = true;
        }

        public void OnUpdateVariables()
        {
            long dailyBonusTimestamp = 0L;
            videoAdsType.value = "";
            videoPlacementKey.value = "";

            if(AccountUtils.HasAccount())
            {
                long currentTimestamp = TimeUtils.GetTimeStamp();
                if(currentTimestamp >= NextCollectTimestamp)
                {
                    timerController.StopTimer();
                    // textValueElement
                    long vipCoins = GetVIPCoin();
                    MetaContextElementUtils.SetText(textValueElement, StringTableUtils.GetString(tableType, "TEXT_SHOP_BONUS", vipCoins));

                    MetaContextElementUtils.SetBooleanProperty(collctButtonElement, true);

                    rootAnimator.SetBool("IsVideo", false);
                }
                else
                {
                    if(CheckVideoADS())
                    {
                        rootAnimator.SetBool("IsVideo", true);

                        // Set VipBonusCooltime Callback;
                        var reserveTimer = gameObject.GetComponent<SimpleReserveTimer>();
                        if(reserveTimer == null)
                            reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();

                        reserveTimer.SetReserveCallback(NextCollectTimestamp,
                            () => 
                            {
                                TimerCallback();
                            }
                        );
                    }
                    else
                    {
                        dailyBonusTimestamp = NextCollectTimestamp;

                        timerController.Init(textValueElement, "TIME_FORMAT_HHMMSS_TOTALHOUR", "SHOP_VIP_BONUS_OUTPUT_FORMAT", "", "Ended", false, TimerCallback);
                        timerController.StartTimer(NextCollectTimestamp, 0);

                        MetaContextElementUtils.SetBooleanProperty(collctButtonElement, false);
                        rootAnimator.SetBool("IsVideo", false);
                    }
                }
            }
            else
            {
                timerController.StopTimer();
                // textValueElement
                // var meTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/tier");
                long vipCoins = TierUtils.GetVipCoins(meTier.value);
                MetaContextElementUtils.SetText(textValueElement, StringTableUtils.GetString(tableType, "TEXT_SHOP_BONUS", vipCoins));

                MetaContextElementUtils.SetBooleanProperty(collctButtonElement, true);
            }

            // Set BB saveAsVipDailyBonusTimestamp.value
            // dailyBonusTimestamp
            BlackboardUtils.SetOrCreateValue<long>(rootBlackboard, "vipDailybonusTimestamp", dailyBonusTimestamp);
        }

        private void DeactivateParticle()
        {
            collectParticleElement.gameObject.SetActive(false);
        }

        private void ActivateParticle()
        {
            CancelInvoke("DeactivateParticle");
            Invoke("DeactivateParticle", 4f);

            collectParticleElement.gameObject.SetActive(false);
            collectParticleElement.gameObject.SetActive(true);
        }

        public void OnCollectTimeBonus(bool isSuccess)
        {
            if(isSuccess)
            {
                ActivateParticle();

                GSManager.Instance.GetHandler("UI_Button_Collect").Play();
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
            }

            Graph.SendGlobalEvent(new EventData("RefreshTier"), null);
            Graph.SendGlobalEvent(new EventData("OnCollectVipBonus"), null);
        }

        private void TimerCallback()
        {
            OnUpdateVariables();
        }

        private bool CheckVideoADS()
        {
            iamTriggerType.value = InAppMessageTriggerType.UNKNOWN;

            // Check Video ADS Cooltime
            // Cooltime by one AD. 
            if(vipDailyBonusTimestamp.value <= videoAdTimestamp.value)
                return false;

            if(inhouseAdsEnabled.value)
            {
                // Check Push Enabled. iOS, Android
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                if(!BlackboardQueryUtils.UserOptionsPushNotification() || !NativeHelper.Instance.GetPushNotificationSubscribed())
                {
                    if(IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF))
                    {
                        iamTriggerType.value = InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF;
                        videoAdsType.value = "inhouse";
                        return true;
                    }
                }
#endif
                if(IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_SHOP))
                {
                    iamTriggerType.value = InAppMessageTriggerType.INHOUSE_ADS_FOR_SHOP;
                    videoAdsType.value = "inhouse";
                    return true;
                }
            }
            else if(videoAdsEnabled.value)
            {
                
                var shopBonusPlacement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/shopBonus").value;
                if(!string.IsNullOrEmpty(shopBonusPlacement) && VideoAdsController.Instance.IsVideoAdsAvailable(shopBonusPlacement))
                {
                    videoAdsType.value = "video";
                    videoPlacementKey.value = shopBonusPlacement;
                    return true;
                }
            }

            return false;
        }

        private long GetVIPCoin()
        {
            //var meTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/tier");
            return TierUtils.GetVipCoins(meTier.value);
        }
        // Apply VIP Lounge Reward
        public void SendGlobalVIPLoungeRewardEvent()
        {
            var eventData = new EventData<Transform>(MetaEventDefine.ON_VIP_LOUNGE_REWARD_EVENT, textValueElement.transform);
            EventSender.SendGlobalEvent(eventData);
        }

        public void UpdateVIPLoungeRewardCredit()
        {
            long vipLoungeRewardNumerator = BlackboardQueryUtils.GetVIPLoungeClubVegasRewardNumerator();
            long vipCoins = GetVIPCoin();
            long resultCoins = NumberUtils.GetMultiplierNumeratorValue(vipCoins, vipLoungeRewardNumerator);

            var increaseNumber = gameObject.GetComponent<MetaIncreaseNumber>();
            increaseNumber?.Reset(textValueElement as IContextText, "TEXT_SHOP_BONUS", vipCoins, resultCoins, 0.2f, 1, false);
        }
    }
}