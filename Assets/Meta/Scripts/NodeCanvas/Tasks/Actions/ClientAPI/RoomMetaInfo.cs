#define NEW_NET
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

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

#if NEW_NET

            NetManager.Instance.Post(RPCName.metaInfo, new Dictionary<string, object>(),
                (res) =>
                {
                    string resStr = res.ToString();

                    if (agent == null) return;

                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/room_meta_info_response");
                    ClientModels.RoomMetaInfoResponse response = JsonUtility.FromJson<ClientModels.RoomMetaInfoResponse>(jsn8.text);

                    string str1 = res["room"].ToString();

                    response.room = JsonUtility.FromJson<Room>(NetManager.ChangeJsonKeyToCameCase(str1));

                    string str2 = JsonUtility.ToJson(response.room);

                    var roomBB = BlackboardUtils.FindVariable<Blackboard>(ContentBlackboard.Get(), "room");

                    if (roomBB != null && roomBB.value != null)
                    {
#if UNITY_EDITOR
                        var seatEditorMode = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "seatEditorMode");
                        if (seatEditorMode == null || !seatEditorMode.value)
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
                    if (agent == null) return;
                    // Desc : don't use error. error is null.
                    EndAction(true);
                }
            );
            return;
#endif


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
