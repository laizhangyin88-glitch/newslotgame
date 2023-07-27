using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ResetReelStrips : ActionTask
{
    public BBParameter<GameObject> slotMachine;

    protected override string info
    {
        get { return string.Format("Reset ReelStrips in {0}", slotMachine); }
    }

    protected override void OnExecute()
    {
        var slot = slotMachine.value.GetComponent<BaseSlotMachine>();

        for (int i = 0; i < slot.reels.Count; ++i)
        {
            var reel = slot.GetReel(i);
            reel.strip = null;
        }

        EndAction();
    }
}

}
