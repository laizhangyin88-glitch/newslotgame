using UnityEngine;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsInformationController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;

        private bool isInit = false;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private ContextElement leftButtonElement;
        private ContextElement rightButtonElement;
        private ContextElement dotsAreaElement;
        private List<ContextElement> pageAreaElements = new List<ContextElement>();

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Play Anim
            anim.SetBool("Active", true);
            anim.SetBool("IsLoading", false);

            // Init Elements
            InitContents();
            InitPageDescription();

            isInit = true;
        }

        private void InitContents()
        {
            // Toggle Elements
            dotsAreaElement = ContextUtils.FindElement(root, "Anchor/Dots Area/Dots Anchor", FULL);

            for (int i = 1; i <= VegasDreams.Defines.INFORMATION_PAGE_COUNT; i++)
            {
                pageAreaElements.Add(ContextUtils.FindElement(root, $"Anchor/Page {i:00}", FULL));
            }

            // Left Right
            leftButtonElement = ContextUtils.FindElement(root, "Anchor/Arrow Left", FULL);
            MetaContextElementUtils.SetClickable(leftButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_SELECT_LEFT_INFO));

            rightButtonElement = ContextUtils.FindElement(root, "Anchor/Arrow Right", FULL);
            MetaContextElementUtils.SetClickable(rightButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_SELECT_RIGHT_INFO));

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Anchor/Button Close", FULL);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));
        }

        private void InitPageDescription()
        {
            // Page 0
            var page00 = ContextUtils.FindElement(root, "Anchor/Page 03/Text Wild Info", FULL);
            MetaContextElementUtils.SetTextGlobal(page00, "VEGAS_DREAMS_INFORMATION_PAGE_1_0", VegasDreams.Utils.WildDepotExp);
        }

        public void UpdateInformationElements(int index)
        {
            leftButtonElement.GetComponent<PIDButton>().interactable = index > 0;
            rightButtonElement.GetComponent<PIDButton>().interactable = index < VegasDreams.Defines.INFORMATION_PAGE_COUNT - 1;

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Anchor/Title Area/Text Title", $"VEGAS_DREAMS_INFORMATION_TITLE_{index}", FULL);

            MetaContextElementUtils.SetIntProperty(dotsAreaElement, index);

            foreach (var page in pageAreaElements)
            {
                MetaContextElementUtils.SetActive(page, false);
            }

            MetaContextElementUtils.SetActive(pageAreaElements[index], true);
        }
    }
}
