using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/PlayerPrefs")]
public class SetBooleanPlayerPrefs : ActionTask
{
	public BBParameter<string> valueA;
	public BBParameter<bool> defaultValue;
	public BoolSetModes setTo = BoolSetModes.True;

	protected override string info
	{
		get
		{
			if (setTo == BoolSetModes.Toggle)
				return "Toggle " + valueA;
			else 
				return "Set " + valueA + " to " + setTo;
		}
	}

	protected override void OnExecute()
	{
		bool value = (PlayerPrefs.GetInt(valueA.value, (defaultValue.value ? 1 : 0)) > 0) ? true : false;
		if (setTo == BoolSetModes.Toggle)
			PlayerPrefs.SetInt(valueA.value, (!value ? 1 : 0));
		else 
			PlayerPrefs.SetInt(valueA.value, (((int)setTo > 0) ? 1 : 0));

		EndAction();
	}
}

}