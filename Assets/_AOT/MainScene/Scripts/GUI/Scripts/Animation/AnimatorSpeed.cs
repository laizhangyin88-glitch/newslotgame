using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[ExecuteInEditMode]
	[AddComponentMenu("SlotMaker/Animator/Animator Speed")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSpeed : MonoBehaviour 
	{
		public float speed = 1f;

		private void Update()
		{
			var animator = GetComponent<Animator>();
			animator.speed = speed;
		}
	}
}