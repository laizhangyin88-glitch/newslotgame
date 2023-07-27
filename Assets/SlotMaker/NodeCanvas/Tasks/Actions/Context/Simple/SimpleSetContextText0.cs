using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextText0 : ActionTask
{
	public BBParameter<string> elementName;
	public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

	public BBParameter<string> arg0;

	protected override string info
	{
		get { return string.Format("{0}.text = {1}", elementName, arg0); }
	}

	protected override void OnExecute()
	{
		ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

		Blackboard bb = agent.GetComponent<Blackboard>();
		object a0 = BlackboardUtils.FindValue(bb, arg0.value);
    	if (a0 == null)
    	{
    		EndAction(false);
    		return;
    	}

        IContextText textElement = element as IContextText;
        if(textElement != null)
            textElement.SetText(a0.ToString());
        else
        {
            if (ApplicationSettings.LogSystem())
                Debug.LogWarning("[Context] " + elementName.value + " is not exist");
        }            
        EndAction();
	}
}

}
