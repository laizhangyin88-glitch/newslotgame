using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Friend")]
public class DismissSuggestFriend : ActionTask <Blackboard> 
{
    public BBParameter<string> userId;

    protected override string info 
    {
        get 
        {
            return string.Format("Dismiss Suggested Friend {0}", userId);
        }
    }

    protected override void OnExecute()
    {
        BlackboardQueryUtils.RemoveSuggest(userId.value);
        BlackboardQueryUtils.UpdateCelebInfo();
        EndAction(true);
    }
}

}
