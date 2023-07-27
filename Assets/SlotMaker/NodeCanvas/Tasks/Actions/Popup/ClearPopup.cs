using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker.Task.Actions
{

[Category("★ SlotMaker/Popup")]
[Description("Clear stacked Popups in PopupManager. For systemReset")]
public class ClearPopup : ActionTask
{
    protected override string info
    {
        get { return "Clear Popups"; }
    }

    protected override void OnExecute()
    {
        MonoManager.current.onLateUpdate += OnLateUpdate;
    }

    protected override void OnStop()
    {
        MonoManager.current.onLateUpdate -= OnLateUpdate;
    }

    void OnLateUpdate()
    {
        PopupManager.Instance.Clear();
        EndAction();
    }
}

}
