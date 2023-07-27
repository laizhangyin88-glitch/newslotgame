using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class SimpleSetActive : ActionTask
{
    public BBParameter<GameObject> target;
    public enum SetActiveMode
    {
        Deactivate = 0,
        Activate   = 1,
        Toggle     = 2
    }
    public SetActiveMode setTo = SetActiveMode.Toggle;

    public bool lateUpdate = false;

    protected override string info
    {
        get { return string.Format("{0}.{1}", target, setTo); }
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
        if (target.isNone || target.isNull)
        {
            EndAction();
            return;
        }

        bool value;

        if (setTo == SetActiveMode.Toggle)
            value = !target.value.activeSelf;
        else
            value = (int)setTo == 1;

        target.value.SetActive(value);
        EndAction();
    }
}

}
