using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_common_click : ActionTask<Blackboard>
{
    public BBParameter<string> eventName;

    protected override string info
    {
        get { return string.Format("BI Common Click {0}", eventName); }
    }

    protected override void OnExecute()
    {
        Analytics.CustomEvent(eventName.value, null);

        EndAction();
    }
}

}
