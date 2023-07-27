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
    public class EpicPassRewardLostController : MonoBehaviour
    {
        private bool isInit = false;
        private ContextElement rootElement;
        private ContextButton buyButton, giveUpButton;

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

            buyButton.AddListenerOnClick(
                context =>
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();
                    customData["context_id"] = EpicPassUtils.contextId;
                    customData["action_type"] = "upgrade";
                    Analytics.CustomEvent("client_season_pass_upgrade_popup", customData);

                    EventSender.SendGlobalEvent("OnPurchase");
                    Destroy(gameObject);
                }
            );

            giveUpButton.AddListenerOnClick(
                context =>
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();
                    customData["context_id"] = EpicPassUtils.contextId;
                    customData["action_type"] = "give_up";
                    Analytics.CustomEvent("client_season_pass_upgrade_popup", customData);

                    EpicPassUtils.RequestEpicPassReset();
                    Destroy(gameObject);
                }
            );

            ContextButton closeButton = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            closeButton.AddListenerOnClick(
                context =>
                {
                    EpicPassUtils.IsClickProcess = false;
                    Destroy(gameObject);
                }
            );

            isInit = true;
        }

        private void UpdateContext()
        {
            ContextTextMeshProUGUI titleText = ContextUtils.FindElement(rootElement, "Title Area Wide/Text", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();

            giveUpButton = ContextUtils.FindElement(rootElement, "Button Give Up", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI giveUpText = ContextUtils.FindElement(giveUpButton, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            buyButton = ContextUtils.FindElement(rootElement, "Button Buy", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI buyText = ContextUtils.FindElement(buyButton, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            var productBB = EpicPassUtils.EpicPassInfo.GetValue<Blackboard>("epicPassProduct");
            MetaContextElementUtils.SetTextGlobal(titleText, "EPIC_PASS_REWARD_LOST_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(giveUpText, "EPIC_PASS_REWARD_LOST_BUTTON_TEXT_0");
            MetaContextElementUtils.SetTextGlobal(buyText, "EPIC_PASS_REWARD_LOST_BUTTON_TEXT_1", productBB.GetValue<double>("price"));
        }
    }
}
