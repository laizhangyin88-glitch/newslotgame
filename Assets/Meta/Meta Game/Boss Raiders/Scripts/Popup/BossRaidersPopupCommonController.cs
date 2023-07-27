using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersPopupCommonController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement adButtonElement;
        private ContextElement purchaseButtonElement;
        private ContextElement messageTextElement;
        private ContextElement closeAreaElement;

        private bool isInit = false;
        private bool isMeta = true;

        public string biType = "";

        public void OnInit()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            adButtonElement = ContextUtils.FindElement(rootElement, "Button Ad", ContextSearchingType.ChildrenSearch);
            purchaseButtonElement = ContextUtils.FindElement(rootElement, "Button Purchase", ContextSearchingType.ChildrenSearch);
            messageTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            closeAreaElement = ContextUtils.FindElement(rootElement, "Button Close Area", ContextSearchingType.ChildrenSearch);
            ContextElement closeButtonElement = ContextUtils.FindElement(closeAreaElement, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                purchaseButtonElement,
                "OnPurchaseClick",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                adButtonElement,
                "OnAdClick",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                "OnCloseClick",
                rootElement,
                null
            );

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

        public void SetIsMeta(bool _isMeta)
        {
            isMeta = _isMeta;
        }

        public void BIClientBossRaidersPopup(string contextId, string type)
        {
            if (isMeta)
                BossRaidersUtils.BIClientBossRaidersPopup(contextId, type);
            else
                BossRaidersUtils.BIClientBossRaidersDealPopup(contextId, type);
        }

        public void BIClientClickBossRaidersPopup(string contextId, string type)
        {
            if (isMeta)
                BossRaidersUtils.BIClientClickBossRaidersPopup(contextId, type);
            else
                BossRaidersUtils.BIClientClickBossRaidersDealPopup(contextId, type);
        }
    }
}