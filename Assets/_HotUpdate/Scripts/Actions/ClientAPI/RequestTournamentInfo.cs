using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestTournamentInfo : ActionTask<Blackboard>
{
    protected override string info { get { return "Request Tournament Info"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.TournamentInfo(
        (response) =>
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "responseTournamentInfo");
            ClientAPI2Blackboard.Serialize(bb, response.info);

            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
