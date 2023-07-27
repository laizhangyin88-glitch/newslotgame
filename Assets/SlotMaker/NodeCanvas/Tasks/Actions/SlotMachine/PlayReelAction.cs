using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class PlayReelAction : ActionTask<Transform>
{
    public BBParameter<int> reelIndex;
	public BBParameter<int> actionIndex;

    protected override string info { get { return string.Format("Reel({0}).Play({1})", reelIndex, actionIndex); } }

    protected override void OnExecute()
    {
        var movement = agent.GetComponent<SlotMachine>().GetReel(reelIndex.value).movement;
        movement.Play(actionIndex.value);

        EndAction();
    }
}

}
