using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.TRL.Utillity;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.TRL.Popup
{
    public class TRLRoundPrizePopup : TRLPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI creditText;

        public int round;
        public List<long> roundCreditData;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRLUtillity.ChangeSnapShot("Content_Popup");
            TRLUtillity.PlaySound("Round Prize Popup");

            yield return StartCoroutine(_CountingCredit(1.75f, roundCreditData[round - 1]));
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            TRLUtillity.ChangeSnapShot("Content_Main");
        }

        private IEnumerator _CountingCredit(float duraction, long credit)
        {
            TRLUtillity.PlaySound("Popup Coin Count");

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

            if (round == 1) TRLUtillity.PlaySound("Vox Round Prize 1");
            else if (round == 2) TRLUtillity.PlaySound("Vox Round Prize 2");
            else if (round == 3) TRLUtillity.PlaySound("Vox Round Prize 3");
            else if (round == 4) TRLUtillity.PlaySound("Vox Round Prize 4");
            else if (round == 5) TRLUtillity.PlaySound("Vox Round Prize 5");
            else if (round > 5) TRLUtillity.PlaySound("Vox Round Prize 5");
            TRLUtillity.StopSound("Popup Coin Count");
            TRLUtillity.PlaySound("Popup Coin Count Stop");
            creditText.text = credit.ToString("#,##0");
        }
    }
}