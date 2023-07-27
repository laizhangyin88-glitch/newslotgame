using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class AddReel : ActionTask<Transform>
{
    public BBParameter<Transform>  reel;

    protected override void OnExecute()
    {
        var slotMachine = agent.GetComponent<BaseSlotMachine>();
        slotMachine.reels.Add(reel.value.GetComponent<BaseReel>());
        EndAction();
    }
}

}
