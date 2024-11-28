using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Utility")]
public class UpdateWeightProgress : ActionTask
{
	public BBParameter<object> weightProgress;
	public BBParameter<string> key;
	public BBParameter<float> progress;

    protected override string info
    {
        get
        {
            return string.Format("Update Weight Progress {0} = {1}", key, progress);
        }
    }

	protected override void OnExecute()
	{
		WeightProgress wp = weightProgress.value as WeightProgress;
		wp.UpdateProgress(key.value, progress.value);
		EndAction();
	}
}

}
