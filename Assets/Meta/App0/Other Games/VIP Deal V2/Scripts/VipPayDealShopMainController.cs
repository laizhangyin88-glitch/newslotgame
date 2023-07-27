using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode
{
    public class VipPayDealShopMainController : EventMonoBehaviour
    {
        private const int DEAL_COUNT = 3;
        private const float TIMER_UPDATE_INTERVAL = 0.2f;

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement titleIconElement;
        private Animator titleIconAnim;
        private ContextElement tierMultiplierElement;
        private ContextElement remainingTextElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            titleIconElement = ContextUtils.FindElement(root, "VIP Deal Icon", CHILDREN);
            titleIconAnim = titleIconElement.GetComponent<Animator>();
            tierMultiplierElement = ContextUtils.FindElement(root, "Icon Tier Multiplier", CHILDREN);
            remainingTextElement = ContextUtils.FindElement(titleIconElement, "Text", CHILDREN);

            var webImageElement = ContextUtils.FindElement(root, "VIP Deal Icon/Multiplier Web Image", FULL);
            string webImageUrl = VipDealV2.Utils.GetIconWebImageUrl();
            MetaContextElementUtils.SetWebImage(webImageElement, webImageUrl, CacheType.FileCache, false, null);

            Deactivate();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_SYSTEM_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_META_GAME, Deactivate);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEAVE_META_GAME, Activate);
            Register(MetaEventDefine.ON_SYSTEM_EVENT, MetaEventDefine.SYSTEM_RESET, Close);
            Register("OnLeavePrevScene", Close);
        }

        public IEnumerator OnAppearCoroutine()
        {
            Activate();

            yield return new WaitForSeconds(0.2f);

            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendGlobalEvent("OnHomeButton"));

            string contextId = SendAE();
            InitCells(contextId);

            titleIconAnim.SetBool("Timer", true);
            titleIconAnim.SetBool("Default", true);

            MetaContextElementUtils.SetBlackboardValue(
                tierMultiplierElement, "multiplierType", TierMultiplierTableType.CoinMultiplier);

            EventSender.SendEvent(tierMultiplierElement.gameObject, "RefreshTier");
        }

        public IEnumerator UpdateTimerCoroutine()
        {
            var vipDealInfo = VipDealV2.Utils.GetInfo();
            long endTimestamp = BlackboardUtils.FindValue<long>(vipDealInfo, "endTimestamp");
            long warningTimestamp = endTimestamp - 3600000; // 1 hour
            while (true)
            {
                string remainingText = TimeUtils.GetRemainingTimeText(endTimestamp, "TIME_FORMAT_HHMMSS_TOTALHOUR", warningTimestamp,
                    "TEXT_SHOP_EVENT", "TEXT_SHOP_EVENT_WARNING", true);

                MetaContextElementUtils.SetText(remainingTextElement, remainingText);

                yield return new WaitForSeconds(TIMER_UPDATE_INTERVAL);
            }
        }

        private void InitCells(string contextId)
        {
            List<Blackboard> dealList = VipDealV2.Utils.GetDealList(true);

            Blackboard freeDeal = dealList.PopTarget(
                (Blackboard deal) => BlackboardUtils.FindVariable<bool>(deal, "isFree")?.value ?? false);

            dealList.Sort(
                (Blackboard a, Blackboard b) =>
                {
                    long aMultiplierNumerator = BlackboardUtils.FindValue<long>(a, "multiplierNumerator");
                    long bMultiplierNumerator = BlackboardUtils.FindValue<long>(b, "multiplierNumerator");
                    return bMultiplierNumerator.CompareTo(aMultiplierNumerator);
                });

            var vipDealInfo = VipDealV2.Utils.GetInfo();
            int dealIndex = 0;
            for (int i = 0; i < DEAL_COUNT; ++i)
            {
                bool isCenter = i == DEAL_COUNT / 2;
                Blackboard dealInfo;
                if (freeDeal != null && isCenter) dealInfo = freeDeal;
                else dealInfo = dealList[dealIndex++];

                int vipDealInfoId = BlackboardUtils.FindValue<int>(vipDealInfo, "vipDealInfoId");

                string cellName = string.Format("{0:00}/Shop Item", i + 1);
                var cellElement = ContextUtils.FindElement(root, cellName, FULL);

                long dealMultiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
                var wheelMultiplierNumeratorList = VipDealV2.Utils.GetWheelMultiplierNumeratorList(true);
                bool isMax = dealMultiplierNumerator == wheelMultiplierNumeratorList.Max();

                var cellBB = cellElement.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "dealInfo", dealInfo);
                BlackboardUtils.SetOrCreateValue(cellBB, "biContextID", contextId);
                BlackboardUtils.SetOrCreateValue(cellBB, "vipDealInfoID", vipDealInfoId);
                BlackboardUtils.SetOrCreateValue(cellBB, "isMax", isMax);
            }
        }

        private string SendAE() // return context id
        {
            ShopType shopType = ShopType.VIP_DEAL;
            var vipDealInfo = VipDealV2.Utils.GetInfo();
            int shopId = BlackboardUtils.FindValue<int>(vipDealInfo, "shopId");
            string contextId = BiEventUtils.GenerateContextID();
            string type = "vipDeal";
            AEUtils.SendAEStoreOpened(shopType, shopId, contextId, type);

            return contextId;
        }

        private void Close()
        {
            anim.SetTrigger("Close");
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void Activate()
        {
            anim.SetBool("Active", true);
        }

        private void Deactivate()
        {
            anim.SetBool("Active", false);
        }
    }
}
