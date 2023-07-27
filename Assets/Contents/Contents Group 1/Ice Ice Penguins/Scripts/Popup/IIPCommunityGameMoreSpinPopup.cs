
using System.Collections;
using GameStudio.Slot.IIP.Utility;
using ParadoxNotion;
using TMPro;
using UnityEngine;

namespace GameStudio.Slot.IIP.Popup
{
    public class IIPCommunityGameMoreSpinPopup : FeatureModule
    {
        [SerializeField] private Animator animator;
        public TextMeshProUGUI extraSpinText;

        public Coroutine OpenPopupCoroutine(int extraSpinCount)
        {
            return StartCoroutine(_OpenPopupCoroutine(extraSpinCount));
        }
        IEnumerator _OpenPopupCoroutine(int extraSpinCount)
        {
            if (extraSpinCount == 1) extraSpinText.text = "+1 SPIN!";
            else extraSpinText.text = $"+{extraSpinCount} SPINS!";
            IIPUtility.PlaySound("Extra Spin");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Appear") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            animator.SetTrigger("Disappear");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        }
    }
}