using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class LoadSlotMachine : ActionTask<Blackboard>
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<string> loadAs;

    protected override string info
    {
        get { return string.Format("Load {0} as {1}", slotMachine, loadAs); }
    }

    protected override void OnExecute()
    {
        var go = BlackboardUtils.FindVariable<GameObject>(agent, loadAs.value).value;
        slotMachine.value.GetComponent<BaseSlotMachine>().Load(go.GetComponent<BaseSlotMachine>());

        EndAction();
    }
}

}
