using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker.IoC.AnimatorBehaviour
{
	public class ResetTrigger : StateMachineBehaviour
	{
	    public bool atExit;
	    public string parameter;
	    public int parameterHashId;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!atExit) Apply(animator);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (atExit) Apply(animator);
		}

		protected void Apply(Animator animator)
		{
			if (!string.IsNullOrEmpty(parameter))
				animator.ResetTrigger(parameter);
			else
				animator.ResetTrigger(parameterHashId);
		}
	}
}