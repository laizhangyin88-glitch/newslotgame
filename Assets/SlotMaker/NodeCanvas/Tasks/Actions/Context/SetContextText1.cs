using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextText1 : ActionTask<Blackboard>
{
	public BBParameter<ContextElement> element;
    public BBParameter<string> key;
    public StringTable.StringTableType tableType;
    public BBParameter<string> arg1;

    protected override string info
    {
        get { return string.Format("{0}.text = ({1}){2}({3})", element, tableType, key, arg1); }
    }

    protected override void OnExecute()
    {
    	object a1 = BlackboardUtils.FindValue(agent, arg1.value);
    	if (a1 == null)
    	{
    		EndAction(false);
    		return;
    	}

        bool error = true;
    	IContextText textElement = element.value as IContextText;
        if(textElement != null)
            textElement.SetText(StringTableUtils.GetString(tableType, key.value, a1, out error));
        EndAction(!error);
    }
}

}