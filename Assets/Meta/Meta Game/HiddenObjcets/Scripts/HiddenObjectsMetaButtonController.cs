using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using System.Collections;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsMetaButtonController : MetaGameEventButtonController
    {
        private const float FINDER_GAUGE_MIN = 0.12f;

        private ContextElement badgeAreaElement;

        private ContextElement finderGaugeElement;
        private ContextElement finderCountElement;

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
                UpdateAnims();
                UpdateFinderCount();
                UpdateLocked();
            }
        }

        public override void InitProperty()
        {
            if (isInit) return;

            base.InitProperty();
            
            // Make Sound Asset
            string bundle = HiddenObjects.Defines.COMMON_BUNDLE;
            MetaObjectUtils.MakePrefab(bundle, "Hidden Objects Common Sounds", transform);

            finderGaugeElement = ContextUtils.FindElement(root, "Gauge Area/Hidden Objects Icon Gauge/Progress Bar", FULL);
            finderCountElement = ContextUtils.FindElement(root, "Gauge Area/Hidden Objects Icon Gauge/Text", FULL);

            iconAreaElement = ContextUtils.FindElement(root, "Icon Area", CHILDREN);
            gaugeAreaElement = ContextUtils.FindElement(root, "Gauge Area", CHILDREN);
            badgeAreaElement = ContextUtils.FindElement(root, "Badge Area", CHILDREN);

            iconElement = ContextUtils.FindElement(iconAreaElement, "Hidden Objects Game Icon", CHILDREN);

            int level = BlackboardQueryUtils.GetFeatureMinLevel(ClientModels.LockedFeatureType.HIDDEN_UNIVERSE);

            ContextElement lockedAreaElement = ContextUtils.FindElement(root, "Locked Area", CHILDREN);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Hidden Objects Icon Locked", CHILDREN);
            MetaContextElementUtils.SimpleSetText(lockedIconElement, "Text", level.ToString());

            if (!ignoreSpeechBalloon)
            {
                lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Hidden Objects Locked Info Speech Balloon", CHILDREN);
                lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();
                MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", CHILDREN, level);
            }

            MakeBadgeIcon();
            UpdateFinderCount();
            UpdateLocked();

            MetaContextElementUtils.SetClickable(
                root, gameObject, HiddenObjects.Events.ON_ENTER_HIDDEN_OBJECTS, false, false);

            RegisterHandleEventType(MetaEventDefine.ON_CREDIT_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_UPDATE_FINDER_COUNT, UpdateFinderCount);

            Register(MetaEventDefine.ON_CREDIT_EVENT, "UpdatedTotalBetCredit", UpdateLocked);

            isInit = true;
        }

        private void UpdateAnims()
        {
            UpdateLocked();
            UpdateBadgeAnim();
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

        private void UpdateBadgeAnim()
        {
            int finder = HiddenObjects.Utils.Finder;
            int lastStageNeedFinder = HiddenObjects.Utils.LastStageNeedFinder;
            int playableCount = 0;

            if (lastStageNeedFinder > 0)
                playableCount = finder / lastStageNeedFinder;

            if (playableCount > 0)
            {
                MetaContextElementUtils.SetActive(badgeElement, true);
                const int BADGE_MAX_NUMBER = 99;
                if (playableCount <= BADGE_MAX_NUMBER)
                {
                    MetaContextElementUtils.SimpleSetText(badgeElement, "Text", playableCount.ToString());
                    MetaContextElementUtils.SetPropertySafty(badgeElement, playableCount);
                }
                else
                {
                    MetaContextElementUtils.SimpleSetText(badgeElement, "Text", "99+");
                    MetaContextElementUtils.SetPropertySafty(badgeElement, BADGE_MAX_NUMBER + 1);
                }
            }
            else
            {
                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", "0");
                MetaContextElementUtils.SetPropertySafty(badgeElement, playableCount);
            }
        }

        private void MakeBadgeIcon()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Badge";
            Transform parent = badgeAreaElement.transform;

            var badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            badgeElement = badgeObj.GetComponent<ContextElement>();
            badgeElement.UpdateContext(true);

            MetaContextElementUtils.SetActive(badgeElement, false);

            UpdateBadgeAnim();
        }

        public IEnumerator MakeHiddenObjectsLoadingSceneCoroutine()
        {
            var orientation = BlackboardQueryUtils.GetOrientation();
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "prevOrientation", orientation);

            if(orientation != Orientation.LANDSCAPE)
            {
                MetaGameUtils.SetOrientation(Orientation.LANDSCAPE, gameObject);

                var callbackTrigger = new EventTrigger(gameObject, "OnFinishedChangeOrientation");
                yield return new WaitUntilTrigger(callbackTrigger);
            }

            string contextId = BiEventUtils.GenerateContextID();

            // Send BI
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "ui_click");

            string bundle = HiddenObjects.Defines.COMMON_BUNDLE;
            string asset = "Hidden Objects Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = MetaObjectUtils.MakeScene(bundle, asset, parent, "");

            MetaPopupUtils.OpenPopup(loadingObj);

            string metaGroupContextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "metaGroupContextId")?.value;

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            loadingBB.AddVariable("_biContextID", contextId);
            loadingBB.AddVariable("isEnter", true);
            loadingBB.AddVariable("isMetaInGame", false);

            if (!string.IsNullOrEmpty(metaGroupContextId))
            {
                BlackboardUtils.SetOrCreateValue(loadingBB, "metaGroupContextId", metaGroupContextId);
            }
        }

        private void UpdateFinderCount()
        {
            var hogInfo = HiddenObjects.Utils.HiddenObjectsInfo;
            if (hogInfo == null) return;

            int maxFinder = HiddenObjects.Utils.MaxFinder;
            int finder = HiddenObjects.Utils.Finder;
            float ratio = (float)finder / maxFinder;
            ratio = Mathf.Lerp(FINDER_GAUGE_MIN, 1f, ratio);
            MetaContextElementUtils.SetSliderValue(finderGaugeElement, ratio);

            MetaContextElementUtils.SetTextGlobal(finderCountElement, "A_PER_B", finder, maxFinder);

            UpdateBadgeAnim();
        }

        public bool CheckActive()
        {
            return BlackboardQueryUtils.IsHiddenObjectsActiveForUser();
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

        protected override void OnEarnMetaGameItem()
        {
            var hogInfo = HiddenObjects.Utils.HiddenObjectsInfo;
            if (hogInfo == null) return;

            var earnFinder = hogInfo.GetVariable<int>("earnFinderFromSpin");
            if (earnFinder == null || earnFinder.value <= 0) return;

            if (this.isActiveAndEnabled)
            {
                isItemEarning = true;

                earnFinder.value = 0;

                string bundle = HiddenObjects.Defines.COMMON_BUNDLE;
                string asset = "Hidden Objects In Game Finder";
                Transform parent = earningItemRootArea;

                earnEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                if (earnEffectObj == null) return;

                float EFFECT_MOVEMENT_TIME = 1.3f;
                var controller = earnEffectObj.GetComponent<EarningMetaGameItemController>();
                controller.flyingTime = EFFECT_MOVEMENT_TIME;

                Vector3 from = MetaGameAppearTransformManager.Instance.GetTransform("SpinButton").position;
                Vector3 to = transform.position;

                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_FINDER_OBTAIN1).Play();

                AsyncActionUtils.ApplyMovement(this,
                    earnEffectObj.transform, from, to, EFFECT_MOVEMENT_TIME,
                    TweenUtils.VectorTweenCollectMove,
                    0f,
                    () =>
                    {
                        OnArriveEarningMetaGameItem();

                        GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_FINDER_OBTAIN2).Play();
                    });
            }
        }

        protected override void UpdateMetaItemInfo()
        {
            UpdateBadgeAnim();
            UpdateFinderCount();
        }
    }
}
