using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckNull : ConditionTask<Blackboard>  
{
    public BBParameter<string> valueA;

    protected override string info
    {
        get { return valueA + " == NULL"; }
    }

    protected override bool OnCheck()
    {
        var variableA = BlackboardUtils.FindVariable(agent, valueA.value);
        return (variableA == null) || (variableA.value == null);
    }
}

[Category("★ SlotMaker/Blackboard")]
public class CheckNullObject : ConditionTask
{
    public BBObjectParameter valueA;
    
    protected override string info
    {
        get { return valueA + " == NULL"; }
    }
    
    protected override bool OnCheck()
    {
        return valueA.value == null;
    }
}

}
