using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using NodeCanvas;
using NodeCanvas.BehaviourTrees;

namespace SlotMaker.BehaviourTrees
{

[Category("★ SlotMaker/Decorators")]
[Color("ff1493")]
[Description("Returns Running until the assigned condition becomes true for AutoUpdate")]
// [Icon("WaitUntil")]
[Icon("Halt")]
public class AutoUpdateWaitUntil : BTDecorator, ITaskAssignable<ConditionTask>
{
    [SerializeField]
    private ConditionTask _condition;
    private bool accessed;

    public NodeCanvas.Framework.Task task{
        get {return condition;}
        set {condition = (ConditionTask)value;}
    }

    private NodeCanvas.Framework.ConditionTask condition{
        get {return _condition;}
        set {_condition = value;}
    }

    protected override Status OnExecute(Component agent, IBlackboard blackboard){

        if (decoratedConnection == null)
            return Status.Resting;

        if (condition == null)
            return decoratedConnection.Execute(agent, blackboard);

        if ( accessed ) return decoratedConnection.Execute(agent, blackboard);

        if (status == Status.Resting)
            condition.Enable(agent, blackboard);

        if (condition.CheckCondition(agent, blackboard))
            accessed = true;

        return accessed? decoratedConnection.Execute(agent, blackboard) : Status.Running;
    }

    protected override void OnReset(){
        accessed = false;
        
        if (condition.isActive)
            condition.Disable();
    }
}

}
