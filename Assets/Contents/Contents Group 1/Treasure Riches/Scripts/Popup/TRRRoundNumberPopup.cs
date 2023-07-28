using System.Collections;
using BagelCode.Slots.TRR.Utillity;
using TMPro;
using UnityEngine;
namespace BagelCode.Slots.TRR.Popup
{
    public class TRRRoundNumberPopup : TRRPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI multiplierText;

        public int currentRound;

        protected override IEnumerator OnPlayCoroutine()
        {
            int multiplier = 0;
            if (currentRound == 1)
            {
                TRRUtillity.PlaySound("Round X3 Popup");
                multiplier = 3;
            }
            else if (currentRound == 2)
            {
                TRRUtillity.PlaySound("Round X4 Popup");
                multiplier = 4;
            }
            else if (currentRound == 3)
            {
                TRRUtillity.PlaySound("Round X5 Popup");
                multiplier = 5;
            }
            else if (currentRound == 4)
            {
                TRRUtillity.PlaySound("Round X10 Popup");
                multiplier = 10;
            }
            else if (currentRound == 5)
            {
                TRRUtillity.PlaySound("Round X15 Popup");
                multiplier = 15;
            }

            multiplierText.text = $"X{multiplier}";
            if (currentRound == 6)
            {
                TRRUtillity.PlaySound("Final Round Popup");
                multiplierText.text = $"FINAL";
            }

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        }
    }
}