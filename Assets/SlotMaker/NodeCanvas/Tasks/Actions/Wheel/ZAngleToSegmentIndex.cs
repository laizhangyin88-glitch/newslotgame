using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel")]
public class ZAngleToSegmentIndex : ActionTask<Transform>
{
    public BBParameter<int> segment;
	public bool halfOffset;

	public BBParameter<int> saveAs;

	protected override void OnExecute()
	{
		float segmentAngle = 360f / (float)segment.value;
		float currentAngle = agent.localEulerAngles.z;
		if (halfOffset)
			currentAngle += segmentAngle * 0.5f;

		int currentIndex = (int)(currentAngle / segmentAngle);
		if (currentIndex < 0)
			currentIndex = currentIndex + segment.value * (-((currentIndex + 1) / segment.value) + 1);
		else if (currentIndex >= segment.value)
			currentIndex = currentIndex - segment.value * (currentIndex / segment.value);

		saveAs.value = currentIndex;

		EndAction();
	}
}

}
