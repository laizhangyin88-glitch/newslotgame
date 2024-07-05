using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class BetButton : EventMonoBehaviour
    {
        public string betDownSound;
        public string betUpSound;

        protected Variable<int> levelRestriction;
        public int LevelRestriction => levelRestriction.value;

        protected Variable<bool> isGameSpin;
        public bool IsGameSpin => isGameSpin.value;

        protected Variable<SpinType> spinType;
        public SpinType SpinType => spinType.value;

        protected Variable<int> gameSpinCount;
        public int GameSpinCount => gameSpinCount.value;

        protected Variable<int> betIndex;
        public int BetIndex => betIndex.value;

        protected Variable<int> extraBetRatioIndex;
        public int ExtraBetRatioIndex => extraBetRatioIndex.value;

        private Variable<int> extendedValue;
        public int ExtendedValue => extendedValue.value;

        protected Variable<List<Blackboard>> extraBetRatioList;
        public List<Blackboard> ExtraBetRatioList => extraBetRatioList.value;

        protected Variable<int> maxBetIndex;
        public int MaxBetIndex => maxBetIndex.value;

        private Variable<long> maxBetCredit;
        public long MaxBetCredit
        {
            set => maxBetCredit.value = value;
        }

        private long totalBetCredit;

        protected ContextElement root;
        protected ContextElement buttonMinus;
        protected ContextElement buttonPlus;
        protected ContextElement textLevelRestriction;
        protected ContextElement restrictionBalloon;
        protected ContextElement textBetCredit;

        private ContextElement highRollerButtonMinus;
        private ContextElement highRollerButtonPlus;
        private ContextElement highRollerElement;
        private ContextElement highRollerTextCreditElement;
        private Animator highRollerAnim;

        public virtual void UpdatedTotalBetCredit(long totalCredit)
        {
            //Debug.LogError("i am UpdatedTotalBetCredit  01");
            totalBetCredit = totalCredit;
            UpdateButtonState();

            ContextUtils.SetGlobalText(textBetCredit, "TEXT_BET_CREDIT", totalCredit);
            if (highRollerTextCreditElement != null)
            {
                ContextUtils.SetGlobalText(highRollerTextCreditElement, "TEXT_BET_CREDIT", totalCredit);
            }
        }

        public void BetMax()
        {
            HideRestrictionBalloon();
        }

        public void BetDown()
        {
            long prevBetCredit = totalBetCredit;
            MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData<int>("UpdateBetIndex", BetIndex - 1));

            if(prevBetCredit != totalBetCredit)
                client_click_bet(prevBetCredit, totalBetCredit);

            GSManager.Instance.GetHandler(betDownSound).Play();
            if (!IsGameSpin)
                HideRestrictionBalloon();
        }

        public void BetUp()
        {
            long prevBetCredit = totalBetCredit;
            MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData<int>("UpdateBetIndex", BetIndex + 1));

            if(prevBetCredit != totalBetCredit)
                client_click_bet(prevBetCredit, totalBetCredit);

            GSManager.Instance.GetHandler(betUpSound).Play();
            if (!IsGameSpin)
            {
                if (LevelRestriction > 0)
                    ShowRestrictionBalloon();
                else 
                    HideRestrictionBalloon();
            }            
        }

        protected override void Awake()
        {
            base.Awake();

            levelRestriction    = BlackboardUtils.FindVariable<int>("./levelRestriction");
            isGameSpin          = BlackboardUtils.FindVariable<bool>("./isGameSpin");
            spinType            = BlackboardUtils.FindVariable<SpinType>("./spinType");
            gameSpinCount       = BlackboardUtils.FindVariable<int>("./gameSpinCount");
            betIndex            = BlackboardUtils.FindVariable<int>("./betIndex");
            extraBetRatioIndex  = BlackboardUtils.GetOrCreateVariable<int>(null, "./extraBetRatioIndex");
            extraBetRatioList   = BlackboardUtils.FindVariable<List<Blackboard>>("./game/extraBetRatioList");
            maxBetIndex         = BlackboardUtils.FindVariable<int>("./maxBetIndex");
            maxBetCredit        = BlackboardUtils.FindVariable<long>("./maxBetCredit");

            extendedValue       = BlackboardUtils.GetOrCreateVariable<int>("/extendedValue");
        }

        protected virtual void Start()
        {
            UpdateContext();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, "UpdateButtonState", UpdateButtonState);
            Register(MetaEventDefine.ON_META_UI_EVENT, "UpdateLevelRestriction", UpdateLevelRestriction);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // levelRestriction.onValueChanged += UpdateLevelRestriction;
            spinType.onValueChanged         += UpdateIsSpinType;
            gameSpinCount.onValueChanged    += UpdateGameSpinCount;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // levelRestriction.onValueChanged -= UpdateLevelRestriction;
            spinType.onValueChanged         -= UpdateIsSpinType;
            gameSpinCount.onValueChanged    -= UpdateGameSpinCount;
        }

        private void UpdateLevelRestriction()
        {
            if (LevelRestriction > 0)
            {
                ContextUtils.SetText(textLevelRestriction, LevelRestriction.ToString());
                buttonPlus.GetComponent<Button>().interactable = false;
                buttonPlus.GetComponent<Animator>().SetBool("Lock", true);
            }
            else
            {
                buttonPlus.GetComponent<Button>().interactable = BetIndex < MaxBetIndex;
                buttonPlus.GetComponent<Animator>().SetBool("Lock", false);
            }
        }

        protected void UpdateLevelRestriction(string name, object value)
        {
            UpdateLevelRestriction();
        }

        protected virtual void UpdateIsSpinType(string name, object value) {}
        protected virtual void UpdateGameSpinCount(string name, object value) {}

        protected virtual void UpdateContext()
        {
            if (root != null) return;

            root = GetComponent<ContextElement>();

            root.UpdateContext();

            buttonMinus = ContextUtils.FindElement(root, "Button Minus", ContextSearchingType.ChildrenSearch);
            buttonPlus = ContextUtils.FindElement(root, "Button Plus", ContextSearchingType.ChildrenSearch);
            textBetCredit = ContextUtils.FindElement(root, "Text Bet", ContextSearchingType.ChildrenSearch);
            textLevelRestriction = ContextUtils.FindElement(buttonPlus, "Icon Lock/Text", ContextSearchingType.FullNameSearch);

            highRollerElement = ContextUtils.FindElement(root, "Spin Cover/Bet High Roller", ContextSearchingType.FullNameSearch);
            if (highRollerElement == null)
                highRollerElement = ContextUtils.FindElement(root, "Spin Cover/Bet High Roller Vertical Mode", ContextSearchingType.FullNameSearch);
            if (highRollerElement == null)
                highRollerElement = ContextUtils.FindElement(root, "Spin Cover/Bet High Roller Video Poker", ContextSearchingType.FullNameSearch);

            if (highRollerElement != null)
            {
                highRollerButtonMinus = ContextUtils.FindElement(highRollerElement, "Button Minus", ContextSearchingType.ChildrenSearch);
                highRollerButtonPlus = ContextUtils.FindElement(highRollerElement, "Button Plus", ContextSearchingType.ChildrenSearch);

                highRollerAnim = highRollerElement.GetComponent<Animator>();
                highRollerTextCreditElement = ContextUtils.FindElement(highRollerElement, "Text Bet", ContextSearchingType.ChildrenSearch);
            }

            restrictionBalloon = ContextUtils.FindElement(buttonPlus, "Chatting Mini Area/Speech Balloon Common", ContextSearchingType.FullNameSearch);
            if (restrictionBalloon != null)
            {
                ContextUtils.SetGlobalText(restrictionBalloon.Find("Text"), "POPUP_INGAME_LEVEL_RESTRICTION_BALLOON_TEXT");
            }
        }

        public void UpdateButtonState()
        {
            bool isAutoSpin = BlackboardQueryUtils.IsAutoSpin();

            // High Roller State
            if (highRollerAnim != null)
            {
                // vip high roller
                bool isActiveHighRoller =
                    ExtendedValue > 0 &&
                    (ExtendedValue > MaxBetIndex - BetIndex);

                highRollerAnim.SetBool("Appear", isActiveHighRoller);

                if (isActiveHighRoller && highRollerButtonMinus != null && highRollerButtonPlus != null)
                {
                    highRollerButtonMinus.GetComponent<Button>().interactable = false;
                    highRollerButtonPlus.GetComponent<Button>().interactable = false;

                    highRollerButtonMinus.GetComponent<Button>().interactable = BetIndex > 0 && !isAutoSpin;
                    highRollerButtonPlus.GetComponent<Button>().interactable = BetIndex < MaxBetIndex && !isAutoSpin;
                }
            }

            // Unity Interactable Bug.
            if (buttonMinus != null && buttonPlus != null)
            {
                buttonMinus.GetComponent<Button>().interactable = false;
                buttonPlus.GetComponent<Button>().interactable = false;

                buttonMinus.GetComponent<Button>().interactable = BetIndex > 0 && !isAutoSpin;
                buttonPlus.GetComponent<Button>().interactable = BetIndex < MaxBetIndex && !isAutoSpin;
            }
        }

        private void ShowRestrictionBalloon()
        {
            if (restrictionBalloon != null)
                restrictionBalloon.GetComponent<Animator>().SetTrigger("Appear");
        }

        private void HideRestrictionBalloon()
        {
            if (restrictionBalloon != null)
                restrictionBalloon.GetComponent<Animator>().SetTrigger("Disappear");
        }

        public void client_click_bet(long prevBetCredit, long currentBetCredit)
        {
            int gameId = BlackboardUtils.FindVariable<int>("./game/gameId").value;

            Analytics.CustomEvent("client_click_bet_change", new Dictionary<string, object>
            {
                { "game_id", gameId },
                { "bet", currentBetCredit },
                { "previous_bet", prevBetCredit }
            });
        }
    }
}
