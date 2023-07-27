using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/PlayerPrefs")]
public class CheckBooleanPlayerPrefs : ConditionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<bool> defaultValue; 
	public BBParameter<bool> valueB;

	protected override string info
	{
		get { return valueA + " == " + valueB; }
	}

	protected override bool OnCheck() 
	{	
		bool value = (PlayerPrefs.GetInt(valueA.value, (defaultValue.value ? 1 : 0)) > 0) ? true : false; 
		return value == valueB.value;
	}
}

}