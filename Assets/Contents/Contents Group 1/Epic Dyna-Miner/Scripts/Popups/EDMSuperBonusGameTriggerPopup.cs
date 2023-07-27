using System.Collections;
using UnityEngine;
namespace GameStudio.Slot.EDM.Popup
{
    public class EDMSuperBonusGameTriggerPopup : EDMPopup
    {
        public Animator animator;
        public override IEnumerator ActiveCoroutine()
        {
            yield return new WaitForSeconds(1.4f);
            animator.SetTrigger("Disappear");
            yield return new WaitForSeconds(1f);
        }

    }
}