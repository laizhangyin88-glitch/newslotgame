using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetGlobalReelStripsObject : ActionTask
{
    public BBParameter<int> reelStripsIndex;
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return string.Format("Get GlobalReelStrips[{0}]", reelStripsIndex); }
    }

    protected override void OnExecute()
    {
        saveAs.value = GlobalReelStrips.Instance.GetReelStrips(reelStripsIndex.value).gameObject;
        EndAction();
    }
}

}
