using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class VipFreeDealWinItemController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private ContextElement redeemButtonElement;
        private ContextElement multiplierElement;

        private Blackboard dealInfo = null;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            dealInfo = BlackboardUtils.FindValue<Blackboard>(bb, "dealInfo");

            long baseCredit = BlackboardUtils.FindValue<long>(dealInfo, "creditAmount");
            long multiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
            long totalCredit = NumberUtils.GetMultiplierNumeratorValue(baseCredit, multiplierNumerator);
            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

            multiplierElement = ContextUtils.FindElement(root, "Multiplier", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(
                multiplierElement, "Text", "VIP_DEAL_SHOP_ITEM_MULTIPLIER", CHILDREN, multiplier);

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Base Credit Text", "VIP_DEAL_SHOP_ITEM_BASE_COIN", CHILDREN, baseCredit);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Total Credit Text", "VIP_DEAL_SHOP_ITEM_TOTAL_COIN", CHILDREN, totalCredit);

            redeemButtonElement = ContextUtils.FindElement(root, "Button Free", CHILDREN);
            MetaContextElementUtils.SetClickable(redeemButtonElement, OnClickRedeem);
            MetaContextElementUtils.SimpleSetTextGlobal(redeemButtonElement, "Text", "VIP_DEAL_V2_WIN_POPUP_REDEEM", CHILDREN);
        }

        private void OnClickRedeem()
        {
            EventSender.SendCalleeCallback(gameObject, VipDealV2.Events.ON_REDEEM);
        }
    }
}