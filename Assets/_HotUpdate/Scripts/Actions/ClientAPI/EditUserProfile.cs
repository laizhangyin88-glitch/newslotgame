using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class EditUserProfile : ActionTask <Blackboard> 
{
	public BBParameter<string> userName;
	public BBParameter<BagelCode.ClientModels.Gender> gender;
	public BBParameter<int> age;
	public BBParameter<string> countryCode;
	public BBParameter<string> message;

    public BBParameter<bool> isSuccess;
	public BBParameter<ClientModels.Error> error;

	protected override string info 
	{ 
		get 
		{ 
			return string.Format("Edit user info");
		} 
	}
	protected override void OnExecute()
	{
        if (string.IsNullOrEmpty(userName.value.Trim()))
        {
            error.value = ClientModels.Error.UNKNOWN;
            isSuccess.value = false;
            EndAction(true);
            return;
        }

        BagelCodeClientAPI.EditUserProfile(userName.value, gender.value, age.value, countryCode.value, message.value, false,
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);

            isSuccess.value = true;
            EndAction(true);
        },
        (serverError) =>
        {
            switch(serverError.errorCode)
            {
                case ClientModels.Error.UTF8_FORMAT_ERROR:
                    error.value = ClientModels.Error.UTF8_FORMAT_ERROR;
                    isSuccess.value = false;
                    EndAction(true);
                    break;
                default:
                    GlobalErrorHandler.GlobalError(serverError);
                    break;
            }
        });
	}
}

}
