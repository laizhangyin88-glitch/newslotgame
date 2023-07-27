using UnityEngine;
using System;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/GameObject")]
public class CheckGameObjectName : ConditionTask<Blackboard>
{
    public BBParameter<GameObject> valueA;
    public BBParameter<string> valueB;

    protected override string info
    {
        get { return valueA + ".name == " + valueB; }
    }

    protected override bool OnCheck()
    {
        return valueA.value.name.Equals(valueB.value, StringComparison.Ordinal);
    }
}

}
