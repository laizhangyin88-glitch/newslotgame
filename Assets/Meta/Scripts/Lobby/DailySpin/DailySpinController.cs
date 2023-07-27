using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class DailySpinController : EventMonoBehaviour
    {
        private ContextElement root;
        private GraphOwner owner;
        private Blackboard bb;

        private ContextElement autoSpinAnchorElement;
        private ContextElement autoSpinButtonElement;
        private ContextElement megaSpinButtonElement;
        private ContextElement buyDailySpinButtonElement;
        private ContextElement buyMegaWheelSpinButtonElement;

        private DailySpinButtonController autoSpinButtonController;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        //

        public void InitProperty()
        {
            owner = GetComponent<GraphOwner>();
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            autoSpinAnchorElement = ContextUtils.FindElement(root, "Button Auto Spin Anchor", FULL);
            autoSpinButtonElement = ContextUtils.FindElement(autoSpinAnchorElement, "Button Auto Spin", CHILDREN);
            megaSpinButtonElement = ContextUtils.FindElement(root, "Button Mega Spin", FULL);
            buyDailySpinButtonElement = ContextUtils.FindElement(root, "Button Buy Daily Spin", FULL);
            buyMegaWheelSpinButtonElement = ContextUtils.FindElement(root, "Button Buy Mega Wheel Spin", FULL);

            MetaContextElementUtils.SimpleSetTextGlobal(autoSpinButtonElement, "Anchor/Spin Text", "BUTTON_DAILY_SPIN_SPIN", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(autoSpinButtonElement, "Anchor/Auto Text", "BUTTON_DAILY_SPIN_HOLD FOR AUTO", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(megaSpinButtonElement, "Text", "BUTTON_DAILY_SPIN_SPIN", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(buyDailySpinButtonElement, "Text", "BUTTON_DAILY_SPIN_BUY_SPINS", CHILDREN);

            MetaContextElementUtils.SetClickable(megaSpinButtonElement, () => owner.SendEvent("OnDailySpin"));
            MetaContextElementUtils.SetClickable(buyDailySpinButtonElement, () => owner.SendEvent("OnDailySpin"));
            MetaContextElementUtils.SetClickable(buyMegaWheelSpinButtonElement, () => owner.SendEvent("OnDailySpin"));

            autoSpinButtonController = autoSpinButtonElement.gameObject.AddComponent<DailySpinButtonController>();
            autoSpinButtonController.owner = owner;
            autoSpinButtonController.ownerBB = bb;

            UpdateSpinButton();
        }

        public IEnumerator OpenSpinShopPopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Daily Bonus Shop Scene";
            Transform popupRoot = MetaPopupUtils.PopupManagerAreaTransform;

            var productGroupList = BlackboardQueryUtils.GetShopProductGroups(ShopType.DAILY_BONUS);
            int productGroupCount = productGroupList.Count;

            GameObject shopPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, popupRoot,
                (SceneLoadOperation scene) => { shopPopupObj = scene.GetScene(); }));

            var shopBB = shopPopupObj.GetComponent<Blackboard>();

            shopBB.AddVariable("caller", gameObject);

            MetaPopupUtils.OpenPopup(shopPopupObj);

            var callbackTrigger = new EventTrigger(this, "OnFinishedCallee");
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator OpenTotalResultPopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Contents Total Result Scene";
            Transform popupRoot = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject totalResultPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, popupRoot,
                (SceneLoadOperation scene) => { totalResultPopupObj = scene.GetScene(); }));

            // Set Total Result Data
            var totalResultBB = totalResultPopupObj.GetComponent<Blackboard>();
            var prizeList = bb.GetVariable<List<long>>("_resultCreditList")?.value ?? new List<long>();
            var isJackpotList = bb.GetVariable<List<bool>>("_resultIsJackpotList")?.value ?? new List<bool>();

            totalResultBB.AddVariable("caller", gameObject);
            totalResultBB.AddVariable("_prizeList", prizeList);
            totalResultBB.AddVariable("_isJackpotList", isJackpotList);
            totalResultBB.AddVariable("_isFromDailySpin", true);

            MetaPopupUtils.OpenPopup(totalResultPopupObj);

            var callbackTrigger = new EventTrigger(this, "OnCalleeCallback");
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public void UpdateSpinButton()
        {
            bool isMega = bb.GetValue<bool>("_megaWheelState");
            int spinCount = bb.GetValue<int>("spinCount");
            bool enableSpin = spinCount > 0;

            autoSpinButtonElement.gameObject.SetActive(enableSpin && !isMega);
            megaSpinButtonElement.gameObject.SetActive(enableSpin && isMega);
            buyDailySpinButtonElement.gameObject.SetActive(!enableSpin && !isMega);
            buyMegaWheelSpinButtonElement.gameObject.SetActive(!enableSpin && isMega);

            // Buy Mega
            if(!enableSpin && isMega)
            {
                var megaWheelProductGroupList = BlackboardQueryUtils.GetShopProductGroups(ClientModels.ShopType.MEGA_WHEEL);
                if(megaWheelProductGroupList != null)
                {
                    var productList = megaWheelProductGroupList[0].GetValue<List<Blackboard>>("productList");
                    var product = productList[0];
                    bb.AddVariable("product", product);

                    double price = product.GetVariable<double>("price")?.value ?? 0.0;

                    MetaContextElementUtils.SimpleSetTextGlobal(
                        buyMegaWheelSpinButtonElement, "Text", "BUTTON_DAILY_SPIN_BUY_MEGA_SPINS", CHILDREN, price);
                }
            }
        }

        public void SetMegaPurchaseCaller(bool isReady)
        {
            if (isReady)
                bb.AddVariable("caller", gameObject);
            else
            {
                var callerMe = bb.GetVariable<GameObject>("caller");
                if (callerMe != null)
                    callerMe.value = null;
            }
        }

        public void ForceStopAuto()
        {
            autoSpinButtonController.SetIsAuto(false);
            autoSpinButtonController.ForceStopAuto();
        }
    }
}