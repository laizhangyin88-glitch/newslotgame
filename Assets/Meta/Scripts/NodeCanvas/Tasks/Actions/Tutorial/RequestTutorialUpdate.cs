using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class RequestTutorialUpdate : ActionTask<Blackboard>
    {
        public BBParameter<bool> isReward;
        public BBParameter<bool> isQuestEnd;
        
        protected override string info
        {
            get { return "Request Tutorial Update "; }
        }

        protected override void OnExecute()
        {
            var stage = BlackboardUtils.FindVariable<int>(null, "/tutorialInfo/stage");
            var count = BlackboardUtils.FindVariable<long>(null, "/tutorialInfo/count");
            var level = BlackboardUtils.FindVariable<int>(null, "/userSyncInfo/level");
            if (level == null) level = BlackboardUtils.FindVariable<int>(null, "/me/level");
            
            if (stage != null && count != null)
            {
                bool sendRequest = false;
                
                if (stage.value == 0)
                {
                    if (count.value >= 7)
                    {
                        stage.value = 1;
                        count.value = 0;
                    }

                    sendRequest = true;
                }
                else if (stage.value == 1 && level.value >= 4)
                {
                    stage.value = 2;
                    sendRequest = true;
                }
                else if (stage.value == 2 && level.value >= 6)
                {
                    stage.value = 3;
                    isQuestEnd.value = true;
                    sendRequest = true;
                }

                if (sendRequest)
                {
                    BagelCodeClientAPI.RequestTutorialUpdate(stage.value, count.value,
                        (response) =>
                        {
                            if (response.rewardResultList != null && response.rewardResultList.Count > 0)
                            {
                                var tutorialUpdateResponse = BlackboardUtils.GetOrCreateBlackboard(agent, "tutorialUpdateResponse");
                                ClientAPI2Blackboard.Serialize(tutorialUpdateResponse, response);
                                var rewardResultList = tutorialUpdateResponse.GetValue<List<Blackboard>>("rewardResultList");
                                BlackboardQueryUtils.ApplyRewardResult(rewardResultList);
                                    
                                isReward.value = true;
                            }                                                        
                        },
                        (error) =>
                        {
//                            GlobalErrorHandler.GlobalError(error);
                            // do nothing
                        });
                }
            }
            
            EndAction();
        }
    }
}