using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/Blackboard Query")]
public class AddBlackboard : ActionTask<Blackboard>
{
    public BBParameter<string> bb;
    public BBParameter<string> location;

    protected override string info { get { return string.Format("Add {0} to {1}", bb, location); } }

    protected override void OnExecute()
    {
        var sourceBB = BlackboardUtils.FindVariable(agent, bb.value);

        if (sourceBB != null)
        {
            IBlackboard locationBB = null;
            string targetBBName = null;

            locationBB = BlackboardUtils.FindBlackboard(locationBB, location.value, ref targetBBName);

            IBlackboard target = BlackboardUtils.GetOrCreateBlackboard(locationBB, targetBBName);
            target.variables.Clear();

            string sourceInfo = ((Blackboard)sourceBB.value).Serialize();
            ((Blackboard)target).Deserialize(sourceInfo);
        }

        EndAction();
    }
}

}
