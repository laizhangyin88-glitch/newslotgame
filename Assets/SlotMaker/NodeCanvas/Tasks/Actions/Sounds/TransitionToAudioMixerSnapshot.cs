using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class TransitionToAudioMixerSnapshot : ActionTask
{
    public BBParameter<string> snapshotName;
    public BBParameter<float> timeToReach;

    protected override string info { get { return "Transition " + snapshotName + " for " + timeToReach; } }

    protected override void OnExecute()
    {
        var snapshot = GSManager.Instance.GetAudioMixerSnapshot(snapshotName.value);
        snapshot.TransitionTo(timeToReach.value);

        EndAction();
    }
}

}
