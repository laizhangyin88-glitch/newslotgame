using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMMoreSpinActiveController : MonoBehaviour
    {
        public DirectionalWeightPositionController positionController;

        public void Initialize(Transform from, Transform to)
        {
            positionController.to = to;
            positionController.from = from;

            transform.position = from.position;
        }
    }
}