using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class AutoUpdateCheckInt32 : ConditionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<int> valueB;

	private Variable<int> variableA;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	override protected void OnEnable()
	{
		variableA = BlackboardUtils.FindVariable<int>(agent, valueA.value);
		variableA.onValueChanged += OnValueChanged;
	}

	override protected void OnDisable()
	{
		variableA.onValueChanged -= OnValueChanged;
	}

	protected override bool OnCheck()
	{
		return OperationUtils.Compare(variableA.value, valueB.value, checkType);
	}

	private void OnValueChanged(string name, object value)
	{
		ownerAgent.GetComponent<ManualGraphOwner>().OnDispatch();
	}
}

}
