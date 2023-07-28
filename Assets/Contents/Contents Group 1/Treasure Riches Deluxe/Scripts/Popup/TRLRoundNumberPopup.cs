using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.TRL.Popup
{
    public class TRLRoundNumberPopup : TRLPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI multiplierText;

        public int currentRound;


        protected override IEnumerator OnPlayCoroutine()
        {
            int multiplier = 0;
            if (currentRound == 1)
            {
                TRLUtillity.PlaySound("Round X3 Popup");
                multiplier = 3;
            }
            else if (currentRound == 2)
            {
                TRLUtillity.PlaySound("Round X4 Popup");
                multiplier = 4;
            }
            else if (currentRound == 3)
            {
                TRLUtillity.PlaySound("Round X5 Popup");
                multiplier = 5;
            }

            else if (currentRound == 4)
            {
                TRLUtillity.PlaySound("Round X10 Popup");
                multiplier = 10;
            }
            else if (currentRound == 5)
            {
                TRLUtillity.PlaySound("Round X15 Popup");
                multiplier = 15;
            }

            multiplierText.text = $"X{multiplier}";
            if (currentRound == 6)
            {
                TRLUtillity.PlaySound("Final Round Popup");
                multiplierText.text = $"FINAL";
            }

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        }
    }
}