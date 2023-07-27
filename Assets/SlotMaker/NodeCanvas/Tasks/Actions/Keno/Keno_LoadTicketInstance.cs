using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Keno.Tasks.Actions
{
[Category("★ SlotMaker/Keno")]
public class Keno_LoadTicketInstance : ActionTask<Blackboard>
{
    public BBParameter<KenoMediator> kenoMediator;
    public BBParameter<string> loadAs;

    protected override string info
    {
        get { return string.Format("Load {0}.ticket as {1}", kenoMediator, loadAs); }
    }

    protected override void OnExecute()
    {
        var go = BlackboardUtils.FindVariable<GameObject>(agent, loadAs.value).value;
        kenoMediator.value.KenoInstance.ticket.GetComponent<TicketInstance>().Load(go.GetComponent<TicketInstance>());

        EndAction();
    }
}

}
