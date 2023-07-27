using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animator/Animator Set Int")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSetInt : MonoBehaviour 
	{
		public string propertyName;

		public void SetInt(int value)
		{
			GetComponent<Animator>().SetInteger(propertyName, value);
		}

		public void Increase()
		{
			Animator animator = GetComponent<Animator>();
			animator.SetInteger(propertyName, animator.GetInteger(propertyName)+1);
		}
	}
}