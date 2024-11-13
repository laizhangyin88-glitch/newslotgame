using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using System;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Challenge")]

public class GetChallengeMissionCompleteBB : ActionTask<Transform>
{
    public BBParameter<Blackboard> completePollData;

    public BBParameter<string> title;

    public BBParameter<GameObject> missionIconObject;
    public BBParameter<bool> useSlotThumbnail;
    public BBParameter<string> iconText;

    public BBParameter<float> missionProgress;
    public BBParameter<string> missionProgressText;

    public BBParameter<RewardType> rewardType;
    public BBParameter<string> rewardText;

    public BBParameter<string> challengeProgressText;

    public BBParameter<bool> isClaimEnabled;
    public BBParameter<int> challengeTypeIndex;

    private Blackboard rewardResult;
    private Blackboard missionInfo;
    private Blackboard simpleChallengeInfo;

    public BBParameter<int> gameID;

    protected override string info
    {
        get { return "Get Challenge Mission Complete Info BB"; }
    }

    protected override void OnExecute()
    {
        gameID.value = -1;
        useSlotThumbnail.value = false;
        isClaimEnabled.value = false;
        challengeTypeIndex.value = 0;
        iconText.value = "";

        var result = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "rewardResult");
        if(result != null)
        {
            rewardResult = result.value;
        }
        // rewardResult = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "rewardResult").value;
        missionInfo = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "mission").value;
        simpleChallengeInfo = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "challenge").value;

        UpdateMission();
        LoadMissionIcon();
        UpdateMissionProgressText();
        UpdateRewardInfo();
        UpdateChallengeInfo();
        // "A_PER_B"

        EndAction();
    }

    public void UpdateMission()
    {
        ChallengeMissionType missionType = BlackboardUtils.FindVariable<ChallengeMissionType>(missionInfo, "missionType").value;


        switch(missionType)
        {
            case ChallengeMissionType.WIN_ANY:
            case ChallengeMissionType.WIN_TARGETED:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_WIN_COUNT");
                break;
            case ChallengeMissionType.SPIN_ANY:
            case ChallengeMissionType.SPIN_TARGETED:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_SPIN_COUNT");
                break;
            case ChallengeMissionType.WIN_BIG_WIN_ANY:
            case ChallengeMissionType.WIN_BIG_WIN_TARGETED:
                {
                    string winType = BlackboardUtils.FindVariable<string>(missionInfo, "winType").value;
                    string key = string.Format("POPUP_CHALLENGE_MISSION_{0}_WIN", winType);

                    title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key);
                }
                break;
            case ChallengeMissionType.ENTER_FREE_SPIN_ANY:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME");
                break;
            case ChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME");
                break;
            // case ChallengeMissionType.SPIN_WITH_MAX_BET_ANY:
            // case ChallengeMissionType.SPIN_WITH_MAX_BET_TARGETED:
            //     title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_MAX_BET_SPIN");
            //     break;
            case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
            case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_SPIN_WIN");
                break;
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_WIN");
                break;
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                {
                    string winType = BlackboardUtils.FindVariable<string>(missionInfo, "winType").value;
                    string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_{0}_WIN", winType);

                    title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key);
                }
                break;
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_FREE_GAME_WIN");
                break;
            case ChallengeMissionType.COLLECT_TIMEBONUS:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_COLLECT_TIME_BONUS");
                break;
            case ChallengeMissionType.COLLECT_LUCKY_SPINS:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_LUCKY_SPIN");
                break;
            case ChallengeMissionType.PURCHASE_ANY:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PURCHASE_COUNT");
                break;
            case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PURCHASE_PAY");
                break;
            case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PURCHASE_TOTAL_PAY");
                break;
            case ChallengeMissionType.WATCH_VIDEO_ADS:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_WATCH_VIDEO_ADS");
                break;
            case ChallengeMissionType.CONNECT_FACEBOOK:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_CONNECT_FACEBOOK");
                break;
            case ChallengeMissionType.ADD_FRIENDS:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_ADD_FRIENDS");
                break;
            case ChallengeMissionType.JOIN_CLUB:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_JOIN_CLUB");
                break;
            case ChallengeMissionType.CONNECT_EMAIL:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_CONNECT_EMAIL");
                break;
            case ChallengeMissionType.VIP_CLUB:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_CONNECT_VIP");
                break;
            case ChallengeMissionType.USE_GEM_MORE_THAN:
                title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_USE_GEM_MORE_THAN");
                break;
            default:
                {
                    title.value = "UNKNOWN";
                    useSlotThumbnail.value = true;
                    gameID.value = -1;
                }
                break;
        }
    }

    private void LoadMissionIcon()
    {
        if(missionIconObject.value != null)
            GameObject.Destroy(missionIconObject.value);

        var gameIdObj = BlackboardUtils.FindVariable<int>(missionInfo, "gameId");
        if(gameIdObj != null)
            gameID.value = gameIdObj.value;

        missionIconObject.value = MetaIconUtils.MakeChallengeMissionIconObject(missionInfo, gameID.value, agent, "Anchor/Base Anchor/Mission/Image Area");

        if(missionIconObject.value == null)
            useSlotThumbnail.value = true;
        else
            missionIconObject.value.transform.SetAsFirstSibling();
    }

    private void UpdateRewardInfo()
    {
        if(rewardResult == null)
        {
            rewardType.value = RewardType.UNKNOWN;
            rewardText.value = "";
        }
        else
        {
            rewardType.value = rewardResult.GetValue<RewardType>("rewardType");
            rewardText.value = BlackboardQueryUtils.GetChallengeMissionRewardResultText(rewardResult);
        }
    }

    private void UpdateChallengeInfo()
    {
        var challengeType = BlackboardUtils.FindVariable<ChallengeType>(simpleChallengeInfo, "challengeType");
        int challengeCompleteCount = BlackboardUtils.FindVariable<int>(simpleChallengeInfo, "challengeProgress").value;
        int minCount = BlackboardQueryUtils.GetChallengeMinCount(challengeType.value);

        if(challengeCompleteCount > minCount)
            challengeCompleteCount = minCount;
        
        challengeProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "A_PER_B", challengeCompleteCount, minCount);

        switch(challengeType.value)
        {
            case ChallengeType.DAILY:
                challengeTypeIndex.value = 0;
                break;
            case ChallengeType.EXPERT:
                challengeTypeIndex.value = 1;
                break;
            case ChallengeType.MASTER:
                challengeTypeIndex.value = 2;
                break;
        }

        bool isEnabled = false;
        int timeZoneOffset = TimeUtils.GetTimeZoneOffset();
        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

        var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

        for (int i = 0; i < challengeInfoList.value.Count; ++i)
        {
            var cType = BlackboardUtils.FindVariable<ChallengeType>(challengeInfoList.value[i], "challengeType");

            if (cType.value == challengeType.value)
            {
                long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
                DateTime startTimeDate = TimeUtils.ParseTimestampToDateTime(startTimestamp);
                startTimeDate = startTimeDate.AddHours(-timeZoneOffset);
                startTimestamp = (long)((startTimeDate - TimeUtils.Jan1St1970).TotalMilliseconds);

                if (startTimestamp < currentTimestamp)
                {
                    var done = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done");
                    var claimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed");

                    isEnabled = (done.value && !claimed.value);
                    break;
                }
            }
        }

        isClaimEnabled.value = isEnabled;
    }

    private void UpdateMissionProgressText()
    {
        long currentCount = BlackboardUtils.FindVariable<long>(missionInfo, "progress").value;
        long completeCount = BlackboardUtils.FindVariable<long>(missionInfo, "completeCount").value;

        missionProgress.value = (float)((double)currentCount/(double)completeCount);

        ChallengeMissionType missionType = BlackboardUtils.FindVariable<ChallengeMissionType>(missionInfo, "missionType").value;

        switch(missionType)
        {
            case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
            case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                {
                    long baseCredit = 0;
                    var targetCredit = BlackboardUtils.FindVariable<long>(missionInfo, "targetCredit");
                    if(currentCount == completeCount)
                        baseCredit = targetCredit.value;

                    missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", baseCredit, targetCredit.value);
                }
                break;
            case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                {
                    var targetPrice = BlackboardUtils.FindVariable<long>(missionInfo, "targetPrice");
                    double currentCurrency = 0.0;
                    double completeCurrency = (double)targetPrice.value/100.0;
                    if(currentCount == completeCount)
                        currentCurrency = completeCurrency;

                    missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_CURRENCY", currentCurrency, completeCurrency);
                }
                break;
            case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                {
                    double currentCurrency = (double)currentCount/100.0;
                    double completeCurrency = (double)completeCount/100.0;
                    missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_CURRENCY", currentCurrency, completeCurrency);
                }
                break;
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
            case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", currentCount, completeCount);
                break;
            default:
                missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_DEFAULT", currentCount, completeCount);
                break;
        }
    }
}

}
