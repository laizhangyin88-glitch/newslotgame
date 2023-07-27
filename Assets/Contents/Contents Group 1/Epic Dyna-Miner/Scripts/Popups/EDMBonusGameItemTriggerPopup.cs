using System.Collections;
using GameStudio.Slot.EDM.Feature;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Popup
{
    public class EDMBonusGameItemTriggerPopup : EDMPopup
    {
        public Animator moreSpinAnimator;
        public Animator expandAnimator;
        public EDMRespinAdmin respinAdmin;
        public override IEnumerator ActiveCoroutine()
        {
            yield return new WaitForSeconds(1.4f);
            expandAnimator.SetTrigger("Appear");
            yield return new WaitForSeconds(0.2f);
            moreSpinAnimator.SetTrigger("Appear");
            yield return new WaitForSeconds(1.4f);
            expandAnimator.SetTrigger("Active");
            respinAdmin.lockReelAnimatorList[0].SetTrigger("Unlock");
            respinAdmin.mainAnimator.SetInteger("Row", 4);
            yield return new WaitForSeconds(1.5f);
            Transform targetAnchor = BlackboardUtils.GetOrCreateVariable<bool>("./customData/isSuperBonus").value == true ? respinAdmin.spinLeftAnimatorList[4].transform : respinAdmin.spinLeftAnimatorList[3].transform;
            DirectionalWeightPositionController positionController = moreSpinAnimator.GetComponentInChildren<DirectionalWeightPositionController>(true);
            positionController.transform.position = moreSpinAnimator.transform.position;
            positionController.to = targetAnchor;
            positionController.from = moreSpinAnimator.transform;
            moreSpinAnimator.SetTrigger("Active");
            yield return new WaitForSeconds(1.333333f);
            respinAdmin.spinLeftAddAnimator.SetBool("SuperBonus", true);
            MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("UpdateSpinCount"));
            yield return new WaitForSeconds(0.25f);
            respinAdmin.spinLeftAnimatorList[3].SetTrigger("Appear");
        }
    }
}