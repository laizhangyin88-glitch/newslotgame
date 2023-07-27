using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestLuckyFiveClaim : ActionTask<Blackboard>
{
    protected override string info { get { return "Request Lucky Five Claim"; } }

    protected override void OnExecute()
    {
        EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.LUCKY_FIVE, true);

        BagelCodeClientAPI.LuckyFiveClaim(metaGameInfo.id,
        (response) =>
        {
            if(agent != null)
            {
                BlackboardQueryUtils.LuckyFiveClaim();
                BlackboardQueryUtils.AddCoins(response.earnCredit);
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
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
