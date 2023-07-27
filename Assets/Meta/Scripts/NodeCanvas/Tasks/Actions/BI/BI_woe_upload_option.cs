using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_woe_upload_option : ActionTask
{
    public BBParameter<bool> state;

    protected override void OnExecute()
    {
        Analytics.CustomEvent("client_woe_upload_option", new Dictionary<string, object>
        {
            { "action", state.value ? "on" : "off" } 
        });

        EndAction();
    }
}

}
