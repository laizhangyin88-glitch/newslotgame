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
    public class EpicPassRewardInboxController : MonoBehaviour
    {
        private bool isInit = false;
        private ContextElement rootElement;
        private ContextButton okButton;

        private void Update()
        {
            if (isInit == false)
                Initialize();
        }

        private void Initialize()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            UpdateContext();

            okButton.AddListenerOnClick(
                context =>
                {
                    EpicPassUtils.OpenRewardScene();
                    Destroy(gameObject);
                }
            );

            ContextButton closeButton = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            closeButton.AddListenerOnClick(
                context =>
                {
                    EpicPassUtils.OpenRewardScene();
                    Destroy(gameObject);
                }
            );

            isInit = true;
        }

        private void UpdateContext()
        {
            ContextTextMeshProUGUI titleText = ContextUtils.FindElement(rootElement, "Title Area Wide/Text", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();

            okButton = ContextUtils.FindElement(rootElement, "Button Ok", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI buyText = ContextUtils.FindElement(okButton, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            MetaContextElementUtils.SetTextGlobal(titleText, "EPIC_PASS_REWARD_INBOX_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(buyText, "EPIC_PASS_REWARD_INBOX_BUTTON_TEXT");
        }
    }
}
