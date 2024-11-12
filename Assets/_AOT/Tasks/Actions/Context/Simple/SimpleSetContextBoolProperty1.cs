using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextBoolProperty1 : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<bool> arg;

    protected override string info
    {
        get 
        { 
            return string.Format("Set {0} to {1}", elementName, arg);     
        }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        
        IContextBooleanProperty property = element as IContextBooleanProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextBooleanProperty");
            EndAction(false);            
        }
        else
        {
            property.SetBooleanProperty(arg.value);
            EndAction();
        }
    }
}

}
