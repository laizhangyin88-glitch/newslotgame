using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Utility")]
public class AddWeightProgress : ActionTask
{
	public BBParameter<object> weightProgress;
	public BBParameter<string> key;
	public BBParameter<float> weight;

    protected override string info
    {
        get
        {
            return string.Format("Add Weight Progress {0} = {1}", key, weight);
        }
    }

	protected override void OnExecute()
	{
		WeightProgress wp = weightProgress.value as WeightProgress;
		wp.AddProgress(key.value, weight.value);
		EndAction();
	}
}

}
