using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Blackboard/PendingAction")]
public class GetPendingAction : ActionTask<Blackboard>  
{
    public BBParameter<ActionType> actionType;
    public BBParameter<Blackboard> saveAs;
    public BBParameter<int>        currentPendingActionIndex;

    protected override string info
    {
        get { return string.Format("{0} = Get PendingAction {1}", saveAs, actionType.value); }
    }

    protected override void OnExecute()
    {
        List<Blackboard> bbList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "pendingAction");

        if (bbList == null)
        {
            EndAction(false);
        }
        else
        {
            for (int i = 0; i < bbList.Count; ++i)
            {
                var type = BlackboardUtils.FindVariable<ActionType>(bbList[i], "action/type");

                if (type != null && type.value == actionType.value)
                {
                    saveAs.value = bbList[i].GetVariable<Blackboard>("action").value;
                    currentPendingActionIndex.value = i;
                    EndAction();
                }
            }
        }
        EndAction(false);
    }
}

}
