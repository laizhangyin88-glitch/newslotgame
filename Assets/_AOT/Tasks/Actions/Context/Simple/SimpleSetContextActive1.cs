using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextActive1 : ActionTask
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public bool lateUpdate = false;
    public BBParameter<bool> setTo;

    protected override string info
    {
        get { return string.Format("{0} {1}", elementName, setTo); }
    }

    protected override void OnExecute()
    {
        if (lateUpdate)
            MonoManager.current.onLateUpdate += DoSetActive;
        else
            DoSetActive();
    }

    protected override void OnStop()
    {
        if (lateUpdate)
            MonoManager.current.onLateUpdate -= DoSetActive;
    }

    private void DoSetActive()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

        if(element != null)
        {
            element.gameObject.SetActive(setTo.value);
            EndAction(true);
        }
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist");
            EndAction(false);
        }
    }
}

}
