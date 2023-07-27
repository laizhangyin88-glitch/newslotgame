using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class GetStringPlayerPrefs : ActionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<string> defaultValue;
	
	[BlackboardOnly]
	public BBParameter<string> saveAs;

	protected override string info
	{
		get { return "Get " + valueA; }
	}

	protected override void OnExecute()
	{
		var value = PlayerPrefs.GetString(valueA.value, defaultValue.value);
		saveAs.value = value;
		EndAction();
	}
}

}