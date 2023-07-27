using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class AddBlackboardList : ActionTask<Blackboard>
{
    public BBParameter<string> parentBlackboard;
    public BBParameter<bool>   parentSelf;

    public BBParameter<string> key;
    public BBParameter<IBlackboard> targetBlackboard;

    protected override string info 
    { 
        get 
        { 
            return string.Format("Add elment to {0} at {1} blackboard", key.value, parentBlackboard.value);     
        } 
    }
    
    protected override void OnExecute()
    {      
        IBlackboard parentBB;

        if(parentSelf.value)
        {
            parentBB = agent;
        }
        else
        {
            if (string.IsNullOrEmpty(parentBlackboard.value))
            {
                parentBB = MainBlackboard.Get();
            }
            else
            {
                parentBB = BlackboardUtils.FindVariable<Blackboard>(agent, parentBlackboard.value).value;
            }
        }

        if (targetBlackboard.value == null)
        {
            BlackboardUtils.GetOrCreateBlackboardList(parentBB, key.value);
        }
        else
        {
            BlackboardUtils.AddToBlackboardList( parentBB, key.value, targetBlackboard.value );     
        }      

        EndAction();
    }
}

}
