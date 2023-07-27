using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_item_click : ActionTask<Blackboard>
{
    public BBParameter<string> productBBValue;

    public BBParameter<string> contextID;
    public BBParameter<bool> isEvent;

    protected override void OnExecute()
    {
        if (string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);
        BiEventUtils.ItemClick( productBB.value,
                                contextID.value,
                                isEvent == null ? false : isEvent.value
        );

        EndAction();
    }
}

}
