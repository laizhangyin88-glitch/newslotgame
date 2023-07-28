using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Utillity;
using TMPro;
using UnityEngine;
namespace BagelCode.Slots.TRR.Popup
{
    public class TRRRoundPrizePopup : TRRPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI creditText;

        public int round;
        public List<long> roundCreditData;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRRUtillity.ChangeSnapShot("Content_Popup");
            TRRUtillity.PlaySound("Round Prize Popup");

            yield return StartCoroutine(_CountingCredit(1.75f, roundCreditData[round - 1]));
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            TRRUtillity.ChangeSnapShot("Content_Main");
        }

        private IEnumerator _CountingCredit(float duraction, long credit)
        {
            TRRUtillity.PlaySound("Popup Coin Count");

            float startTime = Time.time;
            float endTime = Time.time + duraction;
            float currentTime = startTime;

            while (currentTime < endTime)
            {
                currentTime = Time.time;
                long currentCredit = (long)(credit / (duraction / (currentTime - startTime)));
                creditText.text = currentCredit.ToString("#,##0");
                yield return null;
            }

            if (round == 1) TRRUtillity.PlaySound("Vox Round Prize 1");
            else if (round == 2) TRRUtillity.PlaySound("Vox Round Prize 2");
            else if (round == 3) TRRUtillity.PlaySound("Vox Round Prize 3");
            else if (round == 4) TRRUtillity.PlaySound("Vox Round Prize 4");
            else if (round == 5) TRRUtillity.PlaySound("Vox Round Prize 5");
            else if (round > 5) TRRUtillity.PlaySound("Vox Round Prize 5");
            TRRUtillity.StopSound("Popup Coin Count");
            TRRUtillity.PlaySound("Popup Coin Count Stop");
            creditText.text = credit.ToString("#,##0");
        }
    }
}