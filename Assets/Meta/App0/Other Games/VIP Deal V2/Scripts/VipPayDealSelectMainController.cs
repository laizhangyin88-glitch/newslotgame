using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Linq;

namespace BagelCode
{
    public class VipPayDealSelectMainController : VipDealV2SelectMainControllerBase
    {
        private ContextElement skipButtonElement;

        private List<int> remainingIndexList = new List<int>();

        public override void Init()
        {
            base.Init();
            isPayDeal = true;

            for (int i = 0; i < CELL_COUNT; ++i)
                remainingIndexList.Add(i);

            // Skip
            skipButtonElement = ContextUtils.FindElement(root, "Button Skip", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(skipButtonElement, "Text", "VIP_DEAL_V2_PAY_SELECT_SKIP_BUTTON_TEXT", CHILDREN);
            MetaContextElementUtils.SetClickable(skipButtonElement, OnClickSkip);

            // Icon Web Image
            var webImageElement = ContextUtils.FindElement(root, "VIP Deal Icon/Multiplier Web Image", FULL);
            MetaContextElementUtils.SetWebImage(webImageElement, VipDealV2.Utils.GetIconWebImageUrl(), CacheType.MemCache, false, null);
        }

        protected override IEnumerator RequestSelectItemCoroutine()
        {
            var loadingObj = MetaPopupUtils.OpenLoadingPopup();

            int vipDealId = BlackboardUtils.FindValue<int>(vipInfo, "vipDealInfoId");
            bool isDone = false;
            BagelCodeClientAPI.VipPayDealSelectComplete(vipDealId,
            (response) =>
            {
                VipDealV2.Utils.SetIsViewed(true, isPayDeal);
                isDone = true;
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        public override IEnumerator OnSelectCoroutine(int index)
        {
            if (dealSelectCount.value == 0) // first
            {
                yield return StartCoroutine(RequestSelectItemCoroutine());

                bool isViewed = VipDealV2.Utils.GetIsViewed(isPayDeal);
                if (!isViewed) yield break; // request failed
            }

            remainingIndexList.Remove(index);

            var currentDealInfo = dealInfoList[dealSelectCount.value];
            dealSelectCount.value += 1;

            targetItemCellObj = itemCellObjectList[index];
            selectCellObjectList.Add(targetItemCellObj);

            long dealMultiplierNumerator = BlackboardUtils.FindValue<long>(currentDealInfo, "multiplierNumerator");
            bool isMax = dealMultiplierNumerator == wheelMultiplierNumeratorList.Max();

            var cellBB = targetItemCellObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(cellBB, "dealInfo", currentDealInfo);
            BlackboardUtils.SetOrCreateValue(cellBB, "isMax", isMax);

            var eventData = new EventData<int>(VipDealV2.Events.ON_OPEN_CARD, index);
            EventSender.SendGlobalEvent(eventData);
        }

        public override void Close()
        {
            anim.SetTrigger("Close");
        }

        public void SelectRandom()
        {
            int index = remainingIndexList.PopRandom();
            BlackboardUtils.SetOrCreateValue(bb, "selectIndex", index);
        }

        public void EnterShop()
        {
            EventSender.SendGlobalEvent(VipDealV2.Events.ON_ENTER_VIP_DEAL_SHOP);
        }

        private void OnClickSkip()
        {
            MetaContextElementUtils.SetBooleanProperty(skipButtonElement, false);
            BlackboardUtils.SetOrCreateValue(bb, "isEnableSkip", true);
        }
    }
}
