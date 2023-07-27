using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class RequestRedeemScratcher : ActionTask<Blackboard>
    {
        public BBParameter<int> scratcherId;
        
        protected override string info
        {
            get { return "Request Redeem Scratcher"; }
        }

        protected override void OnExecute()
        {
            if (scratcherId != null)
            {
                EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
                BagelCodeClientAPI.CollectingGameScratcherRedeemRequest(metaGameInfo.id, scratcherId.value,
                    (response) =>
                    {
                        BlackboardQueryUtils.UpdateCollectingGameScratcher(response.updatedScratcherInfo.scratcherId, response.updatedScratcherInfo);
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                        var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "_scratcherRewardResult");
                        ClientAPI2Blackboard.Serialize(bb, response.rewardResult);
                        BlackboardQueryUtils.ApplyRewardResult((Blackboard)bb);

                        BlackboardQueryUtils.UpdateCollectingGameStatus();

                        EndAction(true);
                    },
                    (error) =>
                    {
                        EndAction(false);
                        GlobalErrorHandler.GlobalError(error);
                    });
            }

        }
    }
}