using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class ToggleNotification : ActionTask
{
    protected override string info { get { return "Toggle Notification"; } }

    protected override void OnExecute()
    {
        NativeHelper.Instance.TogglePush();
        
        EndAction();
    }
}

}
