using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class GetContextElementAt : ActionTask<ContextElement>
{
    public BBParameter<int> index;
    public BBParameter<bool> fromLast = false;
    public BBParameter<ContextElement> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}[{2}]", saveAs, agentInfo, index.value); } 
    }

    protected override void OnExecute()
    {
    	if(agent.ChildCount <= index.value || index.value < 0)
    	{
    		EndAction(false);
    		return;
    	}

        saveAs.value = agent.GetChildElement(GetIndex(index.value, fromLast.value));
        EndAction();
    }

    private int GetIndex(int index, bool fromLast)
    {
    	return !fromLast ? index : agent.ChildCount - (index + 1);
    }
}

}
