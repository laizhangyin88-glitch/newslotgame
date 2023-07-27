using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMJackpotOpenController : MonoBehaviour
    {
        public DirectionalWeightPositionController positionController;
        private Transform flyingTargetTransform;
        public Transform flyingFromTransform;


        public void Initalize()
        {
            flyingTargetTransform = BlackboardUtils.FindVariable<GameObject>("./chestAnchor").value.transform;
            positionController.to = flyingTargetTransform;
            positionController.from = flyingFromTransform;
        }
    }
}