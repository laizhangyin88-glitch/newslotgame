using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class GetBooleanPlayerPrefs : ActionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<bool> defaultValue;
	
	[BlackboardOnly]
	public BBParameter<bool> saveAs;

	protected override string info
	{
		get { return "Get " + valueA; }
	}

	protected override void OnExecute()
	{
		bool value = (PlayerPrefs.GetInt(valueA.value, (defaultValue.value ? 1 : 0)) > 0) ? true : false;
		saveAs.value = value;
		EndAction();
		
	}
}

}