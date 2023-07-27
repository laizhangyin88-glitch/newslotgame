using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestLuckyFiveWinList : ActionTask<Blackboard>
{
    protected override string info { get { return "Request Lucky Five Win List"; } }

    protected override void OnExecute()
    {
        EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.LUCKY_FIVE, true);

        BagelCodeClientAPI.LuckyFiveWinList(metaGameInfo.id,
        (response) =>
        {
            if(agent != null)
            {
                var winListResponse = BlackboardUtils.GetOrCreateBlackboard(agent, "winListResponse");

                ClientAPI2Blackboard.Serialize(winListResponse, response);
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
