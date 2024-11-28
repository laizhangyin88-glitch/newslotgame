using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode;

namespace SlotMaker.Task.Actions
{

[Category("★ SlotMaker/Popup/Extra")]
public class OpenLobbySessionExpiredPopup : ActionTask<Transform>
{
    private const string ERROR_SESSION_EXPIRED = "ERROR_SESSION_EXPIRED";
    private const string BUTTON_OKAY = "BUTTON_OKAY";

    protected override string info
    {
        get { return "Open lobby session expired Popup"; }
    }

    protected override void OnExecute()
    {
        bool stringError = false;
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.SystemReset;
        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, ERROR_SESSION_EXPIRED, out stringError);
        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, BUTTON_OKAY, out stringError);

        ErrorPopupHandler.Instance.OpenError(info);

        EndAction();
    }
}

}
