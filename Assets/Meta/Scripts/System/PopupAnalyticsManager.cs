using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class PopupAnalyticsManager : MonoSingleton<PopupAnalyticsManager>
    {
        private const string EVENT_POPUP_OPEN = "PopupOpen";
        private const string EVENT_POPUP_CLOSE = "PopupClose";

        private void Start()
        {
            // Init
            InternalEventRouter.Instance.Register(EVENT_POPUP_OPEN, OnOpenMetaPopup);
            InternalEventRouter.Instance.Register(EVENT_POPUP_CLOSE, OnCloseMetaPopup);
        }

        private void OnOpenMetaPopup(EventRouterData eventData)
        {
            if (eventData != null)
                SendAE(true, eventData.objName, eventData.guid);
        }

        private void OnCloseMetaPopup(EventRouterData eventData)
        {
            if (eventData != null)
                SendAE(false, eventData.objName, eventData.guid);
        }

        private void SendAE(bool isOpen, string name, string guid)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["popup_name"] = name;
            customData["type"] = isOpen ? "open" : "close";
            customData["context_id"] = guid;
            Analytics.CustomEvent("client_popup", customData);
        }
    }
}
