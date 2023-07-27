using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class GetIntPlayerPrefs : ActionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<int> defaultValue;
	
	[BlackboardOnly]
	public BBParameter<int> saveAs;

	protected override string info
	{
		get { return "Get " + valueA; }
	}

	protected override void OnExecute()
	{
		var value = PlayerPrefs.GetInt(valueA.value, defaultValue.value);
		saveAs.value = value;
		EndAction();
		
	}
}

}