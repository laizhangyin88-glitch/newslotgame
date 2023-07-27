using GameStudio.Slot.EDM.Popup;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM
{
    public class EDMBonusCalculateJackpotFly : MonoBehaviour
    {
        [HideInInspector] public EDMBonusGameResultBoardPopup resultPopup;
        public DirectionalWeightPositionController positionController;
        public Animator animator;
        private long credit = 0;

        public void Initialize(EDMBonusGameResultBoardPopup popup, Transform fromAnchor, Transform targetAnchor, long creditAmount, int jackpotIndex)
        {
            transform.position = fromAnchor.position;
            resultPopup = popup;
            positionController.from = fromAnchor;
            positionController.to = targetAnchor;
            credit = creditAmount;
            animator.SetInteger("jackpotIndex", jackpotIndex);
        }

        public void OnArrive()
        {
            resultPopup.OnArriveFly(true, credit);
        }
    }
}