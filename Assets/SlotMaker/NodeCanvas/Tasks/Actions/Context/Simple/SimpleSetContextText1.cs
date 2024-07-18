using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextText1 : ActionTask
{
	public BBParameter<string> elementName;
	public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

	public BBParameter<string> key;
    public StringTable.StringTableType tableType;

    public BBParameter<string> arg1;

	protected override string info
	{
		get { return string.Format("{0}.text = {1}.{2}({3})", elementName, tableType, key, arg1); }
	}

	protected override void OnExecute()
	{
		ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

		Blackboard bb = agent.GetComponent<Blackboard>();
		object a1 = BlackboardUtils.FindValue(bb, arg1.value);
    	if (a1 == null)
    	{
    		EndAction(false);
    		return;
    	}
    	
    	bool error = true;
    	IContextText textElement = element as IContextText;
        if(textElement != null)
            textElement.SetText(StringTableUtils.GetString(tableType, key.value, a1, out error));
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist");
        }
        EndAction(!error);
	}
}

}
