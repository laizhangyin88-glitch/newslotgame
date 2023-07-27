using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel/LinearWheel")]
public class ResetAnimationLinearWheel : ActionTask<Transform>
{
    public BBParameter<bool> resetPosition;
    public BBParameter<bool> resetSelection;
    public BBParameter<bool> invokeOnResetWheel;
    protected override string info
    {
        get 
        {
            return string.Format("Reset Animation Linear Wheel");
        }
    }

    protected override void OnExecute()
    {
        var linearWheel = agent.GetComponent<AnimationLinearWheel>();
        linearWheel.ResetWheel(resetPosition.value, resetSelection.value, invokeOnResetWheel.value);
        EndAction();
    }
}

}