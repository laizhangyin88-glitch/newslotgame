using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Inbox")]
public class RemoveInboxItem : ActionTask <Blackboard> 
{
    public BBParameter<int> removeInboxID;

    protected override string info 
    {
        get 
        {
            return string.Format("Remove Inbox Item {0}", removeInboxID);
        }
    }

    protected override void OnExecute()
    {
        BlackboardQueryUtils.RemoveInboxItem(removeInboxID.value);
        BlackboardQueryUtils.UpdateCollectAllCredit();
        EndAction(true);
    }
}

}
