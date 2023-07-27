using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class ClearBackButtonEvent : ActionTask
{
    protected override string info
    {
        get 
        {
            return string.Format("Clear Backbutton Event");
        }
    }

    protected override void OnExecute()
    {
        BackButtonManager.Instance.ClearListenerStack();
        EndAction();
    }
}

}
