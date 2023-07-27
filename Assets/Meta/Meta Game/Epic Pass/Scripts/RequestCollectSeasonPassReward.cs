using UnityEngine;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicPass
{
    [Category("★ BagelCode/Meta Games/Epic Pass")]
    public class RequestCollectSeasonPassReward : ActionTask
    {
        public BBParameter<GameObject> rewardCellObj;

        public BBParameter<bool> saveAsSuccess;
        public BBParameter<List<Blackboard>> saveAsRewardResultList;

        protected override string info
        {
            get { return "Request Collect Season Pass Reward"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if (rewardCellObj.value == null || metaGameInfo == null)
            {
                EndAction();
                return;
            }

            var controller = rewardCellObj.value.GetComponent<EpicPassRewardItemController>();
            if(controller == null)
            {
                EndAction();
                return;
            }

            BagelCodeClientAPI.CollectSeasonPassReward(metaGameInfo.id, controller.rewardLevel, controller.isPaidReward,
                (response) =>
                {
                    if(agent != null)
                    {
                        EpicPassUtils.UpdateEpicPassRewards(response.rewardInfoList);
                        EpicPassUtils.UnclaimedRewardCount = response.unclaimedRewardCount;

                        List<RewardResult> list = new List<RewardResult>();
                        list.Add(response.rewardResult);

                        saveAsRewardResultList.value = EpicPassUtils.UpdateCollectRewards(list);
                        saveAsSuccess.value = true;

                        BlackboardQueryUtils.ApplyRewardResult(saveAsRewardResultList.value);

                        if(controller != null)
                            controller.OnClaimed();

                        EndAction();
                    }

                },
                (error) =>
                {
                    switch(error.errorCode)
                    {
                        case ClientModels.Error.INVALID_SEASON_PASS_LEVEL_OR_REWARD_CONDITION_ERROR:
                            {
                                // Need text alert popup.
                                if(agent != null)
                                    EndAction();
                            }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }
    }
}
