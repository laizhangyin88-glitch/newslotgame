using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextActive : ActionTask
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public bool lateUpdate = false;

    public enum SetActiveMode
    {
        Deactivate = 0,
        Activate   = 1,
        Toggle     = 2
    }

    public SetActiveMode setTo = SetActiveMode.Toggle;

    protected override string info
    {
        get { return string.Format("{0} {1}", setTo, elementName); }
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
            bool value;
            
            if (setTo == SetActiveMode.Toggle){
            
                value = !element.gameObject.activeSelf;
            
            } else {

                value = (int)setTo == 1;
            }

            element.gameObject.SetActive(value);
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
