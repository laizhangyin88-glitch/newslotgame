using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Keno.Tasks.Actions
{
[Category("★ SlotMaker/Keno")]
public class Keno_SaveTicketInstance : ActionTask<Blackboard>
{
    public BBParameter<KenoMediator> kenoMediator;
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("Save {0}.ticket as {1}", kenoMediator, saveAs); }
    }

    protected override void OnExecute()
    {
        string key = null;
        var bb = BlackboardUtils.FindBlackboard(agent, saveAs.value, ref key);

        var go = kenoMediator.value.KenoInstance.ticket.GetComponent<TicketInstance>().Save().gameObject;
        go.transform.SetParent(bb.propertiesBindTarget.GetComponent<Transform>());

        var variable = bb.AddVariable(key, typeof(GameObject));
        variable.value = go;

        EndAction();
    }
}

}
