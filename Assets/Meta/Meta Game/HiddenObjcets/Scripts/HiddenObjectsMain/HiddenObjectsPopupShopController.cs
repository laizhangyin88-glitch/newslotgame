using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsPopupShopController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private List<Blackboard> productGroupList;

        private ContextElement cellAreaElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public IEnumerator InitPropertyCoroutine()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_ENTER_FINDER_SHOP);

            // Set Caller myself for buyitem caller
            MetaObjectUtils.SetCalleeCaller(gameObject, gameObject);

            // Finder
            var finderElement = ContextUtils.FindElement(root, "Finder Area/Finder Gauge", FULL);
            var finderBB = finderElement.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(finderBB, "hideShopButton", true);

            // Anim
            anim.SetTrigger("Active");

            // Close
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, "OnClose"));

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close",
                gameObject, EventSender.ON_CUSTOM_EVENT, "OnClose");

            // Product
            productGroupList = BlackboardQueryUtils.GetShopProductGroups(ShopType.HIDDEN_UNIVERSE);

            // Cells
            cellAreaElement = ContextUtils.FindElement(root, "Shop Cell Area", CHILDREN);

            yield return StartCoroutine(MakeItemCellsCoroutine());
        }

        private IEnumerator MakeItemCellsCoroutine()
        {
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            var cellObjList = new List<GameObject>();

            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Hidden Objects Shop Item Cell Scene";
            Transform parent = cellAreaElement.transform;
            int productCount = productGroupList.Count;
            for (int i = 0; i < productCount; ++i)
            {
                var productGroup = productGroupList[i];
                var product = BlackboardQueryUtils.GetProductList(productGroup)[0];
                var item = BlackboardQueryUtils.GetItemFromProduct(product, ClientModels.ItemType.HIDDEN_UNIVERSE_FINDER);

                GameObject cellObj = null;
                yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                    (GameObject obj) => cellObj = obj));

                var cellBB = cellObj.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "itemIndex", i);
                BlackboardUtils.SetOrCreateValue(cellBB, "price", product.GetValue<double>("price"));
                BlackboardUtils.SetOrCreateValue(cellBB, "earnFinder", item.GetValue<int>("finder"));

                MetaObjectUtils.SetCalleeCaller(cellObj, gameObject);

                cellObjList.Add(cellObj);
            }

            cellObjList.ForEach(o => o.SetActive(true));

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        public void SetPurchaseInfo()
        {
            int itemIndex = bb.GetValue<int>("itemIndex");
            var productGroup = productGroupList[itemIndex];
            var product = BlackboardQueryUtils.GetProductList(productGroup)[0];
            BlackboardUtils.SetOrCreateValue(bb, "product", product);
        }

        public void OnCancelPurchase()
        {
            var mainObj = HiddenObjects.Utils.MainScene;
            EventSender.SendEvent(mainObj, HiddenObjects.Events.ON_CANCEL_PURCHASE);
        }

        public void OnSuccessPurchase()
        {
            var mainObj = HiddenObjects.Utils.MainScene;
            EventSender.SendEvent(mainObj, HiddenObjects.Events.ON_SUCCESS_PURCHASE);
        }

        public void Close()
        {
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_EXIT_FINDER_SHOP);
            anim.SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
