using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextBoolProperty : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BoolSetModes setTo = BoolSetModes.True;

    protected override string info
    {
        get 
        { 
            if (setTo == BoolSetModes.Toggle)
            {
                return "Toggle " + elementName;
            }
            else
            {
                return string.Format("Set {0} to {1}", elementName, setTo);     
            }
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
            if (setTo == BoolSetModes.Toggle)
                property.SetBooleanProperty(!property.GetBooleanProperty());
            else 
                property.SetBooleanProperty((int)setTo == 1);

            EndAction();
        }
    }
}

}
