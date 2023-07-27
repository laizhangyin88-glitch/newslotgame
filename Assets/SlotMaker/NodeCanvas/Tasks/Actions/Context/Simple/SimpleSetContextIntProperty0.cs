using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextIntProperty0 : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<string> arg;

    protected override string info
    {
        get { return string.Format("{0}.intProperty {1} {2}", elementName, OperationUtils.GetOperationString(Operation), arg); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        
        Blackboard bb = agent.GetComponent<Blackboard>();
        object a0 = BlackboardUtils.FindValue(bb, arg.value);
        if (a0 == null)
        {
            EndAction(false);
            return;
        }

        IContextIntProperty property = element as IContextIntProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextIntProperty");
            EndAction(false);            
        }
        else
        {
            property.SetIntProperty( OperationUtils.Operate(property.GetIntProperty(), (int)a0, Operation));
            EndAction();
        }

    }
}

}
