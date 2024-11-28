using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextFloatProperty : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<float> valueB;

    protected override string info
    {
        get { return string.Format("{0}.floatProperty {1} {2})", elementName, OperationUtils.GetOperationString(Operation), valueB); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        
        IContextFloatProperty property = element as IContextFloatProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + element.ContextName + " is not IContextFloatProperty");
            EndAction(false);            
        }
        else
        {
            property.SetFloatProperty( OperationUtils.Operate(property.GetFloatProperty(), valueB.value, Operation));
            EndAction();
        }

    }
}

}
