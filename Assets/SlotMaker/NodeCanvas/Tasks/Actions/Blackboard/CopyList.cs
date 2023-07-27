using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
[Description("Note that it copies shallowly")]
public class CopyList<T> : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    [BlackboardOnly]
    public BBParameter<List<T>> valueB;

    protected override string info
    {
        get { return string.Format("{0}[] = {1}[]", valueA, valueB); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<List<T>>(agent, valueA.value);
        variableA.value = new List<T>();
        for (int i = 0; i < valueB.value.Count; ++i)
        {
            variableA.value.Add(valueB.value[i]);
        }

        EndAction();

    }
}

}
