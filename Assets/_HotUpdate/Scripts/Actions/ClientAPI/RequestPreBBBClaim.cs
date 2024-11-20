using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestPreBBBClaim : ActionTask<Blackboard>
{
    protected override string info { get { return "Request Pre BBB Claim"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.PreBBBClaime(
        (response) =>
        {
            if(agent != null)
            {
                var claimResponse = BlackboardUtils.GetOrCreateBlackboard(agent, "claimResponse");

                ClientAPI2Blackboard.Serialize(claimResponse, response);
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "hasPreBbbReward", false);
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
