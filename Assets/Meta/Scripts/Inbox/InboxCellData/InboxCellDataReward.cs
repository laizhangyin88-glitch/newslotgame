using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;
using ParadoxNotion;
using System.Collections;
using System.Collections.Generic;

using static BagelCode.TextDecoUtils;

namespace BagelCode
{
    public class InboxCellDataReward : InboxCellData
    {
        public RewardType rewardType;
        public RewardCauseType rewardCauseType;

        public EventInfo eventInfo = null;

        private GameObject resultPopupObj = null;

        private const string ON_SUCCESS_PURCHASE = "OnSuccessPurchase";
        private const string ON_CANCEL_PURCHASE = "OnCancelPurchase";
        private const string ON_SUCCESS_INVITE = "OnSuccessInvite";
        private const string ON_CANCEL_INVITE = "OnCancelInvite";

        public InboxCellDataReward(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            switch (rewardType)
            {
                case RewardType.INVITE_INSTALL_WITH_TIER:
                    buttonType = InboxCellController.InboxButtonType.REDEEM;
                    break;
            }
        }

        public override void InboxItemCheckType()
        {
            if (rewardType == RewardType.PURCHASE_COUPON)
            {
                Blackboard productInfo = BlackboardUtils.FindValue<Blackboard>(inboxInfo, "reward/product");
                double price = productInfo.GetValue<double>("price");
                double originalPrice = productInfo.GetValue<double>("originalPrice");

                long eventNumerator = BlackboardUtils.FindValue<long>(inboxInfo, "reward/product/eventMultiplierNumerator");
                if (eventInfo != null)
                {
                    var eventMult = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                    if (eventInfo.id != 0)
                        eventNumerator += eventMult - NumberUtils.GetGlobalDenominator();
                }

                bool isEqualPrice = OperationUtils.Compare(price, originalPrice, CompareMethod.EqualTo, 0.05f);
                if (isEqualPrice)
                {
                    double multiplier = NumberUtils.GetMultiplierFromNumerator(eventNumerator);
                    MetaContextElementUtils.SimpleSetTextGlobal(ownerRootElement,
                        "Coupon Area/Text", "POPUP_VOUCHER_MULTIPLIER", FULL, multiplier);
                }
                else
                {
                    double ratio = (price / originalPrice - 1.0) * -1.0;
                    MetaContextElementUtils.SimpleSetTextGlobal(ownerRootElement,
                        "Coupon Area/Text", "TEXT_PERCENTAGE", FULL, ratio);
                }
            }
            else if (rewardType == RewardType.TIER_UPGRADE)
            {
                int tier = BlackboardUtils.FindValue<int>(inboxInfo, "reward/targetTier");
                int meTier = BlackboardQueryUtils.GetMyTier();

                int target = Mathf.Max(tier, meTier);
                int tierGroup = MetaSystem.GetTierGroup(target);

                ContextElement tierElement = ContextUtils.FindElement(ownerRootElement, "Image Tier Area/Image Tier", FULL);
                Blackboard tierBB = tierElement.GetComponent<Blackboard>();
                tierBB.AddVariable("_tierGroup", tierGroup);
            }
        }

        public override IEnumerator InboxItemCheckExtraCoroutine()
        {
            switch (rewardType)
            {
                case RewardType.PURCHASE_COUPON:
                    {
                        string asset = "Popup Voucher Scene";
                        GameObject voucherPopupObj = null;

                        yield return cell.StartCoroutine(MetaPopupUtils.SimpleOpenPopupCoroutine(cell, asset,
                            (sceneLoadOperation) => voucherPopupObj = sceneLoadOperation.GetScene()));

                        Blackboard voucherPopupBB = voucherPopupObj.GetComponent<Blackboard>();
                        Blackboard rewardInfo = inboxInfo.GetValue<Blackboard>("reward");
                        voucherPopupBB.AddVariable("caller", cell.gameObject);
                        voucherPopupBB.AddVariable("_rewardInfo", rewardInfo);
                        voucherPopupBB.AddVariable("_inboxInfo", inboxInfo);

                        MetaPopupUtils.OpenPopup(voucherPopupObj);

                        var successTrigger = new EventTrigger(cell, ON_SUCCESS_PURCHASE);
                        var cancelTrigger = new EventTrigger(cell, ON_CANCEL_PURCHASE);
                        yield return new WaitUntilTrigger(successTrigger, cancelTrigger);

                        if (successTrigger.IsTrigger)
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                        else if (cancelTrigger.IsTrigger)
                        {
                            InboxController.IsAcceptable = true;
                        }
                    }
                    break;

                case RewardType.GAME_SPIN:
                case RewardType.GAME_PLAY:
                case RewardType.GAME_DEAL:
                    gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
                    break;

                case RewardType.INVITE_INSTALL_WITH_TIER:
                    {
                        if (Common.CheckPlatform(TargetPlatform.Android | TargetPlatform.IOS))
                        {
                            string asset = "Popup VIP Invitation Scene";
                            GameObject vipInvitePopupObj = null;

                            yield return cell.StartCoroutine(MetaPopupUtils.SimpleOpenPopupCoroutine(cell, asset,
                                (sceneLoadOperation) => vipInvitePopupObj = sceneLoadOperation.GetScene()));

                            Blackboard vipInvitePopupBB = vipInvitePopupObj.GetComponent<Blackboard>();
                            Blackboard rewardInfo = inboxInfo.GetValue<Blackboard>("reward");
                            vipInvitePopupBB.AddVariable("caller", cell.gameObject);
                            vipInvitePopupBB.AddVariable("_rewardInfo", rewardInfo);
                            vipInvitePopupBB.AddVariable("_inboxInfo", inboxInfo);

                            MetaPopupUtils.OpenPopup(vipInvitePopupObj);

                            var successTrigger = new EventTrigger(cell, ON_SUCCESS_INVITE);
                            var cancelTrigger = new EventTrigger(cell, ON_CANCEL_INVITE);
                            yield return new WaitUntilTrigger(successTrigger, cancelTrigger);

                            if (successTrigger.IsTrigger)
                            {
                                yield return new WaitForSeconds(0.1f);
                            }
                            else if (cancelTrigger.IsTrigger)
                            {
                                InboxController.IsAcceptable = true;
                            }
                        }
                        else
                        {
                            // PLATFORM EXCEPTION
                            string bundle = ApplicationSettings.MakeApplicationBundleName("system");
                            string asset = "Popup Common Ok Scene";
                            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                            GameObject popupObj = null;
                            yield return cell.StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

                            MetaObjectUtils.SetCalleeCaller(popupObj, cell.gameObject);

                            string message = StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_INVITATION_NOT_SUPPORT_DEVICE");

                            MetaPopupUtils.SetCommonPopupData(popupObj, cell.transform, message, "", "", "OK",
                                "", "", "", "", true, true, true, false, true);

                            MetaPopupUtils.OpenPopup(popupObj);
                            InboxController.IsAcceptable = true;
                        }
                    }
                    break;
            }
        }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            var cellBB = cell.GetComponent<Blackboard>();
            var deliveryInfo = BlackboardUtils.FindVariable<Blackboard>(cellBB, "response/dailyDeliveryInfo");

            if (deliveryInfo != null && deliveryInfo.value != null
                && MetaCommonRewardUtils.CheckShowDailyDeliveryPopup(deliveryInfo.value))
            {
                // Daily Delivery
                yield return cell.StartCoroutine(MetaCommonRewardUtils.OpenDailyDeliveryPopupCoroutine(cell.gameObject, deliveryInfo.value));
            }

            cell.Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_MAKE_RESULT_POPUP, OnMakeResultPopup);

            // applyEarnValues = false
            // Because the InboxUtils.UpdateEarnValues has already processed it.
            yield return cell.StartCoroutine(MetaCommonRewardUtils.RewardResultProcessCoroutine(cell.gameObject, rewardType, false));

            cell.UnRegister(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_MAKE_RESULT_POPUP);
        }

        private void OnMakeResultPopup(EventData eventData) // eventData is ResultPopup
        {
            switch (rewardType)
            {
                case RewardType.SCRATCHER:
                    OnMakeScratcher(eventData);
                    return;
            }
        }

        private void OnMakeScratcher(EventData eventData)
        {
            if (eventData.value == null || !InboxUtils.IsGroup(inboxInfo))
                return;

            resultPopupObj = (GameObject)eventData.value;

            var inboxBB = cell.ownerInboxController.GetComponent<Blackboard>();
            Blackboard resultPopupBB = resultPopupObj.GetComponent<Blackboard>();
            var scratcherRewardResult = resultPopupBB.GetValue<Blackboard>("_scratcherRewardResult");

            // Add reward to lists
            BlackboardQueryUtils.SetScratcherPrizeInfoFromRewardResult(inboxBB, scratcherRewardResult);

            // Set Result Popup Prize Info
            BlackboardQueryUtils.SetScratcherPrizeInfo(resultPopupBB, inboxBB, true); // set all prize info

            if (isFirstReward) // First
            {
                BlackboardUtils.SetOrCreateValue(resultPopupBB, "_remainingCount", cell.InboxInfoGroupCount - 1);
            }
            else // Continuous
            {
                BlackboardUtils.SetOrCreateValue(resultPopupBB, "_remainingCount", cell.InboxInfoGroupCount - 1);
                BlackboardUtils.SetOrCreateValue(resultPopupBB, "_isFirst", false);

                bool isAutoAcceptNext = AcceptEventData != null && (bool)AcceptEventData;
                BlackboardUtils.SetOrCreateValue(resultPopupBB, "_isAuto", isAutoAcceptNext);
            }
        }

        public override void OnRemoveInboxItem()
        {
            if (!isAcceptNext) // last reward
            {
                var inboxBB = cell.ownerInboxController.GetComponent<Blackboard>();
                BlackboardQueryUtils.ClearScratcherPrizeInfo(inboxBB); // clear prize info
            }

            text = GetText();
            cell.UpdateText(text);
        }

        protected override string GetInternalText()
        {
            rewardType = BlackboardUtils.FindValue<RewardType>(inboxInfo, "reward/type");
            rewardCauseType = inboxInfo.GetValue<RewardCauseType>("rewardCauseType");

            string rewardText = "";
            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        disappearType = 1;
                        long coin = BlackboardUtils.FindValue<long>(inboxInfo, "reward/credit");
                        bool isApplyMultiplier = BlackboardUtils.FindValue<bool>(inboxInfo, "reward/applyTierMultiplier");
                        bool isApplyLevelMultiplier = BlackboardUtils.FindValue<bool>(inboxInfo, "reward/applyLevelMultiplier");
                        switch(rewardCauseType)
                        {
                            case RewardCauseType.PURCHASE_RECOVER:
                                {
                                    rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_PURCHASE_COIN", title, message, coin);
                                }
                                break;
                            case RewardCauseType.VIP_LOUNGE_BADGE_EXPIRED:
                            case RewardCauseType.VIP_LOUNGE_EVENT_ENDED:
                            case RewardCauseType.LOUNGE_JACKPOT_COMPENSATION:
                            case RewardCauseType.LEVEL_UP_DASH:
                            case RewardCauseType.SEASON_PASS:
                                {
                                    rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                                    ConvertStringFormat(ref rewardText, "{credit}", ConvertCoinStyleText(coin));
                                }
                                break;
                            default:
                                {
                                    if (isApplyMultiplier || isApplyLevelMultiplier)
                                    {
                                        if (isApplyLevelMultiplier)
                                            coin = LevelUtils.GetLevelMultiplierNumeratorValue(coin, "coin");

                                        if (isApplyMultiplier)
                                        {
                                            var tier = TierUtils.GetMeTier();
                                            var tierMultiplier = TierUtils.GetTierMultiplier(tier);
                                            rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_COIN_WITH_TIER_MULTIPLIER", title, message, coin, tierMultiplier, tier);
                                        }
                                        else
                                            rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_COIN", title, message, coin);
                                    }
                                    else
                                    {
                                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_COIN", title, message, coin);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case RewardType.RP:
                    {
                        disappearType = 2;
                        long rp = BlackboardUtils.FindValue<long>(inboxInfo, "reward/rp");
                        if (rewardCauseType == RewardCauseType.PURCHASE_RECOVER)
                        {
                            rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_PURCHASE_RP", title, message, rp);
                        }
                        else
                        {
                            rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_RP", title, message, rp);
                        }
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        disappearType = 1;
                        var tier = TierUtils.GetMeTier();
                        long baseCoins = BlackboardUtils.FindValue<long>(inboxInfo, "reward/baseCreditPerDay");
                        long totalCoins = TierUtils.GetTierFractionCoin(baseCoins, tier);
                        long baseGem = BlackboardUtils.FindValue<long>(inboxInfo, "reward/baseGemPerDay");
                        long totalGem = TierUtils.GetTierFractionCoin(baseGem, tier);
                        int totalDayCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/totalDayCount");

                        totalCoins = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(totalCoins, BlackboardUtils.FindValue<Blackboard>(inboxInfo, "reward"), "dailyBoost");
                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{credit}", ConvertSimpleCoinStyleText(totalCoins));
                        ConvertStringFormat(ref rewardText, "{gem}", ConvertSimpleGemStyleText(totalGem));
                        ConvertStringFormat(ref rewardText, "{and}", ConvertAndSymbol(totalCoins, totalGem));
                        ConvertStringFormat(ref rewardText, "{day}", totalDayCount.ToString());
                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    {
                        int rewardTier = BlackboardUtils.FindValue<int>(inboxInfo, "reward/targetTier");
                        int meTier = BlackboardQueryUtils.GetMyTier();

                        int targetTier = Mathf.Max(meTier, rewardTier);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{tier}", ConvertTierStyleText(targetTier));
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        int spinCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/spinCount");

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{spin_count}", spinCount.ToString());
                        ConvertStringFormat(ref rewardText, "{play_count}", spinCount.ToString());
                    }
                    break;
                case RewardType.MEGA_WHEEL_SPIN:
                    {
                        int spinCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/spinCount");

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{spin_count}", spinCount.ToString());
                        ConvertStringFormat(ref rewardText, "{play_count}", spinCount.ToString());
                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
                        int spinCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/spinCount");
                        long bet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/bet");
                        long totalBet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/totalBet");
                        // memo : calc on server
                        //if (BlackboardUtils.FindValue<bool>(inboxInfo, "reward/applyLevelMultiplier"))
                        //{
                        //    bet = LevelUtils.GetLevelMultiplierNumeratorValue(bet);
                        //    totalBet = LevelUtils.GetLevelMultiplierNumeratorValue(totalBet);
                        //}

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
                        string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{bet}", ConvertCoinStyleText(bet));
                        ConvertStringFormat(ref rewardText, "{total_bet}", ConvertCoinStyleText(totalBet));
                        ConvertStringFormat(ref rewardText, "{spin_count}", spinCount.ToString());
                        ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
                        int dealCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/count");
                        long betPerHand = BlackboardUtils.FindValue<long>(inboxInfo, "reward/betPerHand");
                        int handCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/handCount");

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
                        string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{bet_per_hand}", ConvertCoinStyleText(betPerHand));
                        ConvertStringFormat(ref rewardText, "{deal_count}", dealCount.ToString());
                        ConvertStringFormat(ref rewardText, "{hand_count}", handCount.ToString());
                        ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);
                    }
                    break;
                case RewardType.GAME_PLAY:
                    {
                        gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
                        int playCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/count");
                        long betPerTicket = BlackboardUtils.FindValue<long>(inboxInfo, "reward/betPerTicket");
                        long ticketCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/ticketCount");
                        long totalBet = betPerTicket * ticketCount;

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
                        string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{total_bet}", ConvertCoinStyleText(totalBet));
                        ConvertStringFormat(ref rewardText, "{play_count}", playCount.ToString());
                        ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);
                    }
                    break;
                case RewardType.PURCHASE_COUPON:
                    {
                        int tier = TierUtils.GetMeTier();
                        Blackboard productInfo = BlackboardUtils.FindValue<Blackboard>(inboxInfo, "reward/product");
                        Blackboard item = BlackboardQueryUtils.GetItemFromProduct(productInfo, ItemType.CREDIT);
                        long baseCoins = item.GetValue<long>("baseCredit");
                        long totalCoins = TierUtils.GetTierFractionCoin(baseCoins, tier);
                        totalCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "coin");
                        long eventMultiplierNumerator = BlackboardUtils.FindValue<long>(inboxInfo, "reward/product/eventMultiplierNumerator");

                        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.VOUCHER_SHOP_EVENT_MULTIPLY);
                        if (eventInfo != null)
                        {
                            var eventMult = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                            if (eventInfo.id != 0)
                                eventMultiplierNumerator += eventMult - NumberUtils.GetGlobalDenominator();
                        }

                        //totalCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "coin");
                        totalCoins = NumberUtils.GetMultiplierNumeratorValue(totalCoins, eventMultiplierNumerator);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{price}", string.Format("${0}", BlackboardUtils.FindValue<double>(inboxInfo, "reward/product/price").ToString()));
                        ConvertStringFormat(ref rewardText, "{credit}", StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_PURCHASE_COUPON_COIN", totalCoins));
                        ConvertStringFormat(ref rewardText, "{rp}", StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_PURCHASE_COUPON_RP", item.GetValue<long>("rp")));
                        break;
                    }
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                    break;
                case RewardType.RANDOM:
                    rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        bonusTag = BlackboardUtils.FindValue<BonusTag>(inboxInfo, "reward/tag");
                        gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
                        long baseBet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/baseBet");
                        long extraBet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/extraBet");

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
                        string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{bet}", ConvertCoinStyleText(baseBet + extraBet));
                        ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);
                    }
                    break;
                case RewardType.SPIN_DEAL:
                    {
                        gameId = BlackboardUtils.FindValue<int>(inboxInfo, "reward/gameId");
                        long totalBet = BlackboardUtils.FindValue<long>(inboxInfo, "reward/totalBet");
                        int spinCount = BlackboardUtils.FindValue<int>(inboxInfo, "reward/spinCount");

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameId.ToString());
                        string gameTitle = StringTableUtils.GetString(GLOBAL, gameTitleFormat);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{spin_count}", spinCount.ToString("N0"));
                        ConvertStringFormat(ref rewardText, "{total_bet}", ConvertCoinStyleText(totalBet));
                        ConvertStringFormat(ref rewardText, "{game_name}", gameTitle);
                    }
                    break;
                case RewardType.SCRATCHER:
                case RewardType.SCRATCHER_FOR_INBOX:
                    {
                        ScratcherName scratcherName = BlackboardUtils.FindValue<ScratcherName>(inboxInfo, "reward/scratcherName");
                        int groupCount = cell.InboxInfoGroupCount;
                        bool isGroup = InboxUtils.IsGroup(inboxInfo);

                        string groupCountText = "{group_count}";
                        if (isGroup)
                            groupCountText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_SCRATCHER_GROUP", groupCount);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{meta_game}", BlackboardQueryUtils.GetScratcherName(scratcherName));
                        ConvertStringFormat(ref rewardText, "{group_count}", groupCountText);

                        break;
                    }
                case RewardType.GEM:
                    {
                        long gem = BlackboardUtils.FindValue<long>(inboxInfo, "reward/gem");

                        if(rewardCauseType == RewardCauseType.LEVEL_UP_DASH || rewardCauseType == RewardCauseType.SEASON_PASS)
                        {
                            rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                            ConvertStringFormat(ref rewardText, "{gem}", ConvertSimpleGemStyleText(gem));
                        }
                        else
                        {
                            rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_GEM", title, message, gem);
                        }
                        break;
                    }
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    {
                        int finder = BlackboardUtils.FindValue<int>(inboxInfo, "reward/finder");
                        string finderCountText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_FINDER_COUNT", finder);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{finder_count}", finderCountText);
                        break;
                    }
                case RewardType.INVITE_INSTALL_WITH_TIER:
                    {
                        int tier = BlackboardUtils.FindValue<int>(inboxInfo, "reward/targetTier");
                        long credit = BlackboardUtils.FindValue<long>(inboxInfo, "reward/snsRewardCredit");

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{credit}", ConvertSimpleCoinStyleText(credit));
                        ConvertStringFormat(ref rewardText, "{tier}", ConvertTierStyleText(tier));
                        break;
                    }
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    {
                        int openDays = BlackboardUtils.FindValue<int>(inboxInfo, "reward/openDays");
                        string openDaysText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_VIP_LOUNGE_OPEN_TICKET_DAYS", openDays);

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{open_days}", openDaysText);
                        break;
                    }
                case RewardType.DEPOT:
                    {
                        var count = BlackboardUtils.FindValue<int>(inboxInfo, "reward/count");
                        var type = BlackboardUtils.FindValue<DepotType>(inboxInfo, "reward/depotType");

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{build_dream}", StringTableUtils.GetString(GLOBAL, "VEGAS_DREAMS_META_NAME"));
                        ConvertStringFormat(ref rewardText, "{depot_type}", StringTableUtils.GetString(GLOBAL, $"VEGAS_DREAMS_DEPOT_{type}"));
                        ConvertStringFormat(ref rewardText, "{depot_count}", count.ToString());
                        break;
                    }
                case RewardType.WILD_PUZZLE:
                    {
                        var count = BlackboardUtils.FindValue<int>(inboxInfo, "reward/count");
                        
                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        ConvertStringFormat(ref rewardText, "{wild_puzzle_count}", count.ToString());
                        break;
                    }
                case RewardType.BUCKS_GIFT:
                    {
                        //long bucks = BlackboardUtils.FindValue<long>(inboxInfo, "reward/paidBucks");
                        //bucks += BlackboardUtils.FindValue<long>(inboxInfo, "reward/bonusBucks");
                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        //ConvertStringFormat(ref rewardText, "{total_vegas_bucks}", bucks.ToString());
                        break;
                    }
                //case RewardType.LEVEL_UP_DASH_ANY_PURCHASE_BOOSTER:
                //    {
                //        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                //        break;
                //    }
                default:
                    {
                        Debug.LogError("Reward type is undefined.");

                        rewardText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
                        break;
                    }
            }

            var userName = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/name");
            if (userName != null && !string.IsNullOrEmpty(userName.value))
                ConvertStringFormat(ref rewardText, "{name}", userName.value);

            return rewardText;
        }

        protected override string GetButtonText()
        {
            var rewardType = BlackboardUtils.FindValue<RewardType>(inboxInfo, "reward/type");
            var rewardCauseType = inboxInfo.GetValue<RewardCauseType>("rewardCauseType");

            if (rewardCauseType == RewardCauseType.PURCHASE_RECOVER)
                return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_CLAIM");

            if (GetViewAd() == true)
                return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_VIDEO");

            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                case RewardType.RP:
                case RewardType.TIER_UPGRADE:
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                case RewardType.MEGA_WHEEL_SPIN:
                case RewardType.RANDOM:
                case RewardType.GEM:
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                case RewardType.BUCKS_GIFT:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_ACCEPT");
                case RewardType.DAILY_BOOST:
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_BEGIN");
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_PLAY");
                case RewardType.PURCHASE_COUPON:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_REDEEM");
                case RewardType.SCRATCHER:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_SCRATCH");
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_COLLECT");
                case RewardType.INVITE_INSTALL_WITH_TIER:
                    long credit = BlackboardUtils.FindValue<long>(inboxInfo, "reward/snsRewardCredit");
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_INVITE", credit);
                default:
                    return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_OK");
            }
        }

        protected override IconType GetIconType()
        {
            var rewardType = BlackboardUtils.FindValue<RewardType>(inboxInfo, "reward/type");
            var rewardCauseType = inboxInfo.GetValue<RewardCauseType>("rewardCauseType");

            string rewardImageURL = inboxInfo.GetVariable<string>("imageUrl")?.value ?? string.Empty;
            if (!string.IsNullOrEmpty(rewardImageURL))
                iconUrl = rewardImageURL;

            if (rewardType == RewardType.SCRATCHER)
                iconUrl = BlackboardUtils.FindValue<string>(inboxInfo, "reward/inboxImageUrl");

            if (!string.IsNullOrEmpty(iconUrl))
                return IconType.WEB_IMAGE;

            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        if (rewardCauseType == RewardCauseType.PURCHASE_RECOVER)
                            return IconType.PURCHASE_RECOVER_CREDIT;
                        else if (rewardCauseType == RewardCauseType.VIP_LOUNGE_BADGE_EXPIRED || rewardCauseType == RewardCauseType.VIP_LOUNGE_EVENT_ENDED)
                            return IconType.VIP_EPIC_LOUNGE;
                        else if (rewardCauseType == RewardCauseType.LOUNGE_JACKPOT_COMPENSATION)
                            return IconType.VIP_LOUNGE_JACKPOT;
                        else
                            return IconType.CREDIT;
                    }
                case RewardType.RP:
                    {
                        if (rewardCauseType == RewardCauseType.PURCHASE_RECOVER)
                            return IconType.PURCHASE_RECOVER_RP;
                        else
                            return IconType.RP;
                    }
                case RewardType.DAILY_BOOST:
                    return IconType.DAILY_BOOST;
                case RewardType.TIER_UPGRADE:
                    return IconType.TIER_UPGRADE;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    return IconType.DAILY_BONUS_WHEEL_SPIN;
                case RewardType.MEGA_WHEEL_SPIN:
                    return IconType.MEGA_WHEEL_SPIN;
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    return IconType.GAME_THUMBNAIL;
                case RewardType.PURCHASE_COUPON:
                    return IconType.PURCHASE_COUPON;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    return IconType.EXP_MULTIPLY;
                case RewardType.RANDOM:
                    return IconType.RANDOM;
                case RewardType.SCRATCHER:
                    return IconType.WEB_IMAGE;
                case RewardType.GEM:
                    return IconType.GEM;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    return IconType.FINDER;
                case RewardType.INVITE_INSTALL_WITH_TIER:
                    return IconType.VIP_INVITE;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    return IconType.VIP_LOUNGE_OPEN_TICKET;
                case RewardType.DEPOT:
                    return IconType.DEPOT;
                case RewardType.WILD_PUZZLE:
                    return IconType.WILD_PUZZLE;
                case RewardType.BUCKS_GIFT:
                    return IconType.BUCKS_GIFT;
                //case RewardType.LEVEL_UP_DASH_ANY_PURCHASE_BOOSTER:
                //    return IconType.LEVEL_UP_EXP_BOOST;
                default:
                    Debug.Log(iconType + " is undefiend IconType.");
                    return IconType.EMPTY;
            }
        }

        public override IEnumerator InboxItemCheckViewAdCoroutine()
        {
            if (GetViewAd() == false)
                yield break;
            if (ApplicationSettings.LogTest())
                Debug.Log("InboxItemCheckViewAdCoroutine : Is Ad");

            string placementKey = BlackboardUtils.FindVariable<string>("/videoAdsPlacementNames/inbox")?.value ?? "";
            var inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(null, "/values/misc/INHOUSE_ADS_ENABLED");
            if (inhouseAdsEnabled != null && inhouseAdsEnabled.value == true)
            {
                // loading scene create -> settings(+BI) -> wait trigger -> close popup scene
                // Open Loading Popup
                GameObject loadingObj = MetaPopupUtils.OpenLoadingPopup();
                MetaSystem.BackupUserSyncInfo();
                System.GC.Collect();
                BIClientVideoAd("click", "inhouse", "", GetBIContextId());

                if (IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_INBOX, cell.gameObject, GetBIContextId()))
                {
                    // OnIAMCallback
                    var iamCallbacklTrigger = new EventTrigger(cell, "OnIAMCallback");
                    yield return new WaitUntilTrigger(iamCallbacklTrigger);
                    BIClientVideoAd("complete", "inhouse", placementKey, GetBIContextId());
                }
                else
                    BIClientVideoAd("complete", "no_video", placementKey, GetBIContextId());

                MetaPopupUtils.ClosePopup(loadingObj);
            }
            else
            {
                var videoAdsEnabled = BlackboardUtils.FindVariable<bool>(null, "/values/misc/VIDEO_ADS_ENABLED");
                if (videoAdsEnabled != null && videoAdsEnabled.value == true && VideoAdsController.Instance.IsVideoAdsAvailable(placementKey))
                {
                    yield return new WaitForSeconds(0.2f);
                    System.GC.Collect();
                    BIClientVideoAd("click", "ironsource", placementKey, GetBIContextId());

                    VideoAdsController.Instance.ShowRewardedVideo(placementKey,
                    () =>
                    {
                        Debug.Log("OnVideoAdsRewarded");
                        cell.gameObject.GetComponent<GraphOwner>().SendEvent("OnVideoAdsRewarded");
                    });
#if !UNITY_EDITOR
                    var videoCallbacklTrigger = new EventTrigger(cell, "OnVideoAdsRewarded");
                    yield return new WaitUntilTrigger(videoCallbacklTrigger);
#endif
                    BIClientVideoAd("complete", "ironsource", placementKey, GetBIContextId());
                }
                else
                    BIClientVideoAd("complete", "no_video", placementKey, GetBIContextId());
            }

            yield break;
        }

        private bool GetViewAd()
        {
            return BlackboardUtils.FindVariable<bool>(inboxInfo, "reward/viewAd")?.value ?? false;
        }

        private string GetBIContextId()
        {
            var cellBB = cell.gameObject.GetComponent<Blackboard>();
            var contextId = BlackboardUtils.GetOrCreateVariable<string>(cellBB, "_biContextID");
            if (contextId == null || string.IsNullOrEmpty(contextId.value))
                contextId.value = BiEventUtils.GenerateContextID();
            return contextId.value;
        }

        private void BIClientVideoAd(string action, string typeOfAd, string placementKey, string biContextId)
        {
            if (typeOfAd != "inhouse" && typeOfAd != "no_video")
            {
#if UNITY_WSA
                typeOfAd = "vungle";
#else
                typeOfAd = "ironsource";
#endif
            }

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["action"] = action;
            BIAppendRewardEventData(customData);
            customData["placement"] = placementKey;
            customData["type_of_ad"] = typeOfAd;
            customData["context_id"] = biContextId;

            Analytics.CustomEvent("client_video_ad", customData);
        }

        private void BIAppendRewardEventData(Dictionary<string, object> eventData)
        {
            var rewardType = BlackboardUtils.FindValue<RewardType>(inboxInfo, "reward/type");
            var tier = TierUtils.GetMeTier();
            switch (rewardType)
            {
                case RewardType.CREDIT:
                    long credit = BlackboardUtils.FindValue<long>(inboxInfo, "reward/credit");
                    if (BlackboardUtils.FindValue<bool>(inboxInfo, "reward/applyTierMultiplier")) credit = TierUtils.GetTierFractionCoin(credit, tier);
                    if (BlackboardUtils.FindValue<bool>(inboxInfo, "reward/applyLevelMultiplier")) credit = LevelUtils.GetLevelMultiplierNumeratorValue(credit, "coin");
                    eventData["type_of_reward"] = "coin";
                    eventData["amount_of_reward"] = credit;
                    break;
                case RewardType.RP:
                    eventData["type_of_reward"] = "rp";
                    eventData["amount_of_reward"] = BlackboardUtils.FindValue<long>(inboxInfo, "reward/rp");
                    break;
                case RewardType.DAILY_BOOST:
                    long baseCredit = TierUtils.GetTierFractionCoin(BlackboardUtils.FindValue<long>(inboxInfo, "reward/baseCreditPerDay"), tier);
                    baseCredit = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(baseCredit, BlackboardUtils.FindValue<Blackboard>(inboxInfo, "reward"), "dailyBoost");
                    eventData["type_of_reward"] = "daily_boost";
                    eventData["amount_of_reward"] = baseCredit;
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    eventData["type_of_reward"] = "daily_spin";
                    eventData["amount_of_reward"] = (long)BlackboardUtils.FindValue<int>(inboxInfo, "reward/spinCount");
                    break;
                case RewardType.GEM:
                    eventData["type_of_reward"] = "gem";
                    eventData["amount_of_reward"] = BlackboardUtils.FindValue<long>(inboxInfo, "reward/gem");
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    eventData["type_of_reward"] = "finder";
                    eventData["amount_of_reward"] = BlackboardUtils.FindValue<long>(inboxInfo, "reward/finder");
                    break;
                case RewardType.TIER_UPGRADE:
                case RewardType.GAME_SPIN:
                default:
                    eventData["type_of_reward"] = "";
                    eventData["amount_of_reward"] = 0L;
                    break;
            }
        }
    }
}
