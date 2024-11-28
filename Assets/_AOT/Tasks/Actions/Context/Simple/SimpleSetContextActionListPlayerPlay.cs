using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextActionListPlayerPlay : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    protected override string info
    {
        get { return string.Format("{0}.ActionListPlayer Play", elementName); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        
        IContextPlayer player = element as IContextPlayer;
        if (player == null)
        {
            Debug.LogError("[Context] " + element.ContextName + " is not IContextPlayer");
            EndAction(false);            
        }
        else
        {
            player.Play();
            EndAction();
        }

    }
}

}
