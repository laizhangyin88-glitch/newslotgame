using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        public static void ApplyPurchaseItems(List<ItemUseResult> itemUseResultList, UserSyncInfo userSyncInfo)
        {
            for (int i = 0; i < itemUseResultList.Count; ++i)
            {
                ApplyPurchaseItem(itemUseResultList[i], userSyncInfo);
            }
        }

        public static void ApplyPurchaseItem(ItemUseResult itemUseResult, UserSyncInfo userSyncInfo)
        {
            if (itemUseResult == null) return;
            if (itemUseResult.result == null) return;

            long earnCoins = 0;
            long earnRp = 0;
            long earnGems = 0;
            int earnFinder = 0;

            // result
            switch (itemUseResult.itemType)
            {
                case ItemType.CREDIT:
                    ItemResultCredit creditResult = itemUseResult.result as ItemResultCredit;
                    if (creditResult != null)
                    {
                        earnCoins += creditResult.earnCredit;
                        earnRp += creditResult.earnRp;
                    }
                    break;
                case ItemType.DAILY_BOOST:
                    ItemResultDailyBoost dailyBoostResult = itemUseResult.result as ItemResultDailyBoost;
                    if (dailyBoostResult != null)
                    {
                        earnCoins += dailyBoostResult.earnCredit;
                        earnRp += dailyBoostResult.earnRp;
                        earnGems += dailyBoostResult.earnGem;

                        BlackboardQueryUtils.ApplyDailyBoostInfo(dailyBoostResult.dailyBoost);
                        BlackboardQueryUtils.UpdateDailyBoostState();
                    }
                    break;
                case ItemType.CREDIT_POT_OF_GOLD:
                    ItemResultCreditPotOfGold creditPotofgoldresult = itemUseResult.result as ItemResultCreditPotOfGold;
                    if (creditPotofgoldresult != null)
                    {
                        earnCoins += creditPotofgoldresult.earnCredit;
                        earnRp += creditPotofgoldresult.earnRp;
                    }
                    break;
                case ItemType.CREDIT_MULTIPLIER_WHEEL:
                    ItemResultCreditMultiplierWheel coinWheelResult = itemUseResult.result as ItemResultCreditMultiplierWheel;
                    if (coinWheelResult != null)
                    {
                        earnCoins += coinWheelResult.earnCredit;
                        earnRp += coinWheelResult.earnRp;
                    }
                    break;
                case ItemType.DAILY_BONUS_WHEEL:
                    ItemResultDailyBonusWheel dailySpinResult = itemUseResult.result as ItemResultDailyBonusWheel;
                    if (dailySpinResult != null)
                    {
                        earnRp += dailySpinResult.earnRp;
                        BlackboardQueryUtils.SetDailySpinCount(dailySpinResult.totalSpinCount, MetaJackpotType.DAILY_BONUS);
                    }
                    break;
                case ItemType.PIGGY_BANK:
                    ItemResultPiggyBank potOfGoldResult = itemUseResult.result as ItemResultPiggyBank;
                    if (potOfGoldResult != null)
                    {
                        earnCoins += potOfGoldResult.earnCredit;
                        earnRp += potOfGoldResult.earnRp;
                        BlackboardQueryUtils.UpdatePotOfGoldProductList(potOfGoldResult.newPiggyProductList);

                        BlackboardQueryUtils.UpdatePiggyBankCoin(userSyncInfo.piggyCredit);
                    }
                    break;
                case ItemType.EARLY_ACCESS:
                    ItemResultEarlyAccess earlyAccessResult = itemUseResult.result as ItemResultEarlyAccess;
                    if (earlyAccessResult != null)
                    {
                        BlackboardQueryUtils.UpdateEarlyAccessGrade(earlyAccessResult.grade);
                    }
                    break;
                case ItemType.CREDIT_WHEEL:
                    ItemResultCreditWheel creditWheelResult = itemUseResult.result as ItemResultCreditWheel;
                    if (creditWheelResult != null)
                    {
                        earnCoins += creditWheelResult.earnCredit;
                        earnRp += creditWheelResult.earnRp;
                    }
                    break;
                case ItemType.TICKETED_BONUS_TICKET:
                    ItemResultTicketedBonusTicket ticketBonusResult = itemUseResult.result as ItemResultTicketedBonusTicket;
                    if (ticketBonusResult != null)
                    {
                        BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "bonusTiecketID", ticketBonusResult.ticketId);
                    }
                    break;
                case ItemType.DAILY_MEGA_WHEEL:
                    ItemResultDailyMegaWheel dailyMegaSpinResult = itemUseResult.result as ItemResultDailyMegaWheel;
                    if (dailyMegaSpinResult != null)
                    {
                        earnRp += dailyMegaSpinResult.earnRp;
                        BlackboardQueryUtils.SetDailySpinCount(dailyMegaSpinResult.totalSpinCount, MetaJackpotType.DAILY_MEGA_WHEEL);
                    }
                    break;
                case ItemType.POG_BOOSTER:
                    ItemResultPogBooster pogbResult = itemUseResult.result as ItemResultPogBooster;
                    if (pogbResult != null)
                    {
                        earnCoins += pogbResult.earnCredit;
                        earnRp += pogbResult.earnRp;
                    }
                    break;
                case ItemType.GEM:
                    ItemResultGem gemResult = itemUseResult.result as ItemResultGem;
                    if (gemResult != null)
                    {
                        earnGems += gemResult.earnGem;
                        earnRp += gemResult.earnRp;
                    }
                    break;
                case ItemType.GEM_BOOSTER:
                    ItemResultGemBooster gemBoosterResult = itemUseResult.result as ItemResultGemBooster;
                    if (gemBoosterResult != null)
                    {
                        earnGems += gemBoosterResult.earnGem;
                        earnRp += gemBoosterResult.earnRp;
                    }
                    break;
                case ItemType.EPIC_PASS:
                    {
                        ItemResultEpicPass epicPassResult = itemUseResult.result as ItemResultEpicPass;
                        if (epicPassResult != null)
                            EpicPassUtils.SetPaid(epicPassResult.unclaimedRewardCount);
                    }
                    break;
                case ItemType.EPIC_PASS_V2:
                    {
                        ItemResultEpicPass epicPassResult = itemUseResult.result as ItemResultEpicPass;
                        if (epicPassResult != null)
                            EpicPassUtilsV2.SetPaid(epicPassResult.unclaimedRewardCount);
                    }
                    break;
                case ItemType.SPIN_BOOST:
                    ItemResultSpinBoost spinBoostResult = itemUseResult.result as ItemResultSpinBoost;
                    if (spinBoostResult != null)
                    {
                        earnCoins += spinBoostResult.earnCredit;
                        earnRp += spinBoostResult.earnRp;
                    }
                    break;
                case ItemType.TIER_BOOST:
                    // Nothing to
                    break;
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    {
                        var finderResult = itemUseResult.result as ItemResultHiddenUniverseFinder;
                        if (finderResult != null)
                        {
                            earnFinder += finderResult.earnFinder;
                        }
                    }
                    break;
                case ItemType.TICKETED_BONUS_BOOSTER:
                    {
                        ItemResultTicketedBonusBooster ticketedBonusBooster = itemUseResult.result as ItemResultTicketedBonusBooster;
                        if (ticketedBonusBooster != null)
                        {
                            earnCoins += ticketedBonusBooster.earnCredit;
                            earnRp += ticketedBonusBooster.earnRp;
                        }
                    }
                    break;
#if DEV
                default:
                    Debug.LogError(string.Format("ItemType({0}) has not been processed.", itemUseResult.itemType));
                    break;
#endif
            }

            if (earnCoins > 0)
                BlackboardQueryUtils.AddCoins(earnCoins);

            if (earnRp > 0)
                BlackboardQueryUtils.AddRP(earnRp);

            if (earnGems > 0)
                BlackboardQueryUtils.AddGems(earnGems);

            if (earnFinder > 0)
                BlackboardQueryUtils.AddFinders(earnFinder);
        }

        public static string GetItemName(ItemType itemType)
        {
            if (itemType == ItemType.UNKNOWN) return null;

            return StringTableUtils.GetString(StringTable.StringTableType.Global, string.Format("ITEM_NAME_{0}", itemType));
        }
    }
}
