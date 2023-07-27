using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using System.Collections;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsMetaButtonController : MetaGameEventButtonController
    {
        private const float FINDER_GAUGE_MIN = 0.12f;

        private ContextElement badgeAreaElement;

        private ContextElement vipLoungeGaugeElement;
        private ContextElement vipLoungeCountElement;

        private ContextElement iconAreaElement;
        private ContextElement iconElement;
        private ContextElement gaugeAreaElement;
        private ContextElement lockedIconElement;
        private ContextElement badgeElement;
        private ContextElement timerAreaElement;

        private Animator lockedSpeechAnimator;
        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private string contextId;
        private int currentDepotCount = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (isInit)
            {
                UpdateLocked();
                UpdateDepotCount();
            }
        }

        public override void InitProperty() // TODO : SOUND
        {
            if (isInit) return;

            base.InitProperty();

            contextId = BiEventUtils.GenerateContextID();

            // Make Sound Asset
            string bundle = VegasDreams.Defines.COMMON_BUNDLE;
            MetaObjectUtils.MakePrefab(bundle, "Vegas Dreams Common Sounds", transform);

            // in game no vip lounge icon
            if (BlackboardQueryUtils.IsIngame())
            {
                string vipBundle = VipLounge.VipLounge.Defines.COMMON_BUNDLE;
                MetaObjectUtils.MakePrefab(vipBundle, "VIP Lounge Common Sounds", transform);
            }

            iconAreaElement = ContextUtils.FindElement(root, "Icon Area", CHILDREN);
            badgeAreaElement = ContextUtils.FindElement(root, "Badge Area", CHILDREN);
            timerAreaElement = ContextUtils.FindElement(root, "Event Timer Area", CHILDREN);
            
            iconElement = ContextUtils.FindElement(iconAreaElement, "Vegas Dreams Icon", CHILDREN);

            lockedIconElement = ContextUtils.FindElement(root, "Locked Area", CHILDREN);

            MakeBadgeIcon();
            UpdateLocked();
            UpdateTimer();

            MetaContextElementUtils.SetClickable(root, () =>
            {
                VegasDreamsAnalytics.click_button_vds("BUILD_DREAM_ICON", contextId);
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_VEGAS_DREAMS_META_ICON);
            });
            
            var remainTime = VegasDreams.Utils.VipLoungeBenefitEndTimestamp - TimeUtils.GetTimeStamp();
            Invoke(nameof(UpdateLocked), remainTime / 1000f + 1); // Add 1 sec for safety expire refresh

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CREDIT_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_END_TURN_META, OnEndTurnMetaGame);
            Register(MetaEventDefine.ON_META_UI_EVENT, VegasDreams.Events.ON_UPDATE_VIP_LOUNGE_INFO, OnUpdateVipLoungeInfo);
            Register(MetaEventDefine.ON_META_UI_EVENT, VegasDreams.Events.ON_UPDATE_VEGAS_DREAMS_INFO, UpdateDepotCount);

            Register(MetaEventDefine.ON_CREDIT_EVENT, "UpdatedTotalBetCredit", UpdateLocked);

            isInit = true;
        }

        private void OnUpdateVipLoungeInfo()
        {
            UpdateLocked();
        }

        private void UpdateTimer()
        {
            var RemainTimestamp = VegasDreams.Utils.SeasonEndTimestamp - TimeUtils.GetTimeStamp();
            
            if (RemainTimestamp > TimeUtils.ONE_DAY_MS * 7)
            {
                MetaContextElementUtils.SetActive(timerAreaElement, false);
            }
            else
            {
                MetaContextElementUtils.SetActive(timerAreaElement, true);
                var timerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer/Remaining Timer/Text", FULL);
                var timer = timerElement.gameObject.AddComponent<RemainingTimerController>();
                timer.Init(timerElement, "TIME_FORMAT_HHMMSS", "TEXT_NORMAL", "", "Ended", true, null);
                timer.StartTimer(VegasDreams.Utils.SeasonEndTimestamp, 0);
            }
        }

        private void MakeBadgeIcon()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Badge";
            Transform parent = badgeAreaElement.transform;

            var badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            badgeElement = badgeObj.GetComponent<ContextElement>();

            UpdateDepotCount();
        }

        private void UpdateDepotCount()
        {
            badgeAreaElement.UpdateContext(false);

            int depotCount = VegasDreams.Utils.TotalDepotCount;
            currentDepotCount = depotCount;

            var depotAnimatorMaxValue = Mathf.Min(100, depotCount);
            var depotBadgeTextString = Mathf.Min(99, depotCount).ToString();
            if (depotCount > 99) depotBadgeTextString += "+";

            MetaContextElementUtils.SimpleSetIntProperty(badgeAreaElement, "Badge", depotAnimatorMaxValue);
            MetaContextElementUtils.SimpleSetText(badgeAreaElement, "Badge/Text", depotBadgeTextString, FULL);
        }

        private void OnEndTurnMetaGame()
        {
            // Need Remove - maybe?
        }

        private void UpdateLocked()
        {
            bool unlockLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.VIP_LOUNGE) > BlackboardQueryUtils.GetMyLevel() || BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.VIP_LOUNGE);
            bool isEnded = VipLounge.VipLounge.Utils.IsEnded || unlockLevel; // Should use Vip Lounge for Event Update

            bool isLocked = isEnded;
            bool isInGame = BlackboardQueryUtils.IsIngame();
            if (isInGame)
            {
                long bet = BlackboardQueryUtils.GetTotalBet();
                long minBet = BlackboardQueryUtils.GetMinEligibleBet(MetaGameType.BUILD_DREAM_SEASON);

                bool isBetEnough = bet >= minBet;
                isLocked = isEnded || !isBetEnough;
            }

            var iconAnim = iconElement.GetComponent<Animator>();
            lockedIconElement.gameObject.SetActive(isEnded);
            iconAnim.SetBool("IsLocked", isLocked);
        }

        public IEnumerator MakeCurrentStateScene()
        {
            if (VipLounge.VipLounge.Utils.IsEnded)
            {
                yield return StartCoroutine(MakeVipLoungeLoadingSceneCoroutine());
            }
            else
            {
                yield return StartCoroutine(MakeVegasDreamsLoadingSceneCoroutine());
            }
        }

        public IEnumerator MakeVegasDreamsLoadingSceneCoroutine()
        {
            // Send BI
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "ui_click");

            string bundle = VegasDreams.Defines.COMMON_BUNDLE;
            string asset = "Vegas Dreams Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = MetaObjectUtils.MakeScene(bundle, asset, parent, "");

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            loadingBB.AddVariable("_biContextID", contextId);
            loadingBB.AddVariable("isEnter", true);
            loadingBB.AddVariable("isMetaInGame", false);
            loadingBB.AddVariable("enter_type", "meta_icon");

            yield return new WaitForEndOfFrame();
        }

        public IEnumerator MakeVipLoungeLoadingSceneCoroutine()
        {
            // Send BI
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "ui_click");

            string bundle = VipLounge.VipLounge.Defines.COMMON_BUNDLE;
            string asset = "VIP Epic Lounge Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = MetaObjectUtils.MakeScene(bundle, asset, parent, "");

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            loadingBB.AddVariable("_biContextID", contextId);
            loadingBB.AddVariable("isEnter", true);
            loadingBB.AddVariable("isMetaInGame", false);
            loadingBB.AddVariable("enter_type", "meta_icon");

            yield return new WaitForEndOfFrame();
        }

        public bool CheckActive()
        {
            return BlackboardQueryUtils.IsVipLoungeEnabled();
        }

        [Sirenix.OdinInspector.Button]
        protected override void OnEarnMetaGameItem()
        {
            if (this.isActiveAndEnabled)
            {
                isItemEarning = true;

                string bundle = VegasDreams.Defines.COMMON_BUNDLE;
                string asset = "In Game Vegas Dreams Depot";
                Transform parent = earningItemRootArea;

                earnEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                if (earnEffectObj == null) return;

                float EFFECT_MOVEMENT_TIME = 1.3f;
                var controller = earnEffectObj.GetComponent<EarningMetaGameItemController>();
                controller.flyingTime = EFFECT_MOVEMENT_TIME;

                Vector3 from = MetaGameAppearTransformManager.Instance.GetTransform("SpinButton").position;
                Vector3 to = transform.position;

                // Sound
                GSManager.Instance.GetHandler(VegasDreams.Defines.DEPOT_OBTAIN).Play();

                AsyncActionUtils.ApplyMovement(this,
                    earnEffectObj.transform, from, to, EFFECT_MOVEMENT_TIME,
                    TweenUtils.VectorTweenCollectMove,
                    0f,
                    OnArriveEarningMetaGameItem);
            }
        }

        protected override void UpdateMetaItemInfo()
        {
            if (isInit)
            {
                UpdateDepotCount();
            }
        }
    }
}
