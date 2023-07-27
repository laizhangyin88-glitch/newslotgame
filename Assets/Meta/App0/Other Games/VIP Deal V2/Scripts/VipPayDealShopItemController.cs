using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class VipPayDealShopItemController : EventMonoBehaviour
    {
        protected const int DEAL_COUNT = 3;

        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        private ContextElement multiplierElement;
        private Animator multiplierAnim;
        private ContextElement buyButtonElement;

        private ContextElement baseCreditTextElement;
        private ContextElement totalCreditTextElement;
        private ContextElement vipPointTextElement;

        private double minimumMultiplierForMaxBadge = 0.0;
        private bool isMax = false;

        private const string ON_BUY = "OnBuy";

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            multiplierElement = ContextUtils.FindElement(root, "Multiplier", CHILDREN);
            multiplierAnim = multiplierElement.GetComponent<Animator>();
            buyButtonElement = ContextUtils.FindElement(root, "Button Buy", CHILDREN);

            baseCreditTextElement = ContextUtils.FindElement(root, "Base Credit Text", CHILDREN);
            totalCreditTextElement = ContextUtils.FindElement(root, "Total Credit Text", CHILDREN);
            vipPointTextElement = ContextUtils.FindElement(root, "VIP Point Text", CHILDREN);

            MetaContextElementUtils.SetClickable(buyButtonElement, gameObject, EventSender.ON_CUSTOM_EVENT, ON_BUY, false);
        }

        public void SetInfo()
        {
            var dealInfo = BlackboardUtils.FindValue<Blackboard>(bb, "dealInfo");
            bool isSoldOut = BlackboardUtils.FindValue<bool>(dealInfo, "isSoldOut");
            BlackboardUtils.SetOrCreateValue(bb, "isSoldOut", isSoldOut);

            var product = BlackboardUtils.FindValue<Blackboard>(dealInfo, "product");
            BlackboardUtils.SetOrCreateValue(bb, "product", product);

            int vipDealInfoID = BlackboardUtils.FindValue<int>(VipDealV2.Utils.GetInfo(), "vipDealInfoId");
            BlackboardUtils.SetOrCreateValue(bb, "_vipDealInfoID", vipDealInfoID);

            string vipDealUuid = BlackboardUtils.FindValue<string>(dealInfo, "dealUuid");
            BlackboardUtils.SetOrCreateValue(bb, "_vipDealUUID", vipDealUuid);

            isMax = BlackboardUtils.FindValue<bool>(bb, "isMax");

            UpdateItemInfo();
        }

        public IEnumerator ActiveDealCoroutine()
        {
            yield return new WaitForSeconds(1f);
            multiplierAnim.SetInteger("On", isMax ? 3 : 1);
        }

        public void UpdateInfo()
        {
            UpdateItemInfo();
        }

        public void SetSoldOut()
        {
            var dealInfo = BlackboardUtils.FindValue<Blackboard>(bb, "dealInfo");
            BlackboardUtils.SetOrCreateValue(dealInfo, "isSoldOut", true);
            anim.SetBool("SoldOut", true);
            MetaContextElementUtils.SetBooleanProperty(buyButtonElement, false);
        }

        private void UpdateItemInfo()
        {
            var dealInfo = BlackboardUtils.FindValue<Blackboard>(bb, "dealInfo");
            var product = BlackboardUtils.FindValue<Blackboard>(dealInfo, "product");

            // Base Coin
            var coinItemBB = BlackboardQueryUtils.GetItemFromProduct(product, ItemType.CREDIT);
            var baseCredit = BlackboardUtils.FindValue<long>(coinItemBB, "baseCredit");

            int tier = TierUtils.GetMeTier();
            long totalBaseCoins = TierUtils.GetTierFractionCoin(baseCredit, tier);

            long additionalCreditMultiplierNumerator = BlackboardUtils.FindValue<long>(coinItemBB, "additionalCreditMultiplierNumerator");
            totalBaseCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalBaseCoins, "vipDeal");
            totalBaseCoins = NumberUtils.GetAdditionalMultiplierNumeratorValue(totalBaseCoins, additionalCreditMultiplierNumerator);

            MetaContextElementUtils.SetTextGlobal(baseCreditTextElement, "VIP_DEAL_SHOP_ITEM_BASE_COIN", totalBaseCoins);

            // Total Coin
            long eventMultiplierNumerator = BlackboardUtils.FindValue<long>(product, "eventMultiplierNumerator");
            long itemMultiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
            long totalCredit = NumberUtils.GetMultiplierNumeratorValue(totalBaseCoins, eventMultiplierNumerator);
            totalCredit = NumberUtils.GetMultiplierNumeratorValue(totalCredit, itemMultiplierNumerator);

            MetaContextElementUtils.SetTextGlobal(totalCreditTextElement, "VIP_DEAL_SHOP_ITEM_TOTAL_COIN", totalCredit);

            // Multiplier
            double multiplier = NumberUtils.GetMultiplierFromNumerator(itemMultiplierNumerator);
            MetaContextElementUtils.SimpleSetTextGlobal(
                multiplierElement, "Text", "VIP_DEAL_SHOP_ITEM_MULTIPLIER", CHILDREN, multiplier);

            // Buy Button
            float price = (float)BlackboardUtils.FindValue<double>(product, "price");
            var rp = BlackboardUtils.FindValue<long>(coinItemBB, "rp");
            MetaContextElementUtils.SimpleSetTextGlobal(buyButtonElement, "Text", "VIP_DEAL_SHOP_ITEM_BUY_PRICE", CHILDREN, price);
            MetaContextElementUtils.SetTextGlobal(vipPointTextElement, "VIP_DEAL_SHOP_ITEM_RP", rp);

            int coinLevel = totalCredit > VipDealV2.Defines.COIN_GRADE_SEPARATOR_2 ? 2 :
                totalCredit > VipDealV2.Defines.COIN_GRADE_SEPARATOR_1 ? 1 : 0;
            anim.SetInteger("Coin", coinLevel); // Todo shk
        }
    }
}
