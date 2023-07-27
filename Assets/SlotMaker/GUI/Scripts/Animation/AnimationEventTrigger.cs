using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animation/Animation Event Trigger")]
	public class AnimationEventTrigger : MonoBehaviour 
	{
		public UnityEvent       onTrigger;
		public UnityIntEvent    onTriggerInt;
		public UnityFloatEvent  onTriggerFloat;
		public UnityStringEvent onTriggerString;

		public virtual void TriggerAnimationEvent()
		{
			if (onTrigger != null)
				onTrigger.Invoke();
		}

		public virtual void TriggerAnimationEventInt(int value)
		{
			if (onTriggerInt != null)
				onTriggerInt.Invoke(value);
		}

		public virtual void TriggerAnimationEventFloat(float value)
		{
			if (onTriggerFloat != null)
				onTriggerFloat.Invoke(value);
		}

		public virtual void TriggerAnimationEventString(string value)
		{
			if (onTriggerString != null)
				onTriggerString.Invoke(value);
		}
	}
}