
using System.Collections;
using GameStudio.Slot.IIP.Utility;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.IIP.Popup
{
    public class IIPCommunityGameCongratulationsPopup : FeatureModule
    {
        [SerializeField] private Animator animator;

        private void OnEnable()
        {
            OpenPopupCoroutine();
        }

        public Coroutine OpenPopupCoroutine()
        {
            return StartCoroutine(_OpenPopupCoroutine());
        }
        IEnumerator _OpenPopupCoroutine()
        {
            IIPUtility.PlaySound("Rescued All Penguin Popup");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Appear") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            animator.SetTrigger("Disappear");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            ContentEvent.SendEvent("IIPEndCongratulationPopup");
        }
    }
}