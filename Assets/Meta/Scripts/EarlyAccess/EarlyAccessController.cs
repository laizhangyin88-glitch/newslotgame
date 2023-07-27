using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode
{
    public class EarlyAccessController : EventMonoBehaviour
    {
        private const string ENTER_CUSTOMER_SUPPORT = "OnEnterCustomerSupport";

        private void Start()
        {
            Register(ENTER_CUSTOMER_SUPPORT, OnOpenCustomerSupport);
        }

        private void OnOpenCustomerSupport(EventData eventData)
        {
            AEUtils.SendAE("client_click_customer_support", ("type", "default"));

            string supportPageUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/SUPPORT_PAGE_URL").value;

#if UNITY_WEBGL && !UNITY_EDITOR
            NativeHelper.Instance.OpenUrl(UrlBuildUtils.GetHelpCenterUrl(supportPageUrl));
#else
            Application.OpenURL(UrlBuildUtils.GetHelpCenterUrl(supportPageUrl));
#endif
        }
    }
}
