using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubLeagueRewardCellInfoBB : ActionTask<Blackboard>
{
    public BBParameter<long> rewardCoin;
    public BBParameter<int> cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1, myClubRank
    public BBParameter<List<GameObject>> rankStateList; // 0 Promote, 1 Demote
    public BBParameter<int> myClubRank;
    public BBParameter<int> indexPromote;
    public BBParameter<int> indexDemote;
    public BBParameter<int> leagueTier;
    public BBParameter<int> maxOpenedTier;

    public BBParameter<string> saveAsRankText;
    public BBParameter<string> saveAsClubTierText;
    public BBParameter<string> saveAsRewardText;

    protected override string info
    {
        get { return "Get Club League Reward Cell Info BB"; }
    }

    protected override void OnExecute()
    {
        UpdateBG();
        UpdateRankState();

        bool isError = false;
        saveAsRankText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LEAGUE_REWARD_CELL_RANK_TEXT", out isError, cellIndex.value + 1);
        saveAsRewardText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LEAGUE_REWARD_CELL_REWARD_TEXT", out isError, rewardCoin.value);

        EndAction();
    }

    private void UpdateBG()
    {
        int cellStyle = cellIndex.value%2;

        if(myClubRank.value == cellIndex.value)
            cellStyle = 2;

        for(int i=0; i<bgList.value.Count; ++i)
        {
            bgList.value[i].SetActive(i==cellStyle);
        }
    }

    private void UpdateRankState()
    {
        int rankState = 2;

        var leagueMaxTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEAGUE/MAX_TIER");
        
        if(leagueTier.value == 0)
        {
            if(cellIndex.value <= indexPromote.value)
                rankState = 0;
        }
        else if(leagueTier.value == leagueMaxTier.value || leagueTier.value >= maxOpenedTier.value)
        {
            if(cellIndex.value >= indexDemote.value)
                rankState = 1;
        }
        else
        {
            if(cellIndex.value <= indexPromote.value)
                rankState = 0;
            else if(cellIndex.value >= indexDemote.value)
                rankState = 1;
        }

        bool isError = false;

        if(rankState == 0)
            saveAsClubTierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_NAME", out isError, leagueTier.value + 1);
        else if(rankState == 1)
            saveAsClubTierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_NAME", out isError, leagueTier.value - 1);
        else
            saveAsClubTierText.value = "";
        

        for(int i=0; i<rankStateList.value.Count; ++i)
        {
            rankStateList.value[i].SetActive(i==rankState);
        }
    }
}

}

