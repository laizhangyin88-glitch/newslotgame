using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_woe_enter : ActionTask
{
    protected override void OnExecute()
    {
        // Debug.LogError("woe_enter");

        Analytics.CustomEvent("client_woe_enter", new Dictionary<string, object>
        {
        });

        EndAction();
    }
}

}
