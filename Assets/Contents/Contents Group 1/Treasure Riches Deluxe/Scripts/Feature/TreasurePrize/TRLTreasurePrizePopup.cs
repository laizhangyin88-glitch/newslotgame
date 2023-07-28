using System.Collections;
using GameStudio.Slot.TRL.Popup;
using GameStudio.Slot.TRL.Utillity;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.TRL
{
    public class TRLTreasurePrizePopup : TRLPopup
    {
        [Header("Component")]
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private TextMeshProUGUI spotNumber;
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private TextMeshProUGUI mutipliedCreditText;

        public long totalBet;
        public float collectMultiplier;
        public long collectAmount;
        public int spotIndex;
        public int stackedColumnCount;

        protected override IEnumerator OnPlayCoroutine()
        {
            spotNumber.text = $"{spotIndex + 1}";
            multiplierText.text = $"X{collectMultiplier}";

            creditText.text = totalBet.ToString("#,##0");
            mutipliedCreditText.text = collectAmount.ToString("#,##0");
            animator.SetInteger("Stack", stackedColumnCount);

            //Active
            animator.SetBool("Active", true);
            animator.Update(Time.deltaTime);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            //BoxOpen
            animator.SetTrigger("BoxOpen");
            animator.Update(Time.deltaTime);
            TRLUtillity.PlaySound("Scatter Open");
            if (collectMultiplier >= 2f)
            {
                yield return new WaitForSeconds(3.03f);
                TRLUtillity.PlaySound("Scatter Multiplier");
            }
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.99f);

            //ClosePopup
            animator.SetBool("Active", false);
            animator.Update(Time.deltaTime);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

    }
}