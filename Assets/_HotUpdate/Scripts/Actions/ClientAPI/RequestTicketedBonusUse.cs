using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.Contents;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestTicketedBonusUse : ActionTask<Blackboard>
{
    public BBParameter<string> ticketIDValue;
    public BBParameter<string> targetBBPath = "./ticketBonus";

    protected override string info { get { return "Request Ticketed Bonus Use"; } }

    protected override void OnExecute()
    {
        var ticketID = BlackboardUtils.FindVariable<int>(agent, ticketIDValue.value);

        BagelCodeClientAPI.TicketedBonusUse(ticketID.value,
        (response) =>
        {
            string variableName = null;
            var bb = BlackboardUtils.FindBlackboard(agent, targetBBPath.value, ref variableName);
            var useResponse = BlackboardUtils.GetOrCreateBlackboard(bb, variableName);
            ClientAPI2Blackboard.Serialize(useResponse, response);

            // BlackboardJson.DeserializeObject(
            //     useResponse,
            //     useResponse.GetValue<string>("contents"),
            //     "TicketedBonus"
            // );

            BlackboardUtils.SetOrCreateValue<int>(useResponse, "requestType", (int)ContentsRequestType.TicketedBonusStart);
            ContentsSerializer.Deserialize(useResponse);

            BlackboardUtils.SetOrCreateValue<int>(useResponse, "ticketId", ticketID.value);

            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

            EndAction();
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
