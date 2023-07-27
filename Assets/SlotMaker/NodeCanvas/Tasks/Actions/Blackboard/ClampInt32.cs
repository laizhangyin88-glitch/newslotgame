using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class ClampInt32 : ActionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public BBParameter<int> min;
	public BBParameter<int> max;

	protected override string info
	{
		get { return string.Format("{0} = Clamp({0}, {1}, {2})", valueA, min, max); }
	}

	protected override void OnExecute()
	{
		var variableA = BlackboardUtils.GetOrCreateVariable<int>(agent, valueA.value);
		if (variableA == null)
		{
			Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
			EndAction(false);
		}
		else
		{
			variableA.value = Mathf.Clamp(variableA.value, min.value, max.value);
			EndAction();
		}
	}
}

}
