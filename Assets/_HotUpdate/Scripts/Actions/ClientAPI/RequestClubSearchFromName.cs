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
public class RequestClubSearchFromName : ActionTask<Blackboard> 
{
    public BBParameter<string> searchName;
    public BBParameter<bool> isSuccess;
    public BBParameter<string> emptyStringKey;

    public BBParameter<string> biContextID;

    protected override string info 
    {
        get { return "Club Search From Name"; }
    }

    protected override void OnExecute()
    {
        isSuccess.value = false;

        if(searchName.value.Length < 2)
        {
            var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "response/clubList");
            variable.value.Clear();

            emptyStringKey.value = "CLUB_SEARCH_ERROR_TWO_CHARACTERS";
            isSuccess.value = true;
            EndAction(true);

            return;
        }

        biContextID.value = "";

        BagelCodeClientAPI.ClubSearchFromName(searchName.value,
        (response) =>
        {
            if(agent != null)
            {
                // override club join list. 
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                biContextID.value = response.analyticContextId;

                emptyStringKey.value = "CLUB_EMPTY_SEARCH";
                isSuccess.value = true;
                EndAction(true);
            }
        },
        (error) =>
        {switch(error.errorCode)
            {
                case ClientModels.Error.INVALID_NAME_FORMAT_ERROR:
                    if(agent != null)
                    {
                        var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "response/clubList");
                        variable.value.Clear();

                        emptyStringKey.value = "CLUB_SEARCH_ERROR_TWO_CHARACTERS";
                        isSuccess.value = true;
                        EndAction(true);
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
