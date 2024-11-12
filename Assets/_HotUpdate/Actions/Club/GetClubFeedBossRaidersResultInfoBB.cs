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
    public class GetClubFeedBossRaidersResultInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<int> cellIndex;
        public BBParameter<List<GameObject>> bgList; // 0, 1

        public BBParameter<string> clubFeedInfoValue;

        public BBParameter<string> messageText;
        public BBParameter<string> likeButtonText;
        public BBParameter<string> leftTimeAgoText;

        protected override string info
        {
            get { return "Get Club Feed BossRaiders Result Info BB"; }
        }

        protected override void OnExecute()
        {
            var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);
            if (clubFeedInfo != null)
            {
                var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
                leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText(TimeUtils.GetTimeStamp() - createdTimestamp);

                bool error = false;
                var rewardGems = GetRewardGem(clubFeedInfo.value);
                if (rewardGems != 0)
                    likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_GEM", out error, rewardGems);
                else
                    likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE", out error);

                ClubFeedType type = clubFeedInfo.value.GetValue<ClubFeedType>("type");
                if (type == ClubFeedType.BOSS_RAIDERS_ROUND_COMPLETE)
                {
                    messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_CLUB_NEWS_FEED_ROUND_COMPLETE",
                        clubFeedInfo.value.GetValue<long>("leaguePoint"));
                }
                else if (type == ClubFeedType.BOSS_RAIDERS_END_REWARD)
                {
                    messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_CLUB_NEWS_FEED_END_REWARD", GetEndRewardRank(clubFeedInfo.value), (rewardGems != 0 ? rewardGems : 0));
                }
                ContextElement iconAreaElement = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), "Boss Riaders Icon Area", ContextSearchingType.ChildrenSearch);
                if (iconAreaElement != null && iconAreaElement.transform.childCount == 0)
                    SetBossRaidersIcon(iconAreaElement, clubFeedInfo.value.GetVariable<int>("themeId")?.value ?? 0);
                UpdateBG();
            }

            EndAction();
        }

        private void UpdateBG()
        {
            int cellStyle = cellIndex.value % 2;

            for (int i = 0; i < bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i == cellStyle);
            }
        }

        private long GetRewardGem(Blackboard clubFeedInfo)
        {
            long rewardGems = 0;

            var rewardBB = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "reward");
            if (rewardBB != null && rewardBB.value != null)
            {
                var rewardType = rewardBB.value.GetValue<RewardType>("type");

                if (rewardType == RewardType.GEM)
                    rewardGems = rewardBB.value.GetValue<long>("gem");
            }

            return rewardGems;
        }

        private string GetEndRewardRank(Blackboard clubFeedInfo)
        {
            var rank = BlackboardUtils.FindVariable<int>(clubFeedInfo, "rank");
            var percentile = BlackboardUtils.FindVariable<int>(clubFeedInfo, "percentile");

            string resultText = "";

            if (rank != null && rank.value <= 100)
            {
                resultText = rank.value + string.Format(StringTableUtils.customProvider, "{0:Ordinal;WithoutValue}", rank.value);
            }
            else if (percentile != null && percentile.value > 0)
            {
                resultText = percentile.value + "%";
            }

            return resultText;
        }

        private void SetBossRaidersIcon(ContextElement iconAreaElement, int themeId)
        {
            MetaObjectUtils.MakePrefab(string.Format("Club News Feed Boss Raiders Icon_{0}", themeId), iconAreaElement.transform);
        }
    }
}