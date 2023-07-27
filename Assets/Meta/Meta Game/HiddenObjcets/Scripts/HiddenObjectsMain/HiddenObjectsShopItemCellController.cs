using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsShopItemCellController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            var sampleArea = ContextUtils.FindElement(root, "Finder Sample Area", CHILDREN);

            int earnFinder = bb.GetValue<int>("earnFinder");

            float multi = earnFinder / HiddenObjects.Utils.MaxFinder;
            int barCount = Mathf.Clamp((int)multi, 1, 3);

            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Bar",
                "HIDDEN_OBJECTS_FINDER_SHOP_ITEM_CELL_TITLE", CHILDREN, barCount);

            // Desc
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Of Finder",
                "HIDDEN_OBJECTS_FINDER_SHOP_ITEM_CELL_DESC", CHILDREN, barCount);

            // Earn Finder
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Item Number",
                "HIDDEN_OBJECTS_FINDER_SHOP_ITEM_CELL_FINDER", CHILDREN, earnFinder);

            // Purchase Button
            double price = bb.GetValue<double>("price");
            var purchaseButton = ContextUtils.FindElement(root, "Button Purchase", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(purchaseButton, "Text",
                "HIDDEN_OBJECTS_FINDER_SHOP_ITEM_CELL_PRICE", CHILDREN, price);

            int itemIndex = bb.GetValue<int>("itemIndex");
            var caller = bb.GetValue<GameObject>("caller");
            var eventData = new EventData<int>("OnPurchase", itemIndex);
            MetaContextElementUtils.SetClickable(purchaseButton, caller, EventSender.ON_CUSTOM_EVENT, eventData, false);

            // Finder Bar Image
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Hidden Objects Finder Sample Cell";
            Transform parent = sampleArea.transform;
            for (int i = 0; i < barCount; ++i)
            {
                var sampleCellObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

                if(i == 0)
                {
                    var sampleCellElement = sampleCellObj.GetComponent<ContextElement>();
                    sampleCellElement.UpdateContext(true);

                    if (multi > 0)
                    {
                        MetaContextElementUtils.SimpleSetTextGlobal(sampleCellElement, "Text Finder Multiple",
                            "HIDDEN_OBJECTS_FINDER_SHOP_ITEM_CELL_MULTI", CHILDREN, multi);
                    }
                }
            }
        }

    }
}
