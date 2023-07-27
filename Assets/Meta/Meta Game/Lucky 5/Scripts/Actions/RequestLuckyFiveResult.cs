using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestLuckyFiveResult : ActionTask<Blackboard>
{
    protected override string info { get { return "Request Lucky Five Result"; } }

    protected override void OnExecute()
    {
        EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.LUCKY_FIVE, true);

        BagelCodeClientAPI.LuckyFiveResult(metaGameInfo.id,
        (response) =>
        {
            if(agent != null)
            {
                var responseBB = BlackboardUtils.GetOrCreateBlackboard(agent, "response") as Blackboard;
                
                ClientAPI2Blackboard.Serialize(responseBB, response);
            }

            EndAction(true);
        },
        (error) =>
        {
            if(agent != null)
            {
                GlobalErrorHandler.GlobalError(error);
            }
        });
    }
}

}
