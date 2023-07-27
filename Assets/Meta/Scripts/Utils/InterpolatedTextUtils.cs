using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static class InterpolatedTextUtils
    {
        private const string LEGACY_SHOP_TYPE_VALUE = "shop";
        private const string SHOP_TYPE_VALUE = "coin";

        public static string ParseTextOfProduct(string text, Blackboard product)
        {
            if (product == null) return text;

            if (text.Contains("{price}"))
            {
                double price = product.GetValue<double>("price");
                text = Regex.Replace(text, "\\{price\\}", price.ToString(), RegexOptions.None);
            }

            if (text.Contains("{multiplier}"))
            {
                if (product.GetVariable<long>("eventMultiplierNumerator") != null)
                {
                    long eventMultiplierNumerator = product.GetValue<long>("eventMultiplierNumerator");
                    double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);
                    text = Regex.Replace(text, "\\{multiplier\\}", eventMultiplier.ToString(), RegexOptions.None);
                }
            }

            if (text.Contains("{event_percent}"))
            {
                if (product.GetVariable<long>("eventMultiplierNumerator") != null)
                {
                    long eventMultiplierNumerator = product.GetValue<long>("eventMultiplierNumerator");
                    long eventPercent = NumberUtils.GetAdditionalPercent(eventMultiplierNumerator);
                    text = Regex.Replace(text, "\\{event_percent\\}", FormatUtility.CommaNumberFormat(eventPercent), RegexOptions.None);
                }
            }

            if (text.Contains("{gem_price}"))
            {
                long gemPrice = product.GetValue<long>("gemPrice");
                string formattedGemPrice = FormatUtility.CommaNumberFormat(gemPrice);
                text = Regex.Replace(text, "\\{gem_price\\}", formattedGemPrice, RegexOptions.None);
            }

            if (Regex.IsMatch(text, "\\{productMultiplier.*?\\}"))
            {
                // ex) productMultiplier, productMultiplier|shop, productMultiplier|shop|previous
                long eventMultiplierNumerator = product.GetValue<long>("eventMultiplierNumerator");

                string patternString = Regex.Match(text, "\\{productMultiplier.*?\\}").Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split('|');

                string tagText = "";

                if (formats.Length == 1)
                    tagText = GetMultiplierNxFormat(eventMultiplierNumerator, LevelUtils.GetLevelMultiplierNumeratorFromPriviousSection(SHOP_TYPE_VALUE));
                else if (formats.Length >= 2)
                {
                    string valueType = formats[1];
                    if (valueType == LEGACY_SHOP_TYPE_VALUE)    // human error (143.0.0)
                        valueType = SHOP_TYPE_VALUE;

                    if (formats.Length >= 3 && "current" == formats[2].ToLower())  // hoxy optional?
                        tagText = GetMultiplierNxFormat(eventMultiplierNumerator, LevelUtils.GetLevelMultiplierNumeratorFromType(valueType));
                    else
                        tagText = GetMultiplierNxFormat(eventMultiplierNumerator, LevelUtils.GetLevelMultiplierNumeratorFromPriviousSection(valueType));
                }

                text = Regex.Replace(text, "\\{productMultiplier.*?\\}", tagText, RegexOptions.None);
            }

            text = ParseProductRewardText(text, product);

            List<Blackboard> itemList = product.GetValue<List<Blackboard>>("itemList");

            if (itemList.Count == 0) return text;

            Blackboard item = product.GetValue<List<Blackboard>>("itemList")[0];
            text = ParseIAMProductItemText(text, product, item);

            if (text.Contains("{day}") && item.GetVariable<int>("totalDayCount") != null)
            {
                int dayCount = item.GetValue<int>("totalDayCount");
                string formattedDay = FormatUtility.CommaNumberFormat(dayCount);
                text = Regex.Replace(text, "\\{day\\}", formattedDay, RegexOptions.None);
            }

            return text;
        }

        public static string ParseCustomText(string text)
        {
            text = ParseCustomFirstNumberPatternText(text, "\\{number.*?\\}");

            return text;
        }

        public static string ParseEconomyMultiplier(string text)
        {
            if (Regex.IsMatch(text, "\\{user_multiplier:.*?\\}"))
            {
                string patternString = Regex.Match(text, "\\{user_multiplier:.*?\\}").Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split(':');

                if(formats.Length >= 2)
                {
                    string valueType = formats[1];
                    // ex) user_multiplier:shop:current
                    if (valueType == LEGACY_SHOP_TYPE_VALUE)    // human error (143.0.0)
                        valueType = SHOP_TYPE_VALUE;

                    if (formats.Length == 2)
                    {
                        text = Regex.Replace(text, "\\{user_multiplier:.*?\\}", LevelUtils.GetLevelMultiplierStringFromPreviousSection(valueType), RegexOptions.None);
                    }
                    else if(formats.Length >= 3)
                    {
                        // optional
                        if(formats[2] == "current")
                        {
                            var numerator = LevelUtils.GetLevelMultiplierNumeratorFromType(valueType);
                            text = Regex.Replace(text, "\\{user_multiplier:.*?\\}", NumberUtils.GetMultiplierFromNumerator(numerator).ToString(), RegexOptions.None);
                        }
                        else
                        {
                            text = Regex.Replace(text, "\\{user_multiplier:.*?\\}", LevelUtils.GetLevelMultiplierStringFromPreviousSection(valueType), RegexOptions.None);
                        }
                    }
                }
            }

            if (Regex.IsMatch(text, "\\{LM:.*?\\}"))
            {
                string patternString = Regex.Match(text, "\\{LM:.*?\\}").Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split(':');

                if(formats.Length >= 2)
                {
                    string valueType = formats[1];

                    if (valueType == LEGACY_SHOP_TYPE_VALUE)    // human error (143.0.0)
                        valueType = SHOP_TYPE_VALUE;
                    // ex) LM:shop:current
                    if(formats.Length == 2)
                    {
                        text = Regex.Replace(text, "\\{LM:.*?\\}", LevelUtils.GetLevelMultiplierPromotionFormatStringFromPreviousSection(valueType), RegexOptions.None);
                    }
                    else if(formats.Length >= 3)
                    {
                        // optional
                        if(formats[2] == "current")
                        {
                            var numerator = LevelUtils.GetLevelMultiplierNumeratorFromType(valueType);
                            text = Regex.Replace(text, "\\{LM:.*?\\}", NumberUtils.GetMultiplierFromNumerator(numerator).ToString(), RegexOptions.None);
                        }
                        else
                        {
                            text = Regex.Replace(text, "\\{LM:.*?\\}", LevelUtils.GetLevelMultiplierPromotionFormatStringFromPreviousSection(valueType), RegexOptions.None);
                        }
                    }
                }
            }

            if (text.Contains("{early_access_expected_credit}"))
            {
                var multiplier = LevelUtils.GetLevelMultiplierNumeratorFromType("earlyAccess");
                long count = BlackboardQueryUtils.GetEarlyAccessMaxCount();

                // var gameInfo = BlackboardQueryUtils.GetEarlyAccessGameInfoList()[1]; // center early access game index is 1
                // var minBet = gameInfo.GetValue<long>("minBet");
                // var gameId = gameInfo.GetValue<int>("gameId");
                // long credit = CalculateBetLevelMultiplier(minBet, 0, BlackboardQueryUtils.GetRawBaseBet(gameId), multiplier);

                var totalBet = BlackboardUtils.FindValue<long>(MainBlackboard.Get(), "earlyAccessTotalBetInfo");
                totalBet = NumberUtils.GetMultiplierNumeratorValue(totalBet, multiplier);

                text = Regex.Replace(text, "\\{early_access_expected_credit\\}", FormatUtility.CommaNumberFormat(totalBet * count * 30), RegexOptions.None);
            }

            if (text.Contains("{purchase_tier_multiplier}"))
            {
                int tier = TierUtils.GetMeTier();
                text = Regex.Replace(text, "\\{purchase_tier_multiplier\\}", TierUtils.GetTierMultiplier(tier).ToString(), RegexOptions.None);
            }

            if (Regex.IsMatch(text, "\\{level_multiplier.*?\\}"))
            {
                // ex) level_multiplier:coin
                string patternString = Regex.Match(text, "\\{level_multiplier.*?\\}").Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split(':');
                string valueType = "";

                if (formats.Length == 1)
                    valueType = SHOP_TYPE_VALUE;
                else if (formats.Length > 1)
                {
                    valueType = formats[1];
                    if (valueType == LEGACY_SHOP_TYPE_VALUE)    // human error (143.0.0)
                        valueType = SHOP_TYPE_VALUE;
                }
                text = Regex.Replace(text, "\\{level_multiplier\\}", NumberUtils.GetMultiplierFromNumerator(LevelUtils.GetLevelMultiplierNumerator(valueType)).ToString(), RegexOptions.None);
            }

            if (Regex.IsMatch(text, "\\{PM:.*?\\}"))
            {
                string patternString = Regex.Match(text, "\\{PM:.*?\\}").Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split(':');
                // ex) PM:coinShop, PM:gemShop:shop, PM:gemShop:shop:previous
                if (formats.Length >= 2)
                {
                    string eventType = formats[1];
                    long pmNumerator = GetPassiveEventNameMultiplier(eventType);
                    string tagText = "";

                    if (formats.Length == 2)
                        tagText = GetMultiplierNxFormat(pmNumerator);
                    else if (formats.Length >= 3)
                    {
                        string valueType = formats[2];
                        if (valueType == LEGACY_SHOP_TYPE_VALUE)    // human error (143.0.0)
                            valueType = SHOP_TYPE_VALUE;

                        if (formats.Length >= 4 && "current" == formats[3].ToLower())
                            tagText = GetMultiplierNxFormat(pmNumerator, LevelUtils.GetLevelMultiplierNumeratorFromType(valueType));

                        else
                            tagText = GetMultiplierNxFormat(pmNumerator, LevelUtils.GetLevelMultiplierNumeratorFromPriviousSection(valueType));

                    }
                    text = Regex.Replace(text, "\\{PM:.*?\\}", tagText, RegexOptions.None);
                }
            }

            return text;
        }

        public static string ParseCommonText(string text)
        {
            if (text.Contains("{tier_name}"))
            {
                int tier = TierUtils.GetMeTier();
                string tierName = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_TIER_NAME", tier);
                text = Regex.Replace(text, "\\{tier_name\\}", tierName, RegexOptions.None);
            }
            if (text.Contains("{tier_style_name}"))
            {
                int tier = TierUtils.GetMeTier();
                string tierName = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_TIER_STYLE", tier);
                text = Regex.Replace(text, "\\{tier_style_name\\}", tierName, RegexOptions.None);
            }
            if (text.Contains("{tier_icon}"))
            {
                int tier = TierUtils.GetMeTier();
                string tierName = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_TIER_ICON", tier);
                text = Regex.Replace(text, "\\{tier_icon\\}", tierName, RegexOptions.None);
            }

            return text;
        }

        private static string ParseCustomFirstNumberPatternText(string text, string pattern)
        {
            // {type|value|Optional...}
            int whileCount = 0;
            while (Regex.IsMatch(text, pattern) && whileCount < 30)
            {
                string origPatternString = Regex.Match(text, pattern).Value;
                string patternString = origPatternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split('|');

                if (formats.Length > 1)
                {
                    try
                    {
                        long value = System.Convert.ToInt64(formats[1]);
                        bool isSimpleFormat = false;

                        for (int i = 2; i < formats.Length; ++i)
                        {
                            if ("simple" == formats[i].ToLower())
                                isSimpleFormat = true;
                            else if ("tier" == formats[i].ToLower())
                                value = TierUtils.GetTierFractionCoin(value, TierUtils.GetMeTier());
                            else
                                value = ApplyUserMultiplierFormat(value, formats[i]);
                        }

                        string formattedText = isSimpleFormat ? FormatUtility.SimpleNumberFormat(value) : FormatUtility.CommaNumberFormat(value);
                        // text = Regex.Replace(text, pattern, formattedText, RegexOptions.None);
                        text = text.Replace(origPatternString, formattedText);
                    }
                    catch (System.FormatException e)
                    {
                        if (ApplicationSettings.LogSystem())
                        {
                            Debug.LogError("IAM Format Error");
                        }

                        break;
                    }
                }

                ++whileCount;
            }

            return text;
        }

        private static long ApplyUserMultiplierFormat(long value, string formatType)
        {
            if (Regex.IsMatch(formatType, "user_multiplier:.*?"))
            {
                string[] formats = formatType.Split(':');

                if(formats.Length >= 2 && !string.IsNullOrEmpty(formats[1]))
                {
                    long numerator = LevelUtils.GetLevelMultiplierNumeratorFromType(formats[1] == LEGACY_SHOP_TYPE_VALUE ? SHOP_TYPE_VALUE : formats[1]);
                    value = NumberUtils.GetMultiplierNumeratorValue(value, numerator);
                }
            }

            return value;
        }

        private static string ParseProductRewardText(string text, Blackboard product)
        {
            if (!Regex.IsMatch(text, "\\{reward_.*?\\}")) return text;

            var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(product, "rewardList");

            if (rewardList != null)
            {
                for (int i = 0; i < rewardList.value.Count; ++i)
                {
                    var rewardType = rewardList.value[i].GetValue<RewardType>("type");

                    switch (rewardType)
                    {
                        case RewardType.GAME_SPIN:
                            {
                                bool applyLevelMultiplier = rewardList.value[i].GetValue<bool>("applyLevelMultiplier");
                                long bet = applyLevelMultiplier ? LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("bet"), "spinDeal") : rewardList.value[i].GetValue<long>("bet");
                                long totalBet = applyLevelMultiplier ? LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("totalBet"), "spinDeal") : rewardList.value[i].GetValue<long>("totalBet");
                                text = ParseRewardPatternText(text, "\\{reward_gamespin_spin_count.*?\\}", product, rewardList.value[i], (long)rewardList.value[i].GetValue<int>("spinCount"));
                                text = ParseRewardPatternText(text, "\\{reward_gamespin_bet.*?\\}", product, rewardList.value[i], bet);
                                text = ParseRewardPatternText(text, "\\{reward_gamespin_totalbet.*?\\}", product, rewardList.value[i], totalBet);
                            }
                            break;
                        case RewardType.GAME_PLAY:
                            {
                                long betPerTicket = BlackboardUtils.FindValue<long>(rewardList.value[i], "betPerTicket");
                                long ticketCount = BlackboardUtils.FindValue<int>(rewardList.value[i], "ticketCount");
                                long totalBet = betPerTicket * ticketCount;
                                text = ParseRewardPatternText(text, "\\{reward_gameplay_count.*?\\}", product, rewardList.value[i], (long)rewardList.value[i].GetValue<int>("count"));
                                text = ParseRewardPatternText(text, "\\{reward_gameplay_ticket_count.*?\\}", product, rewardList.value[i], betPerTicket);
                                text = ParseRewardPatternText(text, "\\{reward_gameplay_totalbet.*?\\}", product, rewardList.value[i], totalBet);
                            }
                            break;
                        case RewardType.SCRATCHER:
                            {
                                long totalMaxWinCredit = LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("maxWinCredit"), "scratcher");
                                totalMaxWinCredit = NumberUtils.GetMultiplierNumeratorValue(totalMaxWinCredit, rewardList.value[i].GetValue<long>("tierMultiplierNumerator"));

                                text = ParseRewardPatternText(text, "\\{reward_scratcher_maxwin.*?\\}", product, rewardList.value[i], totalMaxWinCredit);

                                if (text.Contains("{reward_scratcher_reward_name}"))
                                {
                                    text = Regex.Replace(text, "\\{reward_scratcher_reward_name\\}", rewardList.value[i].GetValue<string>("rewardName"), RegexOptions.None);
                                }
                            }
                            break;
                        case RewardType.SCRATCHER_FOR_INBOX:
                            {
                                long totalMaxWinCredit = LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("maxWinCredit"), "scratcher");

                                if(rewardList.value[i].GetValue<bool>("applyTierMultiplier"))
                                    totalMaxWinCredit = TierUtils.GetTierFractionCoin(totalMaxWinCredit, TierUtils.GetMeTier());

                                text = ParseRewardPatternText(text, "\\{reward_scratcher_for_inbox_maxwin.*?\\}", product, rewardList.value[i], totalMaxWinCredit);
                                text = ParseRewardPatternText(text, "\\{reward_scratcher_for_inbox_count.*?\\}", product, rewardList.value[i], rewardList.value[i].GetValue<int>("count"));

                                if (text.Contains("{reward_scratcher_for_inbox_reward_name}"))
                                {
                                    text = Regex.Replace(text, "\\{reward_scratcher_for_inbox_reward_name\\}", rewardList.value[i].GetValue<string>("rewardName"), RegexOptions.None);
                                }
                            }
                            break;
                        case RewardType.TICKETED_BONUS_TICKET:
                        case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                            {
                                long totalBet = IAMUtils.CalculateBetLevelMultiplier(rewardList.value[i].GetValue<long>("baseBet"), rewardList.value[i].GetValue<long>("extraBet"), BlackboardQueryUtils.GetRawBaseBet(rewardList.value[i].GetValue<int>("gameId")));
                                text = ParseRewardPatternText(text, "\\{reward_ticketed_bonus_totalbet.*?\\}", product, rewardList.value[i], totalBet);
                                text = ParseRewardPatternText(text, "\\{reward_ticketed_bonus_gamecount.*?\\}", product, rewardList.value[i], rewardList.value[i].GetValue<int>("gameCount"));
                            }
                            break;
                        case RewardType.SOCIAL_CREDIT:
                            {
                                long credit = rewardList.value[i].GetValue<long>("credit");
                                text = ParseRewardPatternText(text, "\\{reward_social_credit.*?\\}", product, rewardList.value[i], credit);
                            }
                            break;
                        case RewardType.CLUB_CREDIT:
                            {
                                long credit = rewardList.value[i].GetValue<long>("memberCredit");
                                text = ParseRewardPatternText(text, "\\{reward_club_credit.*?\\}", product, rewardList.value[i], credit);
                            }
                            break;
                        case RewardType.SPIN_DEAL:
                            {
                                text = ParseRewardPatternText(text, "\\{reward_spindeal_spin_count.*?\\}", product, rewardList.value[i], (long)rewardList.value[i].GetValue<int>("spinCount"));
                                text = ParseRewardPatternText(text, "\\{reward_spindeal_bet.*?\\}", product, rewardList.value[i], LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("baseBet"), "spinDeal"));
                                text = ParseRewardPatternText(text, "\\{reward_spindeal_totalbet.*?\\}", product, rewardList.value[i], LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("totalBet"), "spinDeal"));
                            }
                            break;
                        case RewardType.HIDDEN_UNIVERSE_FINDER:
                            {
                                text = ParseRewardPatternText(text, "\\{reward_finder_count.*?\\}", product, rewardList.value[i], rewardList.value[i].GetValue<int>("finder"));
                            }
                            break;
                        case RewardType.HOG_DEAL:
                            {
                                int count = rewardList.value[i].GetValue<int>("count");
                                long totalMaxWinCredit = LevelUtils.GetLevelMultiplierNumeratorValue(rewardList.value[i].GetValue<long>("maxWinCredit"), "hogDeal");

                                text = ParseRewardPatternText(text, "\\{reward_hogdeal_count.*?\\}", product, rewardList.value[i], count);
                                text = ParseRewardPatternText(text, "\\{reward_hogdeal_maxwin.*?\\}", product, rewardList.value[i], totalMaxWinCredit);
                            }
                            break;
                        case RewardType.BOSS_RAIDERS_DEAL:
                            {
                                long baseAttack = rewardList.value[i].GetValue<long>("multiplier") * IAMExtraDataUtils.GetBossRaidersDealBaseAttack();
                                baseAttack = LevelUtils.GetLevelMultiplierNumeratorValue(baseAttack, "bossRaidersDeal");
                                text = ParseRewardPatternText(text, "\\{reward_bossdeal_attack.*?\\}", product, rewardList.value[i], baseAttack);
                            }
                            break;
                    }
                }
            }

            return text;
        }

        private static string ParseIAMProductItemText(string text, Blackboard product, Blackboard item)
        {
            // Common variables.
            if (item.GetVariable<long>("baseCredit") != null || item.GetVariable<long>("baseCreditPerDay") != null)
            {
                long credit = item.GetVariable<long>("baseCredit") != null ? item.GetValue<long>("baseCredit") : item.GetValue<long>("baseCreditPerDay");
                string valueType = item.GetVariable<long>("baseCredit") != null ? SHOP_TYPE_VALUE : "dailyBoost";
                credit = LevelUtils.GetLevelMultiplierNumeratorValue(credit, valueType);

                text = ParseItemPatternText(text, "\\{base_coin.*?\\}", product, item, credit);
                text = ParseItemPatternText(text, "\\{coin.*?\\}", product, item, credit);
            }

            if (item.GetVariable<long>("gem") != null || item.GetVariable<long>("gem") != null)
            {
                long gem = item.GetValue<long>("gem");

                text = ParseItemPatternText(text, "\\{gem.*?\\}", product, item, gem);
            }

            if (item.GetVariable<long>("rp") != null && text.Contains("{rp}"))
            {
                long rp = item.GetValue<long>("rp");
                string formattedRp = FormatUtility.CommaNumberFormat(rp);
                text = Regex.Replace(text, "\\{rp\\}", formattedRp, RegexOptions.None);
            }

            // Item variables.
            if (!Regex.IsMatch(text, "\\{item_.*?\\}")) return text;

            var itemType = item.GetValue<ItemType>("itemType");

            switch (itemType)
            {
                case ItemType.TICKETED_BONUS_TICKET:
                    {
                        long totalBet = IAMUtils.CalculateBetLevelMultiplier(item.GetValue<long>("bet"), item.GetValue<long>("extraBet"), BlackboardQueryUtils.GetRawBaseBet(item.GetValue<int>("gameId")));

                        text = ParseItemPatternText(text, "\\{item_ticketed_bonus_totalbet.*?\\}", product, item, totalBet);

                        var freeSpinCount = BlackboardUtils.FindVariable<int>(item, "extraData/freeSpinCount");
                        if (freeSpinCount != null)
                        {
                            text = ParseItemPatternText(text, "\\{item_ticketed_bonus_gamecount.*?\\}", product, item, freeSpinCount.value);
                        }

                    }
                    break;
            }

            return text;
        }

        private static string ParseRewardPatternText(string text, string pattern, Blackboard product, Blackboard reward, long value)
        {
            if (Regex.IsMatch(text, pattern))
            {
                string patternString = Regex.Match(text, pattern).Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split('|');

                bool isSimpleFormat = false;

                for (int i = 1; i < formats.Length; ++i)
                {
                    if ("simple" == formats[i].ToLower())
                        isSimpleFormat = true;
                    else
                        value = ApplyRewardFormat(value, formats[i], product, reward);
                }

                string formattedText = isSimpleFormat ? FormatUtility.SimpleNumberFormat(value) : FormatUtility.CommaNumberFormat(value);
                text = Regex.Replace(text, pattern, formattedText, RegexOptions.None);
            }

            return text;
        }

        private static string ParseItemPatternText(string text, string pattern, Blackboard product, Blackboard item, long value)
        {
            if (Regex.IsMatch(text, pattern))
            {
                string patternString = Regex.Match(text, pattern).Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split('|');

                bool isSimpleFormat = false;

                for (int i = 1; i < formats.Length; ++i)
                {
                    if ("simple" == formats[i].ToLower())
                        isSimpleFormat = true;
                    else
                        value = ApplyItemFormat(value, formats[i], product, item);
                }

                string formattedText = isSimpleFormat ? FormatUtility.SimpleNumberFormat(value) : FormatUtility.CommaNumberFormat(value);
                text = Regex.Replace(text, pattern, formattedText, RegexOptions.None);
            }

            return text;
        }

        private static long ApplyItemFormat(long value, string formatType, Blackboard product, Blackboard item)
        {
            switch (formatType)
            {
                case "event":
                    {
                        if (product.GetVariable<long>("eventMultiplierNumerator") != null)
                        {
                            long eventMultiplierNumerator = product.GetValue<long>("eventMultiplierNumerator");
                            value = NumberUtils.GetMultiplierNumeratorValue(value, eventMultiplierNumerator);
                        }
                    }
                    break;
                case "tier":
                    {
                        value = TierUtils.GetTierFractionCoin(value, TierUtils.GetMeTier());
                    }
                    break;
                case "additional":
                    {
                        if (item.GetVariable<long>("additionalCreditMultiplierNumerator") != null)
                        {
                            long additionalCreditMultiplierNumerator = item.GetValue<long>("additionalCreditMultiplierNumerator");
                            value = NumberUtils.GetAdditionalMultiplierNumeratorValue(value, additionalCreditMultiplierNumerator);
                        }
                    }
                    break;
                case "day":
                    {
                        if (item.GetVariable<int>("totalDayCount") != null)
                        {
                            int totalDayCount = item.GetValue<int>("totalDayCount");
                            value = value * (long)totalDayCount;
                        }
                    }
                    break;
            }

            return value;
        }

        private static long ApplyRewardFormat(long value, string formatType, Blackboard product, Blackboard rewardInfo)
        {
            if (rewardInfo == null) return value;

            switch (formatType)
            {
                case "tier":
                    {
                        value = TierUtils.GetTierFractionCoin(value, TierUtils.GetMeTier());
                    }
                    break;
            }

            return value;
        }

        private static string GetMultiplierNxFormat(long origValue, long multiplierNumerator = 0L)
        {
            string text = "";
            double totalMultiplier = 0.0;

            if(origValue == 0 || multiplierNumerator == 0)
                totalMultiplier = NumberUtils.GetMultiplierFromNumerator(origValue + multiplierNumerator);
            else
                totalMultiplier = NumberUtils.SecondDecimalCutting(origValue) * NumberUtils.SecondDecimalCutting(multiplierNumerator);

            if(totalMultiplier > 0.0)
            {
                text = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_MULTIPLIER_NX", NumberUtils.SecondDecimalCutting(totalMultiplier));
            }

            return text;
        }

        private static long GetPassiveEventNameMultiplier(string passiveName)
        {
            long multiplierValue = 0L;
            if (string.IsNullOrEmpty(passiveName))
                return multiplierValue;

            try
            {
                EventInfoType infoType = (EventInfoType)System.Enum.Parse(typeof(EventInfoType), passiveName);
                EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(infoType);

                if (eventInfo != null)
                    multiplierValue = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            }
            catch (System.Exception e)
            {
                // ArgumentException?
            }

            return multiplierValue;
        }
    }
}
