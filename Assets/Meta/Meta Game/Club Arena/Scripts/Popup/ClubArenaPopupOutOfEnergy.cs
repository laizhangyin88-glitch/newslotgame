using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using ParadoxNotion.Services;
using ParadoxNotion;

namespace BagelCode.ClubArena
{
    public class ClubArenaPopupOutOfEnergy : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement adButtonElement;
        private ContextElement purchaseButtonElement;
        private ContextElement messageTextElement;
        private ContextElement adFreeEnergyTextElement;
        private ContextElement closeAreaElement;

        private ContextElement callerElement;

        private bool isInit = false;

        public string popupContextID = "";
        public string biType = "";

        public void OnInit()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();
            Blackboard rootBB = gameObject.GetComponent<Blackboard>();

            adButtonElement = ContextUtils.FindElement(rootElement, "Button Ad", ContextSearchingType.ChildrenSearch);
            purchaseButtonElement = ContextUtils.FindElement(rootElement, "Button Purchase", ContextSearchingType.ChildrenSearch);
            messageTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement adElement = ContextUtils.FindElement(adButtonElement, "Ad", ContextSearchingType.ChildrenSearch);
            adFreeEnergyTextElement = ContextUtils.FindElement(adElement, "Free Energy Text", ContextSearchingType.ChildrenSearch);

            ContextElement closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            GameObject caller = BlackboardUtils.FindValue<GameObject>(rootBB, "caller");
            callerElement = caller?.GetComponent<ContextElement>();

            MetaContextElementUtils.SetClickable(
                purchaseButtonElement,
                OnClickPurchase
            );

            MetaContextElementUtils.SetClickable(
                adButtonElement,
                OnClickAds
            );

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                OnClickClose
            );

            adButtonElement.gameObject.SetActive(false);

            isInit = true;
        }

        public void SetAdButton(string text)
        {
            if (adButtonElement != null)
            {
                adButtonElement.gameObject.SetActive(true);
                MetaContextElementUtils.SimpleSetText(adButtonElement, "Text", text);
            }
        }

        public void SetMessageText(string text)
        {
            if (messageTextElement != null)
                MetaContextElementUtils.SetText(messageTextElement, text);
        }

        public void SetFreeEnergyText(string text)
        {
            if (adFreeEnergyTextElement != null)
                MetaContextElementUtils.SetText(adFreeEnergyTextElement, text);
        }

        public void SetPurchaseButton(string text)
        {
            if (purchaseButtonElement != null)
                MetaContextElementUtils.SimpleSetText(purchaseButtonElement, "Text", text);
        }

        public void SetCloseActive(bool isActive = false)
        {
            if (closeAreaElement != null)
                closeAreaElement.gameObject.SetActive(isActive);
        }

        public void SetBIType(string type)
        {
            biType = type;
        }

        public void SetPopupContextID(string contextID)
        {
            popupContextID = contextID;
        }

        public void SetActive(bool isActive)
        {
            rootAnimator?.SetBool("Active", isActive);
        }

        private void OnClickPurchase()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("spin", popupContextID);
            SendCallerEvent("OnPurchaseClick");
        }

        private void OnClickAds()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("watch_ad", popupContextID);
            SendCallerEvent("OnAdsClick");
        }

        private void OnClickClose()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("close", popupContextID);
            SendCallerEvent("OnCloseClick");
        }

        private void SendCallerEvent(string eventName)
        {
            if (callerElement != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(callerElement);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName), callerElement);
            }
        }
    }
}