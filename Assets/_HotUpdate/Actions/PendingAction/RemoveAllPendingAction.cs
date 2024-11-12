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
public class RemoveAllPendingAction : ActionTask<Blackboard>  
{
    public BBParameter<ActionType> removeActionType;

    protected override string info
    {
        get
        {
            if(removeActionType.value == ActionType.UNKNOWN)
            {
                return string.Format("Remove All Pending Actions");
            }

            return string.Format("Remove All {0} type Pending Actions", removeActionType.value);
        }
    }

    protected override void OnExecute()
    {
        List<Blackboard> bbList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "pendingAction");

        if (bbList != null)
        {
            if(removeActionType.value == ActionType.UNKNOWN)
            {
                BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "pendingAction");
            }
            else
            {
                for(int i = 0; i < bbList.Count;)
                {
                    var type = BlackboardUtils.FindVariable<ActionType>(bbList[i], "action/type");

                    if (type != null && type.value == removeActionType.value)
                    {
                        GameObject.Destroy(bbList[i].gameObject);
                        bbList.RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
        }

        EndAction();
    }
}

}
