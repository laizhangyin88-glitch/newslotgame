using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class Nudge : ActionTask <Blackboard> 
{
    public BBParameter<Blackboard> userInfo;
    public BBParameter<bool>       isFriend;

    private const string SUCCESS_NUDGE_EVENTNAME = "OnSuccessNudgeEvent";
    private const string NOT_FRIEND_EVENTNAME = "OnNudgeNotFriendEvent";
    private const string IN_NIGHT_EVENTNAME = "OnNudgeInNightEvent";

    protected override string info 
    {
        get 
        {
            return string.Format("Nudge User");
        }
    }

    protected override void OnExecute()
    {
        bool isNudgeEnabled = false;

        long userClubID = BlackboardUtils.FindVariable<long>(userInfo.value, "clubId").value;
        long meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId").value;

        if(meClubID != 0 && meClubID == userClubID)
            isNudgeEnabled = true;
        else if (isFriend.value)
            isNudgeEnabled = true;

        if(isNudgeEnabled == false)
        {
            QuickSendEvent(NOT_FRIEND_EVENTNAME);
            EndAction(true);
            return;
        }

        int timeOffset = BlackboardUtils.FindVariable<int>(userInfo.value, "timezoneOffset").value;
        DateTime currentDate = BagelCode.TimeUtils.GetCurrentDateTime();
        currentDate = currentDate.AddHours(timeOffset);
        int hours = currentDate.Hour;

        if (hours >= 22 || hours < 9) // pm 10 - am 9 
        {
            QuickSendEvent(IN_NIGHT_EVENTNAME);
            EndAction(true);
            return;            
        }
        
        var userId = BlackboardUtils.FindVariable<string>(agent, "_userId");

        BagelCodeClientAPI.Nudge(userId.value,
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
            QuickSendEvent(SUCCESS_NUDGE_EVENTNAME);
            EndAction(true);
            
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }

    private void QuickSendEvent(string eventName)
    {
        GraphOwner owner = null;

        if(ownerSystem != null)
            owner = ownerSystem.agent.GetComponent<GraphOwner>();

        if(owner != null)
        {
            owner.SendEvent(eventName);
        }
        else
        {
            SendEvent(eventName);
        }        
    }

}

}
