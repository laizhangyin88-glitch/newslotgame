using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class PopupEpicPassV2RewardInboxController : MonoBehaviour
    {
        private bool isInit = false;
        private ContextElement rootElement;
        private ContextButton okButton;
        private ContextButton closeButton;

        private void Update()
        {
            if (isInit == false)
                Initialize();
        }

        private void Initialize()
        {
            if (isInit) return;

            InitProperty();
            InitButtonEvent();

            isInit = true;
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            ContextTextMeshProUGUI titleText = ContextUtils.FindElement(rootElement, "Title Area Wide/Text", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();

            closeButton = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            okButton = ContextUtils.FindElement(rootElement, "Button Ok", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI buyText = ContextUtils.FindElement(okButton, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            MetaContextElementUtils.SetTextGlobal(titleText, "EPIC_PASS_REWARD_INBOX_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(buyText, "EPIC_PASS_REWARD_INBOX_BUTTON_TEXT");
        }

        private void InitButtonEvent()
        {
            closeButton.AddListenerOnClick(
                context =>
                {
                    EpicPassUtilsV2.OpenRestartScene();
                    Destroy(gameObject);
                }
            );

            okButton.AddListenerOnClick(
                context =>
                {
                    EpicPassUtilsV2.OpenRestartScene();
                    Destroy(gameObject);
                }
            );
        }
    }
}