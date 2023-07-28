using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.Slot.TRL.Popup
{
    public class TRLTopWinnerPopup : TRLPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private Image playerImage;

        public string userID;
        public long credit;
        public bool useCountingDirection;

        protected override IEnumerator OnPlayCoroutine()
        {
            playerImage.sprite = TRLUserProfileManager.Instance.GetUserProfileImage(userID);
            TRLUtillity.ChangeSnapShot("Content_Popup");
            TRLUtillity.PlaySound("Top Winner Popup");
            if (useCountingDirection)
                yield return CreditDirectionCoroutine(1.75f, credit);
            else creditText.text = credit.ToString("#,##0");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            TRLUtillity.ChangeSnapShot("Content_Main");
        }

        private Coroutine CreditDirectionCoroutine(float duraction, long credit) => StartCoroutine(_CreditDirectionCorotuine(duraction, credit));
        private IEnumerator _CreditDirectionCorotuine(float duraction, long credit)
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
            TRLUtillity.StopSound("Popup Coin Count");
            TRLUtillity.PlaySound("Popup Coin Count Stop");

            creditText.text = credit.ToString("#,##0");
        }
    }
}