using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel")]
public class SkipWheel : ActionTask<Transform>
{
    protected override string info
    {
        get 
        {
            return string.Format("Skip Wheel");
        }
    }

    protected override void OnExecute()
    {
        agent.GetComponent<BigWheel>().SkipSimulation();
        EndAction();
    }
}

}
