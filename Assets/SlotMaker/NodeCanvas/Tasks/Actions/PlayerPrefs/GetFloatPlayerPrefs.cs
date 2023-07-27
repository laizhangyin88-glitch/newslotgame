using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class GetFloatPlayerPrefs : ActionTask 
{
	public BBParameter<string> valueA;
	public BBParameter<float> defaultValue;
	
	[BlackboardOnly]
	public BBParameter<float> saveAs;

	protected override string info
	{
		get { return "Get " + valueA; }
	}

	protected override void OnExecute()
	{
		var value = PlayerPrefs.GetFloat(valueA.value, defaultValue.value);
		saveAs.value = value;
		EndAction();
	}
}

}