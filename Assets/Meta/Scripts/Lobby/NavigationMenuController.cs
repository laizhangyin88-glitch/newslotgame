using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using TMPro;

namespace BagelCode
{
    public class NavigationMenuController : EventMonoBehaviour
    {
        public BBParameter<GameObject> saveAsEmptyObj;

        private ContextElement root;

        private ContextElement dropdownElement;

        private ContextElement onLineButtonElement;
        private ContextElement rankingButtonElement;
        private ContextElement wallofEpicButtonElement;
        private ContextElement couponButtonElement;
        private ContextElement customerSupportButtonElement;
        private ContextElement statusMatchButtonElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            dropdownElement = ContextUtils.FindElement(root, "Dropdown Menu", CHILDREN);
            MetaContextElementUtils.SetActive(dropdownElement, true);

            // Global Popups
            onLineButtonElement = ContextUtils.FindElement(root, "Button Online", CHILDREN);
            rankingButtonElement = ContextUtils.FindElement(root, "Button Ranking", CHILDREN);
            wallofEpicButtonElement = ContextUtils.FindElement(root, "Button WOE", CHILDREN);
            couponButtonElement = ContextUtils.FindElement(root, "Button Coupon", CHILDREN);
            customerSupportButtonElement = ContextUtils.FindElement(root, "Button Customer Support", CHILDREN);
            statusMatchButtonElement = ContextUtils.FindElement(root, "Button Status Match", CHILDREN);

            // Clickable
            MetaContextElementUtils.SetClickable(onLineButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_ONLINE_PLAYERS, false, false);

            //V1.3.0临时
            //MetaContextElementUtils.SetClickable(rankingButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_LEADERBOARD, true, true);
            MetaContextElementUtils.SetClickable(wallofEpicButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT,  MetaEventDefine.ON_ENTER_WOE, true, true);
            MetaContextElementUtils.SetClickable(couponButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_COUPON, true, true);
            MetaContextElementUtils.SetClickable(customerSupportButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_CUSTOMER_SUPPORT, true, true);

            
            //MetaContextElementUtils.SimpleSetTextGlobal(onLineButtonElement, "Text", "BUTTON_ONLINE_PLAYERS", CHILDREN);
            ContextUtils.FindElement(onLineButtonElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<TextMeshProUGUI>().text = "VERSION: "+ ApplicationSettings.Instance.clientVersion;
            //MetaContextElementUtils.SimpleSetTextGlobal(rankingButtonElement, "Text", "BUTTON_LEADERBOARD", CHILDREN);
            ContextUtils.FindElement(rankingButtonElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<TextMeshProUGUI>().text = "REFUND";
            //V1.3.0临时
            rankingButtonElement.GetComponent<ContextButton>().AddListenerOnClick((context) => SBoxSanboxController.Instance.PrintMoneyOrder());

            MetaContextElementUtils.SimpleSetTextGlobal(wallofEpicButtonElement, "Text", "BUTTON_WALL_OF_EPICS", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(couponButtonElement, "Text", "BUTTON_COUPON", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(customerSupportButtonElement, "Text", "BUTTON_CS", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(statusMatchButtonElement, "Text", "BUTTON_STATUS_MATCH", CHILDREN);

            bool isEarlyAccess = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "/isEarlyAccess")?.value ?? false;
            bool isSimpleMenuButtons = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "/isSimpleMenuButtons")?.value ?? false;

            bool showGlobalButtons = !isEarlyAccess && !isSimpleMenuButtons;

            // Active
           // MetaContextElementUtils.SetActive(customerSupportButtonElement, true);

            MetaContextElementUtils.SetActive(onLineButtonElement, showGlobalButtons);
            MetaContextElementUtils.SetActive(rankingButtonElement, showGlobalButtons);
            MetaContextElementUtils.SetActive(wallofEpicButtonElement, showGlobalButtons);
            MetaContextElementUtils.SetActive(couponButtonElement, showGlobalButtons);

            if (showGlobalButtons)
            {
                bool disableStatusMatchDeeplink = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "/disableStatusMatchDeeplink")?.value ?? false;
                bool showStatusMatchMenu = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "/showStatusMatchMenu")?.value ?? false;

                bool showStatusMatchButton = showStatusMatchMenu && !disableStatusMatchDeeplink;
                MetaContextElementUtils.SetActive(statusMatchButtonElement, showStatusMatchButton);
            }
            else
            {
                MetaContextElementUtils.SetActive(statusMatchButtonElement, false);
            }


#if NEW_NET

            Debug.Log("【close】:关闭大厅抬头右侧按钮列表");
            //MetaContextElementUtils.SetActive(onLineButtonElement, false);
            //MetaContextElementUtils.SetActive(rankingButtonElement, false);
            MetaContextElementUtils.SetActive(rankingButtonElement, ApplicationSettings.Instance.isMachine);
            MetaContextElementUtils.SetActive(wallofEpicButtonElement, false);
            MetaContextElementUtils.SetActive(couponButtonElement, false);
            MetaContextElementUtils.SetActive(customerSupportButtonElement, false);
            MetaContextElementUtils.SetActive(statusMatchButtonElement, false);
#endif

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
        }
    }
}
