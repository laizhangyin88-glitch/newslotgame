using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using System;

namespace BagelCode.VipLounge
{
    public class VipLoungeSpinRewardController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;
        private string contextId;

        private List<Blackboard> productGroupList;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private ContextElement playButtonElement;
        private ContextElement othersButtonElement;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            GSManager.Instance.GetHandler(VipLounge.Defines.SPIN_REWARD_SHOP).Play();

            contextId = BiEventUtils.GenerateContextID();

            // productGroupList = BlackboardQueryUtils.GetShopProductGroups(ShopType.CASHBACK);

            // Play Anim
            anim.SetBool("Active", true);
            anim.SetBool("IsLoading", false);

            // Init Elements
            InitContents();
            InitCashbackCell();

            isInit = true;
        }

        private void InitContents()
        {
            // Product Cell
            // for (int i = 0; i < productGroupList.Count; i++)
            // {
            //     var productGroup = productGroupList[i];
            //     var product = BlackboardQueryUtils.GetProductList(productGroup)[0];
            //     var item = BlackboardQueryUtils.GetItemFromProduct(product, ItemType.CASHBACK);

            //     var iconDayText = ContextUtils.FindElement(root, $"Item Day {i + 1:00}/Text Icon Day", FULL);
            //     MetaContextElementUtils.SetTextGlobal(iconDayText, "TEXT_NORMAL", item.GetValue<int>("dayCount"));

            //     var dayAreaText = ContextUtils.FindElement(root, $"Item Day {i + 1:00}/Text Days", FULL);
            //     MetaContextElementUtils.SetTextGlobal(dayAreaText, "VIP_LOUNGE_SPIN_REWARD_PRODUCT_DAY", item.GetValue<int>("dayCount"));

            //     var priceText = ContextUtils.FindElement(root, $"Item Day {i + 1:00}/Button Purchase/Text", FULL);
            //     MetaContextElementUtils.SetTextGlobal(priceText, "VIP_LOUNGE_SPIN_REWARD_PROUDCT_PRICE", product.GetValue<double>("price"));

            //     var pruchaseButton = ContextUtils.FindElement(root, $"Item Day {i + 1:00}/Button Purchase", FULL);
            //     MetaContextElementUtils.SetClickable(pruchaseButton,
            //         () => Purchase(product));
            // }

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLOSE));
        }

        public void InitCashbackCell()
        {
            var spinRewardElement = ContextUtils.FindElement(root, "Item Collected/Item Spin Rewards", FULL);
            var spinRewardAnimator = spinRewardElement.GetComponent<Animator>();
            spinRewardAnimator.SetBool("Locked", !VipLounge.Utils.IsCashbackActive);

            var timerElement = ContextUtils.FindElement(root, "Item Collected/Text Timer", FULL);
            var timer = timerElement.GetComponent<RemainingTimerController>();
            timer.Init(timerElement, "TIME_FORMAT_HHMMSS", "TEXT_NORMAL", "", "", true,
                () => spinRewardAnimator.SetBool("Locked", !VipLounge.Utils.IsCashbackActive));
            timer.StartTimer(VipLounge.Utils.CashbackEndTimestamp, 0);

            var totalCreditElement = ContextUtils.FindElement(root, "Item Collected/Text Collected Coin", FULL);
            MetaContextElementUtils.SetTextGlobal(totalCreditElement, "TEXT_COMMA_NUMBER", VipLounge.Utils.CashbackTotalCredit);
        }

        private void Purchase(Blackboard product)
        {
            BlackboardUtils.SetOrCreateValue(bb, "product", product);
            EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_PURCHASE);
        }
    }
}
