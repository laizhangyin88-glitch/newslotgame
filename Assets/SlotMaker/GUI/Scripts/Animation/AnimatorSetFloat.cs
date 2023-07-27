using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animator/Animator Set Float")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSetFloat : MonoBehaviour 
	{
		public string propertyName;

		public void SetFloat(float value)
		{
			GetComponent<Animator>().SetFloat(propertyName, value);
		}
	}
}