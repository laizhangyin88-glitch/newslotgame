using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Challenge")]
    public class GetChallengeMissionInfoBB : ActionTask<Transform>
    {
        public BBParameter<Blackboard> missionInfo;

        public BBParameter<string> title;

        public BBParameter<GameObject> missionIconObject;
        public BBParameter<bool> useSlotThumbnail;

        public BBParameter<string> iconText;

        public BBParameter<string> rewardText;

        public BBParameter<string> progressText;
        public BBParameter<float> progress;

        public BBParameter<bool> isComplete;

        public BBParameter<int> gameID;
        public BBParameter<bool> usePlayButton;

        // private int currentCount = 0;
        // private int completeCount = 0;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected override string info
        {
            get { return "Get Mission Info BB"; }
        }

        protected override void OnExecute()
        {
            if (missionInfo.value != null)
            {
                gameID.value = -1;
                usePlayButton.value = false;
                useSlotThumbnail.value = false;
                iconText.value = "";

                var gameIdObj = BlackboardUtils.FindVariable<int>(missionInfo.value, "gameId");
                if (gameIdObj != null)
                {
                    gameID.value = gameIdObj.value;
                    if (gameID.value > -1)
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
            title.value = ChallengeUtils.GetChallengeMissionTitle(missionInfo.value);

            if(string.IsNullOrEmpty(title.value) || title.value == "UNKNOWN")
            {
                usePlayButton.value = false;
                useSlotThumbnail.value = true;
                gameID.value = -1;
            }
        }

        private void LoadMissionIcon()
        {
            if (missionIconObject.value != null)
                GameObject.Destroy(missionIconObject.value);

            missionIconObject.value = MetaIconUtils.MakeChallengeMissionIconObject(missionInfo.value, gameID.value, agent, "Anchor/Mission/Image Area");

            if (missionIconObject.value == null)
                useSlotThumbnail.value = true;
            else
                missionIconObject.value.transform.SetAsFirstSibling();
        }

        private void UpdateProgressText()
        {
            progressText.value = BlackboardQueryUtils.GetChallengeProgressText(missionInfo.value, out float _progress, out bool _isComplete);

            if (progress != null)
                progress.value = _progress;

            if (_isComplete)
                usePlayButton.value = false;

            isComplete.value = _isComplete;
        }

        private void UpdateReward()
        {
            var reward = BlackboardUtils.FindVariable<Blackboard>(missionInfo.value, "reward");

            if (reward == null)
            {
                rewardText.value = "";
            }
            else
            {
                rewardText.value = BlackboardQueryUtils.GetChallengeMissionRewardText(missionInfo.value);
            }
        }
    }
}
