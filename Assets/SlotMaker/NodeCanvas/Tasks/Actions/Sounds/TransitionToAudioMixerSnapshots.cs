using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class TransitionToAudioMixerSnapshots : ActionTask
{
    public BBParameter<List<string>> snapshotNames;
    public BBParameter<float[]> weights;
    public BBParameter<float> timeToReach;

    protected override void OnExecute()
    {
        var snapshots = new AudioMixerSnapshot[snapshotNames.value.Count];
        for (int i = 0; i < snapshotNames.value.Count; ++i)
        {
            var snapshot = GSManager.Instance.GetAudioMixerSnapshot(snapshotNames.value[i]);
            snapshots[i] = snapshot;
        }
        GSManager.Instance.masterMixer.TransitionToSnapshots(snapshots, weights.value, timeToReach.value);

        EndAction();
    }
}

}
