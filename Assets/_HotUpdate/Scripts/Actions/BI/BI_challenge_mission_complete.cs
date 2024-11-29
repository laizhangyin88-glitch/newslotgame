using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_challenge_mission_complete : ActionTask<Blackboard>
    {
        public BBParameter<string> rewardResult;
        public BBParameter<string> mission;
        public BBParameter<string> challenge;

        protected override void OnExecute()
        {
            var rewardResultBB = BlackboardUtils.FindVariable<Blackboard>(agent, rewardResult.value);
            var missionBB = BlackboardUtils.FindVariable<Blackboard>(agent, mission.value);
            var challengeBB = BlackboardUtils.FindVariable<Blackboard>(agent, challenge.value);

            Dictionary<string, object> customData = new Dictionary<string, object>();

            var challengeType = BlackboardUtils.FindVariable<ChallengeType>(challengeBB.value, "challengeType");
            customData["challenge_type"] = challengeType.value.ToString().ToLower();

            var missionType = BlackboardUtils.FindVariable<ChallengeMissionType>(missionBB.value, "missionType");
            customData["mission_type"] = missionType.value.ToString().ToLower();

            var missionID = BlackboardUtils.FindVariable<long>(missionBB.value, "id");
            customData["mission_id"] = missionID.value;

            customData["action_type"] = "mission_complete";

            var missionCompleteCount = BlackboardUtils.FindVariable<long>(missionBB.value, "completeCount");
            customData["mission_target"] = missionCompleteCount.value;

            if (rewardResultBB != null)
            {
                BiEventUtils.AppendCommonRewardEventData(customData, rewardResultBB.value);
            }

            var challengeProgress = BlackboardUtils.FindVariable<int>(challengeBB.value, "challengeProgress");
            customData["challenge_progress"] = challengeProgress.value;

            int challengeCompleteCount = BlackboardQueryUtils.GetChallengeMinCount(challengeType.value);
            customData["challenge_complete_count"] = challengeCompleteCount;

            var gameID = BlackboardUtils.FindVariable<int>(missionBB.value, "gameId");
            if (gameID != null)
                customData["game_id"] = gameID.value;

            var bonusID = BlackboardUtils.FindVariable<int>(missionBB.value, "bonusId");
            if (bonusID != null)
                customData["bonus_id"] = bonusID.value;

            var winType = BlackboardUtils.FindVariable<string>(missionBB.value, "winType");
            if (winType != null)
                customData["win_type"] = winType.value;

            var challengeID = BlackboardUtils.FindVariable<string>(challengeBB.value, "challengeId");
            if (challengeID != null)
                customData["challenge_id"] = challengeID.value;

            var betLimit = BlackboardUtils.FindVariable<long>(missionBB.value, "betLimit");
            if (betLimit != null)
                customData["bet_limit"] = betLimit.value;

            BiEventUtils.AppendLevelMultiplierEventData(customData, "coin");
            Analytics.CustomEvent("client_challenge", customData);

            EndAction();
        }
    }
}
