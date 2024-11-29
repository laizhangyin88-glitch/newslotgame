using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context/Debug")]
public class PrintContextElement : ActionTask<ContextElement>
{
    public BBParameter<string> logName;

    protected override string info
    {
        get { return string.Format("Debug Context({0})", logName); }
    }

    protected override void OnExecute()
    {
#if DEV
        Debug.Log( string.Format("[ContextElement({0})][Action][Debug] {1}", logName, agent.ToString()) );
#endif
        EndAction();
    }
}

}
