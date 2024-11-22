using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class ValidateEmail : ActionTask
{
    public BBParameter<string> email;
    public BBParameter<string> errorPopupStringKey;
    public BBParameter<bool> useValidateEmail;

    protected override string info { get { return "Validate Email"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.SsoValidateEmail(email.value, useValidateEmail.value,
        (response) =>
        {
            Blackboard bb = agent.GetComponent<Blackboard>();
            ClientAPI2Blackboard.Serialize(bb, response);
            EndAction(true);
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.ALREADY_EXIST_EMAIL_ERROR:
                    errorPopupStringKey.value = "POPUP_JOIN_FAIL_TO_EXIST_EMAIL";
                    EndAction(false);
                    break;
                case ClientModels.Error.INVALID_PARAM_FORMAT_ERROR:
                    errorPopupStringKey.value = "POPUP_JOIN_FAIL_TO_EMAIL_FORMAT_ERROR";
                    EndAction(false);
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
