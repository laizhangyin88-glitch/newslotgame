using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
public class GetBlackboardValueAtList<T> : ActionTask<Blackboard>  
{
    public BBParameter<string> valueA;

    public BBParameter<int> index;
    public BBParameter<bool> fromLast;
    
    [BlackboardOnly]
    public BBParameter<T> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}[{2}]", saveAs, valueA, index); }
    }

    protected override void OnExecute()
    {
        var variable = BlackboardUtils.FindVariable<List<T>>(agent, valueA.value);
        if (variable == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
            EndAction(false);
        }
        else
        {
            if(variable.value.Count > 0)
            {
                if(fromLast.value == true)
                {
                    saveAs.value = variable.value[variable.value.Count - 1 - index.value];
                }
                else
                {
                    saveAs.value = variable.value[index.value];
                }
            }

            EndAction();
        }
    }
}

}
