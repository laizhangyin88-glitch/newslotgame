using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class DailyMegaSpin : ActionTask<Blackboard>
{
    protected override string info
    {
        get { return "Request Daily Mega Spin"; }
    }

    protected override void OnExecute()
    {
        var eventId = PassiveEventManager.Instance.GetActiveEventID(EventInfoType.DAILY_WHEEL_MULTIPLY);

        BagelCodeClientAPI.DailyMegaSpin(eventId,
        (response) =>
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");
            BlackboardUtils.ClearBlackboard(bb);

            ClientAPI2Blackboard.Serialize(bb, response);

            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

            BlackboardQueryUtils.AddCoins(response.earnCredit);

            BlackboardQueryUtils.SetDailySpinCount(response.remainSpinCount, MetaJackpotType.DAILY_MEGA_WHEEL);

            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
