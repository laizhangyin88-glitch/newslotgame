using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using TMPro;
using UnityEngine.UI;

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

        //新增退出按钮
        private ContextElement quitButtonElement;

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

            quitButtonElement = ContextUtils.FindElement(root, "Button Quit", CHILDREN);
            MetaContextElementUtils.SetClickable(quitButtonElement, gameObject, MetaEventDefine.ON_SYSTEM_EVENT, MetaEventDefine.SYSTEM_RESET, true, true);
            MetaContextElementUtils.SetActive(quitButtonElement, ApplicationSettings.Instance.isMachine == false);

            // Clickable
            MetaContextElementUtils.SetClickable(onLineButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_ONLINE_PLAYERS, false, false);

            //原代码
            //MetaContextElementUtils.SetClickable(rankingButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_LEADERBOARD, true, true);
            //EditByYeep
            MetaContextElementUtils.SetClickable(rankingButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_REFUND_DIALOG, true, true);

            MetaContextElementUtils.SetClickable(wallofEpicButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT,  MetaEventDefine.ON_ENTER_WOE, true, true);
            MetaContextElementUtils.SetClickable(couponButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_COUPON, true, true);
            MetaContextElementUtils.SetClickable(customerSupportButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_CUSTOMER_SUPPORT, true, true);

            //原代码
            //MetaContextElementUtils.SimpleSetTextGlobal(onLineButtonElement, "Text", "BUTTON_ONLINE_PLAYERS", CHILDREN);
            //EditByYeep
            ContextUtils.FindElement(onLineButtonElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<TextMeshProUGUI>().text = "VERSION: "+ ApplicationSettings.Instance.clientVersion;
            //原代码
            //MetaContextElementUtils.SimpleSetTextGlobal(rankingButtonElement, "Text", "BUTTON_LEADERBOARD", CHILDREN);
            //EditByYeep
            ContextUtils.FindElement(rankingButtonElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<TextMeshProUGUI>().text = "REFUND";

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

            //测试热更
            MetaContextElementUtils.SetActive(wallofEpicButtonElement, true);
            MetaContextElementUtils.SetActive(couponButtonElement, true);
            MetaContextElementUtils.SetActive(customerSupportButtonElement, true);
            MetaContextElementUtils.SetActive(statusMatchButtonElement, true);
#endif

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
        }
    }
}
