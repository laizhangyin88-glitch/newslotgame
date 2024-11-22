using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Popup/Common Reward")]

    public class GetCommonRewardListInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<List<Blackboard>> rewardResultList;
        public BBParameter<string> buttonTextKey;
        public BBParameter<string> titleText;

        protected override void OnExecute()
        {
            buttonTextKey.value = "BUTTON_OK";

            if (rewardResultList != null)
            {
                for (int i = 0; i < rewardResultList.value.Count; ++i)
                {
                    var checkScene = BlackboardUtils.FindVariable<RewardCheckScene>(agent, "rewardCheckScene")?.value ?? RewardCheckScene.DEFAULT;
                    BlackboardUtils.SetOrCreateValue(rewardResultList.value[i], "rewardCheckScene", checkScene);
                }

                for (int i = 0; i < rewardResultList.value.Count; ++i)
                {
                    if (i == 0)
                    {
                        var rewardTitle = BlackboardUtils.FindVariable<string>(rewardResultList.value[i], "title");
                        if (rewardTitle != null && !string.IsNullOrEmpty(rewardTitle.value))
                            titleText.value = rewardTitle.value;
                    }

                    var rewardType = BlackboardUtils.FindVariable<RewardType>(rewardResultList.value[i], "rewardType");
                    if (rewardType.value != RewardType.SOCIAL_CREDIT && rewardType.value != RewardType.CLUB_CREDIT)
                    {
                        buttonTextKey.value = "BUTTON_COLLECT";
                        break;
                    }
                }
            }

            EndAction();
        }
    }
}
