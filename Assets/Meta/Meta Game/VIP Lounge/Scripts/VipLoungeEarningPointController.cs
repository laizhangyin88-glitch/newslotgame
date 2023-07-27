using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections.Generic;

namespace BagelCode.VipLounge
{
    public class VipLoungeEarningPointController : EventMonoBehaviour
    {
        private Blackboard bb;
        private ContextElement root;
        private Animator anim;

        private bool isInit = false;
        private string contextId;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private ContextElement playButtonElement;
        private ContextElement othersButtonElement;

        public void InitProperty()
        {
            if (isInit) return;

            bb = GetComponent<Blackboard>();
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            contextId = BiEventUtils.GenerateContextID();

            // Play Anim
            anim.SetBool("Active", true);
            anim.SetBool("IsLoading", false);

            // Init Elements
            InitContents();
            InitPlayCellContents();

            BIClientVipLoungePopup();
            //SendEarningPointPopupAnalyticsEvent(false);

            isInit = true;
        }

        private void InitContents()
        {
            // Left Right
            playButtonElement = ContextUtils.FindElement(root, "Tab/Tab Play", FULL);
            MetaContextElementUtils.SetClickable(playButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_PLAY));

            othersButtonElement = ContextUtils.FindElement(root, "Tab/Tab Others", FULL);
            MetaContextElementUtils.SetClickable(othersButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_OTHERS));


            var coinShopButtonElement = ContextUtils.FindElement(root, "Others/Others Item 01", FULL);
            MetaContextElementUtils.SetClickable(coinShopButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_COIN_SHOP));

            var gemShopButtonElement = ContextUtils.FindElement(root, "Others/Others Item 02", FULL);
            MetaContextElementUtils.SetClickable(gemShopButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_GEM_SHOP));

            var dailySpinButtonElement = ContextUtils.FindElement(root, "Others/Others Item 08", FULL);
            MetaContextElementUtils.SetClickable(dailySpinButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLICK_DAILY_SPIN));

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VipLounge.Events.ON_CLOSE));
        }

        private void InitPlayCellContents()
        {
            Blackboard mainBB = BlackboardUtils.FindValue<Blackboard>(bb, "mainBB");

            var spinsLoungePointElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 01/Sub Title Area/Text Count", FULL);
            MetaContextElementUtils.SetTextGlobal(spinsLoungePointElement, "TEXT_COMMA_NUMBER", VipLounge.Utils.QualifiedSpinsValue);
            var spinsQualifiedBetElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 01/Text Badge Context", FULL);
            MetaContextElementUtils.SetTextGlobal(spinsQualifiedBetElement, "VIP_LOUNGE_EARNING_POINT_PLAY_CONTENT_CELL_QUALIFIED_SPINS_BADGE", VipLounge.Utils.QualifiedBetAmount(mainBB));

            var maxSpin = VipLounge.Utils.CumulativeSpinMax;
            var spin = VipLounge.Utils.CumulatedSpinCount(mainBB);
            float ratio = (float)spin / maxSpin;

            var spinsTitleElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 01/Text Item Condition", FULL);
            MetaContextElementUtils.SetTextGlobal(spinsTitleElement, "VIP_LOUNGE_EARNING_POINT_PLAY_CONTENT_CELL_QUALIFIED_SPINS_TITLE", maxSpin);
            var spinsgaugeElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 01/Gauge Area/VIP Lounge Item Gage", FULL);
            MetaContextElementUtils.SetSliderValue(spinsgaugeElement, ratio);
            var spinsgaugeTextElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 01/Gauge Area/VIP Lounge Item Gage/Text Gage Info", FULL);
            MetaContextElementUtils.SetTextGlobal(spinsgaugeTextElement, "A_PER_B", spin, maxSpin);

            
            var shopBonusLoungePointElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 02/Sub Title Area/Text Count", FULL);
            MetaContextElementUtils.SetTextGlobal(shopBonusLoungePointElement, "TEXT_COMMA_NUMBER", VipLounge.Utils.CollectShopBonusValue);
            

            var levelLoungePointElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 03/Sub Title Area/Text Count", FULL);
            MetaContextElementUtils.SetTextGlobal(levelLoungePointElement, "TEXT_COMMA_NUMBER", VipLounge.Utils.ReachLevelValue);
            

            var timeBonusLoungePointElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 04/Sub Title Area/Text Count", FULL);
            MetaContextElementUtils.SetTextGlobal(timeBonusLoungePointElement, "TEXT_COMMA_NUMBER", VipLounge.Utils.CollectTimeBonusValue);
            
            
            var luckySpinLoungePointElement = ContextUtils.FindElement(root, "Play/Floating Area/Cell Area/VIP Lounge Earning Point Play Cell 05/Sub Title Area/Text Count", FULL);
            MetaContextElementUtils.SetTextGlobal(luckySpinLoungePointElement, "TEXT_COMMA_NUMBER", VipLounge.Utils.CollectLuckySpinValue);
        }

        public void UpdateToggleState(bool flag)
        {
            //SendEarningPointPopupAnalyticsEvent(flag);
            anim.SetBool("Others", flag);
        }

        public void OpenCoinShop()
        {
            
        }

        private void SendEarningPointPopupAnalyticsEvent(bool flag)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["vip_lounge_point"] = VipLounge.Utils.GetCurrentLoungePoint();
            customData["vip_badge"] = VipLounge.Utils.BadgeCount;
            customData["benefit_end_timestamp"] = VipLounge.Utils.BenefitEndTimestamp;
            customData["active_category"] = flag ? "OTHERS" : "PLAY";
            customData["context_id"] = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;

            Analytics.CustomEvent("client_vip_lounge_point_popup", customData);
        }

        private void BIClientVipLoungePopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["popup_name"] = "vip_lounge_earning_points";
            customData["context_id"] = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;

            Analytics.CustomEvent("client_vip_lounge_popup", customData);
        }
    }
}
