using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/PlayerPrefs")]
public class CheckFloatPlayerPrefs : ConditionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<float> defaultValue; 
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<float> valueB;

	[SliderField(0,0.1f)]
	public float differenceThreshold = 0.05f;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{	
		float value = PlayerPrefs.GetFloat(valueA.value, defaultValue.value); 
		return OperationUtils.Compare(value, valueB.value, checkType, differenceThreshold);
	}
}

}