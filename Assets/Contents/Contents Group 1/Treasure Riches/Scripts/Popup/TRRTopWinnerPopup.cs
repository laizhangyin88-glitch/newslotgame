using System.Collections;
using BagelCode.Slots.TRR.Utillity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Slots.TRR.Popup
{
    public class TRRTopWinnerPopup : TRRPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private Image playerImage;

        public string userID;
        public long credit;
        public bool useCountingDirection;

        protected override IEnumerator OnPlayCoroutine()
        {
            playerImage.sprite = TRRProfileManager.Instance.GetUserProfileImage(userID);
            TRRUtillity.ChangeSnapShot("Content_Popup");
            TRRUtillity.PlaySound("Top Winner Popup");
            if (useCountingDirection)
                yield return CreditDirectionCoroutine(1.75f, credit);
            else creditText.text = credit.ToString("#,##0");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            TRRUtillity.ChangeSnapShot("Content_Main");
        }

        private Coroutine CreditDirectionCoroutine(float duraction, long credit) => StartCoroutine(_CreditDirectionCorotuine(duraction, credit));
        private IEnumerator _CreditDirectionCorotuine(float duraction, long credit)
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
            TRRUtillity.StopSound("Popup Coin Count");
            TRRUtillity.PlaySound("Popup Coin Count Stop");

            creditText.text = credit.ToString("#,##0");
        }
    }
}