using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class RequestFreePack : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return "Request Free Pack"; }
        }

        protected override void OnExecute()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);

            BagelCodeClientAPI.CollectingGamePackFreeRequest(metaGameInfo.id, 
                (response) =>
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "collectingGameInfo");

                    BlackboardUtils.SetOrCreateList(bb, "packList", response.packList, ClientAPI2Blackboard.Serialize);
                    BlackboardUtils.SetOrCreateValue(bb, "lastFreePackCollectTimestamp", response.lastFreePackCollectTimestamp);
                    BlackboardUtils.SetOrCreateValue(bb, "freePackInfo", response.freePackInfo);
                    
                    BlackboardQueryUtils.AddTotalPossessions(1);

                    EndAction(true);
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);
                });
        }
    }
}