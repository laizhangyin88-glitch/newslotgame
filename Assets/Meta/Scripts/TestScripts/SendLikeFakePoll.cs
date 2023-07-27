using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    
[Category("★ BagelCode/Seat")]
public class SendLikeFakePoll : ActionTask<Blackboard>
{
    public BBParameter<string> currentUserId;
    public BBParameter<string> targetUserId;

    protected override void OnExecute()
    {
        string userId   = BlackboardUtils.FindVariable<string>(agent, currentUserId.value).value;
        string targetId = BlackboardUtils.FindVariable<string>(agent, targetUserId.value).value;

        Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard( agent, "LikePoll" ) as Blackboard;

        BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.LIKE);
        BlackboardUtils.SetOrCreateValue<string>(newBB,  "userId", userId);
        BlackboardUtils.SetOrCreateValue<string>(newBB, "targetUserId", targetId);

        GraphOwner.SendGlobalEvent<Blackboard>("USE_LIKE", newBB);

        Debug.Log("SEAT_EVENT: " + "SEND EVENT");

        EndAction();
    }
}

}
