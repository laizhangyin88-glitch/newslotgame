using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode
{
    public class PopupPurchaseResultController : MonoBehaviour
    {
        public RewardType type;

        private Blackboard bb;
        private ContextElement root;

        public void InitProperty()
        {
            bb = GetComponent<Blackboard>();
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            MakeItemIconObj();
        }

        private void MakeItemIconObj()
        {
            bool isCoin = type == RewardType.CREDIT;
            bool isGem = type == RewardType.GEM;

            if (!isCoin && !isGem) return;

            var product = BlackboardUtils.FindVariable<Blackboard>(bb, "product");
            if(product == null || product.value == null) return;

            var price = product.value.GetValue<double>("originalPrice");

            List<double> priceSectionList;
            if (isCoin)
            {
                priceSectionList = BlackboardUtils.FindVariable<List<double>>(MainBlackboard.Get(), "values/misc/SHOP_COIN_ICON_SECTION_LIST")?.value;
            }
            else
            {
                priceSectionList = BlackboardUtils.FindVariable<List<double>>(MainBlackboard.Get(), "values/misc/SHOP_GEM_ICON_SECTION_LIST")?.value;
            }

            int grade = priceSectionList.IndexOfRange(price) + 1;


            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = (isCoin ? "Image Coin Block Lv " : "Image Gem Block Lv ") + grade.ToString();

            var imageAreaElement = ContextUtils.FindElement(root, "Item Area", ContextSearchingType.FullNameSearch);
            if(imageAreaElement == null) return;

            Transform parent = imageAreaElement.transform;

            MetaObjectUtils.MakePrefab(bundle, asset, parent);
        }
    }
}
