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
public class RequestSurvey : ActionTask
{
    public BBParameter<string> hash;
    public BBParameter<bool> isSuccess;
    public BBParameter<long> rewardCredit;
    public BBParameter<ClientModels.Error> errorCode;
    public BBParameter<GameObject> mainFSM;
    
    protected override string info { get { return "RequestSurvey"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.SurveyRequest(hash.value,
        (response) =>
        {
            isSuccess.value = true;
            rewardCredit.value = response.rewardCredit;
            
            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.ApplyUserSyncInfo();

            mainFSM.value = GetMainFSM();
            EndAction(true);
        },
        (error) =>
        {
            isSuccess.value = false;
            errorCode.value = error.errorCode;
            
            switch(error.errorCode)
            {
                case ClientModels.Error.ALREADY_CLAIMED_SURVEY_ERROR:
                {
                    mainFSM.value = GetMainFSM();
                    EndAction(true);
                    break;
                }
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }

    private GameObject GetMainFSM()
    {
        var InAppMessageManager = agent.GetComponent<Blackboard>().GetVariable<GameObject>("caller").value;
        var mainFSMObject = InAppMessageManager.GetComponent<Blackboard>().GetVariable<GameObject>("caller").value;
        return mainFSMObject;
    }
}

}

