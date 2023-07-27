using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode
{
    public class PopupControlSpinBoosterController : PopupWheelBoosterBaseController
    {

        public override void InitProperty()
        {
            base.InitProperty();

            // Play Sound
            GSManager.Instance.GetHandler("UI_Coin_Booster_Intro").Play();
        }

        protected override void InitBaseStringKeySetting()
        {
            titleTextKey = "POPUP_CONTROL_SPIN_BOOSTER_TITLE";
            boosterButtonText = StringTableUtils.GetString(GLOBAL, "POPUP_CONTROL_SPIN_BOOSTER_BOOST_BUTTON", boostProduct.GetValue<double>("price"));
        }

        public override void Refresh()
        {
#if UNITY_EDITOR
            bool isProbsEnable = true;
#else
            bool isProbsEnable = BlackboardUtils.FindVariable<bool>("/values/misc/PROBS_ENABLE")?.value ?? false;
#endif
            anim.SetBool("IsOdds", isProbsEnable);

            anim.SetBool("Active", true);

            // Highlight
            var currentHighlightElement = wheelHighlightElements[0];
            var currentHighlightAnim = currentHighlightElement.GetComponent<Animator>();
            currentHighlightAnim.SetBool("Highlight", true);
        }

        protected override void UpdateResultTexts(int idx)
        {
            MetaContextElementUtils.SetTextGlobal(resultMultiplierElement, "POPUP_CONTROL_SPIN_BOOSTER_RESULT_MULTIPLIER", wheelMultiplierList[idx]);
            MetaContextElementUtils.SetTextGlobal(resultSpinCoinElement, "POPUP_CONTROL_SPIN_BOOSTER_RESULT_COIN_SPIN", wheelCoinList[idx], wheelBetList[idx]);
        }
    }
}
