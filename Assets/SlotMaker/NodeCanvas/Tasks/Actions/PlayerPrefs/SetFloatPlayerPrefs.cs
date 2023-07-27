using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class SetFloatPlayerPrefs : ActionTask
{
	public BBParameter<string> valueA;
	public BBParameter<float> defaultValue;
	public OperationMethod Operation = OperationMethod.Set;
	public BBParameter<float> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetOperationString(Operation) + valueB; }
	}

	protected override void OnExecute()
	{
		float value = PlayerPrefs.GetFloat(valueA.value, defaultValue.value);
		PlayerPrefs.SetFloat(valueA.value, OperationUtils.Operate(value, valueB.value, Operation));
		EndAction();
	}
}

}