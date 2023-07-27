using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextText6 : ActionTask<Blackboard>
{
    public BBParameter<ContextElement> element;
    public BBParameter<string> key;
    public StringTable.StringTableType tableType;
    public BBParameter<string> arg1;
    public BBParameter<string> arg2;
    public BBParameter<string> arg3;
    public BBParameter<string> arg4;
    public BBParameter<string> arg5;
    public BBParameter<string> arg6;

    protected override string info
    {
        get { return string.Format("{0}.text = ({1}){2}({3}, {4}, {5}, {6}, {7}, {8})", element, tableType, key, arg1, arg2, arg3, arg4, arg5, arg6); }
    }

    protected override void OnExecute()
    {
        object a1 = BlackboardUtils.FindValue(agent, arg1.value);
        object a2 = BlackboardUtils.FindValue(agent, arg2.value);
        object a3 = BlackboardUtils.FindValue(agent, arg3.value);
        object a4 = BlackboardUtils.FindValue(agent, arg4.value);
        object a5 = BlackboardUtils.FindValue(agent, arg5.value);
        object a6 = BlackboardUtils.FindValue(agent, arg6.value);
        if (a1 == null || a2 == null || a3 == null || a4 == null || a5 == null || a6 == null)
        {
            EndAction(false);
            return;
        }

        bool error = true;
        IContextText textElement = element.value as IContextText;
        if(textElement != null)
            textElement.SetText(StringTableUtils.GetString(tableType, key.value, a1, a2, a3, a4, a5, a6, out error));
        EndAction(!error);
    }
}

}
