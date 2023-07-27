using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class CheckCreateClubException : ActionTask<Blackboard>
{
    public BBParameter<string>  clubName;
    public BBParameter<string>  message;
    public BBParameter<string>  symbol;

    private bool isCreatable;
    private string alertMessage;

    protected override string info
    {
        get { return "Check Create Club Exception"; }
    }

    protected override void OnExecute()
    {
        isCreatable = false;
        alertMessage = null;

        if(string.IsNullOrEmpty(clubName.value))
        {
            alertMessage = "POPUP_CLUB_CREATE_ALERT_NAME";
        }
        else if(string.IsNullOrEmpty(message.value))
        {
            alertMessage = "POPUP_CLUB_CREATE_ALERT_MESSAGE";
        }
        else if(string.IsNullOrEmpty(symbol.value))
        {
            alertMessage = "POPUP_CLUB_CREATE_ALERT_SYMBOL";
        }
        else
        {
            string filteredName = StringTable.BadWordFilter(clubName.value);
            if(filteredName != clubName.value)
            {
                alertMessage = "POPUP_CLUB_CREATE_ALERT_BADWORD";
            }
            else
            {
                // Creatable
                isCreatable = true;
            }
        }

        if(isCreatable == false)
        {
            ErrorPopupInfo info = new ErrorPopupInfo();

            bool stringError = false;
            info.type = ErrorPopupType.OK;
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, alertMessage, out stringError);
            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
            
            ErrorPopupHandler.Instance.OpenError(info);
        }

        EndAction(isCreatable);
    }
}

}
