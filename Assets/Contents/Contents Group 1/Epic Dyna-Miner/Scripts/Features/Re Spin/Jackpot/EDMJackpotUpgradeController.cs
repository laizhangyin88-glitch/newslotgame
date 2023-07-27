using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMJackpotUpgradeController : MonoBehaviour
    {
        [HideInInspector] public Symbol ownerSymbol;
        public Animator lockAnimator;
        public TextMeshProUGUI valueText;
        public int leftSpinCount = 10;
        public void Initialize(int leftCount)
        {
            ownerSymbol = GetComponentInParent<Symbol>();
            leftSpinCount = leftCount;
            valueText.text = FormatUtility.SimpleNumberFormat(leftSpinCount);

        }

        public void SetValueTextActive(bool isActive)
        {
            lockAnimator.SetBool("Active", isActive);
            lockAnimator.Update(Time.deltaTime);
        }


        public void OnStopSpin()
        {
            int currentRowCount = BlackboardUtils.FindVariable<int>("./customData/currentRowCount").value;
            if ((currentRowCount - 4) - (2 - ownerSymbol.row) >= 0)
            {
                leftSpinCount--;
                valueText.text = FormatUtility.SimpleNumberFormat(leftSpinCount);
            }
        }

        public bool IsOpen()
        {
            return leftSpinCount <= 0;
        }

        public void Skip()
        {
            lockAnimator.SetTrigger("Skip");
            lockAnimator.Update(Time.deltaTime);
        }
    }
}