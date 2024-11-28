using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextIntProperty : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<int> valueB;

    protected override string info
    {
        get { return string.Format("{0}.intProperty {1} {2}", elementName, OperationUtils.GetOperationString(Operation), valueB); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        
        IContextIntProperty property = element as IContextIntProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + element.ContextName + " is not IContextIntProperty");
            EndAction(false);            
        }
        else
        {
            property.SetIntProperty( OperationUtils.Operate(property.GetIntProperty(), valueB.value, Operation));
            EndAction();
        }

    }
}

}
