using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Challenge")]

public class GetClubChallengeMissionCompleteBB : ActionTask<Transform>
{
    public BBParameter<Blackboard> completePollData;

    public BBParameter<string> titleText;

    public BBParameter<GameObject> missionIconObject;
    public BBParameter<bool> useSlotThumbnail;

    public BBParameter<float> missionProgress;
    public BBParameter<string> missionProgressText;

    public BBParameter<string> rewardText;
    public BBParameter<string> bigRewardText;
    public BBParameter<string> firstRewardText;

    public BBParameter<string> challengeProgressText;
    public BBParameter<bool> isCompleteChallenge;

    public BBParameter<bool> showNextStage;
    public BBParameter<string> nextStageText;

    private Blackboard rewardResult;
    private Blackboard missionInfo;
    private Blackboard simpleChallengeInfo;

    public BBParameter<int> gameID;

    private const string BLANK_SPACE = " ";
    private const string ADD_PLUS = " + ";

    protected override string info
    {
        get { return "Get Club Challenge Mission Complete Info BB"; }
    }

    protected override void OnExecute()
    {
        gameID.value = -1;
        useSlotThumbnail.value = false;
        showNextStage.value = false;

        var result = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "rewardResult");
        if(result != null)
        {
            rewardResult = result.value;
        }
        // rewardResult = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "rewardResult").value;
        missionInfo = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "mission").value;
        simpleChallengeInfo = BlackboardUtils.FindVariable<Blackboard>(completePollData.value, "challenge").value;

        int challengeCompleteCount = simpleChallengeInfo.GetValue<int>("challengeProgress");
        int maxChallengeCount = simpleChallengeInfo.GetValue<int>("maxChallengeProgress");
        
        isCompleteChallenge.value = maxChallengeCount == challengeCompleteCount;

        int stage = simpleChallengeInfo.GetValue<int>("stage");
        int maxStage = BlackboardQueryUtils.GetClubChallengeMaxStage(simpleChallengeInfo);

        if(stage >= 0 && isCompleteChallenge.value)
        {
            stage += 1;
            int nextStage = stage + 1;
            if(stage < maxStage)
            {
                nextStageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "FEED_CLUB_CHALLENGE_MISSION_NEXT_STAGE_TEXT", nextStage);
                showNextStage.value = true;
                isCompleteChallenge.value = false;
            }
        }

        challengeProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "A_PER_B", challengeCompleteCount, maxChallengeCount);

        UpdateMission();
        LoadMissionIcon();
        UpdateMissionProgressText();
        UpdateRewardInfo(showNextStage.value);
        // "A_PER_B"

        EndAction();
    }

    public void UpdateMission()
    {
        ClubChallengeMissionType missionType = missionInfo.GetValue<ClubChallengeMissionType>("missionType");

        bool isError = false;

        switch(missionType)
        {
            case ClubChallengeMissionType.WIN_ANY:
            case ClubChallengeMissionType.WIN_TARGETED:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_WIN_COUNT", out isError);
                break;
            case ClubChallengeMissionType.SPIN_ANY:
            case ClubChallengeMissionType.SPIN_TARGETED:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_SPIN_COUNT", out isError);
                break;
            case ClubChallengeMissionType.WIN_BIG_WIN_ANY:
            case ClubChallengeMissionType.WIN_BIG_WIN_TARGETED:
                {
                    string winType = missionInfo.GetValue<string>("winType");
                    string key = string.Format("POPUP_CHALLENGE_MISSION_{0}_WIN", winType);

                    titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                }
                break;
            case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME", out isError);
                break;
            case ClubChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME", out isError);
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_WIN", out isError);
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                {
                    string winType = missionInfo.GetValue<string>("winType");
                    string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_{0}_WIN", winType);

                    titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                }
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_FREE_GAME_WIN", out isError);
                break;
            case ClubChallengeMissionType.WIN_BONUS_GAME_TARGETED:
                titleText.value = missionInfo.GetValue<string>("bonusName");
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
                titleText.value = missionInfo.GetValue<string>("bonusName");
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_ANY", out isError);
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_TARGETED", out isError);
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_FREE_GAMES", out isError);
                break;
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                {
                    string winType = missionInfo.GetValue<string>("winType");
                    string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_LP_{0}_WIN", winType);

                    titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                }
                break;
            default:
                {
                    titleText.value = "UNKNOWN";
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

        missionIconObject.value = MetaIconUtils.MakeClubChallengeMissionIconObject(missionInfo, gameID.value, agent, "Anchor/Base Anchor/Mission/Image Area");

        if(missionIconObject.value == null)
            useSlotThumbnail.value = true;
        else
            missionIconObject.value.transform.SetAsFirstSibling();
    }

    private void UpdateRewardInfo(bool isNextStage)
    {
        if(rewardResult == null)
        {
            rewardText.value = "";
            bigRewardText.value = "";
        }
        else
        {
            rewardText.value = BlackboardQueryUtils.GetChallengeMissionRewardResultText(rewardResult);
            bigRewardText.value = rewardText.value;

            var leaguePoint = missionInfo.GetValue<long>("leaguePoint");

            if(leaguePoint > 0)
            {
                string lpText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_LP", leaguePoint);
                rewardText.value += ADD_PLUS + lpText;
                bigRewardText.value += ADD_PLUS + lpText;
            }

            if(isNextStage)
                bigRewardText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "FEED_CLUB_CHALLENGE_MISSION_AND_STAGE_COMPLTE_TEXT", bigRewardText.value);
            else
                bigRewardText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "FEED_CLUB_CHALLENGE_MISSION_COMPLTE_TEXT", bigRewardText.value);
        }
    }

    private void UpdateMissionProgressText()
    {
        long currentCount = missionInfo.GetValue<long>("progress");
        long completeCount = missionInfo.GetValue<long>("completeCount");

        if(currentCount > completeCount)
            currentCount = completeCount;

        missionProgress.value = (float)((double)currentCount/(double)completeCount);

        ClubChallengeMissionType missionType = missionInfo.GetValue<ClubChallengeMissionType>("missionType");

        switch(missionType)
        {
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
            case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", currentCount, completeCount);
                break;
            default:
                missionProgressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_DEFAULT", currentCount, completeCount);
                break;
        }
    }
}

}
