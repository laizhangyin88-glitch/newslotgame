using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/PlayerPrefs")]
public class CheckIntPlayerPrefs : ConditionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<int> defaultValue; 
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<int> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{	
		int value = PlayerPrefs.GetInt(valueA.value, defaultValue.value); 
		return OperationUtils.Compare(value, valueB.value, checkType);
	}
}

}