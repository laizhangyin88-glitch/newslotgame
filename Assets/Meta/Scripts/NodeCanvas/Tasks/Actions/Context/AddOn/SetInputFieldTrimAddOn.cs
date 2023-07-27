using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context/AddOn")]
public class SetInputFieldTrimAddOn : ActionTask
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    protected override string info
    {
        get { return string.Format("{0} Add Input Field Trim Pung in", elementName); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

        if(element != null)
        {
            element.gameObject.AddComponent<InputFieldTrimAddOn>();
        }

        EndAction(true);
    }
}

}
