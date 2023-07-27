using System.Collections;
using GameStudio.Slot.EDM.Utility;
using UnityEngine;
namespace GameStudio.Slot.EDM.Popup
{
    public class EDMBonusGameTriggerPopup : EDMPopup
    {
        public Animator animator;
        bool isClicked = false;
        public override IEnumerator ActiveCoroutine()
        {
            yield return new WaitForSeconds(1.4f);
            isClicked = false;
            yield return new WaitUntil(() => isClicked == true);
            EDMUtility.PlaySound("Button");
            animator.SetTrigger("Disappear");
            yield return new WaitForSeconds(1f);
        }
        public void OnClickButton()
        {
            isClicked = true;
        }
    }
}