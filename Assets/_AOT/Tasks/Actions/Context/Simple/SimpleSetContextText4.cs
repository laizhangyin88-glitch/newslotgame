using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextText4 : ActionTask
{
	public BBParameter<string> elementName;
	public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

	public BBParameter<string> key;
    public StringTable.StringTableType tableType;

    public BBParameter<string> arg1;
    public BBParameter<string> arg2;
    public BBParameter<string> arg3;
    public BBParameter<string> arg4;

	protected override string info
	{
		get { return string.Format("{0}.text = {1}.{2}({3}, {4}, {5}, {6})", elementName, tableType, key, arg1, arg2, arg3, arg4); }
	}

	protected override void OnExecute()
	{
		ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

		Blackboard bb = agent.GetComponent<Blackboard>();
		object a1 = BlackboardUtils.FindValue(bb, arg1.value);
		object a2 = BlackboardUtils.FindValue(bb, arg2.value);
		object a3 = BlackboardUtils.FindValue(bb, arg3.value);
		object a4 = BlackboardUtils.FindValue(bb, arg4.value);
    	if (a1 == null || a2 == null || a3 == null || a4 == null)
    	{
    		EndAction(false);
    		return;
    	}

    	bool error = true;
    	IContextText textElement = element as IContextText;
        if(textElement != null)
            textElement.SetText(StringTableUtils.GetString(tableType, key.value, a1, a2, a3, a4, out error));
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist");
        }            
        EndAction(!error);
	}
}

}