using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Challenge")]
    public class GetChallengeCompleteBB : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> completePollData;

        public BBParameter<string> titleText;
        public BBParameter<string> rewardText;

        protected override string info
        {
            get { return "Get Challenge Complete BB"; }
        }

        protected override void OnExecute()
        {
            titleText.value = GetTitleText();

            var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(completePollData.value, "challenge/rewardList");
            long eventMultiplierNumerator = GetPassiveEventMultiplierNumerator();
            rewardText.value = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardList.value, eventMultiplierNumerator);

            EndAction();
        }

        public string GetTitleText()
        {
            bool isError = false;
            
            var challengeType = BlackboardUtils.FindVariable<ChallengeType>(completePollData.value, "challenge/challengeType");

            return StringTableUtils.GetString(StringTable.StringTableType.Global, "CHALLENGE_COMPLETE_TITLE", out isError, challengeType.value.ToString());
        }

        public long GetPassiveEventMultiplierNumerator()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);

            var challengeType = BlackboardUtils.FindVariable<ChallengeType>(completePollData.value, "challenge/challengeType");
            if (challengeType.value == ChallengeType.EVENT)
            {
                return NumberUtils.GetGlobalDenominator();
            }

            if (eventInfo != null)
                return PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);

            return NumberUtils.GetGlobalDenominator();
        }
    }
}