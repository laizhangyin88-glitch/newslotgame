using GameStudio.Slot.EDM.Utility;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMKeyBehaviour : SlotMaker.SymbolBehaviour
    {
        public int STOP = 0;
        public int ACTIVE = 1;

        public override void OnEntry()
        {
            animator.SetBool("Text", false);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(ACTIVE).SetActive(false);
        }

        public override void OnStopEffect()
        {
            int rowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            bool isRespin = BlackboardUtils.FindVariable<bool>("./customData/isRespin").value;
            if (isRespin == true && 5 - symbol.row < rowCount)
            {
                PlayAnimation("InActive");
                GetCachedObject(STOP).SetActive(true);
                GetCachedObject(ACTIVE).SetActive(false);
                ContentEvent.SendEvent("EDM_ON_RESET_SPIN_COUNT");
            }
            EDMUtility.PlaySound("SB Key Land");
        }

        public override void OnWin()
        {
        }

        public void Active()
        {
            PlayAnimation("InActive");
            GetCachedObject(STOP).SetActive(false);
            GetCachedObject(ACTIVE).SetActive(true);
        }

        public void SetDirectionalPositionController(Transform from, Transform target)
        {
            DirectionalWeightPositionController positionController = GetCachedObject(ACTIVE).GetComponentInChildren<DirectionalWeightPositionController>();
            positionController.from = from;
            positionController.to = target;

            positionController.transform.position = from.position;
        }
    }
}