using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class UpdateContext : ActionTask<ContextElement>
{
    public BBParameter<bool> isForceUpdate;

    protected override string info
    {
    	get { return string.Format("{0}.UpdateContext({1})", agentInfo, isForceUpdate); }
    }

    protected override void OnExecute()
    {
    	agent.UpdateContext(isForceUpdate.value);
        EndAction(true);
    }
}

}