using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker.IoC.AnimatorBehaviour
{
	public class TransitionSnapshot : StateMachineBehaviour
	{
	    public bool atExit;
	    public string snapshotName;
		public float timeToReach;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!atExit) TransitionTo();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (atExit) TransitionTo();
		}

		private void TransitionTo()
		{
			var mgr = GSManager.Instance;
			if (mgr) mgr.GetAudioMixerSnapshot(snapshotName).TransitionTo(timeToReach);
		}
	}
}