using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    public class BossRaidersCharacterBarController : BossRaidersCharacterBarBase
    {
        protected ContextElement energyGreen;
        protected ContextElement energyRed;

        protected override void InitProperty()
        {
            energyGreen = ContextUtils.FindElement(energySliderElement, "Fill Green", ContextSearchingType.ChildrenSearch);
            energyRed = ContextUtils.FindElement(energySliderElement, "Fill Red", ContextSearchingType.ChildrenSearch);
        }

        public override void SetEnergy(long energy)
        {
            SetProgressBar((int)energy);
            SetProgressColor((int)energy);
        }

        public override void SetEnergyAnimator(bool isActive)
        {
            if (energyAnimator != null)
                energyAnimator.SetBool("Energy", isActive);
        }

        private void SetProgressBar(int energy)
        {
            int energyValue = Mathf.Clamp(energy, 0, ProgressEnergyMax);
            float sliderValue = (float)energyValue / (float)ProgressEnergyMax;
            MetaContextElementUtils.SetFloatProperty(energySliderElement, sliderValue);
            MetaContextElementUtils.SetTextGlobal(textElement, "BOSS_RAIDERS_CHARACTER_ENERGY", energy);
        }

        private void SetProgressColor(int energy)
        {
            bool isGreen = energy > ProgressEnergyRed;
            energyGreen.gameObject.SetActive(isGreen);
            energyRed.gameObject.SetActive(!isGreen);
        }
    }
}