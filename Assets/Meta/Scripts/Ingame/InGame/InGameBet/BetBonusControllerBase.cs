using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public abstract class BetBonusControllerBase : MonoBehaviour
    {
        public BetBonusAreaControllerBase betBonusAreaController;

        // Options -------------
        protected bool isConsiderLuckySpin;
        protected bool isInstantDisplay;
        protected float displayingTime;
        // ---------------------

        private Variable<SpinType> spinType;
        protected Animator anim;
        protected bool isDisplayed; // only changed at Init, Show, Hide
        protected float passedTime;

        protected virtual void Awake()
        {
            InitProperty();
        }

        protected virtual void Update()
        {
            UpdateDisplay();
        }

        // public

        public virtual void InitProperty()
        {
            SetBetBonusOptions();

            anim = GetComponent<Animator>();
            isDisplayed = false;
            passedTime = 0f;

            if (isConsiderLuckySpin)
                spinType = BlackboardUtils.FindVariable<SpinType>("./spinType");
        }

        public virtual void OnReadyGame()
        {
            ShowBetBonus();
        }

        public virtual void OnEnterTurn()
        {
            HideBetBonus();
        }

        public virtual void OnChangeTotalBet(long _totalBet)
        {
            ShowBetBonus();
        }

        // protected

        protected abstract void SetBetBonusOptions();

        protected virtual void ShowBetBonus()
        {
            isDisplayed = true;
            passedTime = 0f;

            bool isLuckySpinEnable = isConsiderLuckySpin && (spinType.value == SpinType.GameSpin);
            if(isLuckySpinEnable)
            {
                anim.SetBool("isActive", false);
                anim.SetBool("isLuckySpin", true);
            }
            else
            {
                anim.SetBool("isLuckySpin", false);
                anim.SetBool("isActive", true);
            }
        }

        protected virtual void HideBetBonus()
        {
            isDisplayed = false;

            anim.SetBool("isActive", false);
            anim.SetBool("isLuckySpin", false);
        }

        // private

        private void UpdateDisplay()
        {
            if (isInstantDisplay && isDisplayed)
            {
                if (passedTime < displayingTime)
                {
                    passedTime += Time.deltaTime;
                }
                else
                {
                    HideBetBonus();
                }
            }
        }
    }

}