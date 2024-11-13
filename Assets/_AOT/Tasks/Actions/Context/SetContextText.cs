using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextText : ActionTask<ContextElement>
{
    public BBParameter<string> key;
    public StringTable.StringTableType tableType;

    protected override string info
    {
        get { return string.Format("{0}.text = {1}.{2}", agentInfo, tableType, key); }
    }

    protected override void OnExecute()
    {
        bool error = true;
        IContextText textElement = agent as IContextText;
        if(textElement != null)
            textElement.SetText(StringTableUtils.GetString(tableType, key.value, out error));
        EndAction(!error);
    }
}

}