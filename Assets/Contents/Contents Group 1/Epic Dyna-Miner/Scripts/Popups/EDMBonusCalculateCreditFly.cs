using GameStudio.Slot.EDM.Popup;
using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM
{
    public class EDMBonusCalculateCreditFly : MonoBehaviour
    {
        [HideInInspector] public EDMBonusGameResultBoardPopup resultPopup;
        public DirectionalWeightPositionController positionController;
        public Animator animator;
        public TextMeshProUGUI creditText;
        private long credit = 0;

        public void Initialize(EDMBonusGameResultBoardPopup popup, Transform fromAnchor, Transform targetAnchor, long creditAmount, int symbolIndex)
        {
            transform.position = fromAnchor.position;
            resultPopup = popup;
            positionController.from = fromAnchor;
            positionController.to = targetAnchor;
            credit = creditAmount;
            animator.SetInteger("symbolIndex", symbolIndex);
            creditText.text = FormatUtility.SimpleNumberFormat(credit);
        }

        public void OnArrive()
        {
            resultPopup.OnArriveFly(false, credit);
        }
    }
}