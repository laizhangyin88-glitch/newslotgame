using UnityEngine;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicPass
{
    [Category("★ BagelCode/Meta Games/Epic Pass")]
    public class RequestCollectAllSeasonPassReward : ActionTask<Blackboard>
    {
        public BBParameter<bool> saveAsSuccess;
        public BBParameter<List<Blackboard>> saveAsRewardResultList;

        protected override string info
        {
            get { return "Request Collect All Season Pass Reward"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if (metaGameInfo == null)
            {
                EndAction();
                return;
            }

            BagelCodeClientAPI.CollectAllSeasonPassReward(metaGameInfo.id,
                (response) =>
                {
                    if(agent != null)
                    {
                        BlackboardUtils.SetOrCreateList(agent, "rewardResultList", response.rewardResultList, ClientAPI2Blackboard.Serialize);
                        saveAsRewardResultList.value = BlackboardUtils.GetOrCreateBlackboardList(agent, "rewardResultList");

                        EpicPassUtils.CollectAll();

                        saveAsSuccess.value = true;

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
