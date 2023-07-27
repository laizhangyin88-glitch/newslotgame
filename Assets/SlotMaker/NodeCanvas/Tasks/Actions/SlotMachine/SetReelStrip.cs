using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetReelStrip : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<int> reelIndex;
    public BBParameter<GameObject> reelStrip;

    protected override string info
    {
        get { return string.Format("Set ReelStrip ({0}, {1}, {2})", slotMachine, reelIndex, reelStrip); }
    }

    protected override void OnExecute()
    {
        var slot = slotMachine.value.GetComponent<BaseSlotMachine>();
        var reel = slot.GetReel(reelIndex.value);

        reel.strip = reelStrip.value.GetComponent<BaseReelStrip>();
        EndAction();
    }
}

}
