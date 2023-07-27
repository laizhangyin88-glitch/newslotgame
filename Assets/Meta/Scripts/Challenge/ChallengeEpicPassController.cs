using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.OSA_Scroll;

namespace BagelCode.EpicPass
{
    public class ChallengeEpicPassController : MonoBehaviour
    {
        private SceneLoadOperation sceneOperation = null;

        private Animator mainAnim = null;
        private Blackboard rootBB;
        private ContextElement root;
        private ContextElement mainContentsElement;
        private ContextElement comingSoonElement;

        private ContextElement backgroundImageElement;
        private ContextElement topProgressBarElement;
        private ContextElement topProgressBarTextElement;
        private ContextElement pointWebImageElement;
        private ContextElement pointGiftIconElement;
        private ContextElement pointGiftTextElement;
        private ContextElement remainingTimerElement;
        private ContextElement collectAllButtonAreaElement;
        private ContextElement collectAllButtonElement;
        private ContextElement collectAllButtonTextElement;
        private ContextElement collectAllButtonLockCoverElement;
        private ContextElement buttonPurchaseElement;
        private ContextElement buttonPurchaseTextElement;
        private ContextElement buttonResetElement;
        private ContextElement buttonResetSmallElement;
        private ContextElement freePointAreaElement;
        private ContextElement freePointButtonElement;
        private ContextElement freePointTextElement;
        private ContextElement freePointIconElement;
        private ContextElement freePointAdsButtonElement;
        private ContextElement freePointAdsTextElement;
        private ContextElement freePointAdsRemainingTimeTextElement;
        private ContextElement freePointAdsIconElement;
        private ContextElement pointParticleElement;

        private ContextElement scrollAreaElement;
        private ContextElement levelProgressElement;

        private RemainingTimerController timerController;
        private OSA_EpicPassV2Rewards osaRewards;
        private EpicPassV2RewardLevelProgressController rewardLevelProgressController;

        public long rewardsScrollVelocity => (long)osaRewards?.Velocity.x;

        private EventInfo eventInfo;
        private UnityAction badgeUpdateEventCallback;

        private string contextId = "";
        private int flyEffectCount = 5;
        private float flyEffectGapDelay = 0.1f;

        public float effectMovementTime = 0.75f;
        public float increaseEffectTime = 1f;

        public void OnInit(Animator _mainAnim)
        {
            mainAnim = _mainAnim;
            InitProperty();

            UpdateEventInfo();

            UpdatePurchaseButtonText();

            CheckActiveEpicPass();
            UpdateWebImages();
            OnUpdateCollectAllButton();
            UpdateEpicPassButtom();
        }

        void Update()
        {
            if (rewardsScrollVelocity != 0L)
                UpdateRewardsProgress();
        }

        private void InitProperty()
        {
            root = gameObject.GetComponent<ContextElement>();
            rootBB = gameObject.GetComponent<Blackboard>();

            contextId = BlackboardUtils.FindVariable<string>(rootBB, "_biContextID")?.value ?? "";

            mainContentsElement = ContextUtils.FindElement(root, "Main Contents", ContextSearchingType.ChildrenSearch);
            comingSoonElement = ContextUtils.FindElement(root, "Coming Soon", ContextSearchingType.ChildrenSearch);

            ContextElement buttonInfoElement = ContextUtils.FindElement(mainContentsElement, "Button Question", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(mainContentsElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);
            // Backgound
            ContextElement backgroundElement = ContextUtils.FindElement(mainContentsElement, "Epic Pass Always Full Background Web Image", ContextSearchingType.ChildrenSearch);
            backgroundImageElement = ContextUtils.FindElement(backgroundElement, "Image", ContextSearchingType.ChildrenSearch);
            // Top Point Gift Gauge
            ContextElement topPointGiftAreaElement = ContextUtils.FindElement(mainContentsElement, "Top Point Gift Gauge Area", ContextSearchingType.ChildrenSearch);
            topProgressBarElement = ContextUtils.FindElement(topPointGiftAreaElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            topProgressBarTextElement = ContextUtils.FindElement(topProgressBarElement, "Text Point", ContextSearchingType.ChildrenSearch);
            ContextElement pointWebElement = ContextUtils.FindElement(topPointGiftAreaElement, "Epic Pass Always Point Web Image", ContextSearchingType.ChildrenSearch);
            pointWebImageElement = ContextUtils.FindElement(pointWebElement, "Image", ContextSearchingType.ChildrenSearch);
            pointGiftIconElement = ContextUtils.FindElement(topPointGiftAreaElement, "Point Gift", ContextSearchingType.ChildrenSearch);
            pointGiftTextElement = ContextUtils.FindElement(pointGiftIconElement, "Text Level", ContextSearchingType.ChildrenSearch);
            // Disable interactable
            MetaContextElementUtils.SetBooleanProperty(topProgressBarElement, false);
            // Collect All Button
            collectAllButtonAreaElement = ContextUtils.FindElement(mainContentsElement, "Button Collect Area", ContextSearchingType.ChildrenSearch);
            collectAllButtonElement = ContextUtils.FindElement(collectAllButtonAreaElement, "Button Collect", ContextSearchingType.ChildrenSearch);
            collectAllButtonTextElement = ContextUtils.FindElement(collectAllButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            collectAllButtonLockCoverElement = ContextUtils.FindElement(collectAllButtonElement, "Lock Cover", ContextSearchingType.ChildrenSearch);
            // Buy Button
            buttonPurchaseElement = ContextUtils.FindElement(mainContentsElement, "Button Buy", ContextSearchingType.ChildrenSearch);
            buttonPurchaseTextElement = ContextUtils.FindElement(buttonPurchaseElement, "Text", ContextSearchingType.ChildrenSearch);
            buttonResetElement = ContextUtils.FindElement(mainContentsElement, "Button Restart", ContextSearchingType.ChildrenSearch);
            // Free Point Button
            freePointAreaElement = ContextUtils.FindElement(mainContentsElement, "Button Free Point Area", ContextSearchingType.ChildrenSearch);
            freePointButtonElement = ContextUtils.FindElement(freePointAreaElement, "Button Free Point", ContextSearchingType.ChildrenSearch);
            freePointIconElement = ContextUtils.FindElement(freePointButtonElement, "Icon Point Area/Epic Pass Always Point Web Image/Image", ContextSearchingType.FullNameSearch);
            freePointTextElement = ContextUtils.FindElement(freePointButtonElement, "Text Point", ContextSearchingType.ChildrenSearch);
            // Free Point Ads Button
            freePointAdsButtonElement = ContextUtils.FindElement(freePointAreaElement, "Button Free Point AD", ContextSearchingType.ChildrenSearch);
            freePointAdsIconElement = ContextUtils.FindElement(freePointAdsButtonElement, "Icon Point Area/Epic Pass Always Point Web Image/Image", ContextSearchingType.FullNameSearch);
            freePointAdsRemainingTimeTextElement = ContextUtils.FindElement(freePointAdsButtonElement, "Text Timer", ContextSearchingType.ChildrenSearch);
            freePointAdsTextElement = ContextUtils.FindElement(freePointAdsButtonElement, "Text Point", ContextSearchingType.ChildrenSearch);
            // Small Reset Button
            ContextElement resetSmallAreaElement = ContextUtils.FindElement(mainContentsElement, "Button Restart Small Area", ContextSearchingType.ChildrenSearch);
            buttonResetSmallElement = ContextUtils.FindElement(resetSmallAreaElement, "Button Restart Small", ContextSearchingType.ChildrenSearch);

            timerController = freePointAdsRemainingTimeTextElement.gameObject.GetComponent<RemainingTimerController>();
            if (timerController == null)
                timerController = freePointAdsRemainingTimeTextElement.gameObject.AddComponent<RemainingTimerController>();
            // Point Particle
            pointParticleElement = ContextUtils.FindElement(mainContentsElement, "Particle Collect", ContextSearchingType.ChildrenSearch);
            // Scroll Area
            scrollAreaElement = ContextUtils.FindElement(mainContentsElement, "Pass Scroll Area", ContextSearchingType.ChildrenSearch);
            if (scrollAreaElement != null)
                osaRewards = scrollAreaElement.GetComponent<OSA_EpicPassV2Rewards>();
            levelProgressElement = ContextUtils.FindElement(mainContentsElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            if (levelProgressElement != null)
                rewardLevelProgressController = levelProgressElement.GetComponent<EpicPassV2RewardLevelProgressController>();
            // Coming Soon
            ContextElement comingSoonButtonAreaElement = ContextUtils.FindElement(comingSoonElement, "Button Area", ContextSearchingType.ChildrenSearch);
            ContextElement comingSoonButtonElement = ContextUtils.FindElement(comingSoonButtonAreaElement, "Button Play", ContextSearchingType.ChildrenSearch);

            //MetaContextElementUtils.SimpleSetTextGlobal(comingSoonButtonElement, "Text", "", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonInfoElement, "Text", "EPIC_PASS_INFORMATION_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonResetElement, "Text", "EPIC_PASS_ALWAYS_RESET_BUTTON_TEXT", ContextSearchingType.ChildrenSearch, EpicPassUtilsV2.ResetGemPrice);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonResetSmallElement, "Text", "EPIC_PASS_RESET_SMALL_BUTTON_TEXT", ContextSearchingType.ChildrenSearch, EpicPassUtilsV2.ResetGemPrice);

            MetaContextElementUtils.SetClickable(buttonInfoElement, PopupOpenInformation);
            MetaContextElementUtils.SetClickable(buttonResetElement, OnReset);
            MetaContextElementUtils.SetClickable(buttonResetSmallElement, OnReset);

            MetaContextElementUtils.SetClickable(comingSoonButtonElement, OnClickComingSoon);

            MetaContextElementUtils.SetClickable(
                buttonPurchaseElement,
                "OnPurchase",
                root,
                null
            );

            MetaContextElementUtils.SetClickable(
                freePointButtonElement,
                "OnClaimFreePoint",
                root,
                null
            );

            MetaContextElementUtils.SetClickable(
                freePointAdsButtonElement,
                "OnShowADS",
                root,
                null
            );

            timerController.Init(
                freePointAdsRemainingTimeTextElement,
                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                "EPIC_PASS_ADS_TIMER_OUT_FORMAT_TEXT",
                "",
                "00:00",
                false,
                OnAdsTimerCallback
            );
        }

        public void ActiveMainEpicPass(bool isActive)
        {
            mainContentsElement.gameObject.SetActive(isActive);
            comingSoonElement.gameObject.SetActive(!isActive);
        }

        public void ActiveComingSoonAnimator()
        {
            if (comingSoonElement == null)
                return;
            Animator anim = comingSoonElement.GetComponent<Animator>();
            anim?.SetBool("Active", true);
        }

        private void CheckActiveEpicPass()
        {
            bool isActive = IsActiveEpicPass();
            ActiveMainEpicPass(isActive);
        }
        // VIP Lounge Welcome Popup
        public IEnumerator CheckWelcomeVIPLoungePopup(GameObject objRewardPopup)
        {
            EpicPassUtilsV2.UpdateMetaIcon();
            if (BlackboardQueryUtils.IsVipLoungeEnabled())
            {
                while (objRewardPopup != null)
                    yield return new WaitForEndOfFrame();

                var eventData = new EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, gameObject);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);

                EpicPassUtilsV2.CheckWelcomePopup = false;
            }
        }

        public bool IsActiveEpicPass()
        {
            return eventInfo != null && !MetaGameUtils.IsMetaGameLevelLocked();
        }

        private bool IsShowADS()
        {
            var inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            var videoAdsType = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "videoAdsType");
            var placementKey = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "placementKey");
            if (inhouseAdsEnabled.value)
            {
                if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_SEASON_PASS))
                {
                    videoAdsType.value = "inhouse";
                    placementKey.value = "";
                    return true;
                }
            }
            else
            {
                var videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

                if (videoAdsEnabled.value)
                {
                    string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/seasonPass").value;
                    if (!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                    {
                        videoAdsType.value = "video";
                        placementKey.value = placement;
                        return true;
                    }
                }
            }

            videoAdsType.value = "";
            placementKey.value = "";

            return false;
        }

        private void OnAdsTimerCallback()
        {
            UpdateFreePointButtons();
        }

        public void OnCollect()
        {
            OnUpdateCollectAllButton();

            EventData eventData = new EventData(EpicPassUtilsV2.ON_REWARD_REFRESH);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);

            EpicPassUtilsV2.UpdateMetaIcon();
        }

        public void OnCollectAll()
        {
            OnUpdateCollectAllButton();

            EventData eventData = new EventData(EpicPassUtilsV2.ON_REWARD_REFRESH);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);

            EpicPassUtilsV2.UpdateMetaIcon();
        }

        private void OnClickComingSoon()
        {
            BIClientClickButtonEpicPass("SEASON_PASS_COMING_SOON");
            EventSender.SendEvent(mainAnim.gameObject, "OnClose");
        }

        public void OnCloseRewardPopup()
        {
            EpicPassUtilsV2.IsClickProcess = false;
            UpdatePurchaseButton();

            if (EpicPassUtilsV2.IsReset)
            {
                EpicPassUtilsV2.IsReset = false;
            }
            else
            {
                OnRefresh();
                EventData eventData = new EventData(EpicPassUtilsV2.ON_REWARD_REFRESH);
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
            }
        }

        public void OnFlyPointEffect()
        {
            if (gameObject.activeInHierarchy)
            {
                if (EpicPassUtilsV2.PrevLevel >= EpicPassUtilsV2.MaxLevel)
                {
                    EventData eventData = new EventData(MetaEventDefine.ON_ARRIVE_EARN_EPIC_PASS_POINT_EFFECT);
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
                    return;
                }

                StartCoroutine(OnFlyPointEffects());
                StartCoroutine(OnArriveEarnEpicPassPointEffect(effectMovementTime));

                pointParticleElement.gameObject.SetActive(false);
                pointParticleElement.gameObject.SetActive(true);
            }
        }

        private IEnumerator OnFlyPointEffects()
        {
            Vector3 from = freePointAreaElement.transform.position;
            Vector3 to = pointGiftTextElement.transform.position;
            GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_FLY_POINT_SCENE).Play();

            for (int i = 0; i < flyEffectCount; ++i)
            {
                GameObject effectObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Epic Pass Always Point Web Image", transform);

                AsyncActionUtils.ApplyMovement(this, effectObj.transform, from, to, effectMovementTime, TweenUtils.VectorTweenCollectMove,
                    0f, () => { if (effectObj != null) Destroy(effectObj); });

                yield return new WaitForSeconds(flyEffectGapDelay);
            }
        }

        private IEnumerator OnArriveEarnEpicPassPointEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            //rootAnimator.SetTrigger("GiftPoint");
            EventData eventData = new EventData(MetaEventDefine.ON_ARRIVE_EARN_EPIC_PASS_POINT_EFFECT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
            pointParticleElement.gameObject.SetActive(false);

            for (int i = 0; i < flyEffectCount; ++i)
            {
                GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_GET_POINT_SCENE).Play();

                yield return new WaitForSeconds(flyEffectGapDelay);
            }
        }

        public void OnIncreaseEXP()
        {
            int currentLevel = EpicPassUtilsV2.Level;
            float currentProgressDelta = (float)((double)EpicPassUtilsV2.Point / (double)EpicPassUtilsV2.RequiredPoint);

            int prevLevel = EpicPassUtilsV2.PrevLevel;
            float prevProgressDelta = (float)((double)EpicPassUtilsV2.PrevPoint / (double)EpicPassUtilsV2.PrevRequiredPoint);

            int levelGap = currentLevel - prevLevel;

            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
            {
                currentProgressDelta = 1f;
                if (levelGap > 0)
                    levelGap -= 1;
            }

            topProgressBarTextElement.gameObject.SetActive(false);
            StartCoroutine(MetaContextElementUtils.IncreaseProgressEffect(
                                    topProgressBarElement,
                                    prevProgressDelta,
                                    currentProgressDelta,
                                    levelGap,
                                    increaseEffectTime,
                                    0f,
                                    IncreaseLevelUp,
                                    EndIncrease)
            );
        }

        private void IncreaseLevelUp(int count)
        {
            // Level up broadcast
            EventData levelUpEventData = new EventData(EpicPassUtilsV2.ON_LEVEL_UP_EVENT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, levelUpEventData);

            UpdateGift();
            OnUpdateCollectAllButton();
        }

        private void EndIncrease()
        {
            EventData eventData = new EventData("OnFinishIncreaseEffect");
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);

            if (EpicPassUtilsV2.PrevLevel < EpicPassUtilsV2.Level)
            {
                var popupObj = EpicPassUtilsV2.GetRewardPopupObject();
                if (SetRewardPopupObject(popupObj, null))
                {
                    Blackboard bb = popupObj.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(bb, "isLevelUp", true);

                    PopupEpicPassV2RewardController controller = popupObj.GetComponent<PopupEpicPassV2RewardController>();
                    controller.OnInit();
                }

                // Level up broadcast
                EventData levelUpEventData = new EventData(EpicPassUtilsV2.ON_LEVEL_UP_EVENT);
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, levelUpEventData);
            }

            EpicPassUtilsV2.UpdateMetaIcon();
            // Apply Previnfo.
            EpicPassUtilsV2.BackUpInfo();

            topProgressBarTextElement.gameObject.SetActive(true);
            UpdateTopProgressBar();
            UpdateRewardsProgress();
            UpdateGift();
            OnUpdateCollectAllButton();
        }

        private void OnLoadInfoPopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;

            GameObject popupGO = sceneOperation.GetScene();
            var infoBB = popupGO.GetComponent<Blackboard>();

            string bundleName = EpicPassUtilsV2.BUNDLE_NAME;
            MetaPopupUtils.SetInformationPopupData(infoBB, 2, bundleName, "Information Dots", "Epic Pass Always Information Page {0:00}", "EPIC_PASS_INFORMATION_TEXT_{0}");

            BlackboardUtils.SetOrCreateValue(infoBB, "caller", gameObject);

            popupGO.SetActive(true);
            PopupManager.Instance.Open(popupGO);
        }

        public void OnRefresh()
        {
            if (EpicPassUtilsV2.EpicPassInfo == null) return;

            UpdatePurchaseButtonText();

            UpdateWebImages();
            UpdateEpicPassTop();
            OnUpdateCollectAllButton();
            UpdateEpicPassButtom();
        }

        private void OnReset()
        {
            if (EpicPassUtilsV2.IsClickProcess) return;
            EpicPassUtilsV2.IsClickProcess = true;

            BIClientClickButtonEpicPass("SEASON_PASS_RESET");
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["context_id"] = EpicPassUtilsV2.contextId;
            if (eventInfo != null && eventInfo.constraints != null)
            {
                EventDataSeasonPassV2 epicPassEventInfo = eventInfo.constraints as EventDataSeasonPassV2;
                customData["season_pass_setting_id"] = (long)epicPassEventInfo.seasonPassSettingV2Id;
                customData["season_pass_setting_name"] = epicPassEventInfo.seasonPassSettingV2Name;
            }
            Analytics.CustomEvent("client_click_season_pass_reset", customData);

            if (EpicPassUtilsV2.ResetGemPrice > BlackboardUtils.FindValue<long>(null, "/me/gem"))
            {
                var popupObj = MetaObjectUtils.MakeScene(
                    MetaStringDefine.LOBBY_BUNDLE_NAME,
                    "Popup Epic Pass Always Gem Shop Scene",
                    PopupManager.Instance.transform,
                    "Area"
                    );
                EpicPassUtilsV2.IsClickProcess = false;
                return;
            }

            if (!EpicPassUtilsV2.Paid)
            {
                EpicPassUtilsV2.OpenRewardLostScene();
            }
            else
            {
                EpicPassUtilsV2.RequestEpicPassReset();
            }
        }

        public void OnUpdateAdsButton()
        {
            UpdateFreePointButtons();
            UpdateResetButton();
        }

        public void OnUpdateCollectAllButton()
        {
            if (EpicPassUtilsV2.IsDisplayCollectAll && EpicPassUtilsV2.UnclaimedRewardCount > 0)
            {
                collectAllButtonAreaElement.gameObject.SetActive(true);

                // purchased
                if (EpicPassUtilsV2.IsEnabledCollectAll)
                {
                    collectAllButtonLockCoverElement.gameObject.SetActive(false);
                    MetaContextElementUtils.SetTextGlobal(collectAllButtonTextElement, "EPIC_PASS_COLLECT_ALL_TEXT");
                    MetaContextElementUtils.SetClickable(collectAllButtonElement, "OnCollectAll", root, null);
                }
                else
                {
                    collectAllButtonLockCoverElement.gameObject.SetActive(true);
                    MetaContextElementUtils.SetTextGlobal(collectAllButtonTextElement, "EPIC_PASS_COLLECT_ALL_LOCKED_TEXT");
                    //MetaContextElementUtils.SetClickable(
                    //    collectAllButtonElement,
                    //    () => {
                    //        if (!collectAllSpeachBalloonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Active"))
                    //            collectAllSpeachBalloonAnimator.SetTrigger("isActive");
                    //    }
                    //);
                }
            }
            else
            {
                collectAllButtonAreaElement.gameObject.SetActive(false);
            }
        }

        private void PopupOpenInformation()
        {
            BIClientClickButtonEpicPass("SEASON_PASS_INFORMATION");
            Transform rootTransform = GameObject.Find("Popup Manager/Area").transform;
            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Information Scene", rootTransform, OnLoadInfoPopup);
        }

        private IEnumerator PopupOpenCollectReward()
        {
            yield return null;
        }

        public void SetAnimLoading(bool isActive)
        {
            mainAnim?.SetBool("EpicPass Loading", isActive);
        }

        private bool SetRewardPopupObject(GameObject rewardPopup, List<Blackboard> rewardResultList)
        {
            if (rewardPopup != null)
            {
                Blackboard popupBB = rewardPopup.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue<GameObject>(popupBB, "caller", gameObject);
                BlackboardUtils.SetOrCreateValue<List<Blackboard>>(popupBB, "rewardResultList", rewardResultList);

                return true;
            }

            return false;
        }

        public void SetBadgeUpdateEventCallback(UnityAction callback)
        {
            if (callback != null)
                badgeUpdateEventCallback = callback;
        }

        private void UpdateBadge()
        {
            if (badgeUpdateEventCallback != null)
                badgeUpdateEventCallback.Invoke();
        }

        public void UpdateEpicPass()
        {
            UpdateEpicPassTop();
            UpdateEpicPassButtom();
        }

        private void UpdateEpicPassButtom()
        {
            UpdatePurchaseButton();
            UpdateResetButton();
            UpdateFreePointButtons();
        }

        private void UpdateEpicPassTop()
        {
            UpdateTimer();
            UpdateTopProgressBar();
            UpdateRewardsProgress();
            UpdateGift();
            UpdateBadge();
        }

        public void UpdateEpicPassTab()
        {
            CheckActiveEpicPass();
            if (IsActiveEpicPass())
            {
                rewardLevelProgressController.OnInit(osaRewards);
                UpdateEpicPass();
            }
        }

        private void UpdateEventInfo()
        {
            eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            //BlackboardUtils.SetOrCreateValue<int>(rootBB, "_metaGameEventID", eventInfo.id);
        }

        private void UpdateFreePointButtons()
        {
            timerController.StopTimer();

            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
            {
                // MAX
                freePointAreaElement.gameObject.SetActive(false);
            }
            else
            {
                freePointAreaElement.gameObject.SetActive(true);

                long lastAdsClaimTimestamp = EpicPassUtilsV2.EpicPassInfo.GetValue<long>("lastAdsClaimTimestamp");
                long lastAdsViewTimestamp = EpicPassUtilsV2.EpicPassInfo.GetValue<long>("lastAdsViewTimestamp");
                long nextAdsResetTimestamp = EpicPassUtilsV2.EpicPassInfo.GetValue<long>("nextAdsResetTimestamp");

                long adsFreePoint = EpicPassUtilsV2.AdsFreePoint;
                MetaContextElementUtils.SetTextGlobal(freePointAdsTextElement, "EPIC_PASS_ADS_FREE_POINT_TEXT", adsFreePoint);
                MetaContextElementUtils.SetTextGlobal(freePointTextElement, "EPIC_PASS_ADS_FREE_POINT_TEXT", adsFreePoint);

                long currentTimestamp = TimeUtils.GetTimeStamp();

                //rootAnimator.SetBool("FreePoint", false);

                if (lastAdsClaimTimestamp < lastAdsViewTimestamp)
                {
                    // Collect point enable.
                    freePointButtonElement.gameObject.SetActive(true);
                    freePointAdsButtonElement.gameObject.SetActive(false);

                    //rootAnimator.SetBool("FreePoint", true);
                }
                else
                {
                    if (nextAdsResetTimestamp < currentTimestamp)
                    {
                        freePointAdsRemainingTimeTextElement.gameObject.SetActive(false);
                        freePointAdsTextElement.gameObject.SetActive(true);
                        freePointAdsIconElement.gameObject.SetActive(true);

                        // Show ADS
                        if (IsShowADS())
                        {
                            freePointButtonElement.gameObject.SetActive(false);
                            freePointAdsButtonElement.gameObject.SetActive(true);

                            // active Button
                            MetaContextElementUtils.SetBooleanProperty(freePointAdsButtonElement, true);

                            //rootAnimator.SetBool("FreePoint", true);
                        }
                        else
                        {
                            freePointButtonElement.gameObject.SetActive(false);
                            freePointAdsButtonElement.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // Cooltime ADS
                        freePointButtonElement.gameObject.SetActive(false);
                        freePointAdsButtonElement.gameObject.SetActive(true);

                        freePointAdsRemainingTimeTextElement.gameObject.SetActive(true);
                        freePointAdsTextElement.gameObject.SetActive(false);
                        freePointAdsIconElement.gameObject.SetActive(false);

                        timerController.StartTimer(nextAdsResetTimestamp, 0);

                        // Inactive Button
                        MetaContextElementUtils.SetBooleanProperty(freePointAdsButtonElement, false);
                    }
                }
            }
        }

        private void UpdateGift()
        {
            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
            {
                // Max Level
                pointGiftIconElement.gameObject.SetActive(false);
            }
            else
            {
                pointGiftIconElement.gameObject.SetActive(true);
                MetaContextElementUtils.SetTextGlobal(pointGiftTextElement, "EPIC_PASS_NEXT_GIFT_LEVEL_TEXT", EpicPassUtilsV2.Level + 1);
            }
        }

        private void UpdatePurchaseButton()
        {   
            MetaContextElementUtils.SetActive(buttonPurchaseElement, !EpicPassUtilsV2.Paid);
        }

        private void UpdatePurchaseButtonText()
        {
            if (eventInfo == null) return;

            BlackboardUtils.SetOrCreateValue(rootBB, "_seasonPassEventID", eventInfo.id);

            var productBB = EpicPassUtilsV2.EpicPassInfo.GetValue<Blackboard>("epicPassProduct");
            BlackboardUtils.SetOrCreateValue(rootBB, "product", productBB);
            EpicPassUtilsV2.contextId = BlackboardUtils.GetOrCreateVariable<string>(rootBB, "_biContextID").value;

            MetaContextElementUtils.SetTextGlobal(buttonPurchaseTextElement, "EPIC_PASS_ALWAYS_PURCHASE_BUTTON_TEXT", productBB.GetValue<double>("price"));
        }

        private void UpdateResetButton()
        {
            bool resetAble = EpicPassUtilsV2.IsMaxLevel && EpicPassUtilsV2.Paid;
            bool resetSmallAble = EpicPassUtilsV2.IsMaxLevel && !EpicPassUtilsV2.Paid;

            MetaContextElementUtils.SetActive(buttonResetElement, resetAble);
            MetaContextElementUtils.SetActive(buttonResetSmallElement, resetSmallAble);
        }

        private void UpdateRewardsProgress()
        {
            rewardLevelProgressController?.UpdateProgress(osaRewards.GetControllers());
        }

        private void UpdateTimer()
        {
            UpdateEventInfo();
            if (IsActiveEpicPass())
            {
                MetaGameUtils.UpdateMetaGameRemainingTimer(remainingTimerElement, remainingTimerElement, eventInfo.endTimestamp);
            }
        }

        private void UpdateTopProgressBar()
        {
            // EXP
            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
            {
                // Max Level
                MetaContextElementUtils.SetFloatProperty(topProgressBarElement, 1f);
                MetaContextElementUtils.SetTextGlobal(topProgressBarTextElement, "EPIC_PASS_PROGRESS_BAR_MAX_TEXT");
                //pointGiftTextElement
            }
            else
            {
                long point = EpicPassUtilsV2.Point;
                long requredPoint = EpicPassUtilsV2.RequiredPoint;
                float exp = (float)point / (float)requredPoint;
                MetaContextElementUtils.SetFloatProperty(topProgressBarElement, exp);

                double expPercent = point < requredPoint ? ((double)point / (double)requredPoint) * 100 : 100.0;
                MetaContextElementUtils.SetTextGlobal(topProgressBarTextElement, "EPIC_PASS_PROGRESS_PERCENT_TEXT", expPercent);
            }
        }

        public void UpdateWebImages()
        {
            string backgroundImageUrl = EpicPassUtilsV2.BackgroundImageUrl;
            if (!string.IsNullOrEmpty(backgroundImageUrl))
                MetaContextElementUtils.SetWebImage(backgroundImageElement, backgroundImageUrl);

            string pointWebImageUrl = EpicPassUtilsV2.PointIconImageUrl;
            if (!string.IsNullOrEmpty(pointWebImageUrl))
                MetaContextElementUtils.SetWebImage(pointWebImageElement, pointWebImageUrl);
        }
        // Request API
        public IEnumerator RequestCollectSeasonPassReward(GameObject rewardCellObj)
        {
            UpdateEventInfo();

            if (rewardCellObj == null || eventInfo == null)
                yield break;

            EpicPassV2RewardItemController itemController = rewardCellObj.GetComponent<EpicPassV2RewardItemController>();
            if (itemController == null)
                yield break;

            bool success = false;
            bool fail = false;

            bool isPaidUser = false;
            List<RewardInfo> availableRewardOnPurchaseList = null;

            BagelCodeClientAPI.CollectSeasonPassRewardV2(eventInfo.id, itemController.rewardLevel, itemController.isPaidReward,
                (response) =>
                {
                    EpicPassUtilsV2.UpdateEpicPassRewards(response.rewardInfoList);
                    EpicPassUtilsV2.UnclaimedRewardCount = response.unclaimedRewardCount;

                    List<Blackboard> rewardList = EpicPassUtilsV2.UpdateCollectRewards(response.rewardResultList, response.isPaidReward);

                    BlackboardQueryUtils.ApplyRewardResult(rewardList);

                    if (itemController != null)
                        itemController.OnClaimed();

                    isPaidUser = response.isPaid;
                    availableRewardOnPurchaseList = response.availableRewardOnPurchaseList;
                    if (!EpicPassUtilsV2.RefreshInbox)
                        EpicPassUtilsV2.CheckRewardToInbox(response.rewardResultList);
                    success = true;
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_LEVEL_OR_REWARD_CONDITION_ERROR:
                            {
                                // Need text alert popup.
                            }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });

            yield return new WaitUntil(() => (success || fail));
            if (gameObject == null)
                yield break;

            if (success)
            {
                GameObject resultPopupObj = EpicPassUtilsV2.GetRewardPopupObject();
                if (resultPopupObj != null)
                {
                    Blackboard popupBB = resultPopupObj.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(popupBB, "rewardResultList", EpicPassUtilsV2.RewardResultList);
                    BlackboardUtils.SetOrCreateValue(popupBB, "isPaidUser", isPaidUser);
                    BlackboardUtils.SetOrCreateList(popupBB, "availableRewardOnPurchaseList", availableRewardOnPurchaseList, ClientAPI2Blackboard.Serialize);

                    PopupManager.Instance.Open(resultPopupObj);
                    PopupEpicPassV2RewardController controller = resultPopupObj.GetComponent<PopupEpicPassV2RewardController>();
                    controller.OnInit();
                }
            }
        }

        public IEnumerator RequestCollectAllSeasonPassReward()
        {
            UpdateEventInfo();

            if (eventInfo == null)
                yield break;

            bool success = false;
            bool fail = false;

            bool isPaidUser = false;
            List<RewardInfo> availableRewardOnPurchaseList = null;

            BagelCodeClientAPI.CollectAllSeasonPassRewardV2(eventInfo.id,
                (response) =>
                {
                    if (gameObject != null)
                    {
                        List<Blackboard> rewardList = EpicPassUtilsV2.UpdateCollectAllRewards(response.freeRewardResultList, response.paidRewardResultList);
                        BlackboardQueryUtils.ApplyRewardResult(rewardList);

                        EpicPassUtilsV2.CollectAll();
                    }

                    isPaidUser = response.isPaid;
                    availableRewardOnPurchaseList = response.availableRewardOnPurchaseList;

                    if (!EpicPassUtilsV2.RefreshInbox)
                    {
                        if (!EpicPassUtilsV2.CheckRewardToInbox(response.freeRewardResultList))
                            EpicPassUtilsV2.CheckRewardToInbox(response.paidRewardResultList);
                    }

                    success = true;

                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_LEVEL_OR_REWARD_CONDITION_ERROR:
                            {
                                // Need text alert popup.
                            }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                    fail = true;
                }
            );

            yield return new WaitUntil(() => (success || fail));

            if (success)
            {
                GameObject resultPopupObj = EpicPassUtilsV2.GetRewardPopupObject();
                if (resultPopupObj != null)
                {
                    Blackboard popupBB = resultPopupObj.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(popupBB, "rewardResultList", EpicPassUtilsV2.RewardResultList);
                    BlackboardUtils.SetOrCreateValue(popupBB, "isPaidUser", isPaidUser);
                    BlackboardUtils.SetOrCreateList(popupBB, "availableRewardOnPurchaseList", availableRewardOnPurchaseList, ClientAPI2Blackboard.Serialize);

                    PopupManager.Instance.Open(resultPopupObj);
                    PopupEpicPassV2RewardController controller = resultPopupObj.GetComponent<PopupEpicPassV2RewardController>();
                    controller.OnInit();
                }
            }
        }

        public IEnumerator RequestSeasonPassAdsViewV2()
        {
            UpdateEventInfo();

            if (eventInfo == null)
                yield break;

            bool success = false;
            bool fail = false;

            BagelCodeClientAPI.RequestSeasonPassAdsViewV2(eventInfo.id,
                (response) =>
                {
                    if (gameObject != null)
                    {
                        EpicPassUtilsV2.UpdateAdsView(response.lastAdsViewTimestamp, response.nextAdsResetTimestamp);
                        success = true;
                    }

                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                    fail = true;
                }
            );
            yield return new WaitUntil(() => (success || fail));
        }

        public IEnumerator RequestSeasonPassAdsClaim()
        {
            UpdateEventInfo();

            if (eventInfo == null)
                yield break;

            bool success = false;
            bool fail = false;

            BagelCodeClientAPI.RequestSeasonPassAdsClaimV2(eventInfo.id,
                (response) =>
                {
                    if (gameObject != null)
                    {
                        EpicPassUtilsV2.UpdateAdsClaim(response.updateInfo, response.adsFreePoint, response.lastAdsClaimTimestamp);
                        success = true;
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                    fail = true;
                }
            );

            yield return new WaitUntil(() => (success || fail));
        }
        // 154 AE
        public void BIClientClickButtonEpicPass(string buttonName)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["button_name"] = buttonName;
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_click_button", customData);
        }
    }
}