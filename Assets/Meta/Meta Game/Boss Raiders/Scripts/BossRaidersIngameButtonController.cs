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

namespace BagelCode.BossRaiders
{
    public class BossRaidersIngameButtonController : MetaGameEventButtonController
    {
        public float effectMovementTime = 0.75f;

        private Blackboard rootBB;
        private Animator buttonAnimator;
        private BossRaidersButtonIconController iconController;
        private BossRaidersBetProgressController betProgressController;
        private Animator betProgressAnim;

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
        private const string EFFECT_PREFAB_NAME = "In Game Boss Raiders Energy";

        private long backupEarnEnergy = 0L;

        private bool isEndTime = false;
        private bool isLocked = false;

        public string bundleName = "";

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

            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.Register("OnContentUIEvent", delegates.Delegate);

            if (isInit)
            {
                buttonAnimator.SetBool("IsActive", true);

                iconController.UpdateValues();
                iconController.SetRewardCallback(() => { UpdateBadge(); });

                UpdateUnlockedLevel();
                UpdateTimer();
                UpdateBadge();
                UpdateClubber();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke();
            MessageDispatcher.UnRegister("OnContentUIEvent", delegates.Delegate);
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
            if (isEndTime) ClearMetaBlackboard();
        }

        protected override ContextElement GetButtonClickable()
        {
            return buttonElement;
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if(!isInit) return;
            // Refresh lock icon.
            if(eventData.name == MetaEventDefine.ON_CLUB_REQUEST_ACCEPTED)
            {
                iconController?.UpdateValues();
                iconController?.SetLocked(false);
            }
        }

        private void ShowUI(EventData eventData)
        {
            //Invoke("ResumeEndTurn", 1.5f);
        }

        private void HideUI(EventData eventData)
        {
            CancelInvoke();
        }

        public override void InitProperty()
        {
            if (isInit) return;

            base.InitProperty();

            rootBB = gameObject.GetComponent<Blackboard>();

            buttonElement = ContextUtils.FindElement(root, "Boss Raiders Button", ContextSearchingType.ChildrenSearch);

            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(false); // todo : event ID extraction
            bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo, false);
            string sharedBundle = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, false);
            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(sharedBundle, "Boss Raiders Common Shared Sounds", transform);

            betButtonTransform = FindObjectOfType<BetButton>().transform;

            buttonAnimator = buttonElement.gameObject.GetComponent<Animator>();

            iconAreaElement = ContextUtils.FindElement(buttonElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            var iconElement = ContextUtils.FindElement(iconAreaElement, "Boss Raiders Icon", ContextSearchingType.ChildrenSearch);
            iconController = iconElement.gameObject.GetComponent<BossRaidersButtonIconController>();
            iconController.InitProperty();

            var progressElement = ContextUtils.FindElement(root, "Boss Raiders Bet Progress", CHILDREN);
            betProgressController = progressElement.GetComponent<BossRaidersBetProgressController>();
            betProgressAnim = betProgressController.GetComponent<Animator>();

            timerAreaElement = ContextUtils.FindElement(buttonElement, "Event Timer Area", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer/Remaining Timer", ContextSearchingType.FullNameSearch);

            badgeAreaElement = ContextUtils.FindElement(buttonElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAnimator = badgeObj.GetComponent<ContextAnimator>();
            badgeAnimator.UpdateContext(false);
            badgeAnimator.isPreserve = true;
            badgeAnimator.propertyName = "value";

            badgeTextElement = ContextUtils.FindElement(badgeAnimator, "Text", ContextSearchingType.ChildrenSearch);
            badgeTextElement.gameObject.SetActive(false);

            ContextElement lockedAreaElement = ContextUtils.FindElement(buttonElement, "Locked Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Boss Raiders Locked Icon", ContextSearchingType.ChildrenSearch);
            lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Boss Raiders Locked Info Speech Balloon", ContextSearchingType.ChildrenSearch);
            lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();

            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnEnterBossRaiders",
                root,
                null,
                false
            );

            BlackboardUtils.SetOrCreateValue(remainingTimerElement.GetComponent<Blackboard>(), "caller", gameObject);

            effectPrefabBundleName = sharedBundle;

            BossRaidersUtils.InitBossRaiders();
            InitBlackboard();

            isLocked = !MetaGameUtils.IsMetaGameLevelLocked();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_OPEN_META_EVENT_GROUP, OnOpenMetaEventGroup);

            isInit = true;
        }

        private void OnOpenMetaEventGroup()
        {
            if (isInit)
            {
                lockedSpeechAnimator.SetBool("IsActive", false);
                betProgressAnim.SetBool("IsActive", false);
            }
        }

        private void InitBlackboard()
        {
            if (isInit) return;

            BlackboardUtils.SetOrCreateValue(rootBB, "buttonElement", buttonElement);
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

            buttonAnimator.SetBool("IsActive", true);

            iconController.UpdateValues();
            iconController.SetRewardCallback(() => { UpdateBadge(); });

            UpdateTimer();
            UpdateBadge();
            UpdateUnlockedLevel();

            BossRaidersUtils.CheckMetaStart();
        }

        public void IncreaseExp()
        {
            iconController.GettingPoint(backupEarnEnergy);
            backupEarnEnergy = 0;
        }

        public void OnReadyGame()
        {
            if (!isInit) return;

            var betIndex = BlackboardUtils.FindVariable<int>("./betIndex");
            if (betIndex != null)
                UpdateBetIndex(betIndex.value);
        }

        public void UpdateBetIndex(int idx)
        {
            if (!isInit) return;

            Blackboard eligibleBetInfoBB = BossRaidersUtils.GetEnergyBundleInfoBB(idx);
            bool isEligible = eligibleBetInfoBB != null;

            iconController.SetLocked(!isEligible);
        }

        public void UpdateBetCredit(long betCredit)
        {
            if (!isInit) return;

            Blackboard eligibleBetInfoBB = BossRaidersUtils.GetEnergyBundleInfoBB(betCredit);
            bool isEligible = (eligibleBetInfoBB != null && eligibleBetInfoBB.GetValue<long>("totalEnergyBundleEarning") > 0L);
            iconController.SetLocked(!isEligible);
        }

        public void CheckIconLocked()
        {
            // Check : Bet Credit & isClubber
            UpdateBetCredit(BlackboardUtils.GetOrCreateVariable<long>(null, "./betCredit").value);
        }

        private void UpdateTimer()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BOSS_RAIDERS);
            if(eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        private void UpdateBadge()
        {
            if (badgeAnimator == null) return;
            // Unclaimed count
            int totalCount = BossRaidersUtils.SpinPossibleCount > 0 ? 1 : 0;
            badgeAnimator.SetIntProperty(totalCount);
            buttonAnimator.SetBool("IsBadge", totalCount > 0);

            if (totalCount > 99)
                MetaContextElementUtils.SetText(badgeTextElement, "99+");
            else
                MetaContextElementUtils.SetText(badgeTextElement, totalCount.ToString());
        }

        public void UpdateUnlockedLevel()
        {
            bool isLockedNew = MetaGameUtils.IsMetaGameLevelLocked();

            if (isLocked == isLockedNew) return;

            iconAreaElement.gameObject.SetActive(!isLockedNew);
            badgeAreaElement.gameObject.SetActive(!isLockedNew);

            lockedIconElement.gameObject.SetActive(isLockedNew);
            lockedSpeechBalloonElement.gameObject.SetActive(isLockedNew);

            isLocked = isLockedNew;
        }

        public void UpdateClubber()
        {
            //iconController?.UpdateClubber();
            bool isClubber = GetClubber();
            iconController.SetLocked(!isClubber);
        }

        public void ReturnSceneToIngame()
        {
            UpdateBadge();
            iconController.UpdateExpGauge();
        }

        public void PlayEarnPointEffect()
        {
            BossRaidersUtils.UpdateBossRaidersData();

            if (this.isActiveAndEnabled)
            {
                if (BossRaidersUtils.EarnEnergy <= 0L) return;
                backupEarnEnergy = BossRaidersUtils.EarnEnergy;

                isItemEarning = true;

                earnEffectObj = MetaObjectUtils.MakePrefab(effectPrefabBundleName, EFFECT_PREFAB_NAME, earningItemRootArea);
                if (earnEffectObj == null) return;

                var controller = earnEffectObj.GetComponent<EarningMetaGameItemController>();
                controller.flyingTime = effectMovementTime;

                Vector3 from = betButtonTransform.position;
                Vector3 to = transform.position;

                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POINTS_FLY).Play();

                AsyncActionUtils.ApplyMovement(
                    this,
                    earnEffectObj.transform,
                    from,
                    to,
                    effectMovementTime,
                    TweenUtils.VectorTweenCollectMove,
                    0f,
                    OnArriveEarningMetaGameItem
                );
            }
        }

        protected override void UpdateMetaItemInfo()
        {
            IncreaseExp();
        }

        public bool GetClubber()
        {
            return ClubUtils.IsClubber();
        }

        public void OpenNonClubberPopup()
        {
            bool stringError = false;
            ErrorPopupInfo info = new ErrorPopupInfo();
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_NON_CLUBBER_TEXT", out stringError);
            info.type = ErrorPopupType.OK;
            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

            ErrorPopupHandler.Instance.OpenError(info);
        }

        public IEnumerator OpenPopupClubCoroutine(string biContextId)
        {
            if (!MetaGameUtils.IsPlayingMetaGame())
            {
                OpenNonClubberPopup();
            }
            else
            {
                var reqSceneBB = MetaObjectUtils.MakeScene<Blackboard>("Popup Club Join Scene", PopupManager.Instance.transform);
                PopupManager.Instance.Open(reqSceneBB.gameObject);
                BlackboardUtils.SetOrCreateValue(reqSceneBB, "caller", gameObject);
                BlackboardUtils.SetOrCreateValue(reqSceneBB, "_biContextID", biContextId);
                BlackboardUtils.SetOrCreateValue(reqSceneBB, "fromType", "in_game_boss_raiders");

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }
        }

        public void ClearMetaBlackboard()
        {
            isEndTime = true;
            BossRaidersUtils.ClearBlackboard();
        }

        public bool CheckMetaGameData()
        {
            return BossRaidersUtils.BossRaidersInfo != null;
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

        public void SetMetaGameCrashReport()
        {
            BlackboardQueryUtils.MetaGameCrashReport(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_NAME"));
        }

        public void BIClientClickBossRaidersIcon(string contextId, string type)
        {
            BossRaidersUtils.BIClientClickBossRaidersIcon(contextId, type);
        }

#if UNITY_EDITOR

        [Button]
        private void TestPlayEarnPointEffect()
        {
            if (this.isActiveAndEnabled)
            {
                Debug.Log("Play Test Effect");

                GameObject effectGO = MetaObjectUtils.MakePrefab(effectPrefabBundleName, EFFECT_PREFAB_NAME, earningItemRootArea);
                Debug.Log("Created Game Object: " + effectGO);

                Vector3 from = betButtonTransform.position;
                Vector3 to = transform.position;

                AsyncActionUtils.ApplyMovement(this, effectGO.transform, from, to, effectMovementTime, TweenUtils.VectorTweenCollectMove,
                    0f, () => { if (!(effectGO is null)) Destroy(effectGO); });
            }
        }

        [Button]
        private void TestPopupButton(string sceneName)
        {
            if (this.isActiveAndEnabled)
            {
                var reqSceneBB = MetaObjectUtils.MakeScene<Blackboard>(sceneName, PopupManager.Instance.transform);
                PopupManager.Instance.Open(reqSceneBB.gameObject);

                reqSceneBB.SetValue("caller", gameObject);
            }
        }
#endif
    }
}
