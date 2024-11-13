using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextText : ActionTask
{
	public BBParameter<string> elementName;
	public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

	public BBParameter<string> key;
    public StringTable.StringTableType tableType;

	protected override string info
	{
		get { return string.Format("{0}.text = ({1}){2}", elementName, tableType, key); }
	}

	protected override void OnExecute()
	{
		ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

		bool error = false;
        IContextText textElement = element as IContextText;
        if (textElement != null)
            textElement.SetText(StringTableUtils.GetString(tableType, key.value, out error));
        else
        {
            if (ApplicationSettings.LogSystem())
                Debug.LogWarning("[Context] " + elementName.value + " is not exist");
        }
        EndAction(!error);
	}
}

}
