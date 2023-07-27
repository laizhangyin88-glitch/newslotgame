using NodeCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.BehaviourTrees
{

[Name("Repeat One Frame")]
[Category("★ SlotMaker/Decorators")]
[Color("ff1493")]
[Description("Repeat the child either x times or until it returns the specified status, or forever, it repeat everything in one frame")]
[Icon("Repeat")]
public class OneFrameRepeater : NodeCanvas.BehaviourTrees.BTDecorator
{
	public enum RepeaterMode
	{
		RepeatTimes,
		RepeatUntil,
		RepeatForever
	}

	public enum RepeatUntilStatus
	{
		Failure = 0,
		Success = 1
	}

	public RepeaterMode repeaterMode           = RepeaterMode.RepeatTimes;
	public RepeatUntilStatus repeatUntilStatus = RepeatUntilStatus.Success;
	public BBParameter<int> repeatTimes        = 1;

	private int currentIteration = 1;

	protected override Status OnExecute(Component agent, IBlackboard blackboard)
	{
		while(true)
		{
			if (decoratedConnection == null)
				return Status.Resting;

			if (decoratedConnection.status == Status.Success || decoratedConnection.status == Status.Failure)
				decoratedConnection.Reset();

			status = decoratedConnection.Execute(agent, blackboard);

			switch(status)
            {
			    case Status.Resting:
			        return Status.Running;
			    case Status.Running:
			        return Status.Running;
			}

			switch(repeaterMode)
			{
				case RepeaterMode.RepeatTimes:

					if (currentIteration >= repeatTimes.value)
						return status;

					currentIteration++;
					break;

				case RepeaterMode.RepeatUntil:

					if ((int)status == (int)repeatUntilStatus)
						return status;

					break;
			}
		}
	}

	protected override void OnReset()
	{
		currentIteration = 1;
	}


	/////////////////////////////////////////
	/////////GUI AND EDITOR STUFF////////////
	/////////////////////////////////////////
	#if UNITY_EDITOR

	protected override void OnNodeGUI()
	{
		if (repeaterMode == RepeaterMode.RepeatTimes)
		{
			GUILayout.Label(repeatTimes + " Times");
			if (Application.isPlaying)
				GUILayout.Label("Iteration: " + currentIteration.ToString());
		}
		else if (repeaterMode == RepeaterMode.RepeatUntil)
			GUILayout.Label("Until " + repeatUntilStatus);
		else
			GUILayout.Label("Repeat Forever");
	}

	protected override void OnNodeInspectorGUI()
	{
		repeaterMode = (RepeaterMode)UnityEditor.EditorGUILayout.EnumPopup("Repeat Type", repeaterMode);

		if (repeaterMode == RepeaterMode.RepeatTimes)
			repeatTimes = (BBParameter<int>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Repeat Times", repeatTimes);
		else if (repeaterMode == RepeaterMode.RepeatUntil)
			repeatUntilStatus = (RepeatUntilStatus)UnityEditor.EditorGUILayout.EnumPopup("Repeat Until", repeatUntilStatus);
	}

	#endif
}

}