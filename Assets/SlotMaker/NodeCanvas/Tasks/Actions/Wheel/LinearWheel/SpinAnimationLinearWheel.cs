using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel/LinearWheel")]
public class SpinAnimationLinearWheel : ActionTask<Transform>
{
    public BBParameter<int> target;

    protected override string info
    {
        get 
        {
            return string.Format("Spin Animation Linear Wheel to {0}", target);
        }
    }

    protected override void OnExecute()
    {
        var linearWheel = agent.GetComponent<AnimationLinearWheel>();
        
        if (target != null && !target.isNone)
            linearWheel.SetDesiredSegment(target.value);
        
        linearWheel.Simulation();
        
        EndAction();
    }
}

}
