using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class ReelMotionBlur : MonoBehaviour {
		public Animator animator;
		public string parameter;
		public float maximumVelocityMagnitude;
		public BaseReel reel;

		public void Apply()
		{
			float v = reel.movement.GetVelocity() / maximumVelocityMagnitude;
			if (v < 0) v *= -1f;

			animator.SetFloat(parameter, v);
		}

		public void Update()
		{
			Apply();
		}
	}
}