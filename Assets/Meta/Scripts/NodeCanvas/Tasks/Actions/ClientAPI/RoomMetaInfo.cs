using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RoomMetaInfo : ActionTask <Blackboard> 
{
    protected override string info 
    {
        get 
        {
            return "Request Room Meta Info";
        }
    }

    protected override void OnExecute()
    {
        var roomID = BlackboardUtils.FindVariable<string>(ContentBlackboard.Get(), "room/roomId");

        if(roomID != null && !string.IsNullOrEmpty(roomID.value))
        {
            BagelCodeClientAPI.RoomMetaInfo(roomID.value,
            (response) =>
            {
                if(agent == null) return;

                var roomBB =  BlackboardUtils.FindVariable<Blackboard>(ContentBlackboard.Get(), "room");

                if(roomBB != null && roomBB.value != null)
                {
#if UNITY_EDITOR
                    var seatEditorMode = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "seatEditorMode");
                    if(seatEditorMode == null || !seatEditorMode.value)
                    {
                        ClientAPI2Blackboard.Serialize(roomBB.value, response.room);
                        BlackboardQueryUtils.UpdateSeat(response.room);
                    }
#else
                    ClientAPI2Blackboard.Serialize(roomBB.value, response.room);
                    BlackboardQueryUtils.UpdateSeat(response.room);
#endif

                    BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
                }

                EndAction(true);
            },
            (error) =>
            {
                if(agent == null) return;
                
                // Desc : don't use error. error is null.

                EndAction(true);
            });
        }
        else
        {
            if(agent == null) return;

            EndAction(false);
        }
    }
}

}
