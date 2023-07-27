using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using NodeCanvas.Framework;

namespace BagelCode.VipLounge
{
    public class VipLoungeInformationController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
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
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // Play Anim
            anim.SetBool("Active", true);
            anim.SetBool("IsLoading", false);

            // Init Elements
            InitContents();
            InitPageDescription();

            BIClientVipLoungePopup();
            isInit = true;
        }

        private void InitContents()
        {
            // Toggle Elements
            dotsAreaElement = ContextUtils.FindElement(root, "Anchor/Dots Area/Dots Anchor", FULL);

            for (int i = 1; i <= VipLounge.Defines.INFORMATION_PAGE_COUNT; i++)
            {
                pageAreaElements.Add(ContextUtils.FindElement(root, $"Anchor/Page {i:00}", FULL));
            }

            // Left Right
            leftButtonElement = ContextUtils.FindElement(root, "Anchor/Arrow Left", FULL);
            MetaContextElementUtils.SetClickable(leftButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_SELECT_LEFT_INFO));

            rightButtonElement = ContextUtils.FindElement(root, "Anchor/Arrow Right", FULL);
            MetaContextElementUtils.SetClickable(rightButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_SELECT_RIGHT_INFO));

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Anchor/Button Close", FULL);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLOSE));
        }

        private void InitPageDescription()
        {
            string openCooltime = bb.GetValue<string>("openCooltime");
            string jackpotCooltime = bb.GetValue<string>("jackpotCooltime");
            // Page 0
            var page00 = ContextUtils.FindElement(root, "Anchor/Page 01/01/Text Contents", FULL);
            MetaContextElementUtils.SetTextGlobal(page00, "VIP_LOUNGE_INFORMATION_PAGE_0_0", VipLounge.Utils.MaxLoungePoint, openCooltime);

            var page01 = ContextUtils.FindElement(root, "Anchor/Page 01/02/Text Contents", FULL);
            MetaContextElementUtils.SetTextGlobal(page01, "VIP_LOUNGE_INFORMATION_PAGE_0_1", VipLounge.Utils.MaxLoungePoint, openCooltime);

            var page02 = ContextUtils.FindElement(root, "Anchor/Page 01/03/Text Contents", FULL);
            MetaContextElementUtils.SetTextGlobal(page02, "VIP_LOUNGE_INFORMATION_PAGE_0_2", jackpotCooltime);

            var page15 = ContextUtils.FindElement(root, "Anchor/Page 02/05/Text Contents", FULL);
            MetaContextElementUtils.SetTextGlobal(page15, "VIP_LOUNGE_INFORMATION_PAGE_1_4", jackpotCooltime.ToUpper());
        }

        public void UpdateInformationElements(int index)
        {
            leftButtonElement.GetComponent<PIDButton>().interactable = index > 0;
            rightButtonElement.GetComponent<PIDButton>().interactable = index < VipLounge.Defines.INFORMATION_PAGE_COUNT - 1;

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Anchor/Title Area/Text Title", $"VIP_LOUNGE_INFORMATION_TITLE_{index}", FULL);

            MetaContextElementUtils.SetIntProperty(dotsAreaElement, index);

            foreach (var page in pageAreaElements)
            {
                MetaContextElementUtils.SetActive(page, false);
            }

            MetaContextElementUtils.SetActive(pageAreaElements[index], true);
        }

        private void BIClientVipLoungePopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["popup_name"] = "vip_lounge_info";
            customData["context_id"] = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;

            Analytics.CustomEvent("client_vip_lounge_popup", customData);
        }
    }
}
