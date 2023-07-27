using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestEpicAlbumEnter : ActionTask <Blackboard>
{
    public BBParameter<bool> success;
    
    protected override string info
    { 
        get 
        { 
            return "Request Epic Album Enter";
        } 
    }
    protected override void OnExecute()
    {
        BagelCodeClientAPI.RequestEpicAlbumEnter(
        (response) =>
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "epicAlbumEnterInfo");
            ClientAPI2Blackboard.Serialize(bb, response);
            success.value = true;
            EndAction();
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
