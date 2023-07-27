using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public class EpicPassIngameButtonController : MetaGameEventButtonController
    {
        public float effectMovementTime = 0.75f;

        private Animator buttonAnimator;

        private EpicPassButtonIconController iconController;

        private GameObject gs_managerObj;

        private ContextElement iconAreaElement;

        private ContextElement buttonElement;
        private ContextElement timerAreaElement;
        private ContextElement remainingTimerElement;

        private ContextAnimator badgeAnimator;
        private ContextElement badgeAreaElement;
        private ContextElement badgeTextElement;

        private ContextElement lockedIconElement;
        private ContextElement lockedSpeechBalloonElement;
        private Animator lockedSpeechAnimator;

        private Transform betButtonTransform;

        private string effectPrefabBundleName;
        private string effectPrefabAssetName;

        private const string EFFECT_PREFAB_ASSET_NAME = "Epic Pass Point Web Image Fx";

        private bool isLockedLevelBackup = false;

        private bool isInterruptFlyEffect = false;
        private bool isReserveFlyEffect = false;

        private MessageDelegates delegates;
        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        protected override void Awake()
        {
            base.Awake();
            delegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "ShowUI", ShowUI },
                    { "HideUI", HideUI }
                }
            );
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MessageDispatcher.Register("OnContentUIEvent", delegates.Delegate);

            if (isInit)
            {
                UpdateBadge();
                UpdateTimer();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke();
            MessageDispatcher.UnRegister("OnContentUIEvent", delegates.Delegate);
        }

        private void ShowUI(EventData eventData)
        {
            Invoke("ResumeEndTurn", 1.5f);
        }

        private void HideUI(EventData eventData)
        {
            isInterruptFlyEffect = true;
            isReserveFlyEffect = false;
            CancelInvoke();
        }

        protected override ContextElement GetButtonClickable()
        {
            if (buttonElement == null)
                buttonElement = ContextUtils.FindElement(root, "Epic Pass Button", ContextSearchingType.ChildrenSearch);
            return buttonElement;
        }

        public override void InitProperty()
        {
            if(isInit) return;

            base.InitProperty();

            if (buttonElement == null)
                buttonElement = ContextUtils.FindElement(root, "Epic Pass Button", ContextSearchingType.ChildrenSearch);

            string bundle = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS);
            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(bundle, "Epic Pass Common Sounds", transform);

            // buttonAnimator = buttonElement.gameObject.GetComponent<Animator>();

            iconAreaElement = ContextUtils.FindElement(buttonElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            var iconElement = ContextUtils.FindElement(iconAreaElement, "Epic Pass Icon", ContextSearchingType.ChildrenSearch);
            iconController = iconElement.gameObject.GetComponent<EpicPassButtonIconController>();
            iconController.InitProperty();

            timerAreaElement = ContextUtils.FindElement(buttonElement, "Event Timer Area", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer/Remaining Timer", ContextSearchingType.FullNameSearch);

            badgeAreaElement = ContextUtils.FindElement(buttonElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAnimator = badgeObj.GetComponent<ContextAnimator>();
            badgeAnimator.UpdateContext(false);
            badgeAnimator.isPreserve = true;
            badgeAnimator.propertyName = "value";

            badgeTextElement = ContextUtils.FindElement(badgeAnimator, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement lockedAreaElement = ContextUtils.FindElement(buttonElement, "Locked Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Epic Pass Icon Locked", ContextSearchingType.ChildrenSearch);
            lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Epic Pass Locked Info Speech Balloon", ContextSearchingType.ChildrenSearch);
            lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();

            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnEnterSeasonPass",
                root,
                null,
                false
            );

            betButtonTransform = FindObjectOfType<BetButton>().transform;

            effectPrefabBundleName = bundle;
            effectPrefabAssetName = EFFECT_PREFAB_ASSET_NAME;

            isLockedLevelBackup = !MetaGameUtils.IsMetaGameLevelLocked();

            isInit = true;
        }

        private void InitText()
        {
            int level = MetaGameUtils.GetMetaUnlockedLevel();
            MetaContextElementUtils.SimpleSetText(lockedIconElement, "Text", level.ToString());
            MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", ContextSearchingType.ChildrenSearch, level);
        }

        public void OnInit()
        {
            InitProperty();
            InitText();

            iconController.UpdateValues();
            iconController.SetRewardCallback(()=>{UpdateBadge();});

            UpdateTimer();
            UpdateBadge();
            UpdateUnlockedLevel();

            // buttonAnimator.SetBool("IsActive", true);
        }

        public void ResumeEndTurn()
        {
            isInterruptFlyEffect = false;
            if(isReserveFlyEffect)
                OnEarnMetaGameItem();
        }

        protected override void UpdateMetaItemInfo()
        {
            iconController.GettingPointEnd();
        }

        protected override void OnArriveEarningMetaGameItem()
        {
            isItemEarning = false;

            if (earnEffectObj != null) Destroy(earnEffectObj);

            iconController.GettingPoint();
        }

        protected override void OnEarnMetaGameItem()
        {
            isItemEarning = true;

            if(isInterruptFlyEffect)
            {
                isReserveFlyEffect = true;
            }
            else
            {
                isReserveFlyEffect = false;
                PlayEarnPointEffect();
            }
        }

        public void IncreaseExp()
        {
            isItemEarning = false;
            iconController.GettingPoint();
        }

        public void OnReadyGame()
        {
            if (!isInit) return;

            var totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            if(totalBetCredit != null)
                UpdateTotalBet(totalBetCredit.value);
        }

        public void UpdateTotalBet(long totalBetCredit)
        {
            if (!isInit) return;

            Blackboard eligibleBetInfoBB = EpicPassUtils.GetEligibleBetInfoBB(totalBetCredit);
            bool isEligible = eligibleBetInfoBB != null;

            iconController.SetLocked(!isEligible);
        }

        private void UpdateTimer()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.SEASON_PASS);
            if(eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        // from FSM
        private void UpdateBadge()
        {
            if(badgeAnimator == null) return;
            // Unclaimed count
            int totalCount = EpicPassUtils.UnclaimedRewardCount;
            badgeAnimator.SetIntProperty(totalCount);

            if (totalCount > 99)
                MetaContextElementUtils.SetText(badgeTextElement, "99+");
            else
                MetaContextElementUtils.SetText(badgeTextElement, totalCount.ToString());
        }

        public void UpdateUnlockedLevel()
        {
            bool isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

            if (isLockedLevelBackup == isLockedLevel) return;

            iconAreaElement.gameObject.SetActive(!isLockedLevel);
            badgeAreaElement.gameObject.SetActive(!isLockedLevel);

            lockedIconElement.gameObject.SetActive(isLockedLevel);
            lockedSpeechBalloonElement.gameObject.SetActive(isLockedLevel);

            isLockedLevelBackup = isLockedLevel;
        }

        public void ReturnSceneToIngame()
        {
            UpdateBadge();
            iconController.UpdateExpGauge();
        }

        private void PlayEarnPointEffect()
        {
            if (this.isActiveAndEnabled)
            {
                if (EpicPassUtils.PrevLevel >= EpicPassUtils.MaxLevel) return;
                if (EpicPassUtils.EarnPoint <= 0) return;

                earnEffectObj = MetaObjectUtils.MakePrefab(effectPrefabBundleName, effectPrefabAssetName, earningItemRootArea);
                if(earnEffectObj is null)
                {
                    Debug.LogWarning("PlayEarnPointEffect.MakePrefab failure. effectPrefabBundleName: " + effectPrefabBundleName + ", effectPrefabAssetName: " + effectPrefabAssetName);
                    return;
                }

                var controller = earnEffectObj.GetComponent<EarningMetaGameItemController>();
                controller.flyingTime = effectMovementTime;

                Vector3 from = betButtonTransform.position;
                Vector3 to = transform.position;

                GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_FLY_POINT).Play();

                AsyncActionUtils.ApplyMovement(
                    this,
                    earnEffectObj.transform,
                    from,
                    to,
                    effectMovementTime,
                    TweenUtils.VectorTweenCollectMove,
                    0f,
                    OnArriveEarningMetaGameItem);
            }
        }

        public bool CheckUnlockedLevel()
        {
            return MetaGameUtils.IsMetaGameLevelLocked();
        }

        public void ActiveLevelLockedSpeechBalloon()
        {
            if (levelLockedSpeechBalloonEnumerator != null)
                StopCoroutine(levelLockedSpeechBalloonEnumerator);
            levelLockedSpeechBalloonEnumerator = StartCoroutine(LevelLockedSpeechBalloonEnumerator());
        }

        private IEnumerator LevelLockedSpeechBalloonEnumerator()
        {
            lockedSpeechAnimator?.SetBool("IsActive", true);
            yield return new WaitForSeconds(1.0f);
            lockedSpeechAnimator?.SetBool("IsActive", false);
        }

        public void SetActiveButtonIcon(bool isActive)
        {
            buttonElement.gameObject.SetActive(isActive);
        }

        public void SetMetaGameCrashReport()
        {
            BlackboardQueryUtils.MetaGameCrashReport(StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_NAME"));
        }

#if UNITY_EDITOR

        [Button]
        private void TestPlayEarnPointEffect()
        {
            if (this.isActiveAndEnabled)
            {
                Debug.Log("Play Test Effect");

                GameObject effectGO = MetaObjectUtils.MakePrefab(effectPrefabBundleName, effectPrefabAssetName, betButtonTransform);
                Debug.Log("Created Game Object: " + effectGO);

                Vector3 from = betButtonTransform.position;
                Vector3 to = transform.position;

                AsyncActionUtils.ApplyMovement(this, effectGO.transform, from, to, effectMovementTime, TweenUtils.VectorTweenCollectMove,
                    0f, () => { if (!(effectGO is null)) Destroy(effectGO); });
            }
        }
#endif
    }
}
