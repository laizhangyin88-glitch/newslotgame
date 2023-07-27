using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.ClubArena
{
    public class ClubArenaIngameButtonController : ClubArenaButtonBaseController
    {
        public float effectMovementTime = 0.75f;

        private Blackboard rootBB;
        private Animator buttonAnimator;

        private Animator betProgressAnim;

        private ContextElement buttonElement;

        private Transform betButtonTransform;

        private string effectPrefabBundleName;
        private const string EFFECT_PREFAB_NAME = "In Game Club Arena Energy";

        private long backupEarnEnergy = 0;

        private bool isEndTime = false;
        private bool isLockedLevelBackup = false;

        private MessageDelegates delegates;

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
                buttonAnimator.SetBool("IsActive", true);

                UpdateUnlockedLevel();
                UpdateTimer();
                UpdateBadge();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke();
            MessageDispatcher.UnRegister("OnContentUIEvent", delegates.Delegate);
            if (isEndTime) ClearMetaBlackboard();
        }

        private void ShowUI(EventData eventData)
        {
            //Invoke("ResumeEndTurn", 1.5f);
        }

        private void HideUI(EventData eventData)
        {
            CancelInvoke();
        }

        protected override ContextElement GetButtonClickable()
        {
            if (buttonElement == null)
                buttonElement = ContextUtils.FindElement(root, "Club Arena Button", ContextSearchingType.ChildrenSearch);
            return buttonElement;
        }

        public override void InitProperty()
        {
            if (isInit) return;

            rootBB = gameObject.GetComponent<Blackboard>();

            base.InitProperty();

            if (buttonElement == null)
                buttonElement = ContextUtils.FindElement(root, "Club Arena Button", ContextSearchingType.ChildrenSearch);
            buttonAnimator = buttonElement.gameObject.GetComponent<Animator>();

            InitProperty(buttonElement);

            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnEnterClubArena",
                root,
                null,
                false
            );

            betButtonTransform = FindObjectOfType<BetButton>().transform;

            effectPrefabBundleName = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.CLUB_ARENA);

            var betProgressElement = ContextUtils.FindElement(root, "Club Arena Bet Progress", CHILDREN);
            betProgressAnim = betProgressElement.GetComponent<Animator>();

            InitBlackboard();

            isLockedLevelBackup = !MetaGameUtils.IsMetaGameLevelLocked();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_OPEN_META_EVENT_GROUP, OnOpenMetaEventGroup);

            isInit = true;
        }

        private void OnOpenMetaEventGroup()
        {
            if (isInit)
            {
                betProgressAnim.SetBool("IsActive", false);
            }
        }

        private void InitBlackboard()
        {
            BlackboardUtils.SetOrCreateValue(rootBB, "buttonElement", buttonElement);
        }

        protected override void InitIconController()
        {
            buttonAnimator.SetBool("IsActive", true);

            base.InitIconController();
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

            Blackboard eligibleBetInfoBB = ClubArenaUtils.GetEnergyBundleInfoBB(idx);
            bool isEligible = eligibleBetInfoBB != null;

            iconController.SetLocked(!isEligible);
        }

        public void UpdateBetCredit(long betCredit)
        {
            if (!isInit) return;

            Blackboard eligibleBetInfoBB = ClubArenaUtils.GetEnergyBundleInfoBB(betCredit);
            bool isEligible = (eligibleBetInfoBB != null && eligibleBetInfoBB.GetValue<long>("maximumEnergy") > 0);

            iconController.SetLocked(!isEligible);
        }

        public void CheckIconLocked()
        {
            //iconController.UpdateValues();
            UpdateBetCredit(BlackboardUtils.GetOrCreateVariable<long>(null, "./betCredit").value);
        }

        protected override void UpdateBadge()
        {
            base.UpdateBadge();
            buttonAnimator.SetBool("IsBadge", ClubArenaUtils.SpinPossibleCount > 0);
        }

        public void PlayEarnPointEffect()
        {
            ClubArenaUtils.UpdateClubArenaData();

            if (this.isActiveAndEnabled)
            {
                if (ClubArenaUtils.EarnEnergy <= 0) return;
                backupEarnEnergy = ClubArenaUtils.EarnEnergy;
                ClubArenaUtils.EarnEnergy = 0;

                isItemEarning = true;

                earnEffectObj = MetaObjectUtils.MakePrefab(effectPrefabBundleName, EFFECT_PREFAB_NAME, earningItemRootArea);
                var controller = earnEffectObj.GetComponent<EarningMetaGameItemController>();
                controller.flyingTime = effectMovementTime;

                Vector3 from = betButtonTransform.position;
                Vector3 to = transform.position;

                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_ENERGY_FLY).Play();

                AsyncActionUtils.ApplyMovement(
                    this,
                    earnEffectObj.transform,
                    from,
                    to,
                    effectMovementTime,
                    TweenUtils.VectorTweenCollectMove,
                    0f,
                    () =>
                    {
                        BlackboardQueryUtils.ClearMetaGameSpinInterrupt();
                    }
                );

                StartCoroutine(OnArriveEarnEffect(effectMovementTime));
            }
        }

        protected override void OnArriveEarningMetaGameItem()
        {
            base.OnArriveEarningMetaGameItem();

            IncreaseExp();
            BlackboardQueryUtils.ClearMetaGameSpinInterrupt();
        }

        private IEnumerator OnArriveEarnEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            isItemEarning = false;
            earnEffectObj = null;

            IncreaseExp();
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
                BlackboardUtils.SetOrCreateValue(reqSceneBB, "fromType", "in_game_club_arena");

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }
        }

        public void ReturnSceneToIngame()
        {
            UpdateBadge();
            iconController.UpdateExpGauge();
        }

        public override void UpdateUnlockedLevel()
        {
            if (isLockedLevelBackup != MetaGameUtils.IsMetaGameLevelLocked())
            {
                isLockedLevelBackup = MetaGameUtils.IsMetaGameLevelLocked();
                base.UpdateUnlockedLevel();
            }
        }
    }
}
