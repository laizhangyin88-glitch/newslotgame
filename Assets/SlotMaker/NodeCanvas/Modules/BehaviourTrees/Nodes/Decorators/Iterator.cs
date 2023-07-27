using System.Collections;
using System.Collections.Generic;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.BehaviourTrees
{

[Name("Iterate")]
[Category("★ SlotMaker/Decorators")]
[Description("Iterate any type of list and execute the child node for each element in the list. Keeps iterating until the Termination Condition is met or the whole list is iterated and return the child node status")]
[Icon("List")]
public class Iterator : NodeCanvas.BehaviourTrees.BTDecorator
{
	public enum TerminationConditions
	{
		None,
		FirstSuccess,
		FirstFailure
	}

	[RequiredField] [BlackboardOnly]
	public BBObjectParameter targetList;
	public bool reverseIterator;
	[BlackboardOnly]
	public BBObjectParameter current;
	[BlackboardOnly]
	public BBParameter<int> storeIndex;

	public BBParameter<int> maxIteration = -1;

	public TerminationConditions terminationCondition = TerminationConditions.None;
	public bool resetIndex = true;

	private int currentIndex = -1;

	private IList list
	{
		get
		{
			if (targetList == null)
				return null;

			return targetList.value as IList;
		}
	}

	protected override Status OnExecute(Component agent, IBlackboard blackboard)
	{
		if (currentIndex < 0)
			OnReset();

		if (decoratedConnection == null)
			return Status.Resting;

		if (list == null || list.Count == 0)
			return Status.Failure;

		for (int i = currentIndex; HasNext(i); i = GetNextIndex(i))
		{
			current.value = list[i];
			storeIndex.value = i;
			status = decoratedConnection.Execute(agent, blackboard);

			if (status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess)
				return Status.Success;

			if (status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure)
				return Status.Failure;

			if (status == Status.Running)
			{
				currentIndex = i;
				return Status.Running;
			}

			if (IsLast() || IsMaxIteration())
				return status;

			decoratedConnection.Reset();
			currentIndex = GetNextIndex(currentIndex);
		}

		return Status.Running;
	}

	protected override void OnReset()
	{
		if (resetIndex)
		{
			currentIndex = GetBeginIndex();
		}
	}

	protected int GetBeginIndex()
	{
		if (reverseIterator && list != null)
			return list.Count - 1;

		return 0;
	}

	protected int GetNextIndex(int index)
	{
		return !reverseIterator ? ++index : --index;
	}

	protected bool HasNext(int index)
	{
		return !reverseIterator ? (index < list.Count) : (index >= 0);
	}

	protected bool IsLast()
	{
		return !reverseIterator ? (currentIndex == list.Count - 1) : (currentIndex == 0);
	}

	protected bool IsMaxIteration()
	{
		return !reverseIterator ? (currentIndex == maxIteration.value - 1) : (currentIndex == maxIteration.value + 1);
	}

	////////////////////////////////////////
	///////////GUI AND EDITOR STUFF/////////
	////////////////////////////////////////
	#if UNITY_EDITOR

	protected override void OnNodeGUI()
	{
		var leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
		leftLabelStyle.richText = true;
		leftLabelStyle.alignment = TextAnchor.UpperLeft;

		GUILayout.Label("For Each \t" + current + "\nIn \t" + targetList, leftLabelStyle);
		if (terminationCondition != TerminationConditions.None)
			GUILayout.Label("Exit on " + terminationCondition.ToString());

		if (Application.isPlaying)
			GUILayout.Label("Index: " + currentIndex.ToString() + " / " + (list != null && list.Count != 0? (list.Count -1).ToString() : "?") );
	}

	protected override void OnNodeInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("Select Type"))
		{
			EditorUtils.ShowPreferedTypesSelectionMenu(typeof(object), (t) =>
			{
				targetList.SetType(t);
				var argType = t.GetGenericArguments()[0];
				current.SetType(argType);
			});
		}
	}

	#endif
}

}
