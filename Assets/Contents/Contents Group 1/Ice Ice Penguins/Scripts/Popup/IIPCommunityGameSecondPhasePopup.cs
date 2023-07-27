using System.Collections;
using GameStudio.Slot.IIP.Utility;
using ParadoxNotion;
using UnityEngine;

namespace GameStudio.Slot.IIP.Popup
{
    public class IIPCommunityGameSecondPhasePopup : FeatureModule
    {
        [SerializeField] private Animator animator;

        public Coroutine OpenPopupCoroutine()
        {
            return StartCoroutine(_OpenPopupCoroutine());
        }
        IEnumerator _OpenPopupCoroutine()
        {
            IIPUtility.PlaySound("Second Phase Popup");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Appear") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            animator.SetTrigger("Disappear");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        }
    }
}