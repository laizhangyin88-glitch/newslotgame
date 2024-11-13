using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bingo")]

public class GetDailyBingoRewardInfoBB : ActionTask<ContextElement>
{
    public BBParameter<Blackboard> rewardInfoBB;
    public BBParameter<string> textColorCode;
    public BBParameter<int> tier;

    public BBParameter<string> saveAsText;
    public BBParameter<GameObject> saveAsIconObject;
    public BBParameter<GameObject> saveAsThumbnailObject;

    protected override string info
    {
        get { return string.Format("Get Daily Bingo Reward Info BB"); }
    }

    protected override void OnExecute()
    {
        // Icon Object.
        var rewardType = BlackboardUtils.FindVariable<RewardType>(rewardInfoBB.value, "type");
        bool isError = false;

        Transform iconArea = GameObject.Find("Icon Area").transform;

        if(saveAsIconObject.value != null)
            saveAsIconObject.value.SetActive(false);
        saveAsIconObject.value = null;

        if(saveAsThumbnailObject.value != null)
            GameObject.Destroy(saveAsThumbnailObject.value);
        saveAsThumbnailObject.value = null;

        switch(rewardType.value)
        {
            case RewardType.CREDIT:
            case RewardType.CREDIT_WITH_MULTIPLIER:
                {
                    saveAsIconObject.value = iconArea.Find("Icon Coin").gameObject;

                    // var credit = BlackboardUtils.FindVariable<long>(rewardInfoBB.value, "credit");
                    var credit = BlackboardQueryUtils.GetRewardCoin(rewardInfoBB.value, true);

                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_COIN_TEXT", out isError, textColorCode.value, credit);
                }
                break;
            case RewardType.RP:
                {
                    saveAsIconObject.value = iconArea.Find("Icon Vip Point").gameObject;

                    var rp = BlackboardUtils.FindVariable<long>(rewardInfoBB.value, "rp");

                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_RP_TEXT", out isError, textColorCode.value, rp.value);
                }
                break;
            case RewardType.DAILY_BONUS_WHEEL_SPIN:
                {
                    saveAsIconObject.value = iconArea.Find("Icon Wheel").gameObject;

                    var spinCount = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "spinCount");

                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_DAILY_WHEEL_TEXT", out isError, textColorCode.value, spinCount.value);
                }
                break;
            // case RewardType.PURCHASE_COUPON:
            //     break;
            case RewardType.GAME_SPIN:
                {
                    var gameID = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "gameId");
                    var spinCount = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "spinCount");
                    var bet = BlackboardUtils.FindVariable<long>(rewardInfoBB.value, "bet");

                    saveAsThumbnailObject.value = MetaIconUtils.MakeSlotThumbnailIconObject(gameID.value, iconArea, null);

                    // Debug.LogError(string.Format("{0}, {1}, {2}", gameID.value, betIndex.value, betCoin));

                    // <color=#{0}>{1:Plural; spin; spins} with <style=coin>{2:SimpleNumber} bet
                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_GAME_SPIN_TEXT", out isError, textColorCode.value, spinCount.value, bet.value);
                }
                break;
            case RewardType.GAME_DEAL:
                {
                    var gameID = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "gameId");
                    var spinCount = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "count");
                    var betPerHand = BlackboardUtils.FindVariable<long>(rewardInfoBB.value, "betPerHand");
                    var handCount = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "handCount");

                    saveAsThumbnailObject.value = MetaIconUtils.MakeSlotThumbnailIconObject(gameID.value, iconArea, null);

                    // Debug.LogError(string.Format("{0}, {1}, {2}", gameID.value, betIndex.value, betCoin));

                    // <color=#{0}>{1:Plural; spin; spins} with <style=coin>{2:SimpleNumber} bet
                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_GAME_DEAL_TEXT", out isError, textColorCode.value, spinCount.value, betPerHand.value, handCount.value);
                }
                break;
            case RewardType.GAME_PLAY:
                {
                    var gameID = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "gameId");
                    var playCount = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "count");
                    long betPerTicket = BlackboardUtils.FindValue<long>(rewardInfoBB.value, "betPerTicket");
                    long ticketCount = BlackboardUtils.FindValue<int>(rewardInfoBB.value, "ticketCount");
                    long totalBet = betPerTicket * ticketCount;

                    saveAsThumbnailObject.value = MetaIconUtils.MakeSlotThumbnailIconObject(gameID.value, iconArea, null);

                    // Debug.LogError(string.Format("{0}, {1}, {2}", gameID.value, betIndex.value, betCoin));

                    // <color=#{0}>{1:Plural; spin; spins} with <style=coin>{2:SimpleNumber} bet
                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_GAME_SPIN_TEXT", out isError, textColorCode.value, playCount.value, totalBet);
                }
                break;
            case RewardType.EXP_MULTIPLY:
            case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                    saveAsIconObject.value = iconArea.Find("Icon Exp Boost").gameObject;

                    long multiplierNumerator = rewardInfoBB.value.GetValue<long>("multiplierNumerator");
                    double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_EXP_BOOST_TEXT", out isError, textColorCode.value, multiplier);
                }
                break;
            case RewardType.DAILY_BOOST:
                {
                    saveAsIconObject.value = iconArea.Find("Icon Daily Boost").gameObject;

                    var baseCoinPerDay = BlackboardUtils.FindVariable<long>(rewardInfoBB.value, "baseCreditPerDay");
                    var dayCount = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "totalDayCount");

                    // <color=#{0}>{1:CommaNumber} for {2:Plural; day; days}
                    long totalCoin = TierUtils.GetTierFractionCoin(baseCoinPerDay.value, tier.value);
                    totalCoin = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(totalCoin, rewardInfoBB.value, "dailyBoost");
                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_DAILY_BOOST_TEXT", out isError, textColorCode.value, totalCoin, dayCount.value);
                }
                break;
            case RewardType.TIER_UPGRADE:
                {
                    saveAsIconObject.value = iconArea.Find("Icon Tier").gameObject;

                    var targetTier = BlackboardUtils.FindVariable<int>(rewardInfoBB.value, "targetTier");
                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_TIER_UP_TEXT", out isError, textColorCode.value, targetTier.value);

                    Blackboard bb = saveAsIconObject.value.GetComponent<Blackboard>();
                    if(bb != null)
                    {
                        BlackboardUtils.SetOrCreateValue<int>(bb, "tierGroup", TierUtils.GetTierGroup(targetTier.value));
                    }
                }
                break;
            case RewardType.GEM:
                {
                    saveAsIconObject.value = iconArea.Find("Icon Gem").gameObject;

                    var gem = BlackboardUtils.FindVariable<long>(rewardInfoBB.value, "gem");

                    saveAsText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_GEM_TEXT", out isError, textColorCode.value, gem.value);
                }
                break;
            default:
                saveAsText.value = "";
                saveAsIconObject.value = null;
                break;
        }

        if(saveAsIconObject.value != null)
            saveAsIconObject.value.SetActive(true);

        EndAction();
    }
}

}
