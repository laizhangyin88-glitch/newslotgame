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

    public class GetClubLeagueResultInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<string>  leagueResultValue;

        public BBParameter<string>  clubTierText;
        public BBParameter<string>  message;
        public BBParameter<bool>    isReward;

        private const string LEAGUE_REMAIN_KEY                  = "POPUP_LEAGUE_RESULT_REMAIN_TEXT";
        private const string LEAGUE_REMAIN_WITH_OUT_REWARD_KEY  = "POPUP_LEAGUE_RESULT_REMAIN_WITH_OUT_REWARD_TEXT";
        private const string LEAGUE_CHANGED_KEY                 = "POPUP_LEAGUE_RESULT_CHANGED_TEXT";
        private const string LEAGUE_CHANGED_WITH_OUT_REWARD_KEY = "POPUP_LEAGUE_RESULT_CHANGED_WITH_OUT_REWARD_TEXT";

        protected override string info
        {
            get { return "Get Club League Result Info BB"; }
        }

        protected override void OnExecute()
        {
            var leagueResultInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, leagueResultValue.value);

            if(leagueResultInfoBB != null)
            {
                var leagueTier      = leagueResultInfoBB.value.GetValue<int>("leagueTier");
                var rank            = leagueResultInfoBB.value.GetValue<int>("rank");
                var tierChange      = leagueResultInfoBB.value.GetValue<TierChangeType>("tierChange");
                var expireTimestamp = leagueResultInfoBB.value.GetValue<long>("rewardExpireTimestamp");
                var rewardCoins     = leagueResultInfoBB.value.GetValue<long>("rewardCredit");
                var clubMultiplier  = ClubUtils.GetClubRewardMultiplierNumerator();

                if (rewardCoins > 0 && expireTimestamp > TimeUtils.GetTimeStamp())
                    isReward.value = true;
                else
                    isReward.value = false;

                string stringKey = "";

                ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
                ContextElement arrowUp = ContextUtils.FindElement(agentElement, "Icon Arrow Up", ContextSearchingType.ChildrenSearch);
                ContextElement arrowRemain = ContextUtils.FindElement(agentElement, "Icon Arrow Remain", ContextSearchingType.ChildrenSearch);
                ContextElement arrowDown = ContextUtils.FindElement(agentElement, "Icon Arrow Down", ContextSearchingType.ChildrenSearch);

                arrowUp.gameObject.SetActive(tierChange == TierChangeType.PROMOTE);
                arrowRemain.gameObject.SetActive(tierChange == TierChangeType.REMAIN);
                arrowDown.gameObject.SetActive(tierChange == TierChangeType.DEMOTE);

                if(tierChange == TierChangeType.REMAIN)
                {
                    if(isReward.value)
                        stringKey = LEAGUE_REMAIN_KEY;
                    else
                        stringKey = LEAGUE_REMAIN_WITH_OUT_REWARD_KEY;
                }
                else
                {
                    if(isReward.value)
                        stringKey = LEAGUE_CHANGED_KEY;
                    else
                        stringKey = LEAGUE_CHANGED_WITH_OUT_REWARD_KEY;
                }

                clubTierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LEAGUE_CLUB_TIER_TEXT", leagueTier);
                message.value = StringTableUtils.GetString(StringTable.StringTableType.Global, stringKey, rank + 1, leagueTier, NumberUtils.GetMultiplierNumeratorValue(rewardCoins, clubMultiplier));
            }

            EndAction();
        }
    }
}
