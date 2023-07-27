using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel")]
public class GetWheelSegmentAngle : ActionTask<Transform>
{
    public BBParameter<int> index;
    public BBParameter<int> segment;

	public BBParameter<float> saveAs;

	protected override void OnExecute()
	{
        // var wheel = agent.GetComponent<BigWheel>();
		saveAs.value = (360f / (float)segment.value) * (float)index.value;

		EndAction();
	}
}

}
