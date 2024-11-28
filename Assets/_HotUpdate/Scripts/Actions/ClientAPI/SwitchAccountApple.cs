using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class SwitchAccountApple : ActionTask<Blackboard>
{
    public BBParameter<string> tokenID;
    protected override string info { get { return "Switch Account Apple"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.SsoSwitchAccountApple(tokenID.value,
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);
            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
            EndAction(true);
        });
    }
}

}
