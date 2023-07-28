using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.Slot.TRL.Popup
{
    public class TRLTreasureBonusResultPopup : TRLPopup
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
            TRLUtillity.ChangeSnapShot("Content_Popup");
            TRLUtillity.PlaySound("Bonus Game Result Popup");
            isClicked = false;
            creditText.text = credit.ToString("#,##0");
            playerImage.sprite = TRLUserProfileManager.Instance.GetUserProfileImage(userId);
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

            button.onClick.AddListener(OnClickedCollectButton);
            yield return new WaitUntil(() => isClicked == true);
            button.onClick.RemoveListener(OnClickedCollectButton);

            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

            TRLUtillity.ChangeSnapShot("Content_Main");
        }

        private void OnClickedCollectButton()
        {
            animator.SetTrigger("Disappear");
            isClicked = true;
            TRLUtillity.PlaySound("Button");
        }
    }
}