using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using static BagelCode.MysteryTest;

namespace BagelCode
{
    public class MetaCommonRewardUtils
    {
        public const string ON_FINISH_REWARD = "OnFinishReward";

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public static string GetRewardText(Blackboard rewardInfo)
        {
            RewardType type = rewardInfo.GetVariable<RewardType>("type")?.value ?? RewardType.UNKNOWN;
            switch (type)
            {
                case RewardType.CREDIT:
                    long credit = rewardInfo.GetVariable<long>("credit")?.value ?? 0L;
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_TOTAL_REWARD_COIN", credit);
                case RewardType.GEM:
                    long gem = rewardInfo.GetVariable<long>("gem")?.value ?? 0L;
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_TOTAL_REWARD_GEM", gem);
                default:
                    Debug.LogWarning("GetRewardText failure. The type is undefined: " + type);
                    break;
            }

            return string.Empty;
        }

        public static GameObject SetCommonRewardSlotImage(Blackboard rewardInfo, GameObject imageObj, RewardType type = RewardType.UNKNOWN, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            GameObject iconObj = null;

            if (type == RewardType.UNKNOWN)
            {
                type = BlackboardUtils.FindVariable<RewardType>(rewardInfo, "type")?.value ??
                    BlackboardUtils.FindVariable<RewardType>(rewardInfo, "rewardType")?.value ??
                    RewardType.UNKNOWN;
            }

            switch (type)
            {
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.SPIN_DEAL:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        var gameID = BlackboardUtils.FindVariable<int>(rewardInfo, "gameId");
                        Blackboard gameInfo = BlackboardQueryUtils.GetGameInfo(gameID.value);

                        Transform parent = imageObj.transform;
                        if(checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            var imageElement = imageObj.GetComponent<ContextElement>();
                            parent = ContextUtils.FindElement(imageElement, "Slot Thumbnail Area", ContextSearchingType.ChildrenSearch).transform;
                        }

                        var gameTitle = BlackboardUtils.FindVariable<string>(gameInfo, "gameTitle").value;
                        iconObj = MetaIconUtils.MakeSlotThumbnailIconObjectFromGameTitle(gameTitle, parent, null);
                    }
                    break;
            }

            return iconObj;
        }

        public static string GetCommonRewardAmountSimpleText(Blackboard rewardInfo, bool isInbox, RewardType type = RewardType.UNKNOWN, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            long rewardAmount = GetRewardAmount(rewardInfo, isInbox, type, checkScene);
            string text = rewardAmount.ToString();

            if (type == RewardType.UNKNOWN)
            {
                type = BlackboardUtils.FindVariable<RewardType>(rewardInfo, "type")?.value ??
                    BlackboardUtils.FindVariable<RewardType>(rewardInfo, "rewardType")?.value ??
                    RewardType.UNKNOWN;
            }

            switch (type)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                case RewardType.SOCIAL_CREDIT:
                case RewardType.CLUB_CREDIT:
                    {
                        long credit = rewardAmount;
                        text = StringTableUtils.GetString(GLOBAL, "SIMPLE_CREDIT", credit);
                    }
                    break;
                case RewardType.RP:
                case RewardType.GEM:
                    {
                        long count = rewardAmount;
                        text = StringTableUtils.GetString(GLOBAL, "TEXT_COMMA_NUMBER", count);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    break;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        if(checkScene == RewardCheckScene.DEFAULT)
                        {
                            long multiplierNumerator = BlackboardUtils.FindValue<long>(rewardInfo, "multiplierNumerator");
                            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

                            text = TextDecoUtils.RatioToPercentage((float)multiplier);
                        }
                        else if(checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            long seconds = rewardAmount;
                            text = StringTableUtils.GetString(GLOBAL, "LEVEL_UP_DASH_SIMPLE_REWARD_BOOSTER_TIME_TEXT", seconds);
                        }
                    }
                    break;
                case RewardType.RANDOM:
                case RewardType.PURCHASE_COUPON:
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.DAILY_BOOST:
                case RewardType.SCRATCHER:
                case RewardType.COLLECTING_GAME_PACK:
                case RewardType.SCRATCHER_FOR_INBOX:
                case RewardType.BOSS_RAIDERS_ENERGY:
                case RewardType.CLUB_ARENA_ENERGY:
                case RewardType.SPIN_DEAL:
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                case RewardType.HOG_DEAL:
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                case RewardType.BOSS_RAIDERS_DEAL:
                case RewardType.DEPOT:
                case RewardType.WILD_PUZZLE:
                    {
                        long count = rewardAmount;
                        text = StringTableUtils.GetString(GLOBAL, "TEXT_NUMERABLE", count);
                    }
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        long amount = rewardAmount;
                        text = StringTableUtils.GetString(GLOBAL, "SIMPLE_REWARD_TICKETED_BONUS_TICKET_AMOUNT", amount);
                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    {
                        text = StringTableUtils.GetString(GLOBAL, "SIMPLE_REWARD_TICKETED_BONUS_TICKET_AMOUNT");
                    }
                    break;
            }

            return text;
        }

        public static long GetRewardAmount(Blackboard rewardInfo, bool isInbox, RewardType type = RewardType.UNKNOWN, RewardCheckScene checkScene = RewardCheckScene.DEFAULT, bool applyMultiplier = true)
        {
            long amount = 1L;

            if (type == RewardType.UNKNOWN)
            {
                type = BlackboardUtils.FindVariable<RewardType>(rewardInfo, "type")?.value ??
                    BlackboardUtils.FindVariable<RewardType>(rewardInfo, "rewardType")?.value ??
                    RewardType.UNKNOWN;
            }

            switch (type)
            {
                case RewardType.CREDIT:
                    {
                        long credit = BlackboardUtils.FindValue<long>(rewardInfo, "credit");

                        if (isInbox && applyMultiplier)
                        {
                            var applyTierMultiplier = BlackboardUtils.GetOrCreateVariable<bool>(rewardInfo, "applyTierMultiplier")?.value ?? false;
                            if (applyTierMultiplier)
                                credit = TierUtils.GetTierFractionCoin(credit, TierUtils.GetMeTier());
                            var applyLevelMultiplier = BlackboardUtils.GetOrCreateVariable<bool>(rewardInfo, "applyLevelMultiplier")?.value ?? false;
                            if (applyLevelMultiplier)
                                credit = LevelUtils.GetLevelMultiplierNumeratorValue(credit, "coin");
                        }

                        amount = credit;
                    }
                    break;
                case RewardType.CREDIT_WITH_MULTIPLIER:
                case RewardType.SOCIAL_CREDIT:
                    {
                        amount = BlackboardUtils.FindValue<long>(rewardInfo, "credit");
                    }
                    break;
                case RewardType.RP:
                    {
                        amount = BlackboardUtils.FindValue<long>(rewardInfo, "rp");
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        amount = isInbox ?
                            BlackboardUtils.FindValue<int>(rewardInfo, "spinCount"):
                            BlackboardUtils.FindValue<int>(rewardInfo, "addedSpinCount");
                    }
                    break;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        if(checkScene == RewardCheckScene.DEFAULT)
                        {
                            long multiplierNumerator = BlackboardUtils.FindValue<long>(rewardInfo, "multiplierNumerator");
                            amount = multiplierNumerator;
                        }
                        else if(checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "durationSec");
                        }
                    }
                    break;
                case RewardType.RANDOM:
                case RewardType.PURCHASE_COUPON:
                    break;
                case RewardType.GAME_SPIN:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, isInbox ? "spinCount" : "addedSpinCount");
                    }
                    break;
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, isInbox ? "count" : "addedCount");
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, "totalDayCount");
                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, isInbox ? "targetTier" : "upgradedTier");
                    }
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        if (isInbox)
                        {
                            if (checkScene == RewardCheckScene.EPIC_PASS_V2)
                            {
                                var ticketCount = BlackboardUtils.FindVariable<int>(rewardInfo, "ticketCount");
                                if (ticketCount != null)
                                    amount = ticketCount.value;
                                else
                                {
                                    amount = 1;
                                    BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "ticketCount", (int)amount);
                                }
                            }
                            else
                            {
                                long baseBet = BlackboardUtils.FindValue<long>(rewardInfo, "baseBet");
                                long extraBet = BlackboardUtils.FindValue<long>(rewardInfo, "extraBet");
                                long totalBet = baseBet + extraBet;
                                amount = applyMultiplier ?
                                    MultiplierUtils.GetRewardMultiplierValue(totalBet, rewardInfo, "ticketedBonus") :
                                    totalBet;
                            }
                        }
                    }
                    break;
                case RewardType.CLUB_CREDIT:
                    {
                        amount = BlackboardUtils.FindValue<long>(rewardInfo, "memberCredit");
                    }
                    break;
                case RewardType.SCRATCHER:
                    break;
                case RewardType.GEM:
                    {
                        amount = BlackboardUtils.FindValue<long>(rewardInfo, "gem");
                    }
                    break;
                case RewardType.COLLECTING_GAME_PACK:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, "rewardInfo/count");
                    }
                    break;
                case RewardType.SCRATCHER_FOR_INBOX:
                    {
                        if (isInbox)
                        {
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "count");
                        }
                    }
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                case RewardType.CLUB_ARENA_ENERGY:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, "energy");
                    }
                    break;
                case RewardType.SPIN_DEAL:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, "spinCount");
                    }
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    {
                        if (isInbox)
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "finder");
                        else
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "earnFinder");
                    }
                    break;
                case RewardType.HOG_DEAL:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, "count");
                    }
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    {
                        if (isInbox)
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "openDays");
                        else
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "addedDays");
                    }
                    break;
                case RewardType.BOSS_RAIDERS_DEAL:
                    {
                        amount = BlackboardUtils.FindValue<int>(rewardInfo, "dealCount");
                    }
                    break;
                case RewardType.WILD_PUZZLE:
                case RewardType.VIP_LOUNGE_POINT:
                    {
                        if (isInbox)
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "count");
                        else
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "addedCount");
                    }
                    break;
                case RewardType.DEPOT: // depotType
                    {
                        if (isInbox)
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "count");
                        else
                            amount = BlackboardUtils.FindValue<int>(rewardInfo, "addedCount");
                    }
                    break;
                default:
                    {
                        Debug.LogWarning($"MetaCommonReward.GetRewardAmount failure. {type} is undefined type.");
                    }
                    break;
            }

            return amount;
        }

        public static void SetRewardAmount(Blackboard rewardInfo, long amount, bool isInbox, RewardType type = RewardType.UNKNOWN, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            if (type == RewardType.UNKNOWN)
            {
                type = BlackboardUtils.FindVariable<RewardType>(rewardInfo, "type")?.value ??
                    BlackboardUtils.FindVariable<RewardType>(rewardInfo, "rewardType")?.value ??
                    RewardType.UNKNOWN;
            }

            switch (type)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                case RewardType.SOCIAL_CREDIT:
                    {
                        BlackboardUtils.SetOrCreateValue<long>(rewardInfo, "credit", amount);
                    }
                    break;
                case RewardType.RP:
                    {
                        BlackboardUtils.SetOrCreateValue<long>(rewardInfo, "rp", amount);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                case RewardType.GAME_SPIN:
                    {
                        if (isInbox)
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "spinCount", (int)amount);
                        else
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "addedSpinCount", (int)amount);                    }
                    break;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        if (checkScene == RewardCheckScene.DEFAULT)
                        {
                            BlackboardUtils.SetOrCreateValue<long>(rewardInfo, "multiplierNumerator", amount);
                        }
                        else if (checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "durationSec", (int)amount);
                        }
                    }
                    break;
                case RewardType.RANDOM:
                case RewardType.PURCHASE_COUPON:
                    break;
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                    {
                        if (isInbox)
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "count", (int)amount);
                        else
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "addedCount", (int)amount);
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "totalDayCount", (int)amount);
                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        if (isInbox)
                        {
                            if(checkScene == RewardCheckScene.EPIC_PASS_V2)
                            {
                                BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "ticketCount", (int)amount);
                            }
                            else
                            {
                                BlackboardUtils.SetOrCreateValue<long>(rewardInfo, "baseBet", amount);
                            }
                        }
                    }
                    break;
                case RewardType.CLUB_CREDIT:
                    {
                        BlackboardUtils.SetOrCreateValue<long>(rewardInfo, "memberCredit", amount);
                    }
                    break;
                case RewardType.SCRATCHER:
                    break;
                case RewardType.GEM:
                    {
                        BlackboardUtils.SetOrCreateValue<long>(rewardInfo, "gem", amount);
                    }
                    break;
                case RewardType.COLLECTING_GAME_PACK:
                    {
                        BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "rewardInfo/count", (int)amount);
                    }
                    break;
                case RewardType.SCRATCHER_FOR_INBOX:
                    {
                        if (isInbox)
                        {
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "count", (int)amount);
                        }
                    }
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                case RewardType.CLUB_ARENA_ENERGY:
                    {
                        BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "energy", (int)amount);
                    }
                    break;
                case RewardType.SPIN_DEAL:
                    {
                        BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "spinCount", (int)amount);
                    }
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    {
                        if (isInbox)
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "finder", (int)amount);
                        else
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "earnFinder", (int)amount);
                    }
                    break;
                case RewardType.HOG_DEAL:
                    {
                        BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "count", (int)amount);
                    }
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    {
                        if (isInbox)
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "openDays", (int)amount);
                        else
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "addedDays", (int)amount);
                    }
                    break;
                case RewardType.BOSS_RAIDERS_DEAL:
                    {
                        BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "dealCount", (int)amount);
                    }
                    break;
                case RewardType.WILD_PUZZLE:
                case RewardType.VIP_LOUNGE_POINT:
                    {
                        if (isInbox)
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "count", (int)amount);
                        else
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "addedCount", (int)amount);
                    }
                    break;
                case RewardType.DEPOT: // depotType
                    {
                        if (isInbox)
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "count", (int)amount);
                        else
                            BlackboardUtils.SetOrCreateValue<int>(rewardInfo, "addedCount", (int)amount);
                    }
                    break;
                default:
                    {
                        Debug.LogWarning($"MetaCommonReward.SetRewardAmount failure. {type} is undefined type.");
                    }
                    break;
            }
        }
        // RewardResultList or RewardInfoList
        public static List<Blackboard> CreateSimpleRewardResultOrInfoList(IBlackboard bb, string key, List<Blackboard> rewardList, RewardCheckScene rewardCheckScene = RewardCheckScene.DEFAULT)
        {
            if (rewardList == null || rewardList.Count == 0) return rewardList;

            var newList = BlackboardUtils.GetOrCreateBlackboardList(bb, key);

            foreach (var reward in rewardList)
            {
                bool matched = false;
                foreach (var uniqueReward in newList)
                {
                    if (IsMatchedReweardResult(reward, uniqueReward))
                    {
                        matched = true;
                        Blackboard uniqueInfo = BlackboardUtils.FindVariable<Blackboard>(uniqueReward, "rewardInfo")?.value ?? uniqueReward;
                        Blackboard rewardInfo = BlackboardUtils.FindVariable<Blackboard>(reward, "rewardInfo")?.value ?? reward;
                        SumReward(uniqueInfo, rewardInfo, true);
                        break;
                    }
                }
                if (!matched)
                {
                    MetaBlackboardUtils.AddCopyToBlackboardList(bb, key, reward);
                }
            }

            void SumReward(Blackboard a, Blackboard b, bool isInbox)
            {
                long target = GetRewardAmount(a, isInbox, RewardType.UNKNOWN, rewardCheckScene, false);
                long current = GetRewardAmount(b, isInbox, RewardType.UNKNOWN, rewardCheckScene, false);
                long sum = target + current;
                SetRewardAmount(a, sum, isInbox, RewardType.UNKNOWN, rewardCheckScene);
            }

            bool IsMatchedReweardResult(Blackboard a, Blackboard b)
            {
                Variable<RewardType> aType = BlackboardUtils.FindVariable<RewardType>(a, "rewardType");
                Variable<RewardType> bType = BlackboardUtils.FindVariable<RewardType>(b, "rewardType");

                Blackboard aRewardInfo, bRewardInfo;
                if (aType == null)
                {
                    aType = BlackboardUtils.FindVariable<RewardType>(a, "type");
                    aRewardInfo = a;
                }
                else
                    aRewardInfo = BlackboardUtils.FindValue<Blackboard>(a, "rewardInfo");
                if (bType == null)
                {
                    bType = BlackboardUtils.FindVariable<RewardType>(b, "type");
                    bRewardInfo = b;
                }
                else
                    bRewardInfo = BlackboardUtils.FindValue<Blackboard>(b, "rewardInfo");

                if (aType.value == RewardType.DEPOT)
                {
                    DepotType aDepotType = BlackboardUtils.FindVariable<DepotType>(aRewardInfo, "depotType")?.value ?? DepotType.UNKNOWN;
                    DepotType bDepotType = BlackboardUtils.FindVariable<DepotType>(bRewardInfo, "depotType")?.value ?? DepotType.UNKNOWN;
                    return aDepotType == bDepotType;
                }
                else if (aType.value == RewardType.TICKETED_BONUS_TICKET)
                {
                    BonusTag aTag = BlackboardUtils.FindVariable<BonusTag>(aRewardInfo, "tag")?.value ?? BonusTag.UNKNOWN;
                    BonusTag bTag = BlackboardUtils.FindVariable<BonusTag>(bRewardInfo, "tag")?.value ?? BonusTag.UNKNOWN;
                    return aTag == bTag;
                }

                return aType.value == bType.value;
            }

            return newList;
        }

        public static string GetRewardImageAssetName(RewardType rewardType, Blackboard rewardInfo, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            string assetName = "";

            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    assetName = "Image Common Reward Coin";
                    break;
                case RewardType.RP:
                    assetName = "Image Common Reward Vip Point";
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    assetName = "Image Common Reward Daily Spin";
                    break;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    if (checkScene == RewardCheckScene.DEFAULT)
                        assetName = "Image Common Reward Exp Boost";
                    else if (checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        assetName = "Image Common Reward Level Up Exp Boost";
                    break;
                case RewardType.RANDOM:
                    assetName = "Image Common Reward Mystery";
                    break;
                // case RewardType.PURCHASE_COUPON:
                //     {
                //         text = "";
                //     }
                //     break;
                case RewardType.GAME_SPIN:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.SPIN_DEAL:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    if(checkScene == RewardCheckScene.DEFAULT)
                    assetName = "Image Common Reward Slot Thumbnail Area";
                    else if(checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        assetName = "Image Level Up Dash Reward Ticketed Bonus";
                    break;
                case RewardType.DAILY_BOOST:
                    assetName = "Image Common Reward Daily Boost";
                    break;
                case RewardType.TIER_UPGRADE:
                    assetName = "Image Common Reward VIP Upgrade";
                    break;
                case RewardType.SOCIAL_CREDIT:
                case RewardType.CLUB_CREDIT:
                    assetName = "Image Common Reward Gift To Friends";
                    break;
                case RewardType.GEM:
                    assetName = "Image Common Reward Gem";
                    break;
                case RewardType.COLLECTING_GAME_PACK:
                    assetName = "Image Common Reward Chest";
                    break;
                case RewardType.SCRATCHER:
                    if (checkScene == RewardCheckScene.DEFAULT)
                        assetName = "Image Common Reward Web Image";
                    else if (checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        assetName = "LM Shop Reward Scratcher";
                    break;
                case RewardType.SCRATCHER_FOR_INBOX:
                    if (checkScene == RewardCheckScene.DEFAULT)
                        assetName = "Image Common Reward Scratcher";
                    else if (checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        assetName = "LM Shop Reward Scratcher";
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                case RewardType.CLUB_ARENA_ENERGY:
                    assetName = "Image Common Reward Energy";
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    assetName = "Image Common Reward Finder";
                    break;
                case RewardType.HOG_DEAL:
                    assetName = "Image Common Reward Hog Deal";
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    assetName = "Image Common Reward VIP Lounge Ticket";
                    break;
                case RewardType.BOSS_RAIDERS_DEAL:
                    assetName = "Image Common Reward BossRaiders Deal";
                    break;
                case RewardType.DEPOT:
                    var depotType = rewardInfo.GetValue<DepotType>("depotType");
                    assetName = $"Image Common Reward Depot {depotType}";
                    break;
                case RewardType.WILD_PUZZLE:
                    assetName = $"Image Common Reward Wild Puzzle";
                    break;
                case RewardType.VIP_LOUNGE_POINT:
                    assetName = "Image Common Reward VIP Lounge Point";
                    break;
            }

            return assetName;
        }

        public static string GetRewardSoundName(RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    return "UI_Coin_Add_Independent";
                case RewardType.GEM:
                    return "UI_Gem_Add";
            }

            return "UI_Gift_Item";
        }

        public static bool IsVisibleRewardList(List<Blackboard> rewardList)
        {
            if (rewardList == null || rewardList.Count == 0) return false;

            foreach(var reward in rewardList)
            {
                RewardType type = BlackboardUtils.FindValue<RewardType>(reward, "rewardType");
                switch (type)
                {
                    case RewardType.CREDIT:
                    case RewardType.CREDIT_WITH_MULTIPLIER:
                    case RewardType.RP:
                    case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    case RewardType.EXP_MULTIPLY:
                    case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    case RewardType.RANDOM:
                    case RewardType.GAME_SPIN:
                    case RewardType.TICKETED_BONUS_TICKET:
                    case RewardType.GAME_DEAL:
                    case RewardType.GAME_PLAY:
                    case RewardType.SPIN_DEAL:
                    case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    case RewardType.DAILY_BOOST:
                    case RewardType.TIER_UPGRADE:
                    case RewardType.SOCIAL_CREDIT:
                    case RewardType.CLUB_CREDIT:
                    case RewardType.GEM:
                    case RewardType.COLLECTING_GAME_PACK:
                    case RewardType.SCRATCHER:
                    case RewardType.SCRATCHER_FOR_INBOX:
                    case RewardType.BOSS_RAIDERS_ENERGY:
                    case RewardType.CLUB_ARENA_ENERGY:
                    case RewardType.HIDDEN_UNIVERSE_FINDER:
                    case RewardType.HOG_DEAL:
                    case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    case RewardType.BOSS_RAIDERS_DEAL:
                    case RewardType.DEPOT:
                    case RewardType.WILD_PUZZLE:
                    case RewardType.VIP_LOUNGE_POINT:
                        return true;
                }
            }
            return false;
        }

        public static GameObject OpenCommonRewardResultPopup(GameObject caller, List<Blackboard> rewardList, bool applyEarnValues, bool useTitleTag = false, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            if (rewardList == null || rewardList.Count == 0) return null;

            if (!IsVisibleRewardList(rewardList)) return null;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = useTitleTag ? "Popup Common Reward Title Tag Scene" : "Popup Common Reward Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            var popupBB = popupObj.GetComponent<Blackboard>();

            string title = useTitleTag ?
                StringTableUtils.GetString(GLOBAL, "LEVEL_UP_DASH_MAIN_SCENE_REWARD_CONGRATULATIONS") :
                StringTableUtils.GetString(GLOBAL, "POPUP_PURCHASE_REWARD_TITLE");

            BlackboardUtils.SetOrCreateValue(popupBB, "rewardList", rewardList);
            BlackboardUtils.SetOrCreateValue(popupBB, "title", title);
            BlackboardUtils.SetOrCreateValue(popupBB, "applyEarnValues", applyEarnValues);
            BlackboardUtils.SetOrCreateValue(popupBB, "rewardCheckScene", checkScene);
            MetaObjectUtils.SetCalleeCaller(popupObj, caller);

            MetaPopupUtils.OpenPopup(popupObj);

            return popupObj;
        }

        public static void CommonRewardResultSetter(ContextElement imageAreaElement, ContextElement rewardTextElement, Blackboard rewardInfo, RewardType rewardType, bool isInbox, bool isMultiline, Blackboard extraDataBB = null, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            imageAreaElement.UpdateContext(true);

            string assetName = GetRewardImageAssetName(rewardType, rewardInfo, checkScene);
            GameObject iconObj;
            var iconElement = ContextUtils.FindElement(imageAreaElement, assetName, ContextSearchingType.ChildrenSearch);
            if (iconElement != null)
            {
                iconObj = iconElement.gameObject;
            }
            else
            {
                iconObj = MetaIconUtils.MakeCommonRewardImageObject(rewardType, imageAreaElement.transform, null, rewardInfo, checkScene);
                if (iconObj != null)
                {
                    iconElement = iconObj.GetComponent<ContextElement>();
                    iconElement.UpdateContext();
                }
            }

            if(iconObj != null)
            {
                var iconBB = iconObj.GetComponent<Blackboard>();
                if(iconBB != null)
                {
                    BlackboardUtils.SetOrCreateValue(iconBB, "rewardInfo", rewardInfo);
                    BlackboardUtils.SetOrCreateValue(iconBB, "rewardType", rewardType);
                }
            }

            ContextElement badgeAreaElement = null;
            if (iconElement != null)
            {
                badgeAreaElement = ContextUtils.FindElement(iconElement, "Badge Area", ContextSearchingType.ChildrenSearch);
            }

            string text = "";

            long rewardAmount = GetRewardAmount(rewardInfo, isInbox, rewardType, checkScene);

            switch (rewardType)
            {
                case RewardType.CREDIT:
                    {
                        long credit = rewardAmount;

                        if (isInbox)
                        {
                            var applyTierMultiplier = BlackboardUtils.FindVariable<bool>(rewardInfo, "applyTierMultiplier");
                            if (applyTierMultiplier != null && applyTierMultiplier.value)
                                credit = TierUtils.GetTierFractionCoin(credit, TierUtils.GetMeTier());
                            var applyLevelMultiplier = BlackboardUtils.FindVariable<bool>(rewardInfo, "applyLevelMultiplier");
                            if (applyLevelMultiplier != null && applyLevelMultiplier.value)
                                credit = LevelUtils.GetLevelMultiplierNumeratorValue(credit, "coin");
                        }

                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COIN_BONUS", credit);
                    }
                    break;
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        long credit = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COIN_BONUS", credit);

                        long multiplierNumerator = rewardInfo.GetValue<long>("multiplierNumerator");
                        double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

                        var badgeText = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_MULTIPLIER", multiplier);

                        MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge Event", badgeAreaElement.transform, null, "Badge Event");

                        iconElement.UpdateContext(true);
                        MetaContextElementUtils.SimpleSetText(badgeAreaElement, "Text", badgeText);
                    }
                    break;
                case RewardType.RP:
                    {
                        long rp = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_VIP_BONUS", rp);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        long spinCount = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_DAILY_SPIN_TEXT", spinCount);
                    }
                    break;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        if (checkScene == RewardCheckScene.DEFAULT)
                        {
                            long multiplierNumerator = rewardAmount;
                            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

                            text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_EXP_BOOSTER_TEXT", multiplier);
                        }
                        else if (checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            var textElement = ContextUtils.FindElement(iconElement, "Text Exp Multi", ContextSearchingType.ChildrenSearch);
                            long multiplierNumerator = BlackboardUtils.FindValue<long>(rewardInfo, "multiplierNumerator");
                            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);
                            string multiText = StringTableUtils.GetString(GLOBAL, "LEVEL_UP_DASH_MAIN_SCENE_BOOSTER_MULTIPLY", multiplier);
                            MetaContextElementUtils.SetText(textElement, multiText);

                            long seconds = rewardAmount;
                            text = StringTableUtils.GetString(GLOBAL, "LEVEL_UP_DASH_COMMON_REWARD_BOOSTER_TIME_TEXT", seconds);
                        }
                    }
                    break;
                case RewardType.RANDOM:
                    {
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_MYSTERY_TEXT");

                        if (extraDataBB != null)
                        {
                            var count = BlackboardUtils.GetOrCreateVariable<int>(extraDataBB, "count")?.value ?? 0;

                            if (count > 1)
                            {
                                var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                                ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                                badgeElement.UpdateContext(false);

                                badgeObj.GetComponent<Animator>().SetInteger("value", count);
                                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", count.ToString());
                            }
                        }
                    }
                    break;
                case RewardType.PURCHASE_COUPON:
                    {
                        text = "";
                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        SetCommonRewardSlotImage(rewardInfo, iconObj, rewardType, checkScene);

                        var totalBet = BlackboardUtils.FindVariable<long>(rewardInfo, "totalBet");
                        long spinCount = rewardAmount;

                        string textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_GAME_SPIN_LINE_TEXT" : "POPUP_COMMON_REWARD_RESULT_GAME_SPIN_TEXT";

                        var applyLevelMultiplier = BlackboardUtils.FindVariable<bool>(rewardInfo, isInbox ? "applyLevelMultiplier" : "rewardInfo/applyLevelMultiplier");
                        if (applyLevelMultiplier != null && applyLevelMultiplier.value)
                            totalBet.value = LevelUtils.GetLevelMultiplierNumeratorValue(totalBet.value, "spinDeal");

                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, spinCount, totalBet.value);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        SetCommonRewardSlotImage(rewardInfo, iconObj, rewardType, checkScene);

                        var betPerHand = BlackboardUtils.FindVariable<long>(rewardInfo, "betPerHand");
                        var handCount = BlackboardUtils.FindVariable<int>(rewardInfo, "handCount");
                        long spinCount = rewardAmount;

                        string textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_GAME_DEAL_LINE_TEXT" : "POPUP_COMMON_REWARD_RESULT_GAME_DEAL_TEXT";

                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, spinCount, betPerHand.value, handCount.value);
                    }
                    break;
                case RewardType.GAME_PLAY:
                    {
                        SetCommonRewardSlotImage(rewardInfo, iconObj, rewardType, checkScene);

                        long spinCount = rewardAmount;
                        long betPerTicket = BlackboardUtils.FindValue<long>(rewardInfo, "betPerTicket");
                        long ticketCount = BlackboardUtils.FindValue<int>(rewardInfo, "ticketCount");
                        long totalBet = betPerTicket * ticketCount;

                        string textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_GAME_SPIN_LINE_TEXT" : "POPUP_COMMON_REWARD_RESULT_GAME_SPIN_TEXT";

                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, spinCount, totalBet);
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        var tier = BlackboardUtils.FindValue<int>(null, "/me/tier");

                        var perCredit = BlackboardUtils.FindVariable<long>(rewardInfo, "baseCreditPerDay");
                        var perGem = BlackboardUtils.FindVariable<long>(rewardInfo, "baseGemPerDay");

                        long totalDayCount = rewardAmount;
                        long totalCoin = TierUtils.GetTierFractionCoin(perCredit.value, tier);
                        totalCoin = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(totalCoin, rewardInfo, "dailyBoost");
                        long totalGem = TierUtils.GetTierFractionCoin(perGem.value, tier);

                        string textKey;

                        if (perCredit.value > 0 && perGem.value > 0)
                        {
                            textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_DAILY_BOOST_LINE_COIN_GEM_TEXT" : "POPUP_COMMON_REWARD_RESULT_DAILY_BOOST_COIN_GEM_TEXT";
                            text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, totalCoin, totalGem, totalDayCount);
                        }
                        else if (perGem.value > 0)
                        {
                            textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_DAILY_BOOST_LINE_GEM_TEXT" : "POPUP_COMMON_REWARD_RESULT_DAILY_BOOST_GEM_TEXT";
                            text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, totalGem, totalDayCount);
                        }
                        else
                        {
                            textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_DAILY_BOOST_LINE_TEXT" : "POPUP_COMMON_REWARD_RESULT_DAILY_BOOST_TEXT";
                            text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, totalCoin, totalDayCount);
                        }

                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    {
                        long rewardTier = rewardAmount;
                        int meTier = BlackboardQueryUtils.GetMyTier();
                        int targetTier = Mathf.Max(meTier, (int)rewardTier);
                        
                        var tierGroup = TierUtils.GetTierGroup(targetTier);

                        string textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_TIER_UPGRADE_LINE_TEXT" : "POPUP_COMMON_REWARD_RESULT_TIER_UPGRADE_TEXT";

                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, textKey, targetTier);

                        if (targetTier > meTier)
                        {
                            if (iconObj.TryGetComponent<Animator>(out var iconAnim))
                                iconAnim.SetInteger("Tier", tierGroup);
                        }
                    }
                    break;
                case RewardType.SOCIAL_CREDIT:
                    {
                        long credit = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_SOCIAL_COIN_TEXT", credit);

                        var friendList = BlackboardUtils.FindValue<List<Blackboard>>(rewardInfo, "friendList");

                        if (friendList.Count > 0)
                        {
                            for (int i = 0; i < (friendList.Count > 5 ? 5 : friendList.Count); i++)
                            {
                                var profileAreaElement = ContextUtils.FindElement(iconElement, string.Format("Profile Area {0}", (i + 1)), ContextSearchingType.ChildrenSearch);
                                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Profile Picture Small", profileAreaElement.transform, null, "Profile");

                                profileAreaElement.UpdateContext(true);
                                var profileElement = ContextUtils.FindElement(profileAreaElement, "Profile", ContextSearchingType.ChildrenSearch);

                                profileElement.GetComponent<Animator>().SetInteger("Tier", TierUtils.GetTierGroup(friendList[i].GetValue<int>("tier")));

                                var imageElement = ContextUtils.FindElement(profileElement, "Image", ContextSearchingType.ChildrenSearch);
                                MetaContextElementUtils.SetWebImage(imageElement, friendList[i].GetValue<string>("profileUrl"), CacheType.FileCache, true, null);
                            }
                        }
                        else
                        {
                            var imageFriendsElement = ContextUtils.FindElement(iconElement, "Image Friends", ContextSearchingType.ChildrenSearch);
                            MetaContextElementUtils.SetActive(imageFriendsElement, true);
                        }
                    }
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                    {
                        if (isInbox)
                            text = CommonRewardResultTicketedBonusTicketSetter(rewardInfo, iconObj, checkScene);
                    }
                    break;
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        if (isInbox)
                            text = CommonRewardResultTicketedBonusTicketSetter(rewardInfo, iconObj, checkScene);
                        else
                            text = CommonRewardResultTicketedBonusTicketSetter(rewardInfo.GetValue<Blackboard>("rewardInfo"), iconObj, checkScene);
                    }
                    break;
                case RewardType.CLUB_CREDIT:
                    {
                        long credit = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_CLUB_SOCIAL_COINT_TEXT", credit);

                        var clubMemberList = BlackboardUtils.FindValue<List<Blackboard>>(rewardInfo, "clubMemberProfileList");

                        if (clubMemberList.Count > 0)
                        {
                            for (int i = 0; i < (clubMemberList.Count > 5 ? 5 : clubMemberList.Count); i++)
                            {
                                var profileAreaElement = ContextUtils.FindElement(iconElement, "Profile Area " + (i + 1), ContextSearchingType.ChildrenSearch);
                                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Profile Picture Small", profileAreaElement.transform, null, "Profile");

                                profileAreaElement.UpdateContext(true);
                                var profileElement = ContextUtils.FindElement(profileAreaElement, "Profile", ContextSearchingType.ChildrenSearch);

                                profileElement.GetComponent<Animator>().SetInteger("Tier", TierUtils.GetTierGroup(clubMemberList[i].GetValue<int>("tier")));

                                var imageElement = ContextUtils.FindElement(profileElement, "Image", ContextSearchingType.ChildrenSearch);
                                MetaContextElementUtils.SetWebImage(imageElement, clubMemberList[i].GetValue<string>("profileUrl"), CacheType.FileCache, true, null);
                            }
                        }
                        else
                        {
                            var imageFriendsElement = ContextUtils.FindElement(iconElement, "Image Friends", ContextSearchingType.ChildrenSearch);
                            MetaContextElementUtils.SetActive(imageFriendsElement, true);
                        }
                    }
                    break;
                case RewardType.SCRATCHER:
                    {
                        if(checkScene == RewardCheckScene.DEFAULT)
                        {
                            var rewardName = BlackboardUtils.FindVariable<string>(rewardInfo, "rewardName");
                            if (rewardName == null || string.IsNullOrEmpty(rewardName.value))
                            {
                                var scratcherName = BlackboardUtils.FindVariable<ScratcherName>(rewardInfo, "scratcherName");
                                text = BlackboardQueryUtils.GetScratcherName(scratcherName.value);
                            }
                            else
                            {
                                text = rewardName.value;
                            }

                            var webImage = iconObj.GetComponent<ContextImage>();

                            var rewardImageUrl = BlackboardUtils.FindVariable<string>(rewardInfo, "rewardImageUrl");
                            MetaContextElementUtils.SetWebImage(
                                webImage,
                                rewardImageUrl.value,
                                CacheType.FileCache,
                                false,
                                () => { webImage.GetComponent<Animator>().SetBool("Active", true); }
                            );

                            long maxPrize = rewardInfo.GetVariable<long>("maxWinCredit")?.value ?? 0L;
                            maxPrize = LevelUtils.GetLevelMultiplierNumeratorValue(maxPrize, "scratcher");

                            int version = rewardInfo.GetVariable<int>("version")?.value ?? 0;
                            var topPrizeArea = ContextUtils.FindElement(iconElement, "Image/Top Prize Area", ContextSearchingType.FullNameSearch);

                            if (version == 0 || maxPrize == 0L)
                            {
                                topPrizeArea.gameObject.SetActive(false);
                            }
                            else
                            {
                                maxPrize = NumberUtils.GetMultiplierNumeratorValue(maxPrize, rewardInfo.GetValue<long>("tierMultiplierNumerator"));

                                topPrizeArea.gameObject.SetActive(true);

                                var simpleImagePrizeTextElement = ContextUtils.FindElement(topPrizeArea, "Text", ContextSearchingType.ChildrenSearch);
                                MetaContextElementUtils.SetTextGlobal(simpleImagePrizeTextElement, "COLLECTING_GAME_SIMPLE_IMAGE_PRIZE_TEXT", maxPrize);
                            }
                        }
                        else if(checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            var rewardName = BlackboardUtils.FindVariable<string>(rewardInfo, "rewardName");
                            if (rewardName == null || string.IsNullOrEmpty(rewardName.value))
                            {
                                var scratcherName = BlackboardUtils.FindVariable<ScratcherName>(rewardInfo, "scratcherName");
                                text = BlackboardQueryUtils.GetScratcherName(scratcherName.value);
                            }
                            else
                            {
                                text = rewardName.value;
                            }

                            long count = rewardAmount;
                            text += " " + StringTableUtils.GetString(GLOBAL, "TEXT_NUMERABLE", count);
                        }
                    }
                    break;
                case RewardType.SCRATCHER_FOR_INBOX:
                    {
                        if (checkScene == RewardCheckScene.DEFAULT)
                        {
                            if (isInbox)
                            {
                                ContextElement webImageElement = ContextUtils.FindElement(iconElement, "Image", ContextSearchingType.ChildrenSearch);

                                var rewardImageUrl = BlackboardUtils.FindVariable<string>(rewardInfo, "rewardImageUrl");
                                long count = rewardAmount;

                                var rewardName = BlackboardUtils.FindVariable<string>(rewardInfo, "rewardName");
                                if (rewardName == null || string.IsNullOrEmpty(rewardName.value))
                                {
                                    var scratcherName = BlackboardUtils.FindVariable<ScratcherName>(rewardInfo, "scratcherName");

                                    text = BlackboardQueryUtils.GetScratcherName(scratcherName.value);
                                    if (count > 1L)
                                        text = count.ToString() + " " + text + "s";
                                }
                                else
                                {
                                    text = rewardName.value;
                                }

                                MetaContextElementUtils.SetWebImage(
                                    webImageElement.GetComponent<ContextImage>(),
                                    rewardImageUrl.value,
                                    CacheType.FileCache,
                                    false,
                                    () =>
                                    {
                                        if (webImageElement != null)
                                            webImageElement.GetComponent<Animator>().SetBool("Active", true);
                                    }
                                );

                                var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                                ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                                badgeElement.UpdateContext(false);

                                badgeObj.GetComponent<Animator>().SetInteger("value", (int)count);
                                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", count.ToString());

                                long maxPrize = rewardInfo.GetVariable<long>("maxWinCredit")?.value ?? 0L;
                                maxPrize = LevelUtils.GetLevelMultiplierNumeratorValue(maxPrize, "scratcher");

                                int version = rewardInfo.GetVariable<int>("version")?.value ?? 0;
                                var topPrizeArea = ContextUtils.FindElement(iconElement, "Image/Top Prize Area", ContextSearchingType.FullNameSearch);

                                if (version == 0 || maxPrize == 0L)
                                {
                                    topPrizeArea.gameObject.SetActive(false);
                                }
                                else
                                {
                                    if (rewardInfo.GetValue<bool>("applyTierMultiplier"))
                                        maxPrize = TierUtils.GetTierFractionCoin(maxPrize, TierUtils.GetMeTier());

                                    topPrizeArea.gameObject.SetActive(true);
                                    var simpleImagePrizeTextElement = ContextUtils.FindElement(topPrizeArea, "Text", ContextSearchingType.ChildrenSearch);
                                    MetaContextElementUtils.SetTextGlobal(simpleImagePrizeTextElement, "COLLECTING_GAME_SIMPLE_IMAGE_PRIZE_TEXT", maxPrize);
                                }
                            }
                        }
                        else if (checkScene == RewardCheckScene.LEVEL_UP_DASH)
                        {
                            var rewardName = BlackboardUtils.FindVariable<string>(rewardInfo, "rewardName");
                            if (rewardName == null || string.IsNullOrEmpty(rewardName.value))
                            {
                                var scratcherName = BlackboardUtils.FindVariable<ScratcherName>(rewardInfo, "scratcherName");
                                text = BlackboardQueryUtils.GetScratcherName(scratcherName.value);
                            }
                            else
                            {
                                text = rewardName.value;
                            }

                            long count = rewardAmount;
                            text += " " + StringTableUtils.GetString(GLOBAL, "TEXT_NUMERABLE", count);
                        }
                    }
                    break;
                case RewardType.GEM:
                    {
                        long gem = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_GEM_BONUS", gem);
                    }
                    break;
                case RewardType.COLLECTING_GAME_PACK:
                    {
                        var chestId = BlackboardUtils.FindVariable<int>(rewardInfo, "rewardInfo/packId");
                        long count = rewardAmount;
                        text = BlackboardQueryUtils.GetChestNameForReward(chestId.value);

                        if (count > 1L)
                            text = count.ToString() + " " + text + "s";

                        ContextElement chestImageElement = ContextUtils.FindElement(iconElement, "Image", ContextSearchingType.ChildrenSearch);
                        Sprite chestImage = iconObj.GetComponent<Scratcher.CollectingGameChestData>().chestAssets.assets[BlackboardQueryUtils.GetChestIndex(chestId.value)];
                        MetaContextElementUtils.SetSprite(chestImageElement, chestImage);

                        if (count > 1L)
                        {
                            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                            ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                            badgeElement.UpdateContext(false);
                            badgeObj.GetComponent<Animator>().SetInteger("value", (int)count);

                            if (count > 99L)
                                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", "99+");
                            else
                                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", count.ToString());
                        }

                    }
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                    {
                        long energy = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_BONUS_REWARD_ENERGY_TEXT", energy);
                    }
                    break;
                case RewardType.CLUB_ARENA_ENERGY:
                    {
                        long energy = rewardAmount;
                        text = StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_POPUP_BONUS_REWARD_ENERGY_TEXT", energy);
                    }
                    break;
                case RewardType.SPIN_DEAL:
                    {
                        SetCommonRewardSlotImage(rewardInfo, iconObj, rewardType, checkScene);

                        var totalBet = BlackboardUtils.FindVariable<long>(rewardInfo, "totalBet");
                        long spinCount = rewardAmount;

                        string textKey = isMultiline ? "POPUP_COMMON_REWARD_RESULT_GAME_SPIN_LINE_TEXT" : "POPUP_COMMON_REWARD_RESULT_GAME_SPIN_TEXT";

                        text = StringTableUtils.GetString(GLOBAL, textKey, spinCount, LevelUtils.GetLevelMultiplierNumeratorValue(totalBet.value, "spinDeal"));
                    }
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    {
                        long count = rewardAmount;
                        if (count > 1L)
                        {
                            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                            ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                            badgeElement.UpdateContext(false);
                            badgeObj.GetComponent<Animator>().SetInteger("value", (int)count);

                            if (count > 999L)
                                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", "999+");
                            else
                                MetaContextElementUtils.SimpleSetText(badgeElement, "Text", count.ToString());
                        }

                        text = StringTableUtils.GetString(GLOBAL, "HIDDEN_OBJECTS_POPUP_BONUS_REWARD_FINDER_TEXT", count);
                    }
                    break;
                case RewardType.HOG_DEAL:
                    {
                        long count = rewardAmount;

                        var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                        ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                        badgeElement.UpdateContext(false);

                        badgeObj.GetComponent<Animator>().SetInteger("value", (int)count);
                        MetaContextElementUtils.SimpleSetText(badgeElement, "Text", count.ToString());
                    }
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    {
                        long openDays = rewardAmount;
                        text = StringTableUtils.GetString(GLOBAL, "EPIC_PASS_REWARD_VIP_LOUNGE_TICKET_TEXT", openDays);
                    }
                    break;
                case RewardType.BOSS_RAIDERS_DEAL:
                    {
                        long dealCount = rewardAmount;

                        var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                        ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                        badgeElement.UpdateContext(false);

                        badgeObj.GetComponent<Animator>().SetInteger("value", (int)dealCount);
                        MetaContextElementUtils.SimpleSetText(badgeElement, "Text", dealCount.ToString());
                    }
                    break;
                case RewardType.DEPOT:
                    {
                        long addedCount = rewardAmount;
                        var depotType = rewardInfo.GetValue<DepotType>("depotType");

                        long depotAnimatorMaxValue = System.Math.Min(100L, addedCount);
                        string depotBadgeTextString = System.Math.Min(99L, addedCount).ToString();
                        if (addedCount > 99L) depotBadgeTextString += "+";

                        var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                        ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                        badgeElement.UpdateContext(false);

                        badgeObj.GetComponent<Animator>().SetInteger("value", (int)depotAnimatorMaxValue);
                        MetaContextElementUtils.SimpleSetText(badgeElement, "Text", depotBadgeTextString);

                        text = StringTableUtils.GetString(GLOBAL, $"VEGAS_DREAMS_COMMON_REWARD_{depotType}", addedCount);
                    }
                    break;
                case RewardType.WILD_PUZZLE:
                    {
                        long count = rewardAmount;

                        long badgeAnimatorMaxValue = System.Math.Min(100L, count);
                        string badgeTextString = System.Math.Min(99L, count).ToString();
                        if (count > 99L) badgeTextString += "+";

                        var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, null, "Badge");
                        ContextElement badgeElement = badgeObj.GetComponent<ContextElement>();
                        badgeElement.UpdateContext(false);

                        badgeObj.GetComponent<Animator>().SetInteger("value", (int)badgeAnimatorMaxValue);
                        MetaContextElementUtils.SimpleSetText(badgeElement, "Text", badgeTextString);

                        text = StringTableUtils.GetString(GLOBAL, $"VEGAS_DREAMS_COMMON_REWARD_WILD_PUZZLE", count);
                    }
                    break;
                case RewardType.VIP_LOUNGE_POINT:
                    {
                        // todo 
                    }
                    break;
                    // case RewardType.vip_deal
                default:
                    text = "";
                    break;
            }

            if (rewardTextElement != null)
                MetaContextElementUtils.SetText(rewardTextElement, text);
        }

        public static string CommonRewardResultTicketedBonusTicketSetter(Blackboard rewardInfo, GameObject iconObj, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            SetCommonRewardSlotImage(rewardInfo, iconObj, RewardType.TICKETED_BONUS_TICKET, checkScene);

            var gameID = BlackboardUtils.FindVariable<int>(rewardInfo, "gameId");
            var baseBet = BlackboardUtils.FindVariable<long>(rewardInfo, "baseBet")?.value ?? 0L;
            var extraBet = BlackboardUtils.FindVariable<long>(rewardInfo, "extraBet")?.value ?? 0L;
            long rawBaseBet = BlackboardQueryUtils.GetRawBaseBet(gameID.value);

            long multipliedBet = MultiplierUtils.GetRewardMultiplierValue(baseBet, rewardInfo, "ticketedBonus");

            long bet = IAMUtils.CalculateBet(baseBet, multipliedBet, extraBet, rawBaseBet);

            string text = "";

            var tag = BlackboardUtils.FindVariable<BonusTag>(rewardInfo, "tag");
            if (tag != null)
            {
                if (tag.value == BonusTag.BUY_A_BONUS)
                {
                    text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_TICKETED_BONUS_TICKET_BUY_A_BONUS_TEXT", bet);
                }
                else if (tag.value == BonusTag.SUPER_BONUS)
                {
                    text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_TICKETED_BONUS_TICKET_SUPER_BONUS_TEXT", bet);
                }
                else if (tag.value == BonusTag.INSTANT_BONUS)
                {
                    text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_COMMON_REWARD_RESULT_TICKETED_BONUS_TICKET_INSTANT_BONUS_TEXT", bet);
                }
            }

            if(checkScene == RewardCheckScene.LEVEL_UP_DASH)
            {
                if (tag != null)
                {
                    var iconElement = iconObj.GetComponent<ContextElement>();
                    var tagArea = ContextUtils.FindElement(iconElement, "Ticketed Bonus Area", ContextSearchingType.ChildrenSearch);

                    string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                    Transform parent = tagArea.transform;
                    if (tag.value == BonusTag.BUY_A_BONUS)
                    {
                        string asset = "Icon Buy A Bonus";
                        MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    }
                    else if (tag.value == BonusTag.SUPER_BONUS)
                    {
                        string asset = "Icon Super Bonus";
                        MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    }
                    else if (tag.value == BonusTag.INSTANT_BONUS)
                    {
                        string asset = "Icon Instant Bonus";
                        MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    }
                }
            }

            return text;
        }

        public static IEnumerator RewardResultProcessCoroutine(GameObject caller, RewardType rewardType, bool applyEarnValues)
        {
            Blackboard callerBB = caller.GetComponent<Blackboard>();
            MonoBehaviour agent = caller.GetComponent<MonoBehaviour>();

            bool isSkip = false;

            switch (rewardType)
            {
                case RewardType.DAILY_DELIVERY:
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                case RewardType.DAILY_BOOST:
                case RewardType.RANDOM:
                case RewardType.MEGA_WHEEL_SPIN:
                case RewardType.SCRATCHER:
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    // Open Reward Result Popup
                    {
                        yield return agent.StartCoroutine(OpenRewardResultPopupCoroutine(caller, rewardType, applyEarnValues));
                    }
                    break;
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    // Enter Game
                    {
                        Blackboard response = callerBB.GetValue<Blackboard>("response");
                        int gameID = response.GetVariable<int>("gameId")?.value ?? -1;
                        int bonusTicketID = response.GetVariable<int>("ticketId")?.value ?? -1;
                        string fromType = BlackboardUtils.GetOrCreateVariable<string>(callerBB, "_fromType").value;

                        BlackboardQueryUtils.SetEnterGameInfo(
                            gameID,
                            "EnterGame",
                            fromType,
                            null,
                            0,
                            null,
                            false,
                            bonusTicketID,
                            null);

                        EventSender.SendGlobalEvent("OnEnterGame");
                    }
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    // VIP Lounge Welcome Popup
                    {
                        if (BlackboardQueryUtils.IsVipLoungeEnabled() && VipLounge.VipLounge.Utils.PrevBadgeCount < VipLounge.VipLounge.Defines.MAX_BADGE_COUNT)
                        {
                            var eventData = new EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, caller);
                            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                        }
                        else
                            isSkip = true;
                    }
                    break;
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                case RewardType.RP: 
                case RewardType.GEM:
                    GSManager.Instance.GetHandler("UI_Coin_Add").Play();
                    isSkip = true;
                    break;
                default:
                    isSkip = true;
                    break;
            }

            yield return new WaitUntilTrigger(
                new WaitUntilConditionTrigger(() => isSkip),
                new EventTrigger(caller, EventSender.ON_CALLEE_CALLBACK),
                new EventTrigger(caller, MetaEventDefine.ON_META_UI_EVENT, ON_FINISH_REWARD),
                new EventTrigger(caller, ON_FINISH_REWARD),
                new EventTrigger(caller, VipLounge.VipLounge.Events.ON_CLOSE_WELCOME_POPUP));

            yield return agent.StartCoroutine(CheckTierUpCoroutine(caller));
        }

        public static bool CheckShowDailyDeliveryPopup(Blackboard dailyDeliveryInfo)
        {
            if (dailyDeliveryInfo == null) return false;

            // Get Local Timestamp
            if (dailyDeliveryInfo.GetValue<int>("nextClaimRewardIdx") > 1
             && !dailyDeliveryInfo.GetValue<bool>("isLastReward")
             && dailyDeliveryInfo.GetValue<long>("nextClaimTimestamp") > MetaSystem.GetLocalTimeStamp())
            {
                return true;
            }

            return false;
        }

        public static IEnumerator CheckTierUpCoroutine(GameObject caller)
        {
            MonoBehaviour agent = caller.GetComponent<MonoBehaviour>();

            int tier = BlackboardUtils.FindValue<int>("/me/tier");
            long accRp = BlackboardUtils.FindValue<int>("/me/tier");
            int targetTier = TierUtils.GetTier(accRp);

            if (targetTier > tier)
            {
                GameObject tierUpPopupObj = null;

                yield return agent.StartCoroutine(MetaPopupUtils.SimpleOpenPopupCoroutine(agent, "Popup Tier Up Scene",
                    (SceneLoadOperation scene) => tierUpPopupObj = scene.GetScene()));

                MetaPopupUtils.OpenPopup(tierUpPopupObj);

                yield return new WaitUntilTrigger(new EventTrigger(agent, "RefreshTier"));
            }
        }

        public static IEnumerator OpenDailyDeliveryPopupCoroutine(GameObject caller, Blackboard deliveryInfoBB)
        {
            Blackboard callerBB = caller.GetComponent<Blackboard>();
            MonoBehaviour agent = caller.GetComponent<MonoBehaviour>();

            GameObject loadingPopupObj = null;
            yield return agent.StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject obj) => loadingPopupObj = obj));

            GameObject popupObj = null;
            yield return agent.StartCoroutine(MetaPopupUtils.SimpleOpenPopupCoroutine(agent, "Popup Daily Delivery New Scene",
                (SceneLoadOperation scene) => popupObj = scene.GetScene()));

            Blackboard popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "dailyDeliveryBB", deliveryInfoBB);

            MetaPopupUtils.OpenPopup(popupObj);
            MetaPopupUtils.ClosePopup(loadingPopupObj);

            yield return new WaitUntil(() => popupObj == null);
        }

        private static IEnumerator OpenRewardResultPopupCoroutine(GameObject caller, RewardType rewardType, bool applyEarnValues)
        {
            Blackboard callerBB = caller.GetComponent<Blackboard>();
            MonoBehaviour agent = caller.GetComponent<MonoBehaviour>();
            Blackboard response = callerBB.GetValue<Blackboard>("response");

            if (!CheckResponse(caller, rewardType)) yield break;

            bool useLoading = false;
            if (rewardType == RewardType.SCRATCHER) useLoading = true;

            // Open Loading
            GameObject loadingPopupObj = null;
            if (useLoading)
                yield return agent.StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                    (GameObject obj) => loadingPopupObj = obj));

            string assetName = GetRewardPopupAssetName(rewardType, response);

            // Open Result Popup
            GameObject popupObj = null;
            yield return agent.StartCoroutine(MetaPopupUtils.SimpleOpenPopupCoroutine(agent, assetName,
                (SceneLoadOperation scene) => popupObj = scene.GetScene()));

            // Close Loading
            if (useLoading)
                MetaPopupUtils.ClosePopup(loadingPopupObj);

            SetRewardResultPopupInfo(caller, rewardType, popupObj, applyEarnValues);

            // Notify result popup open
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                new EventData<GameObject>(MetaEventDefine.ON_MAKE_RESULT_POPUP, popupObj));

            if (IsRewardTypeCommonRewarPopup(rewardType))
                EventSender.SendGlobalEvent(MetaEventDefine.ON_OPEN_COMMON_REWARD_POPUP);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        private static bool IsRewardTypeCommonRewarPopup(RewardType rewardType)
        {
            return GetRewardPopupAssetName(rewardType) == "Popup Common Reward Scene";
        }

        private static bool CheckResponse(GameObject caller, RewardType rewardType)
        {
            Blackboard callerBB = caller.GetComponent<Blackboard>();
            Blackboard response = callerBB.GetValue<Blackboard>("response");

            if (rewardType == RewardType.DAILY_DELIVERY)
            {
                Blackboard dailyDeliveryInfo = response.GetVariable<Blackboard>("dailyDeliveryInfo")?.value;
                if (dailyDeliveryInfo != null)
                {
                    int nextClaimRewardIdx = dailyDeliveryInfo.GetValue<int>("nextClaimRewardIdx");
                    bool isLastReward = dailyDeliveryInfo.GetValue<bool>("isLastReward");
                    long currentTimestamp = MetaSystem.GetLocalTimeStamp();
                    long nextClaimTimestamp = dailyDeliveryInfo.GetValue<long>("nextClaimTimestamp");

                    // Ignore First, Expire delivery
                    bool isClaimNext = (nextClaimRewardIdx > 1) && !isLastReward && (nextClaimTimestamp > currentTimestamp);
                    return isClaimNext;
                }
            }

            return true;
        }

        private static void SetRewardResultPopupInfo(GameObject caller, RewardType rewardType, GameObject popupObj, bool applyEarnValues)
        {
            Blackboard callerBB = caller.GetComponent<Blackboard>();
            Blackboard popupBB = popupObj.GetComponent<Blackboard>();
            Blackboard response = callerBB.GetValue<Blackboard>("response");

            popupBB.AddVariable("caller", caller);
            popupBB.AddVariable("applyEarnValues", applyEarnValues);

            switch (rewardType)
            {
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        string expRewardTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_INBOX_EXP_BOOST_REWARD_TITLE");
                        popupBB.AddVariable("title", expRewardTitle);

                        var rewardList = new List<Blackboard>() { response };
                        callerBB.AddVariable("rewardList", rewardList);
                        popupBB.AddVariable("rewardList", rewardList);
                    }
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    {
                        string finderRewardTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_INBOX_FINDER_REWARD_TITLE");
                        popupBB.AddVariable("title", finderRewardTitle);

                        var rewardList = new List<Blackboard>() { response };
                        callerBB.AddVariable("rewardList", rewardList);
                        popupBB.AddVariable("rewardList", rewardList);
                    }
                    break;
                case RewardType.SCRATCHER:
                    {
                        callerBB.AddVariable("_scratcherRewardResult", response);
                        popupBB.AddVariable("_scratcherRewardResult", response);

                        popupBB.AddVariable("_isAuto", false);
                        popupBB.AddVariable("_isReward", true);
                        popupBB.AddVariable("_remainingCount", 0);
                        popupBB.AddVariable("_scratcherNameList", new List<string>());
                        popupBB.AddVariable("_prizeList", new List<long>());
                        popupBB.AddVariable("_winTypeList", new List<int>());
                    }
                    break;
                case RewardType.RANDOM:
                    {
                        if (caller.GetComponent<InboxCellController>() is InboxCellController cell)
                        {
                            var rewardList = new List<Blackboard>() { cell.responseInfo };
                            callerBB.AddVariable("rewardList", rewardList);

                            string title = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_MYSTERY_REWARD_TITLE");
                            popupBB.AddVariable("title", title);
                            popupBB.AddVariable("rewardList", rewardList);
                            popupBB.AddVariable("applyEarnValues", false);
                        }
                    }
                    break;
                case RewardType.DAILY_DELIVERY:
                    {
                        Blackboard dailyDeliveryInfo = response.GetVariable<Blackboard>("dailyDeliveryInfo")?.value;
                        popupBB.AddVariable("_dailyDeliveryBB", dailyDeliveryInfo);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                case RewardType.MEGA_WHEEL_SPIN:
                case RewardType.DAILY_BOOST:
                    {
                        popupBB.AddVariable("response", response);
                    }
                    break;
            }
        }

        private static string GetRewardPopupAssetName(RewardType rewardType, Blackboard scratcherRewardResult = null)
        {
            switch (rewardType)
            {
                case RewardType.RANDOM:
                    return "Popup Mystery Reward Scene";
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    return "Popup Reward Daily Spin Scene";
                case RewardType.DAILY_BOOST:
                    return "Popup Reward Daily Boost Scene";
                case RewardType.MEGA_WHEEL_SPIN:
                    return "Popup Reward Mega Spin Scene";
                case RewardType.SCRATCHER:
                    if (scratcherRewardResult != null)
                    {
                        ScratcherName scratcherName = scratcherRewardResult.GetValue<ScratcherName>("scratcherName");
                        return PopupScratcherUtils.GetScratcherSceneName(scratcherName);
                    }
                    else
                    {
                        Debug.LogWarning("GetRewardPopupAssetName Failure. scratcherRewardResult is null.");
                        return string.Empty;
                    }
                //case RewardType.HIDDEN_UNIVERSE_FINDER:
                //case RewardType.EXP_MULTIPLY:
                default:
                    return "Popup Common Reward Scene";
            }
        }
    }
}
