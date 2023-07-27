using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.GemJackpot
{
    [Category("★ BagelCode/Meta Games/Gem Jackpot")]
    public class RequestGemJackpotCollectReward : ActionTask
    {
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Gem Jackpot Collect Reward"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;
            BagelCodeClientAPI.RequestGemJackpotCollectReward(
                (response) =>
                {
                    var bb = GemJackpotUtils.GemJackpotInfo;
                    ClientAPI2Blackboard.Serialize(bb, response);

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    saveAsSuccess.value = true;
                    EndAction();
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case ClientModels.Error.INVALID_GEM_JACKPOT_REQUEST_ERROR:
                            EndAction(false);
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }
    }
}