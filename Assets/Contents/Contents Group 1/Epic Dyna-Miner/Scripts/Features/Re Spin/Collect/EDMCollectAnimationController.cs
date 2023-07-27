using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMCollectAnimationController : MonoBehaviour
    {
        public Animator animator;
        private EDMCollectController controller;
        private Symbol symbol;
        public TextMeshProUGUI valueText;
        private long credit = 0;

        private void OnEnable()
        {
            credit = 0;
            valueText.gameObject.SetActive(false);
        }

        public void Initialize(EDMCollectController collectController, long initalCredit)
        {
            controller = collectController;
            credit = initalCredit;
            symbol = GetComponentInParent<Symbol>();
        }
        public void OnFlyArrive(long addCredit)
        {
            credit += addCredit;
            symbol.Play("UpdateValueText");
            valueText.gameObject.SetActive(true);
            animator.SetTrigger("Arrive");
        }

        public void OnCollect()
        {
            symbol.Play("UpdateValueText");
            controller.OnCollect();
        }
    }
}