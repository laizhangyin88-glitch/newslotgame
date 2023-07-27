using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel")]
public class SpinWheel : ActionTask<Transform>
{
    public BBParameter<float> initialTorque;
    public BBParameter<float> desiredAngle;
    public BBParameter<int> additionalRotationCount;

	protected override string info
	{
		get 
        {
            return string.Format("Spin Wheel");
        }
	}

	protected override void OnExecute()
	{
        agent.GetComponent<BigWheel>().Simulation(initialTorque.value, desiredAngle.value, additionalRotationCount.value);
		EndAction();
	}
}

}
