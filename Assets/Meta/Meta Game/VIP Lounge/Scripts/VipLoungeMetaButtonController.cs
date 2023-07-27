using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using System.Collections;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.VipLounge
{
    public class VipLoungeMetaButtonController : MetaGameEventButtonController
    {
        private const float FINDER_GAUGE_MIN = 0.12f;

        private ContextElement badgeAreaElement;

        private ContextElement vipLoungeGaugeElement;
        private ContextElement vipLoungeCountElement;

        private ContextElement iconAreaElement;
        private ContextElement iconElement;
        private ContextElement gaugeAreaElement;
        private ContextElement lockedIconElement;
        private ContextElement lockedSpeechBalloonElement;
        private ContextElement badgeElement;

        private Animator lockedSpeechAnimator;
        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (isInit)
            {
                UpdateLoungePoint();
                UpdateLocked();
                UpdateDepotCount();
            }
        }

        public override void InitProperty() // TODO : SOUND
        {
            if (isInit) return;

            base.InitProperty();

            root = gameObject.GetComponent<ContextElement>();

            root.UpdateContext(false);

            // Make Sound Asset
            string bundle = VipLounge.Defines.COMMON_BUNDLE;
            MetaObjectUtils.MakePrefab(bundle, "VIP Lounge Common Sounds", transform);

            vipLoungeGaugeElement = ContextUtils.FindElement(root, "Gauge Area/VIP Lounge Icon Gage", FULL);
            vipLoungeCountElement = ContextUtils.FindElement(root, "Gauge Area/VIP Lounge Icon Gage/Text Gage Info", FULL);

            iconAreaElement = ContextUtils.FindElement(root, "Icon Area", CHILDREN);
            gaugeAreaElement = ContextUtils.FindElement(root, "Gauge Area", CHILDREN);
            badgeAreaElement = ContextUtils.FindElement(root, "Badge Area", CHILDREN);

            iconElement = ContextUtils.FindElement(iconAreaElement, "VIP Epic Lounge Icon", CHILDREN);

            int level = BlackboardQueryUtils.GetFeatureMinLevel(ClientModels.LockedFeatureType.VIP_LOUNGE);

            ContextElement lockedAreaElement = ContextUtils.FindElement(root, "Locked Area", CHILDREN);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "VIP Epic Lounge Locked Icon", CHILDREN);
            MetaContextElementUtils.SimpleSetText(lockedIconElement, "Text", level.ToString());

            if (!ignoreSpeechBalloon)
            {
                lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "VIP Lounge Locked Info Speech Balloon", CHILDREN);
                lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();
                MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", CHILDREN, level);
            }

            MakeBadgeIcon();
            UpdateLoungePoint();
            UpdateLocked();

            MetaContextElementUtils.SetClickable(
                root,
                gameObject,
                VipLounge.Events.ON_ENTER_VIP_LOUNGE,
                false
                );

            Register(MetaEventDefine.ON_META_UI_EVENT, VipLounge.Events.ON_UPDATE_VIP_LOUNGE_INFO, UpdateLoungePoint);

            isInit = true;
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

            int depotCount = VegasDreams.VegasDreams.Utils.TotalDepotCount;

            var depotAnimatorMaxValue = Mathf.Min(100, depotCount);
            var depotBadgeTextString = Mathf.Min(99, depotCount).ToString();
            if (depotCount > 99) depotBadgeTextString += "+";

            MetaContextElementUtils.SimpleSetIntProperty(badgeAreaElement, "Badge", depotAnimatorMaxValue);
            MetaContextElementUtils.SimpleSetText(badgeAreaElement, "Badge/Text", depotBadgeTextString, FULL);
        }

        private void UpdateLocked()
        {
            bool isInGame = BlackboardQueryUtils.IsIngame();
            if (!isInGame) return;

            var iconAnim = iconElement.GetComponent<Animator>();

            long bet = BlackboardQueryUtils.GetTotalBet();
            long minBet = BlackboardQueryUtils.GetMinEligibleBetList(ItemType.HIDDEN_UNIVERSE_FINDER)?[0] ?? 0L;

            bool isBetEnough = bet >= minBet;
            iconAnim.SetBool("IsLocked", !isBetEnough);
        }

        public IEnumerator MakeVipLoungeLoadingSceneCoroutine()
        {
            string contextId = BiEventUtils.GenerateContextID();

            // Send BI
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "ui_click");

            string bundle = VipLounge.Defines.COMMON_BUNDLE;
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

        private void UpdateLoungePoint()
        {
            if (VipLounge.Utils.BadgeCount == VipLounge.Defines.MAX_BADGE_COUNT)
            {
                MetaContextElementUtils.SetSliderValue(vipLoungeGaugeElement, 1f);
                RemainingTimerController benefitTimer = vipLoungeCountElement.gameObject.GetComponent<RemainingTimerController>();
                if (benefitTimer == null)
                    benefitTimer = vipLoungeCountElement.gameObject.AddComponent<RemainingTimerController>();
                benefitTimer.Init(
                    vipLoungeCountElement,
                    "TIME_FORMAT_HHMMSS",
                    "TEXT_NORMAL",
                    "",
                    StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_LOUNGE_GAUGE_PROGRESS_FULL_MAX_TEXT"),
                    true,
                    () => { UpdateLoungePoint(); });
                benefitTimer.StartTimer(VipLounge.Utils.BenefitEndTimestamp, 0);
            }
            else
            {
                var maxLoungePoint = VipLounge.Utils.MaxLoungePoint;
                var loungePoint = VipLounge.Utils.LoungePoint;
                float ratio = (float)loungePoint / maxLoungePoint;
                
                MetaContextElementUtils.SetSliderValue(vipLoungeGaugeElement, ratio);
                MetaContextElementUtils.SetTextGlobal(vipLoungeCountElement, "TEXT_COMMA_NUMBER", loungePoint);
            }
        }

        public bool CheckActive()
        {
            return BlackboardQueryUtils.IsVipLoungeEnabled();
        }

        public void UpdateUnlockedLevel()
        {
            bool isActive = CheckActive();

            iconAreaElement.gameObject.SetActive(isActive);
            gaugeAreaElement.gameObject.SetActive(isActive);

            lockedIconElement.gameObject.SetActive(!isActive);
            lockedSpeechBalloonElement?.gameObject.SetActive(!isActive);
        }

        public void ActiveLevelLockedSpeechBalloon()
        {
            if (ignoreSpeechBalloon) return;
            if (levelLockedSpeechBalloonEnumerator != null)
                StopCoroutine(levelLockedSpeechBalloonEnumerator);
            levelLockedSpeechBalloonEnumerator = StartCoroutine(LevelLockedSpeechBalloonEnumerator());
        }

        private IEnumerator LevelLockedSpeechBalloonEnumerator()
        {
            lockedSpeechAnimator.SetBool("IsActive", true);
            yield return new WaitForSeconds(1.0f);
            lockedSpeechAnimator.SetBool("IsActive", false);
        }

        protected override void UpdateMetaItemInfo()
        {
            UpdateLoungePoint();
        }
    }
}
