using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class GetContextElement : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<ContextElement> saveAs;

    protected override string info
    {
        get 
        { 
            if (elementName == null || string.IsNullOrEmpty(elementName.value))
                return string.Format("{0} = ({1}){2}", saveAs, searchingType, agentInfo);
            else 
                return string.Format("{0} = ({1}){2}/{3}", saveAs, searchingType, agentInfo, elementName); 
        } 
    }

    protected override void OnExecute()
    {
        saveAs.value = ContextUtils.FindElement(agent, elementName.value, searchingType);
        EndAction();
    }
}

}
