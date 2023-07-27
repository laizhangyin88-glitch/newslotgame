using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class SetStringPlayerPrefs : ActionTask
{
	public BBParameter<string> valueA;
	public BBParameter<string> defaultValue;
	public BBParameter<string> valueB;

	protected override string info
	{
		get { return "Set  " + valueB + " with key " + valueA; }
	}

	protected override void OnExecute()
	{
		PlayerPrefs.GetString(valueA.value, defaultValue.value);
		PlayerPrefs.SetString(valueA.value, valueB.value);
		EndAction();
	}
}

}
