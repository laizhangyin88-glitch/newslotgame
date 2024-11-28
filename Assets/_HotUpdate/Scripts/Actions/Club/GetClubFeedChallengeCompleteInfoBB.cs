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

    public class GetClubFeedChallengeCompleteInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<int> cellIndex;
        public BBParameter<List<GameObject>> bgList; // 0, 1

        public BBParameter<string> clubFeedInfoValue;
        public BBParameter<string> leftTimeAgoText;
        public BBParameter<string> likeButtonText;
        public BBParameter<string> messageText;

        protected override string info
        {
            get { return "Get Club Feed Challenge Complete Info BB"; }
        }

        protected override void OnExecute()
        {
            var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

            if (clubFeedInfo != null)
            {
                var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
                leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText(TimeUtils.GetTimeStamp() - createdTimestamp);

                likeButtonText.value = GetRewardText(clubFeedInfo.value);

                var leaguePoint = clubFeedInfo.value.GetValue<long>("leaguePoint");

                if (leaguePoint > 0)
                    messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_CHALLENGE_COMPLETE_TEXT_WITH_LP", leaguePoint);
                else
                    messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_CHALLENGE_COMPLETE_TEXT");

                UpdateBG();
            }

            EndAction();
        }

        private string GetRewardText(Blackboard clubFeedInfo)
        {
            var rewardBB = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "reward");
            if (rewardBB != null && rewardBB.value != null)
            {
                var rewardType = rewardBB.value.GetValue<RewardType>("type");

                switch (rewardType)
                {
                    case RewardType.CREDIT:
                        {
                            long credit = rewardBB.value.GetValue<long>("credit");
                            var clubMultiplier  = ClubUtils.GetClubRewardMultiplierNumerator();
                            credit = NumberUtils.GetMultiplierNumeratorValue(credit, clubMultiplier);
                            if (credit > 0L)
                            {
                                return StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_COIN", credit);
                            }
                        }
                        break;
                    case RewardType.GEM:
                        {
                            long gem = rewardBB.value.GetValue<long>("gem");
                            if (gem > 0L)
                            {
                                return StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_GEM", gem);
                            }
                        }
                        break;
                }
            }

            return StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE");
        }

        private void UpdateBG()
        {
            int cellStyle = cellIndex.value % 2;

            for (int i = 0; i < bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i == cellStyle);
            }
        }

        private long GetRewardCoin(Blackboard clubFeedInfo)
        {
            long rewardCoins = 0;

            var rewardBB = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "reward");
            if (rewardBB != null && rewardBB.value != null)
            {
                var rewardType = rewardBB.value.GetValue<RewardType>("type");

                if (rewardType == RewardType.CREDIT)
                    rewardCoins = rewardBB.value.GetValue<long>("credit");
            }

            return rewardCoins;
        }
    }
}