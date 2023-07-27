using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class RequestOpenPack : ActionTask<Blackboard>
    {
        public BBParameter<int> chestId;
        public BBParameter<int> chestCount;
        
        protected override string info
        {
            get { return "Request Open Pack"; }
        }

        protected override void OnExecute()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);

            if (chestId != null)
            {
                BagelCodeClientAPI.CollectingGamePackOpenRequest(metaGameInfo.id, chestId.value,
                    (response) =>
                    {
                        chestCount.value = BlackboardQueryUtils.GetPackInfo(chestId.value).GetValue<int>("possessions");
                        
                        var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "collectingGameInfo");

                        BlackboardUtils.SetOrCreateList(bb, "packOpenResultList", response.packOpenResultList, ClientAPI2Blackboard.Serialize);
                        List<Blackboard> packOpenResultList = BlackboardQueryUtils.GetResultPieceList();
                        
                        Blackboard pieceInfo;
                        for (int i = 0; i < response.packOpenResultList.Count; i++)
                        {
                            pieceInfo = BlackboardQueryUtils.GetPieceInfo(response.packOpenResultList[i].pieceId);
                            if (pieceInfo.GetValue<int>("possessions") == 0)
                            {
                                packOpenResultList[i].SetValue("new", true);
                            }
                        }
                        
                        BlackboardUtils.SetOrCreateList(bb, "scratcherList", response.scratcherList, ClientAPI2Blackboard.Serialize);
                        BlackboardUtils.SetOrCreateList(bb, "packList", response.packList, ClientAPI2Blackboard.Serialize);
//                        BlackboardUtils.SetOrCreateList(bb, "converterPieceList", response.packOpenResultList, ClientAPI2Blackboard.Serialize);

                        BlackboardQueryUtils.UpdateCollectingGameStatus();
                        
                        for (int j = 0; j < response.packOpenResultList.Count; j++)
                            BlackboardQueryUtils.GetPieceNewBadge(response.packOpenResultList[j].pieceId).SetValue("new", true);

//                        packImageId.value = BlackboardQueryUtils.GetPackInfo(packId.value).GetValue<int>("packImageId");
                        
                        EndAction(true);
                    },
                    (error) =>
                    {
                        GlobalErrorHandler.GlobalError(error);
                    });
                
            }
        }
    }
}