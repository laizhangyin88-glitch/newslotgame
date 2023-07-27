using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
public class GetBlackboardValueAtDictionary<T> : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<string> key;

    [BlackboardOnly]
    public BBParameter<T> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}[{2}]", saveAs, valueA, key); }
    }

    protected override void OnExecute()
    {
        var variable = BlackboardUtils.FindVariable<Dictionary<string,T>>(agent, valueA.value);
        if (variable == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
            EndAction(false);
        }
        else
        {
            T result;
            if (!variable.value.TryGetValue(key.value, out result))
            {
                EndAction(false);
            }
            else 
            {
                saveAs.value = result;
                EndAction();
            }                
        }
    }
}

}
