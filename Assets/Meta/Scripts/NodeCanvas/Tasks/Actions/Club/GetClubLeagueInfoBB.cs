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
    public class GetClubLeagueInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<string> leagueInfoResponse;

        public BBParameter<GameObject> promoteObject;
        public BBParameter<GameObject> demoteObject;
        public BBParameter<GameObject> remainObject;

        public BBParameter<string> saveMyClubRankText;
        public BBParameter<string> saveRewardCoinText;
        public BBParameter<string> saveResultTierText;

        private int currentClubRank = -1;

        protected override string info
        {
            get { return "Get Club League Info BB"; }
        }

        protected override void OnExecute()
        {
            var responseBB = BlackboardUtils.FindVariable<Blackboard>(agent, leagueInfoResponse.value);
            var clubMultiplier = ClubUtils.GetClubRewardMultiplierNumerator();

            saveMyClubRankText.value = "-";
            saveRewardCoinText.value = "0";
            saveResultTierText.value = "";
            promoteObject.value.SetActive(false);
            demoteObject.value.SetActive(false);
            remainObject.value.SetActive(false);

            if(responseBB != null)
            {
                var rewardList = BlackboardUtils.FindVariable<List<long>>(responseBB.value, "rewardCreditList");

                // Update Rewards
                if(rewardList.value.Count > 0)
                {
                    var clubList = BlackboardUtils.FindVariable<List<Blackboard>>(responseBB.value, "clubList");
                    var myClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");

                    for(int i=0; i<clubList.value.Count; ++i)
                    {
                        var clubID = clubList.value[i].GetValue<long>("id");
                        if(clubID == myClubID.value)
                        {
                            currentClubRank = i;
                            var rewardCoin = rewardList.value[i];
                            saveRewardCoinText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_REWARD_COIN_TEXT", NumberUtils.GetMultiplierNumeratorValue(rewardCoin, clubMultiplier));
                            saveMyClubRankText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CURRENT_RANK_TEXT", i + 1);
                            break;
                        }
                    }
                }

                if(currentClubRank > -1)
                {
                    var clubTierInfoBB = BlackboardUtils.FindVariable<Blackboard>(responseBB.value, "tierInfo");
                    var clubTier = clubTierInfoBB.value.GetValue<int>("leagueTier");
                    var indexPromote = responseBB.value.GetValue<int>("indexPromote");
                    var indexDemote = responseBB.value.GetValue<int>("indexDemote");
                    var openMaxTier = responseBB.value.GetValue<int>("maxOpenedLeagueTier");
                    var leagueMaxTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEAGUE/MAX_TIER");

                    // Set Tier, Result, 
                    int rankState = 2; // 0:Promote, 1: Demote, 3:Remain

                    if(clubTier == 0)
                    {
                        if(currentClubRank <= indexPromote)
                            rankState = 0;
                    }
                    else if(clubTier == leagueMaxTier.value || clubTier >= openMaxTier)
                    {
                        if(currentClubRank >= indexDemote)
                            rankState = 1;
                    }
                    else
                    {
                        if(currentClubRank <= indexPromote)
                            rankState = 0;
                        else if(currentClubRank >= indexDemote)
                            rankState = 1;
                    }

                    if(rankState == 0)
                    {
                        saveResultTierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", clubTier + 1);
                        promoteObject.value.SetActive(true);
                    }
                    else if(rankState == 1)
                    {
                        saveResultTierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", clubTier - 1);
                        demoteObject.value.SetActive(true);
                    }
                    else
                    {
                        saveResultTierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", clubTier);
                        remainObject.value.SetActive(true);
                    }
                }
            }
        
            EndAction();
        }
    }
}
