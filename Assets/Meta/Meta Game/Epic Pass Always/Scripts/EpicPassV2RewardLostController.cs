using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class EpicPassV2RewardLostController : MonoBehaviour
    {
        private bool isInit = false;
        private ContextElement rootElement;
        private ContextButton buyButtonElement;
        private ContextButton giveUpButtonElement;

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

            buyButtonElement.AddListenerOnClick(
                context =>
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();
                    customData["context_id"] = EpicPassUtilsV2.contextId;
                    customData["action_type"] = "upgrade";
                    Analytics.CustomEvent("client_season_pass_upgrade_popup", customData);

                    EventSender.SendGlobalEvent("OnPurchase");
                    Destroy(gameObject);
                }
            );

            giveUpButtonElement.AddListenerOnClick(
                context =>
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();
                    customData["context_id"] = EpicPassUtilsV2.contextId;
                    customData["action_type"] = "give_up";
                    Analytics.CustomEvent("client_season_pass_upgrade_popup", customData);

                    EpicPassUtilsV2.RequestEpicPassReset();
                    Destroy(gameObject);
                }
            );

            ContextButton closeButton = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            closeButton.AddListenerOnClick(
                context =>
                {
                    EpicPassUtilsV2.IsClickProcess = false;
                    EpicPassUtilsV2.SendWelcomeVIPLoungePopup(gameObject);
                    Destroy(gameObject);
                }
            );

            isInit = true;
        }

        private void UpdateContext()
        {
            ContextTextMeshProUGUI titleText = ContextUtils.FindElement(rootElement, "Title Area Wide/Text", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();

            giveUpButtonElement = ContextUtils.FindElement(rootElement, "Button Give Up", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI giveUpText = ContextUtils.FindElement(giveUpButtonElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            buyButtonElement = ContextUtils.FindElement(rootElement, "Button Buy", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI buyText = ContextUtils.FindElement(buyButtonElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            var productBB = EpicPassUtilsV2.EpicPassInfo.GetValue<Blackboard>("epicPassProduct");
            MetaContextElementUtils.SetTextGlobal(titleText, "EPIC_PASS_REWARD_LOST_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(giveUpText, "EPIC_PASS_REWARD_LOST_BUTTON_TEXT_0");
            MetaContextElementUtils.SetTextGlobal(buyText, "EPIC_PASS_REWARD_LOST_BUTTON_TEXT_1", productBB.GetValue<double>("price"));
        }
    }
}