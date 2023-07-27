using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
public class ExtractBlackboardList<T> : ActionTask<Blackboard>
{
    public BBParameter<string> targetList;
    public BBParameter<string> key;
    [BlackboardOnly]
    public BBParameter<List<T>> saveAs;

    protected override string info
    {
        get { return string.Format("{0}[i] = {1}[i].{2}", saveAs, targetList, key); }
    }

    protected override void OnExecute()
    {
        List<Blackboard> bbList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, targetList.value).value;
        List<T> list = new List<T>();
        if (bbList == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + targetList.value);
            EndAction(false);
        }
        else
        {
            foreach(Blackboard bb in bbList) {
                list.Add(BlackboardUtils.FindVariable<T>(bb, key.value).value);
            }
            saveAs.value = list;
            EndAction();
        }
    }
}

}
