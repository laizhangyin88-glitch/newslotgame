using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders.Deal
{
    public class BossRaidersDealWheel : BossRaidersWheelBase
    {
        private ContextElement spinCountTextElement;
        private ContextElement spinDescriptionElement;

        public override void OnInit(ContextElement root, Animator animator, int wheelCount)
        {
            base.OnInit(root, animator, wheelCount);

            wheelElements = new ContextElement[wheelItemCount];

            ContextElement baseElement = ContextUtils.FindElement(rootElement, "Base", ContextSearchingType.ChildrenSearch);
            for (int i = 0; i < wheelItemCount; ++i)
                wheelElements[i] = ContextUtils.FindElement(baseElement, string.Format("{0:00}", i + 1), ContextSearchingType.ChildrenSearch);

            highlightElement = ContextUtils.FindElement(rootElement, "Highlight", ContextSearchingType.ChildrenSearch);

            ContextElement spinCountInfoElement = ContextUtils.FindElement(rootAnimator.GetComponent<ContextElement>(), "Spin Count Info/Info", ContextSearchingType.FullNameSearch);
            spinCountTextElement = ContextUtils.FindElement(spinCountInfoElement, "Text Spin Count", ContextSearchingType.ChildrenSearch);
            spinDescriptionElement = ContextUtils.FindElement(spinCountInfoElement, "Text Spin Description", ContextSearchingType.ChildrenSearch);
        }

        public override void SetWheelData(long spinCount)
        {
            MetaContextElementUtils.SetText(spinCountTextElement, spinCount.ToString());
            MetaContextElementUtils.SetTextGlobal(spinDescriptionElement, spinCount > 1 ? "POPUP_BOSS_RAIDERS_DEAL_DESCRIPTION_SPINS" : "POPUP_BOSS_RAIDERS_DEAL_DESCRIPTION_SPIN");
        }
    }
}