using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class GetClubChallengeMissionInfoBB : ActionTask<Transform>
    {
        public BBParameter<Blackboard> challengeInfo;
        public BBParameter<Blackboard> missionInfo;
        public BBParameter<string> missionIconPath;

        public BBParameter<string> titleText;

        public BBParameter<GameObject> missionIconObject;
        public BBParameter<bool> useSlotThumbnail;

        public BBParameter<string> iconText;

        public BBParameter<string> rewardText;

        public BBParameter<string> progressText;
        public BBParameter<float> progress;

        public BBParameter<bool> isComplete;

        public BBParameter<int> gameID;
        public BBParameter<bool> usePlayButton;

        private const string PLUS = " + ";

        protected override string info
        {
            get { return "Get Club Mission Info BB"; }
        }

        protected override void OnExecute()
        {
            if(missionInfo.value != null)
            {
                gameID.value = -1;
                usePlayButton.value = false;
                useSlotThumbnail.value = false;
                iconText.value = "";

                var gameIdObj = BlackboardUtils.FindVariable<int>(missionInfo.value, "gameId");
                if(gameIdObj != null)
                {
                    gameID.value = gameIdObj.value;
                    if(gameID.value > -1)
                        usePlayButton.value = true;
                }

                UpdateMission();
                UpdateProgressText();
                LoadMissionIcon();
                UpdateReward();
            }

            EndAction();
        }

        public void UpdateMission()
        {
            ClubChallengeMissionType missionType = BlackboardUtils.FindVariable<ClubChallengeMissionType>(missionInfo.value, "missionType").value;

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
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo.value, "winType").value;
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
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo.value, "winType").value;
                        string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_{0}_WIN", winType);

                        titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                    }
                    break;
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                    titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_FREE_GAME_WIN", out isError);
                    break;
                case ClubChallengeMissionType.WIN_BONUS_GAME_TARGETED:
                    titleText.value = BlackboardUtils.FindVariable<string>(missionInfo.value, "bonusName").value;
                    break;
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
                    titleText.value = BlackboardUtils.FindVariable<string>(missionInfo.value, "bonusName").value;
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
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo.value, "winType").value;
                        string key = string.Format("POPUP_CHALLENGE_MISSION_TOTAL_LP_{0}_WIN", winType);

                        titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, key, out isError);
                    }
                    break;
                default:
                    {
                        titleText.value = "UNKNOWN";
                        usePlayButton.value = false;
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

            missionIconObject.value = MetaIconUtils.MakeClubChallengeMissionIconObject(missionInfo.value, gameID.value, agent, missionIconPath.value);

            if(missionIconObject.value == null)
                useSlotThumbnail.value = true;
            else
                missionIconObject.value.transform.SetAsFirstSibling();
        }

        private void UpdateProgressText()
        {
            long currentCount = BlackboardUtils.FindVariable<long>(missionInfo.value, "progress").value;
            long completeCount = BlackboardUtils.FindVariable<long>(missionInfo.value, "completeCount").value;

            if(currentCount > completeCount)
                currentCount = completeCount;

            progress.value = (float)((double)currentCount/(double)completeCount);

            if(currentCount == completeCount)
                usePlayButton.value = false;

            isComplete.value = BlackboardUtils.FindVariable<bool>(missionInfo.value, "done").value;

            if(challengeInfo.value != null)
            {
                int stage = challengeInfo.value.GetValue<int>("stage");
                int completedStage = missionInfo.value.GetValue<int>("completedStage");
                if(stage >= 0 && stage == completedStage)
                    isComplete.value = true;
            }
        
            ClubChallengeMissionType missionType = BlackboardUtils.FindVariable<ClubChallengeMissionType>(missionInfo.value, "missionType").value;

            bool isError = false;

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
                    progressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", out isError, currentCount, completeCount);
                    break;
                default:
                    progressText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_PROGRESS_DEFAULT", out isError, currentCount, completeCount);
                    break;
            }
        }

        private void UpdateReward()
        {
            var reward = BlackboardUtils.FindVariable<Blackboard>(missionInfo.value, "reward");
            var leaguePoint = missionInfo.value.GetValue<long>("leaguePoint");
            var clubMultiplier = ClubUtils.GetClubRewardMultiplierNumerator();

            if (reward == null)
            {
                rewardText.value = "";
            }
            else
            {
                rewardText.value = BlackboardQueryUtils.GetChallengeMissionRewardMultiplierText(missionInfo.value, clubMultiplier);

                if(leaguePoint > 0)
                {
                    bool isError = false;
                    rewardText.value += PLUS + StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_LP", out isError, leaguePoint);
                }
            }
        }
    }
}
