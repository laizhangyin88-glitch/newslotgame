using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Utility")]
public class GetWeightProgress : ActionTask
{
	public BBParameter<string> valueA;
	public BBParameter<bool> clear;
	[BlackboardOnly]
	public BBParameter<object> saveAs;

	protected override void OnExecute()
	{
		var variable = BlackboardUtils.GetOrCreateVariable<object>(null, valueA.value);
		if (variable.value == null)
			variable.value = new WeightProgress();

		WeightProgress wp = variable.value as WeightProgress;
		if (clear.value) wp.Clear();

		saveAs.value = variable.value;
		EndAction();
	}
}

}
