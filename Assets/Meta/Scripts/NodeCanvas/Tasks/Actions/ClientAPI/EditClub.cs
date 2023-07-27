using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class EditClub : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> clubBB;
    public BBParameter<string>  message;
    public BBParameter<int>     watcher;
    public BBParameter<string>  symbol;
    public BBParameter<int>     minLevel;
    public BBParameter<int>     joinTypeIndex;

    public BBParameter<bool> isSuccess;

    protected override string info 
    {
        get { return "Edit Club"; }
    }

    protected override void OnExecute()
    {
        if(clubBB.value == null)
        {
            isSuccess.value = false;
            EndAction(true);
        }

        var clubID = clubBB.value.GetValue<long>("id");

        ClubJoinType joinType = joinTypeIndex.value == 0 ? ClubJoinType.PUBLIC : ClubJoinType.PRIVATE;

        BagelCodeClientAPI.EditClub(watcher.value, clubID, message.value, symbol.value, minLevel.value, joinType,
        (response) =>
        {
            if(agent == null) return;

            if(clubID == response.clubInfo.id)
            {
                var updateMessage = BlackboardUtils.FindVariable<string>(clubBB.value, "motd");
                var updateSymbol = BlackboardUtils.FindVariable<string>(clubBB.value, "symbol");
                var updateWatcher = BlackboardUtils.FindVariable<int>(clubBB.value, "watcher");
                var updateMinPlayerLevel = BlackboardUtils.FindVariable<int>(clubBB.value, "minPlayerLevel");
                var updateJoinType = BlackboardUtils.FindVariable<ClubJoinType>(clubBB.value, "joinType");

                updateMessage.value = response.clubInfo.motd;
                updateSymbol.value = response.clubInfo.symbol;
                updateWatcher.value = response.clubInfo.watcher;
                updateMinPlayerLevel.value = response.clubInfo.minPlayerLevel;
                updateJoinType.value = response.clubInfo.joinType;
            }

            isSuccess.value = true;
            EndAction(true);
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.INVALID_NAME_FORMAT_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    isSuccess.value = false;
                    EndAction(true);
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
