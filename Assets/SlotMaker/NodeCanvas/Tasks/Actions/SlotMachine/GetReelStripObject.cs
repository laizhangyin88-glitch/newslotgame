using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetReelStripObject : ActionTask
{
    public BBParameter<GameObject> reelStripsObject;
    public BBParameter<int> reelIndex;
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return string.Format("Get ReelStrips[{0}]", reelIndex); }
    }

    protected override void OnExecute()
    {
        var reelStrips = (reelStripsObject.isNull || reelStripsObject.isNone) ? GlobalReelStrips.Instance.GetReelStrips() : reelStripsObject.value.GetComponent<ReelStrips>();
        saveAs.value = reelStrips.GetReelStrip(reelIndex.value).gameObject;
        EndAction();
    }
}

}
