using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Friend")]
public class CheckMyFriend : ActionTask<Blackboard>
{
    [BlackboardOnly]
    public BBParameter<string> userId;
    public BBParameter<BagelCode.ClientModels.FriendType> saveType;
    public BBParameter<bool> saveAs;

    protected override string info
    {
        get { return "Check My Friend"; }
    }

    protected override void OnExecute()
    {
        
        List<Blackboard> requestedList = BlackboardQueryUtils.GetFriendList(true);
        Blackboard friendInfo = BlackboardQueryUtils.GetFriendInfo(userId.value, requestedList);
        saveAs.value = BlackboardQueryUtils.IsContainsUser(userId.value, requestedList);
        if(friendInfo != null)
        {
            saveType.value = friendInfo.GetValue<BagelCode.ClientModels.FriendType>("type");
        }
        EndAction(true);
    }
}

}
