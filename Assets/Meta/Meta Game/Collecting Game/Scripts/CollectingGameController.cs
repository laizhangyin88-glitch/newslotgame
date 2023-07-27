using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class CollectingGameController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private UnityEngine.CanvasGroup canvas;
        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            canvas = GetComponent<UnityEngine.CanvasGroup>();

            root.UpdateContext(false);
        }

        public IEnumerator BuyChestCoroutine()
        {
            // Collecting Game Interactable disable
            canvas.interactable = false;

            // Check IAM
            bool purchased = false;
            bool triggered = IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.GEM_CHEST_SHOP, gameObject, null);
            if (triggered)
            {
                var iamCallbackTrigger = new EventTrigger(gameObject, IAMUtils.ON_IAM_CALLBACK_EVENT);
                yield return new WaitUntilTrigger(iamCallbackTrigger);

                purchased = bb.GetVariable<bool>("isPurchased")?.value ?? false;
                bb.AddVariable("isPurchased", false);
            }

            // Not Purchased Go Shop
            if (!triggered || !purchased)
            {
                yield return StartCoroutine(MakeShopCoroutine());
            }
        }
        private IEnumerator MakeShopCoroutine()
        {
            //Loading
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine((obj) => loadingObj = obj));

            // Make Shop Scene
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Shop Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            GameObject shopSceneObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => shopSceneObj = sceneObj));

            // Send Bi
            var contextId = BiEventUtils.GenerateContextID();
            SendBiEvent("client_click_collecting_game_buy_pack", contextId);

            var shopBB = shopSceneObj.GetComponent<Blackboard>();
            shopBB.AddVariable("_biContextID", contextId);
            shopBB.AddVariable("initTabIndex", 0);
            shopBB.AddVariable("IsEnableCloseIAM", true);

            MetaPopupUtils.OpenPopup(shopSceneObj);

            MetaPopupUtils.ClosePopup(loadingObj);

            var closeShopTrigger = new EventTrigger(gameObject, MetaEventDefine.ON_META_UI_EVENT, "CloseShop");
            yield return new WaitUntilTrigger(closeShopTrigger);
        }

        private void SendBiEvent(string eventName, string contextId)
        {
            var customData = new Dictionary<string, object>
            {
                ["context_id"] = contextId
            };

            BiEventUtils.AppendCollectingGameEventData(customData);

            Analytics.CustomEvent(eventName, customData);
        }

    }
}
