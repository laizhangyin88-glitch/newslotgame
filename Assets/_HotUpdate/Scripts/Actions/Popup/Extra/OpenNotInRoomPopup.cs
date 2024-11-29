using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode;

namespace SlotMaker.Task.Actions
{

[Category("★ SlotMaker/Popup/Extra")]
public class OpenNotInRoomPopup : ActionTask<Transform>
{
    private const string ERROR_NOT_EXIST_ROOM = "ERROR_NOT_EXIST_ROOM";
    private const string BUTTON_OKAY = "BUTTON_OKAY";
    private const string ON_LEAVE_GAME_EVENT = "LeaveGame";
    private const string ON_CONTENT_EVENT = "OnContentEvent";

    protected override string info
    {
        get { return "Open not in room Popup"; }
    }

    protected override void OnExecute()
    {
        bool stringError = false;
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, ERROR_NOT_EXIST_ROOM, out stringError);
        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, BUTTON_OKAY, out stringError);

        info.callback1 = delegate
        {
            EventData eventData = new EventData(ON_LEAVE_GAME_EVENT);
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, eventData);
            // GraphOwner.SendGlobalEvent(ON_LEAVE_GAME_EVENT);
        };

        ErrorPopupHandler.Instance.OpenError(info);

        EndAction();
    }
}

}
