using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextTextLong : ActionTask
{
	public BBParameter<ContextElement> element;
    public BBParameter<string> format;
	public BBParameter<long> arg1;

    protected override string info
    {
        get { return string.Format("{0}.text = {1}({2})", element, format, arg1); }
    }

    protected override void OnExecute()
    {
		IContextText textElement = element.value as IContextText;
        if(textElement != null)
			textElement.SetText(string.Format(StringTableUtils.customProvider, format.value, arg1.value));

        EndAction();
    }
}

}
