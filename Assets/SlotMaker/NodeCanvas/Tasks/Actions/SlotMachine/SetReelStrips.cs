using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetReelStrips : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<GameObject> reelStripsObject;

    protected override string info
    {
        get { return string.Format("Set ReelStrips ({0}, {1})", slotMachine, reelStripsObject); }
    }

    protected override void OnExecute()
    {
        var slot = slotMachine.value.GetComponent<BaseSlotMachine>();
        var reelStrips = reelStripsObject.value.GetComponent<ReelStrips>();

        for (int i = 0; i < slot.reels.Count; ++i)
        {
            var reel = slot.GetReel(i);
            reel.strip = reelStrips.GetReelStrip(i);
        }

        EndAction();
    }
}

}
