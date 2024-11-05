using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestWallOfEpicRegister : ActionTask<Blackboard>
{
    public BBParameter<int> gameID;
    public BBParameter<long> betCoins;
    public BBParameter<long> winCoins;

    public BBParameter<int> woeId;

    protected override string info { get { return "Request Wall Of Epic Register"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.WallOfEpicRegister(gameID.value, betCoins.value, winCoins.value,
        (response) =>
        {
            if(agent != null)
            {
                woeId.value = response.woeInfo.id;
                
                if(BlackboardQueryUtils.CheckUsableEpicAlbum(response.woeInfo.gameId))
                {
                    var earlyAccessInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(response.woeInfo.gameId);
                    if (BlackboardQueryUtils.CheckIfNewEpicRecord(response.woeInfo.gameId, winCoins.value) && earlyAccessInfo == null)
                    {
                        BlackboardQueryUtils.SetNewWOE(response.woeInfo.id, true);
                        BlackboardQueryUtils.SetNewWOEForCategory(response.woeInfo.id, true);
                    }

                    BlackboardQueryUtils.UpdateWOEInfo(response.woeInfo);
                    // BlackboardUtils.SetOrCreateList(MainBlackboard.Get(), "woeInfoList", response.woeInfoList, ClientAPI2Blackboard.Serialize);
                }
            }
            EndAction();
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
