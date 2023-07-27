using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode.EpicPass
{
    public class EpicPassSceneController : MonoBehaviour
    {
        public float effectMovementTime = 0.75f;

        public float increaseEffectTime = 1f;

        private SceneLoadOperation sceneOperation;

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement progressBarElement;
        private ContextElement progressBarTextElement;
        private ContextElement nextGiftIconElement;
        private ContextElement nextLevelTextElement;

        private ContextElement titleImageElement;
        private ContextElement leftImageElement;
        private ContextElement rightImageElement;
        private ContextElement gaugeIconImageElement;

        private ContextElement backgroundWebImageAreaElement;
        private ContextElement backgroundImageElement;
        private ContextElement defaultBackgroundElement;

        private ContextElement eventTimerElement;
        private ContextElement remainingTimerElement;

        private ContextElement buttonCloseElement;
        private ContextElement buttonPurchaseElement;
        private ContextElement buttonPurchaseTextElement;

        private ContextButton buttonResetElement;
        private ContextElement buttonResetTextElement;
        private ContextButton buttonResetSmallElement;
        private ContextElement buttonResetSmallTextElement;

        private ContextElement collectAllButtonAreaElement;
        private ContextElement collectAllButtonElement;
        private ContextElement collectAllButtonLockCoverElement;
        private ContextElement collectAllButtonTextElement;
        private ContextElement collectAllSpeachBalloonTextElement;
        private Animator collectAllSpeachBalloonAnimator;

        private ContextElement buttonInfoElement;

        private ContextElement freePointAreaElement;

        private ContextElement freePointButtonElement;
        private ContextElement freePointTextElement;
        private ContextElement freePointIconElement;

        private ContextElement freePointAdsButtonElement;
        private ContextElement freePointAdsTextElement;
        private ContextElement freePointAdsRemainingTimeTextElement;
        private ContextElement freePointAdsIconElement;

        private ContextElement pointParticleElement;

        private RemainingTimerController timerController;

        private bool isInit = false;

        private int flyEffectCount = 5;
        private float flyEffectGapDelay = 0.1f;


        private Blackboard _metaEnterInfoBB;
        private Blackboard MetaEnterInfoBB
        {
            get
            {
                if(_metaEnterInfoBB == null)
                    _metaEnterInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
                return _metaEnterInfoBB;
            }
        }

        private EventInfo eventInfo;

        private const int INFO_PAGE_COUNT = 2;
        private const string INFO_SCENE_ASSET_NAME = "Popup Information Scene";
        private const string INFO_TEXT = "EPIC_PASS_INFORMATION_TEXT_{0}";
        private const string INFO_PAGE_FORMAT = "Information Page {0:00}";
        private const string EFFECT_PREFAB_ASSET_NAME = "Epic Pass Point Web Image";
        private const string COLLECT_ALL = "EPIC_PASS_COLLECT_ALL_TEXT";
        private const string COLLECT_ALL_LOCKED = "EPIC_PASS_COLLECT_ALL_LOCKED_TEXT";
        private const string COLLECT_ALL_BALLOON = "EPIC_PASS_COLLECT_ALL_BALLOON";

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            // Images
            titleImageElement = ContextUtils.FindElement(rootElement, "Logo Web Image/Image", ContextSearchingType.FullNameSearch);
            leftImageElement = ContextUtils.FindElement(rootElement, "Left Deco Web Image/Image", ContextSearchingType.FullNameSearch);
            rightImageElement = ContextUtils.FindElement(rootElement, "Right Deco Web Image/Image", ContextSearchingType.FullNameSearch);
            gaugeIconImageElement = ContextUtils.FindElement(rootElement, "Epic Pass Point Web Image/Image", ContextSearchingType.FullNameSearch);

            defaultBackgroundElement = ContextUtils.FindElement(rootElement, "Epic Pass Background Default", ContextSearchingType.ChildrenSearch);
            backgroundWebImageAreaElement = ContextUtils.FindElement(rootElement, "Epic Pass Background Web Image", ContextSearchingType.ChildrenSearch);
            backgroundImageElement = ContextUtils.FindElement(backgroundWebImageAreaElement, "Anchor/Background", ContextSearchingType.FullNameSearch);

            // Progress
            progressBarElement = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            progressBarTextElement = ContextUtils.FindElement(progressBarElement, "Text Point", ContextSearchingType.ChildrenSearch);
            nextGiftIconElement = ContextUtils.FindElement(rootElement, "Point Gift", ContextSearchingType.ChildrenSearch);
            nextLevelTextElement = ContextUtils.FindElement(nextGiftIconElement, "Text Level", ContextSearchingType.ChildrenSearch);

            // Disable interactable
            MetaContextElementUtils.SetBooleanProperty(progressBarElement, false);

            // Timer
            eventTimerElement = ContextUtils.FindElement(rootElement, "Event Timer", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(eventTimerElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);

            // Free Point Buttons
            freePointAreaElement = ContextUtils.FindElement(rootElement, "Button Free Point Area", ContextSearchingType.ChildrenSearch);

            freePointButtonElement = ContextUtils.FindElement(freePointAreaElement, "Button Free Point", ContextSearchingType.ChildrenSearch);
            freePointIconElement = ContextUtils.FindElement(freePointButtonElement, "Icon Point Area/Epic Pass Point Web Image/Image", ContextSearchingType.FullNameSearch);
            freePointTextElement = ContextUtils.FindElement(freePointButtonElement, "Text Point", ContextSearchingType.ChildrenSearch);

            freePointAdsButtonElement = ContextUtils.FindElement(freePointAreaElement, "Button Free Point AD", ContextSearchingType.ChildrenSearch);
            freePointAdsIconElement = ContextUtils.FindElement(freePointAdsButtonElement, "Icon Point Area/Epic Pass Point Web Image/Image", ContextSearchingType.FullNameSearch);
            freePointAdsRemainingTimeTextElement = ContextUtils.FindElement(freePointAdsButtonElement, "Text Timer", ContextSearchingType.ChildrenSearch);
            freePointAdsTextElement = ContextUtils.FindElement(freePointAdsButtonElement, "Text Point", ContextSearchingType.ChildrenSearch);

            // Point Particle
            pointParticleElement = ContextUtils.FindElement(rootElement, "Particle Collect", ContextSearchingType.ChildrenSearch);

            // Buy Button
            buttonPurchaseElement = ContextUtils.FindElement(rootElement, "Button Buy", ContextSearchingType.ChildrenSearch);
            buttonPurchaseTextElement = ContextUtils.FindElement(buttonPurchaseElement, "Text", ContextSearchingType.ChildrenSearch);
            buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            buttonInfoElement = ContextUtils.FindElement(rootElement, "Button Question", ContextSearchingType.ChildrenSearch);

            // Reset Button
            buttonResetElement = ContextUtils.FindElement(rootElement, "Button Restart", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            buttonResetTextElement = ContextUtils.FindElement(buttonResetElement, "Text", ContextSearchingType.ChildrenSearch);

            buttonResetSmallElement = ContextUtils.FindElement(rootElement, "Button Restart Small Area/Button Restart Small", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();
            buttonResetSmallTextElement = ContextUtils.FindElement(buttonResetSmallElement, "Text", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetTextGlobal(buttonResetTextElement, "EPIC_PASS_RESET_BUTTON_TEXT", EpicPassUtils.ResetGemPrice);
            MetaContextElementUtils.SetTextGlobal(buttonResetSmallTextElement, "EPIC_PASS_RESET_SMALL_BUTTON_TEXT", EpicPassUtils.ResetGemPrice);

            MetaContextElementUtils.SimpleSetTextGlobal(buttonInfoElement, "Text", "EPIC_PASS_INFORMATION_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);

            // Collect all button
            collectAllButtonAreaElement = ContextUtils.FindElement(rootElement, "Button Collect Area", ContextSearchingType.ChildrenSearch);
            var collectAllSpeachBalloonElement  = ContextUtils.FindElement(collectAllButtonAreaElement, "Speech Balloon", ContextSearchingType.ChildrenSearch);
            collectAllSpeachBalloonAnimator = collectAllSpeachBalloonElement.gameObject.GetComponent<Animator>();
            collectAllSpeachBalloonTextElement = ContextUtils.FindElement(collectAllSpeachBalloonElement, "Text", ContextSearchingType.ChildrenSearch);
            collectAllButtonElement = ContextUtils.FindElement(collectAllButtonAreaElement, "Button Collect", ContextSearchingType.ChildrenSearch);
            collectAllButtonLockCoverElement = ContextUtils.FindElement(collectAllButtonElement, "Lock Cover", ContextSearchingType.ChildrenSearch);
            collectAllButtonTextElement = ContextUtils.FindElement(collectAllButtonElement, "Text", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetTextGlobal(collectAllSpeachBalloonTextElement, COLLECT_ALL_BALLOON);

            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                "OnClose",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonPurchaseElement,
                "OnPurchase",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonInfoElement,
                "OnClickInfo",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                freePointButtonElement,
                "OnClaimFreePoint",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                freePointAdsButtonElement,
                "OnShowADS",
                rootElement,
                null
            );

            buttonResetElement.AddListenerOnClick(
                context =>
                {
                    OnReset();
                }
            );

            buttonResetSmallElement.AddListenerOnClick(
                context =>
                {
                    OnReset();
                }
            );

            timerController = freePointAdsRemainingTimeTextElement.gameObject.GetComponent<RemainingTimerController>();
            if(timerController == null)
                timerController = freePointAdsRemainingTimeTextElement.gameObject.AddComponent<RemainingTimerController>();

            timerController.Init(
                freePointAdsRemainingTimeTextElement,
                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                "EPIC_PASS_ADS_TIMER_OUT_FORMAT_TEXT",
                "",
                "00:00",
                false,
                OnAdsTimerCallback
            );

            long adsFreePoint = EpicPassUtils.AdsFreePoint;
            MetaContextElementUtils.SetTextGlobal(freePointAdsTextElement, "EPIC_PASS_ADS_FREE_POINT_TEXT", adsFreePoint);
            MetaContextElementUtils.SetTextGlobal(freePointTextElement, "EPIC_PASS_ADS_FREE_POINT_TEXT", adsFreePoint);

            GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_ENTER_SCENE).Play();

            isInit = true;
        }

        public void OnInit()
        {
            InitProperty();

            if(EpicPassUtils.EpicPassInfo == null) return;

            eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            BlackboardUtils.SetOrCreateValue<int>(rootBB, "_metaGameEventID", eventInfo.id);

            var productBB = EpicPassUtils.EpicPassInfo.GetValue<Blackboard>("epicPassProduct");
            BlackboardUtils.SetOrCreateValue<Blackboard>(rootBB, "product", productBB);
            EpicPassUtils.contextId = rootBB.GetValue<string>("_biContextID");

            MetaContextElementUtils.SetTextGlobal(buttonPurchaseTextElement, "EPIC_PASS_PURCHASE_BUTTON_TEXT", productBB.GetValue<double>("price"));

            UpdateWebImages();
            UpdateProgressBar();
            UpdatePurchaseButton();
            UpdateResetButton();
            UpdateTimer();
            UpdateFreePointButtons();
            UpdateGift();
            OnUpdateCollectAllButton();

            rootAnimator.SetBool("Active", true);
        }

        public void OpenInfoPopup()
        {
            Transform rootTransform = GameObject.Find("Popup Manager/Area").transform;

            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, INFO_SCENE_ASSET_NAME, rootTransform, OnLoadInfoPopup);
        }

        public void OnCloseRewardPopup()
        {
            EpicPassUtils.IsClickProcess = false;
            UpdatePurchaseButton();

            if (EpicPassUtils.IsReset)
            {
                EpicPassUtils.IsReset = false;
            }
            else
            {
                OnRefresh();
                EventData eventData = new EventData(EpicPassUtils.ON_REWARD_REFRESH);
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
                // EventData eventData = new EventData(EpicPassUtils.ON_REFRESH_EVENT);
                // MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
            }
        }

        public void OnIncreaseEXP()
        {
            int currentLevel = EpicPassUtils.Level;
            float currentProgressDelta = (float)((double)EpicPassUtils.Point / (double)EpicPassUtils.RequiredPoint);

            int prevLevel = EpicPassUtils.PrevLevel;
            float prevProgressDelta = (float)((double)EpicPassUtils.PrevPoint/(double)EpicPassUtils.PrevRequiredPoint);

            int levelGap = currentLevel - prevLevel;

            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
            {
                currentProgressDelta = 1f;
                if(levelGap > 0)
                    levelGap -= 1;
            }

            progressBarTextElement.gameObject.SetActive(false);
            StartCoroutine(MetaContextElementUtils.IncreaseProgressEffect(
                                    progressBarElement,
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
            EventData levelUpEventData = new EventData(EpicPassUtils.ON_LEVEL_UP_EVENT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, levelUpEventData);

            UpdateGift();
            OnUpdateCollectAllButton();
        }

        private void EndIncrease()
        {
            EventData eventData = new EventData("OnFinishIncreaseEffect");
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);

            if(EpicPassUtils.PrevLevel < EpicPassUtils.Level)
            {
                var popupObj = MetaObjectUtils.MakeScene(
                                    BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS),
                                    "Popup Epic Pass Reward Scene",
                                    PopupManager.Instance.transform,
                                    "Area"
                                );

                // Level up broadcast
                EventData levelUpEventData = new EventData(EpicPassUtils.ON_LEVEL_UP_EVENT);
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, levelUpEventData);
            }

            // Apply Previnfo.
            EpicPassUtils.BackUpInfo();

            progressBarTextElement.gameObject.SetActive(true);
            UpdateProgressBar();
            UpdateGift();
            OnUpdateCollectAllButton();
        }

        public void OnFlyPointEffect()
        {
            if(gameObject.activeInHierarchy)
            {
                if(EpicPassUtils.PrevLevel >= EpicPassUtils.MaxLevel)
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
            Vector3 to = nextLevelTextElement.transform.position;

            GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_FLY_POINT_SCENE).Play();

            for(int i=0; i < flyEffectCount; ++i)
            {
                GameObject effectObj = MetaObjectUtils.MakePrefab(BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS), EFFECT_PREFAB_ASSET_NAME, transform);

                AsyncActionUtils.ApplyMovement(this, effectObj.transform, from, to, effectMovementTime, TweenUtils.VectorTweenCollectMove,
                    0f, () => { if (effectObj != null) Destroy(effectObj); });

                yield return new WaitForSeconds(flyEffectGapDelay);
            }
        }

        private IEnumerator OnArriveEarnEpicPassPointEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            rootAnimator.SetTrigger("GiftPoint");
            EventData eventData = new EventData(MetaEventDefine.ON_ARRIVE_EARN_EPIC_PASS_POINT_EFFECT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
            pointParticleElement.gameObject.SetActive(false);

            for(int i=0; i < flyEffectCount; ++i)
            {
                GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_GET_POINT_SCENE).Play();

                yield return new WaitForSeconds(flyEffectGapDelay);
            }
        }

        public void OnUpdateAdsButton()
        {
            UpdateFreePointButtons();
            UpdateResetButton();

            // EventData eventData = new EventData(EpicPassUtils.ON_LEVEL_UP_EVENT);
            // MessageDispatcher.Dispatch(MetaStringDefine.ON_META_UI_EVENT, eventData);
        }

        public void OnUpdateCollectAllButton()
        {
            if(EpicPassUtils.IsDisplayCollectAll && EpicPassUtils.UnclaimedRewardCount > 0)
            {
                collectAllButtonAreaElement.gameObject.SetActive(true);

                // purchased
                if(EpicPassUtils.IsEnabledCollectAll)
                 {
                    collectAllButtonLockCoverElement.gameObject.SetActive(false);
                    MetaContextElementUtils.SetTextGlobal(collectAllButtonTextElement, COLLECT_ALL);

                    MetaContextElementUtils.SetClickable(
                        collectAllButtonElement,
                        "OnCollectAll",
                        rootElement,
                        null
                    );
                }
                else
                {
                    collectAllButtonLockCoverElement.gameObject.SetActive(true);
                    MetaContextElementUtils.SetTextGlobal(collectAllButtonTextElement, COLLECT_ALL_LOCKED);

                    MetaContextElementUtils.SetClickable(
                        collectAllButtonElement,
                        ()=> {
                            if( !collectAllSpeachBalloonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Active") )
                                collectAllSpeachBalloonAnimator.SetTrigger("isActive");
                        }
                    );
                }
            }
            else
            {
                collectAllButtonAreaElement.gameObject.SetActive(false);
            }
        }

        private void UpdateWebImages()
        {
            string titleImageUrl = EpicPassUtils.EpicPassInfo.GetValue<string>("titleImageUrl");
            if(!string.IsNullOrEmpty(titleImageUrl))
                MetaContextElementUtils.SetWebImage(titleImageElement, titleImageUrl);

            string decorationLeftImageUrl = EpicPassUtils.EpicPassInfo.GetValue<string>("decorationLeftImageUrl");
            if(!string.IsNullOrEmpty(decorationLeftImageUrl))
                MetaContextElementUtils.SetWebImage(leftImageElement, decorationLeftImageUrl);

            string decorationRightImageUrl = EpicPassUtils.EpicPassInfo.GetValue<string>("decorationRightImageUrl");
            if(!string.IsNullOrEmpty(decorationRightImageUrl))
                MetaContextElementUtils.SetWebImage(rightImageElement, decorationRightImageUrl);

            string pointIconImageUrl = EpicPassUtils.EpicPassInfo.GetValue<string>("pointIconImageUrl");
            if(!string.IsNullOrEmpty(pointIconImageUrl))
            {
                MetaContextElementUtils.SetWebImage(gaugeIconImageElement, pointIconImageUrl);
                MetaContextElementUtils.SetWebImage(freePointIconElement, pointIconImageUrl);
                MetaContextElementUtils.SetWebImage(freePointAdsIconElement, pointIconImageUrl);
            }

            string backgroundImageURL = EpicPassUtils.EpicPassInfo.GetValue<string>("backgroundImageUrl");

            if(MetaContextElementUtils.SetWebImage(backgroundImageElement, backgroundImageURL))
            {
                // Active
                defaultBackgroundElement.gameObject.SetActive(false);
            }
            else
            {
                // Deactive
                defaultBackgroundElement.gameObject.SetActive(true);
            }
        }

        private void UpdateProgressBar()
        {
            // EXP
            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
            {
                // Max Level
                MetaContextElementUtils.SetFloatProperty(progressBarElement, 1f);
                MetaContextElementUtils.SetTextGlobal(progressBarTextElement, "EPIC_PASS_PROGRESS_BAR_MAX_TEXT");

            }
            else
            {
                long point = EpicPassUtils.Point;
                long requredPoint = EpicPassUtils.RequiredPoint;
                float exp = (float)point / (float)requredPoint;
                MetaContextElementUtils.SetFloatProperty(progressBarElement, exp);

                double expPercent = point < requredPoint ? ((double)point / (double)requredPoint) * 100 : 100.0;
                MetaContextElementUtils.SetTextGlobal(progressBarTextElement, "EPIC_PASS_PROGRESS_PERCENT_TEXT", expPercent );
            }
        }

        private void UpdateGift()
        {
            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
            {
                // Max Level
                nextGiftIconElement.gameObject.SetActive(false);
            }
            else
            {
                nextGiftIconElement.gameObject.SetActive(true);
                MetaContextElementUtils.SetTextGlobal(nextLevelTextElement, "EPIC_PASS_NEXT_GIFT_LEVEL_TEXT", EpicPassUtils.Level + 1);
            }
        }

        private void UpdateTimer()
        {
            if(eventInfo != null)
            {
                MetaGameUtils.UpdateMetaGameRemainingTimer(eventTimerElement, remainingTimerElement, eventInfo.endTimestamp);
            }
        }

        private void UpdateFreePointButtons()
        {
            timerController.StopTimer();

            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
            {
                // MAX
                freePointAreaElement.gameObject.SetActive(false);
            }
            else
            {
                freePointAreaElement.gameObject.SetActive(true);

                long lastAdsClaimTimestamp = EpicPassUtils.EpicPassInfo.GetValue<long>("lastAdsClaimTimestamp");
                long lastAdsViewTimestamp = EpicPassUtils.EpicPassInfo.GetValue<long>("lastAdsViewTimestamp");
                long nextAdsResetTimestamp = EpicPassUtils.EpicPassInfo.GetValue<long>("nextAdsResetTimestamp");

                long adsFreePoint = EpicPassUtils.AdsFreePoint;
                MetaContextElementUtils.SetTextGlobal(freePointAdsTextElement, "EPIC_PASS_ADS_FREE_POINT_TEXT", adsFreePoint);
                MetaContextElementUtils.SetTextGlobal(freePointTextElement, "EPIC_PASS_ADS_FREE_POINT_TEXT", adsFreePoint);

                long currentTimestamp = TimeUtils.GetTimeStamp();

                rootAnimator.SetBool("FreePoint", false);

                if(lastAdsClaimTimestamp < lastAdsViewTimestamp)
                {
                    // Collect point enable.
                    freePointButtonElement.gameObject.SetActive(true);
                    freePointAdsButtonElement.gameObject.SetActive(false);

                    rootAnimator.SetBool("FreePoint", true);
                }
                else
                {
                    if(nextAdsResetTimestamp  < currentTimestamp)
                    {
                        freePointAdsRemainingTimeTextElement.gameObject.SetActive(false);
                        freePointAdsTextElement.gameObject.SetActive(true);
                        freePointAdsIconElement.gameObject.SetActive(true);

                        // Show ADS
                        if(IsShowADS())
                        {
                            freePointButtonElement.gameObject.SetActive(false);
                            freePointAdsButtonElement.gameObject.SetActive(true);

                            // active Button
                            MetaContextElementUtils.SetBooleanProperty(freePointAdsButtonElement, true);

                            rootAnimator.SetBool("FreePoint", true);
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

        private bool IsShowADS()
        {
            var inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            if(inhouseAdsEnabled.value)
            {
                if(IAMRouter.Instance.CheckTriggerIAM(ClientModels.InAppMessageTriggerType.INHOUSE_ADS_FOR_SEASON_PASS))
                {
                    rootBB.SetValue("videoAdsType", "inhouse");
                    rootBB.SetValue("placementKey", "");
                    return true;
                }
            }
            else
            {
                var videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

                if(videoAdsEnabled.value)
                {
                    var placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/seasonPass").value;
                    if(!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                    {
                        rootBB.SetValue("videoAdsType", "video");
                        rootBB.SetValue("placementKey", placement);
                        return true;
                    }
                }
            }

            rootBB.SetValue("videoAdsType", "");
            rootBB.SetValue("placementKey", "");

            return false;
        }

        private void OnAdsTimerCallback()
        {
            UpdateFreePointButtons();
        }

        private void UpdatePurchaseButton()
        {
            MetaContextElementUtils.SetActive(buttonPurchaseElement, !EpicPassUtils.Paid);
        }

        private void UpdateResetButton()
        {
            bool resetAble = EpicPassUtils.IsMaxLevel && EpicPassUtils.Paid;
            bool resetSmallAble = EpicPassUtils.IsMaxLevel && !EpicPassUtils.Paid;

            MetaContextElementUtils.SetActive(buttonResetElement, resetAble);
            MetaContextElementUtils.SetActive(buttonResetSmallElement, resetSmallAble);
        }

        private void OnLoadInfoPopup(SceneLoadOperation _sceneOperation)
        {
            sceneOperation = _sceneOperation;

            GameObject popupGO = sceneOperation.GetScene();
            var infoBB = popupGO.GetComponent<Blackboard>();

            string bundleName = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS, true);
            MetaPopupUtils.SetInformationPopupData(infoBB, INFO_PAGE_COUNT, bundleName, "Information Dots", INFO_PAGE_FORMAT, INFO_TEXT);

            BlackboardUtils.SetOrCreateValue(infoBB, "caller", gameObject);

            popupGO.SetActive(true);
            PopupManager.Instance.Open(popupGO);
        }

        private void OnReset()
        {
            if (EpicPassUtils.IsClickProcess) return;
            EpicPassUtils.IsClickProcess = true;

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["context_id"] = EpicPassUtils.contextId;
            if (eventInfo != null && eventInfo.constraints != null)
            {
                EventDataSeasonPass epicPassEventInfo = eventInfo.constraints as EventDataSeasonPass;
                customData["season_pass_setting_id"] = (long)epicPassEventInfo.seasonPassSettingId;
                customData["season_pass_setting_name"] = epicPassEventInfo.seasonPassSettingName;
            }
            Analytics.CustomEvent("client_click_season_pass_reset", customData);

            if (EpicPassUtils.ResetGemPrice > BlackboardUtils.FindValue<long>(null, "/me/gem"))
            {
                var popupObj = MetaObjectUtils.MakeScene(
                    MetaStringDefine.LOBBY_BUNDLE_NAME,
                    "Popup Gem Shop Scene",
                    PopupManager.Instance.transform,
                    "Area"
                    );
                EpicPassUtils.IsClickProcess = false;
                return;
            }

            if (!EpicPassUtils.Paid)
            {
                EpicPassUtils.OpenRewardLostScene();
            }
            else
            {
                EpicPassUtils.RequestEpicPassReset();
            }
        }

        public void OnRefresh()
        {
            if (EpicPassUtils.EpicPassInfo == null) return;

            eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            BlackboardUtils.SetOrCreateValue<int>(rootBB, "_metaGameEventID", eventInfo.id);

            var productBB = EpicPassUtils.EpicPassInfo.GetValue<Blackboard>("epicPassProduct");
            BlackboardUtils.SetOrCreateValue<Blackboard>(rootBB, "product", productBB);

            MetaContextElementUtils.SetTextGlobal(buttonPurchaseTextElement, "EPIC_PASS_PURCHASE_BUTTON_TEXT", productBB.GetValue<double>("price"));

            UpdateWebImages();
            UpdateProgressBar();
            UpdatePurchaseButton();
            UpdateResetButton();
            UpdateTimer();
            UpdateFreePointButtons();
            UpdateGift();
            OnUpdateCollectAllButton();
        }

        public void OnCollectReward()
        {
            OnUpdateCollectAllButton();
        }

        public void OnCollectAll()
        {
            OnUpdateCollectAllButton();

            EventData eventData = new EventData(EpicPassUtils.ON_REWARD_REFRESH);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        public void OnCollect()
        {
            OnUpdateCollectAllButton();

            EventData eventData = new EventData(EpicPassUtils.ON_REWARD_REFRESH);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        public void OnDisable()
        {
            EpicPassUtils.UpdateUnClaimedItem();
        }

        public IEnumerator CheckWelcomeVIPLoungePopup(GameObject objRewardPopup)
        {
            if (BlackboardQueryUtils.IsVipLoungeEnabled())
            {
                while (objRewardPopup != null)
                    yield return new WaitForEndOfFrame();

                var eventData = new EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, gameObject);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
            }
        }

#if UNITY_EDITOR
        int testLevel = 1;
        [Button]
        public void Test()
        {
            BlackboardUtils.SetOrCreateValue<int>(EpicPassUtils.EpicPassInfo, "level", testLevel);
            ++testLevel;

            EventData eventData = new EventData(EpicPassUtils.ON_LEVEL_UP_EVENT);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        [Button]
        public void TestLevelUp()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                                BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS),
                                "Popup Epic Pass Reward Scene",
                                PopupManager.Instance.transform,
                                "Area"
                            );
        }

        [Button]
        public void TestUnlock()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                                BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS),
                                "Popup Epic Pass Reward Scene",
                                PopupManager.Instance.transform,
                                "Area"
                            );

            BlackboardUtils.SetOrCreateValue(popupObj.GetComponent<Blackboard>(), "isUnlock", true);
        }

        [Button]
        public void TestGainPoint()
        {
            OnFlyPointEffect();
        }
#endif
    }
}
