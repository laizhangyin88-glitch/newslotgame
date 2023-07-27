using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class SaveSlotMachine : ActionTask<Blackboard>
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("Save {0} as {1}", slotMachine, saveAs); }
    }

    protected override void OnExecute()
    {
        string key = null;
        var bb = BlackboardUtils.FindBlackboard(agent, saveAs.value, ref key);

        var go = slotMachine.value.GetComponent<BaseSlotMachine>().Save().gameObject;
        go.transform.SetParent(bb.propertiesBindTarget.GetComponent<Transform>());

        var variable = bb.AddVariable(key, typeof(GameObject));
        variable.value = go;

        EndAction();
    }
}

}
