using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class RemoveAtBlackboardList : ActionTask
{
    [BlackboardOnly]
    public BBParameter<Blackboard> parentBB;
    public BBParameter<string> targetKey;
    public BBParameter<int> targetIndex;

    protected override string info
    {
        get 
        {
            return string.Format("Remove At {0} From {1} in {2}", targetIndex, targetKey, (parentBB == null || parentBB.value) ? MainBlackboard.Get() : parentBB );
        }
    }

    protected override void OnExecute()
    {
        if (parentBB == null || parentBB.value == null)
        {
            BlackboardUtils.RemoveAtBlackboardList((IBlackboard)MainBlackboard.Get(), targetKey.value, targetIndex.value);
        }
        else
        {
            BlackboardUtils.RemoveAtBlackboardList((IBlackboard)parentBB.value, targetKey.value, targetIndex.value);
        }

        EndAction();
    }
}

}
