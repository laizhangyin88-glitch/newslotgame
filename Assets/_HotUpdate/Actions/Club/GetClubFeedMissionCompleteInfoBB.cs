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

    public class GetClubFeedMissionCompleteInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<int> cellIndex;
        public BBParameter<List<GameObject>> bgList; // 0, 1

        public BBParameter<string> clubFeedInfoValue;
        public BBParameter<string> messageText;
        public BBParameter<string> leftTimeAgoText;
        public BBParameter<string> likeButtonText;

        public BBParameter<bool> useSlotThumbnail;
        public BBParameter<GameObject> missionIconObject;
        public BBParameter<int> gameID;

        protected override string info
        {
            get { return "Get Club Feed Mission Complete Info BB"; }
        }

        protected override void OnExecute()
        {
            var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

            if (clubFeedInfo != null)
            {
                gameID.value = -1;
                useSlotThumbnail.value = false;

                var missionInfo = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo.value, "mission");
                var gameIdObj = BlackboardUtils.FindVariable<int>(missionInfo.value, "gameId");
                if (gameIdObj != null)
                    gameID.value = gameIdObj.value;
                else
                    gameID.value = -1;

                var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
                leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText(TimeUtils.GetTimeStamp() - createdTimestamp);

                likeButtonText.value = GetRewardText(clubFeedInfo.value);

                messageText.value = GetMessage(clubFeedInfo.value);

                LoadMissionIcon(clubFeedInfo.value);
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
                            if(credit > 0L)
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

        private string GetMessage(Blackboard clubFeedInfo)
        {
            string message = "";

            var missionInfo = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "mission");

            if (missionInfo != null)
            {
                ClubChallengeMissionType missionType = missionInfo.value.GetValue<ClubChallengeMissionType>("missionType");

                bool isError = false;

                string missionTitle = null;

                switch (missionType)
                {
                    case ClubChallengeMissionType.WIN_ANY:
                    case ClubChallengeMissionType.WIN_TARGETED:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_WIN_COUNT", out isError);
                        break;
                    case ClubChallengeMissionType.SPIN_ANY:
                    case ClubChallengeMissionType.SPIN_TARGETED:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_SPIN_COUNT", out isError);
                        break;
                    case ClubChallengeMissionType.WIN_BIG_WIN_ANY:
                    case ClubChallengeMissionType.WIN_BIG_WIN_TARGETED:
                        {
                            string winType = BlackboardUtils.FindVariable<string>(missionInfo.value, "winType").value;
                            string key = string.Format("POPUP_CHALLENGE_MISSION_{0}_WIN", winType);

                            missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                        }
                        break;
                    case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME", out isError);
                        break;
                    case ClubChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME", out isError);
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_WIN", out isError);
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                        {
                            string winType = BlackboardUtils.FindVariable<string>(missionInfo.value, "winType").value;
                            string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_{0}_WIN", winType);

                            missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                        }
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_FREE_GAME_WIN", out isError);
                        break;
                    case ClubChallengeMissionType.WIN_BONUS_GAME_TARGETED:
                        missionTitle = BlackboardUtils.FindVariable<string>(missionInfo.value, "bonusName").value;
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
                        missionTitle = BlackboardUtils.FindVariable<string>(missionInfo.value, "bonusName").value;
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_ANY", out isError);
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_TARGETED", out isError);
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                        missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_FREE_GAMES", out isError);
                        break;
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                        {
                            string winType = BlackboardUtils.FindVariable<string>(missionInfo.value, "winType").value;
                            string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_LP_{0}_WIN", winType);

                            missionTitle = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                        }
                        break;
                    default:
                        {
                            gameID.value = -1;
                            useSlotThumbnail.value = true;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(missionTitle))
                {
                    var leaguePoint = clubFeedInfo.GetValue<long>("leaguePoint");

                    if (leaguePoint > 0)
                        message = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_MISSION_COMPLETE_TEXT_WITH_LP", out isError, missionTitle, leaguePoint);
                    else
                        message = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_MISSION_COMPLETE_TEXT", out isError, missionTitle);
                }
            }

            return message;
        }

        private void LoadMissionIcon(Blackboard clubFeedInfo)
        {
            if (missionIconObject.value != null)
                GameObject.Destroy(missionIconObject.value);

            var missionInfo = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "mission");

            if (missionInfo != null)
            {
                missionIconObject.value = MetaIconUtils.MakeClubChallengeMissionIconObject(missionInfo.value, gameID.value, agent.transform, "Anchor/Contents/Club Symbol Area");

                if (missionIconObject.value == null)
                    useSlotThumbnail.value = true;
                else
                    missionIconObject.value.transform.SetAsLastSibling();
            }
        }
    }
}