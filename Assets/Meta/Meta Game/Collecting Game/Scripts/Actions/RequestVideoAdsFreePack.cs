using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class RequestVideoAdsFreePack : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return "Request Video Ads Free Pack"; }
        }

        protected override void OnExecute()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);

            BagelCodeClientAPI.CollectingGamePackVideoAdsFreeRequest(metaGameInfo.id, 
                (response) =>
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "collectingGameInfo");

                    BlackboardUtils.SetOrCreateList(bb, "packList", response.packList, ClientAPI2Blackboard.Serialize);
                    BlackboardUtils.SetOrCreateValue(bb, "lastFreeChestVideoAdsClaimTimestamp", response.lastFreeChestVideoAdsClaimTimestamp);
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