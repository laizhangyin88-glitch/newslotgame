using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animator/Animator Set Trigger")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSetTrigger : MonoBehaviour 
	{
		public string propertyName;

		private Animator animator;

		public void SetTrigger()
		{
			Validate();
			
			animator.SetTrigger(propertyName);
		}

		public void SetTriggerByName(string name)
		{
			Validate();
			
			animator.SetTrigger(name);	
		}

		public void SetTriggerByIndex(int index)
		{
			Validate();
			
			animator.SetTrigger(propertyName + index);
		}

		private void Validate()
		{
			if (animator == null)
				animator = GetComponent<Animator>();
		}
	}
}