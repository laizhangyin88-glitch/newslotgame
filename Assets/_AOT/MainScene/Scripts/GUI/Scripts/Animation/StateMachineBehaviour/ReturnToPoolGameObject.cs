using UnityEngine;
using System.Collections;

namespace SlotMaker.AnimatorBehaviour
{
	public class ReturnToPoolGameObject : StateMachineBehaviour
	{
		public bool atExit;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!atExit) animator.GetComponent<PooledObject>().ReturnToPool();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (atExit) animator.GetComponent<PooledObject>().ReturnToPool();
		}
	}
}
