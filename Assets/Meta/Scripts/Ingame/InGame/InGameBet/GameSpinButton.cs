using System.Collections;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class GameSpinButton : BetButton
    {
        public Animator animator;
        public string appearGameSpinSound;
        public string disappearGameSpinSound;

        protected ContextElement textGameSpinCount;
        protected ContextElement textGameSpinCountPlural;
        protected IContextText textSpinType;

        private bool gameReady;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        public void ReadyGame()
        {
            gameReady = true;
        }

        public override void UpdatedTotalBetCredit(long totalCredit)
        {
            UpdateButtonState();

            if(IsGameSpin)
                ContextUtils.SetGlobalText(textBetCredit, "TEXT_BET_CREDIT", totalCredit);
        }

        protected override void UpdateIsSpinType(string name, object value)
        {
            bool appear = IsGameSpin && (SpinType != SpinType.BuyABonus);
            bool stateChanged = (appear != animator.GetBool("Appear"));
            animator.SetBool("Appear", appear);

            string buttonTextKey = null;

            switch(SpinType)
            {
                case SpinType.GameSpin:
                    buttonTextKey = "GAME_SPIN_TEXT_LUCKY";
                    break;
                case SpinType.BonusSpin:
                    buttonTextKey = "GAME_SPIN_TEXT_BONUS";
                    break;
            }

            if(!string.IsNullOrEmpty(buttonTextKey))
            {
                bool error;
                textSpinType.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, buttonTextKey, out error));
            }

            //
            if (gameReady && stateChanged)
                GSManager.Instance.GetHandler(appear ? appearGameSpinSound : disappearGameSpinSound).Play();
        }

        protected override void UpdateGameSpinCount(string name, object value)
        {
            ContextUtils.SetGlobalText(textGameSpinCount, "TEXT_COMMA_NUMBER", GameSpinCount);
            ContextUtils.SetGlobalText(textGameSpinCountPlural, "INGAME_GAME_SPIN_PLURAL_TEXT", GameSpinCount);
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();

            textGameSpinCount = root.Find("Count");
            textGameSpinCountPlural = root.FindWithFullName("Spins");
            textSpinType = (IContextText)root.FindWithFullName("Lucky");
        }
    }
}
