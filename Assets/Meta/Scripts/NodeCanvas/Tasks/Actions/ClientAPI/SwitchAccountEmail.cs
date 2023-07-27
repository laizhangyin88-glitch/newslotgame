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
public class SwitchAccountEmail : ActionTask<Blackboard>
{
    public BBParameter<string> email;
    public BBParameter<string> validationCode;
    protected override string info { get { return "Switch Account Email"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.SsoSwitchAccountEmail(email.value, validationCode.value,
        (response) =>
        {
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
