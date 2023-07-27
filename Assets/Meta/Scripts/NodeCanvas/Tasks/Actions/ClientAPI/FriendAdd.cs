using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class FriendAdd : ActionTask <Blackboard> 
{
    public BBParameter<string> userId;
    public BBParameter<string> result;

    protected override string info 
    { 
        get 
        { 
            return string.Format("Add friend {0} and {1} saves the result", userId, result);
        } 
    }

// TODO : Already Friend case. then just change accpted -> true 
    protected override void OnExecute()
    {
        if (userId.value != null)
        {
            BagelCodeClientAPI.FriendAdd(userId.value,
            (response) =>
            {
                // var friendList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "friendList").value;

                if (!BlackboardQueryUtils.UpdateFriendItemAsAccepted(userId.value))
                {
                    BlackboardQueryUtils.AddFriend(response.friendInfo);
                }
                
                bool error;
                result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "COMMON_ADD_FRIEND_OK", out error);
                
                BlackboardQueryUtils.UpdateCelebInfo();

                EndAction(true);

            },
            (error) =>
            {
                bool stringError;
                switch(error.errorCode) 
                {
                    case BagelCode.ClientModels.Error.TARGET_ALREADY_FRIEND_ERROR:
                        result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "COMMON_ADD_FRIEND_"+error.errorCode, out stringError);
                        break;
                    case BagelCode.ClientModels.Error.FRIEND_COUNT_MAX_ERROR:
                        result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "ERROR_EXCEED_MAX_FRIEND", out stringError);
                        break;                            
                    case BagelCode.ClientModels.Error.CANT_ADD_MYSELF_FRIEND_ERROR:
                        result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "ERROR_CANT_ADD_MYSELF_FRIEND", out stringError);
                        break;                                                
                    default:
                        result.value = "<style=body>OTHER ERROR</style>";
                        break;
                }
                EndAction(true);
            });
        }
        else
        {
            Debug.LogError("No UserId found." + agent.gameObject.name);
        }
    }
}

}
