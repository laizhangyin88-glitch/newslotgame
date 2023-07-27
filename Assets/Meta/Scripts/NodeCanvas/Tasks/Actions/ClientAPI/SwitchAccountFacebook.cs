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
public class SwitchAccountFacebook : ActionTask<Blackboard>
{
    public BBParameter<string> id;
    protected override string info { get { return "Switch Account Facebook"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.SsoSwitchAccountFacebook(id.value,
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
