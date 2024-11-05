using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Utility")]
public class GetTotalWeightProgress : ActionTask
{
	public BBParameter<object> weightProgress;
	public BBParameter<float> progress;

	protected override void OnExecute()
	{
		WeightProgress wp = weightProgress.value as WeightProgress;
		progress.value = wp.GetTotalProgress();
		EndAction();
	}
}

}
