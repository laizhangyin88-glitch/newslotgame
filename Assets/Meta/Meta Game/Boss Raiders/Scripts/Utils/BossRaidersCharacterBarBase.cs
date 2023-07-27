using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    public abstract class BossRaidersCharacterBarBase
    {
        protected Animator energyAnimator;
        protected ContextElement energySliderElement;
        protected ContextElement textElement;

        protected const int ProgressEnergyMax = 100;
        protected const int ProgressEnergyRed = 10;

        public virtual void OnInit(Animator _energyAnimator, ContextElement _energySliderElement, ContextElement _textElement)
        {
            energyAnimator = _energyAnimator;
            energySliderElement = _energySliderElement;
            textElement = _textElement;

            InitProperty();
        }

        protected abstract void InitProperty();
        public abstract void SetEnergy(long energy);
        public abstract void SetEnergyAnimator(bool isActive);
    }
}