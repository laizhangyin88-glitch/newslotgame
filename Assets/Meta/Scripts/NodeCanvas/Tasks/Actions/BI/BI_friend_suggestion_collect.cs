using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_friend_suggestion_collect : ActionTask<Blackboard>
{
    public BBParameter<int> requestCount;
    public BBParameter<int> beforeFriendCount;
    public BBParameter<string> type;

    protected override void OnExecute()
    {
        var addEncourage = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "friendAddEncourageListResponse").value;

        Analytics.CustomEvent("client_friend_suggestion_collect", new Dictionary<string, object>
        {
            { "earn_coin", addEncourage.GetValue<long>("earnCredit") },
            { "friend_request_count", requestCount.value },
            { "friend_count", beforeFriendCount.value },
            { "type", type.value }
        });

        EndAction();
    }
}

}
