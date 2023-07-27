using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class FromToAnimatorSetTrigger : MonoBehaviour 
	{
		public void SetTriggerFrom(string triggerName)
		{
			GetComponent<IFromToTransformPair>().From.GetComponent<Animator>().SetTrigger(triggerName);
		}

		public void SetTriggerTo(string triggerName)
		{
			GetComponent<IFromToTransformPair>().To.GetComponent<Animator>().SetTrigger(triggerName);
		}
	}
}