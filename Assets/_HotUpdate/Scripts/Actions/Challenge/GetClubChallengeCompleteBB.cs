using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Challenge")]
    public class GetClubChallengeCompleteBB : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> completePollData;

        public BBParameter<string> rewardText;

        protected override string info
        {
            get { return "Get Club Challenge Complete BB"; }
        }

        protected override void OnExecute()
        {
            var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(completePollData.value, "challenge/rewardList");
            var leaguePoint = BlackboardUtils.FindVariable<long>(completePollData.value, "challenge/leaguePoint");

            string result = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardList.value);

            if (leaguePoint != null && leaguePoint.value > 0L)
                result += " + " + StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_LP", leaguePoint.value);

            rewardText.value = result;

            EndAction();
        }
    }
}