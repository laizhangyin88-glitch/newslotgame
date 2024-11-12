using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class UsefcfsTicket : ActionTask
{
    public BBParameter<string> uri;
    public BBParameter<long> ticketId;
    public BBParameter<string> popupTitle;
    public BBParameter<string> popupContent;

    protected override string info { get { return "Usefcfs Ticket"; } }
    
    protected override void OnExecute()
    {
        bool stringError = false;

        BagelCodeClientAPI.UseFCFSTicket(ticketId.value, uri.value,
        (response) =>
        {
            Blackboard bb = agent.GetComponent<Blackboard>();
            ClientAPI2Blackboard.Serialize(bb, response);
            
            popupTitle.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_SUCCESS_TITLE", out stringError);
            popupContent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_SUCCESS_TEXT", out stringError);

            EndAction(true);
        },
        (error) =>
        {           
            popupTitle.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_FAIL_TITLE", out stringError);
            
            switch(error.errorCode)
            {
                case ClientModels.Error.NOT_EXIST_FCFS_TICKET_ERROR:
                    popupContent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_FAIL_EXPIRE_TEXT", out stringError);
                    break;
                case ClientModels.Error.ALREADY_USED_FCFS_TICKET_ERROR:
                    popupContent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_FAIL_ALREADY_USED_TEXT", out stringError);
                    break;
                case ClientModels.Error.NOT_ELIGIBLE_FCFS_TICKET_ERROR:
                    popupContent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_FAIL_NOT_ELIGIBLE_TEXT", out stringError);
                    break;
                case ClientModels.Error.FCFS_TICKET_EXPIRED_ERROR:
                    popupContent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_FCFS_FAIL_EXPIRE_TEXT", out stringError);
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }

            EndAction(true);
        });
    }
}

}
