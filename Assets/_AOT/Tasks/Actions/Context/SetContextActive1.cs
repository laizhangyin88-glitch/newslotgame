using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextActive1 : ActionTask
{
    public BBParameter<ContextElement> element;
    public bool lateUpdate = false;
    public BBParameter<bool> setTo;

    protected override string info
    {
        get { return string.Format("{0} {1}", element, setTo); }
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
        if(element.value != null)
        {
            element.value.gameObject.SetActive(setTo.value);
            EndAction(true);
        }
        else
        {
            Debug.LogError("[Context] " + element + " is not exist");
            EndAction(false);
        }
    }
}

}
