using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel")]
public class SpinPBWheel : ActionTask<Transform>
{
    public BBParameter<float> desiredAngle;

	protected override string info
	{
		get 
        {
            return "DEPRECATED!!";
        }
	}

	protected override void OnExecute()
	{
        // agent.GetComponent<PBBigWheel>().Spin(desiredAngle.value);
		EndAction();
	}
}

}
