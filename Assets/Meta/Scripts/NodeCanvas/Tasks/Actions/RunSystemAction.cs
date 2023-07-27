using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class RunSystemAction : ActionTask
{
    public BBParameter<System.Action> action;

    protected override string info
    {
        get
        {
            return string.Format("Run System.Action");
        }
    }

    protected override void OnExecute()
    {
        action.value();
    }

}

}
