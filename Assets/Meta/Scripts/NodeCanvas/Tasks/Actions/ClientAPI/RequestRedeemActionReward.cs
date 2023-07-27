using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class RequestRedeemActionReward : ActionTask<Blackboard>
    {
        public BBParameter<string> actionRewardId;
        public BBParameter<bool> isSuccess;
        public BBParameter<ClientModels.Error> errorCode;

        protected override string info { get { return "RequestRedeemActionReward"; } }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.RedeemActionRewardRequest(actionRewardId.value,
                (response) =>
                {
                    isSuccess.value = true;

                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "rewardResponse");
                    ClientAPI2Blackboard.Serialize(bb, response);

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                    // Do not use ApplyUserSyncInfo. becuase apply reward coin miss match.
                    // BlackboardQueryUtils.ApplyUserSyncInfo();

                    EndAction(true);
                },
                (error) =>
                {
                    isSuccess.value = false;
                    errorCode.value = error.errorCode;

                    switch(error.errorCode)
                    {
                        case ClientModels.Error.ALREADY_REDEEMED_ACTION_REWARD_ERROR:
                        {
                            EndAction(true);
                            break;
                        }
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }
    }
}
