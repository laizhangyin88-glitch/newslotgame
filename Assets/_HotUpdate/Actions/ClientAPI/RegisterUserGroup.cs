using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RegisterUserGroup : ActionTask<Blackboard>
{
    public BBParameter<string>  targetGroupIDValue;

    protected override string info { get { return "Register User Group"; } }

    protected override void OnExecute()
    {
        var groupID = BlackboardUtils.FindVariable<int>(agent, targetGroupIDValue.value);

        if(groupID != null)
        {
            BagelCodeClientAPI.RegisterUserGroup(groupID.value,
            (response) =>
            {
                {
                    bool stringError = false;
                    
                    ErrorPopupInfo info = new ErrorPopupInfo();
                    
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_REGIST_GROUP_SUCCESS", out stringError);
                    info.type = ErrorPopupType.OK;
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                    
                    ErrorPopupHandler.Instance.OpenError(info);
                }

                EndAction();

            },
            (error) =>
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.ALREADY_EXIST_IN_USER_GROUP_ERROR:
                        {
                            bool stringError = false;
                    
                            ErrorPopupInfo info = new ErrorPopupInfo();
                            
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_REGIST_GROUP_ALREADY", out stringError);
                            info.type = ErrorPopupType.OK;
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                            
                            ErrorPopupHandler.Instance.OpenError(info);
                            
                            EndAction();
                        }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });
        }
        else
        {
            EndAction();
        }
    }
}

}
