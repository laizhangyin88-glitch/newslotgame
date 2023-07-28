using System.Collections;
using BagelCode.Slots.TRR.Utillity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Slots.TRR.Popup
{
    public class TRRTreasureBonusResultPopup : TRRPopup
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private Image playerImage;
        [SerializeField] private Button button;

        public long credit;
        public string userId;

        private bool isClicked = false;

        protected override IEnumerator OnPlayCoroutine()
        {
            TRRUtillity.ChangeSnapShot("Content_Popup");
            TRRUtillity.PlaySound("Bonus Game Result Popup");
            isClicked = false;
            creditText.text = credit.ToString("#,##0");
            playerImage.sprite = TRRProfileManager.Instance.GetUserProfileImage(userId);
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

            button.onClick.AddListener(OnClickedCollectButton);
            yield return new WaitUntil(() => isClicked == true);
            button.onClick.RemoveListener(OnClickedCollectButton);

            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

            TRRUtillity.ChangeSnapShot("Content_Main");
        }

        private void OnClickedCollectButton()
        {
            animator.SetTrigger("Disappear");
            isClicked = true;
            TRRUtillity.PlaySound("Button");
        }
    }
}