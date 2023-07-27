using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMJackpotStopController : MonoBehaviour
    {
        [HideInInspector] public Symbol ownerSymbol;
        public Animator mainAnimator;
        public Animator lockAnimator;
        public TextMeshProUGUI valueText;
        public int leftSpinCount = 10;
        public void Initialize()
        {
            ownerSymbol = GetComponentInParent<Symbol>();
            leftSpinCount = 10;
            valueText.text = FormatUtility.SimpleNumberFormat(leftSpinCount);
            mainAnimator.SetInteger("spinLeft", leftSpinCount);
        }
        public void SetSpinLeftCount(int Count)
        {
            leftSpinCount = Count;
            valueText.text = FormatUtility.SimpleNumberFormat(leftSpinCount);
        }

        public void SetValueTextActive(bool isActive)
        {
            lockAnimator.SetBool("Active", isActive);
        }
        public void ForceIdle()
        {
            mainAnimator.SetTrigger("Idle");
        }

        public void Skip()
        {
            lockAnimator.SetTrigger("Skip");
        }
        public void OnStopSpin()
        {
            int currentRowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            if ((currentRowCount - 4) - (2 - ownerSymbol.row) >= 0)
            {
                leftSpinCount--;
                mainAnimator.SetInteger("spinLeft", leftSpinCount);
                valueText.text = FormatUtility.SimpleNumberFormat(leftSpinCount);
            }
        }

        public bool IsOpen()
        {
            return leftSpinCount <= 0;
        }
    }
}