using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextText0 : ActionTask<Blackboard>
{
	public BBParameter<ContextElement> element;
    public BBParameter<string> arg0;

    protected override string info
    {
        get { return string.Format("{0}.text = {1}", element, arg0); }
    }

    protected override void OnExecute()
    {
    	object a0 = BlackboardUtils.FindValue(agent, arg0.value);
    	if (a0 == null)
    	{
    		EndAction(false);
    		return;
    	}

    	IContextText textElement = element.value as IContextText;
        if(textElement != null)
            textElement.SetText(a0.ToString());
        EndAction();
    }
}

}