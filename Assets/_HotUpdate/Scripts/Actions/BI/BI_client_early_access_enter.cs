using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_early_access_enter : ActionTask<Blackboard>
{
    public BBParameter<string> enterType;

    protected override void OnExecute()
    {
        BICustomEvents.EarlyAccessEnter(enterType.value);

        EndAction();
    }
}

}
