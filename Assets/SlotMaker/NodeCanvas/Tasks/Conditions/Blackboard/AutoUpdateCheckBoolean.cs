using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class AutoUpdateCheckBoolean : ConditionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public BBParameter<bool> valueB = true;

	private Variable<bool> variableA;

	protected override string info
	{
		get { return valueA + " == " + valueB; }
	}

	override protected void OnEnable()
	{
		variableA = BlackboardUtils.FindVariable<bool>(agent, valueA.value);
		variableA.onValueChanged += OnValueChanged;
	}

	override protected void OnDisable()
	{
		variableA.onValueChanged -= OnValueChanged;
	}

	protected override bool OnCheck()
	{
		return variableA.value == valueB.value;
	}

	private void OnValueChanged(string name, object value)
	{
		ownerAgent.GetComponent<ManualGraphOwner>().OnDispatch();
	}
}

}
