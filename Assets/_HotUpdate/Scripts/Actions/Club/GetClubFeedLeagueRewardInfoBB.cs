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
    public class GetClubFeedLeagueRewardInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<int> cellIndex;
        public BBParameter<List<GameObject>> bgList; // 0, 1

        public BBParameter<string>  clubFeedInfoValue;
        public BBParameter<string>  leftTimeAgoText;
        public BBParameter<string>  likeButtonText;
        public BBParameter<string>  messageText;

        protected override string info
        {
            get { return "Get Club Feed League Reward Info BB"; }
        }

        protected override void OnExecute()
        {
            var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

            if(clubFeedInfo != null)
            {
                var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
                leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText( TimeUtils.GetTimeStamp() - createdTimestamp );

                bool error = false;
                var rewardCoins = GetRewardCoin(clubFeedInfo.value);
                var clubMultiplier = ClubUtils.GetClubRewardMultiplierNumerator();

                if (rewardCoins != 0)
                    likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_COIN", out error, NumberUtils.GetMultiplierNumeratorValue(rewardCoins, clubMultiplier));
                else
                    likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE", out error);

                var leagueRank = clubFeedInfo.value.GetValue<int>("rank");
                messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEAGUE_REWARD_TEXT", out error, leagueRank + 1);

                UpdateBG();
            }

            EndAction();
        }

        private void UpdateBG()
        {
            int cellStyle = cellIndex.value%2;

            for(int i=0; i<bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i==cellStyle);
            }
        }

        private long GetRewardCoin(Blackboard clubFeedInfo)
        {
            long rewardCoins = 0;

            var rewardBB = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "reward");
            if(rewardBB != null && rewardBB.value != null)
            {
                var rewardType = rewardBB.value.GetValue<RewardType>("type");

                if(rewardType == RewardType.CREDIT)
                    rewardCoins = rewardBB.value.GetValue<long>("credit");
            }

            return rewardCoins;
        }
    }
}
