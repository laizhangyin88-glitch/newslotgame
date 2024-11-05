using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using TMPro;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context/AddOn")]
public class SetLinkTagTextMeshProUGUI : ActionTask
{
    public enum LinkTagType
    {
        HyperLink,
    }

    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public LinkTagType tagType;

    protected override string info
    {
        get { return string.Format("{0} Link Tag Use. {1}", elementName, tagType); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

        if(element != null)
        {
            switch(tagType)
            {
                case LinkTagType.HyperLink:
                    AddHyperLink(element);
                    break;
            }
        }

        EndAction(true);
    }

    private void AddHyperLink(ContextElement element)
    {
        element.gameObject.AddComponent<AddOnHyperLinkTextMeshProUGUI>();
    }
}

}
