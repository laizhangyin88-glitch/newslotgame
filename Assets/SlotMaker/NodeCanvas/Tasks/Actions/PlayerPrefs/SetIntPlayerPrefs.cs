using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class SetIntPlayerPrefs : ActionTask
{
	public BBParameter<string> valueA;
	public BBParameter<int> defaultValue;
	public OperationMethod Operation = OperationMethod.Set;
	public BBParameter<int> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetOperationString(Operation) + valueB; }
	}

	protected override void OnExecute()
	{
		int value = PlayerPrefs.GetInt(valueA.value, defaultValue.value);
		PlayerPrefs.SetInt(valueA.value, OperationUtils.Operate(value, valueB.value, Operation));
		EndAction();
	}
}

}