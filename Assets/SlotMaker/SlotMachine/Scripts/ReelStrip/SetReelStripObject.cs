using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
[Description("Note that it set reelStrip copied shallowly")]
public class SetReelStripObject : ActionTask
{
    public BBParameter<GameObject> sourceReelStripObject;
    public BBParameter<GameObject> reelStripsObject;
    public BBParameter<int> reelIndex;

    protected override string info
    {
        get { return $"{reelStripsObject}[{reelIndex}] = {sourceReelStripObject}"; }
    }

    protected override void OnExecute()
    {
        var reelStrips = (reelStripsObject.isNull || reelStripsObject.isNone) ? GlobalReelStrips.Instance.GetReelStrips() : reelStripsObject.value.GetComponent<ReelStrips>();
        reelStrips.SetReelStrip(reelIndex.value, (BaseReelStrip)sourceReelStripObject.value.GetComponent<ReelStrip>());
        EndAction();
    }
}

}
