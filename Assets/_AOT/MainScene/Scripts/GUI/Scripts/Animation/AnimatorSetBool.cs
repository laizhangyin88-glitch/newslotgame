using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animator/Animator Set Bool")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSetBool : MonoBehaviour 
	{
		public string propertyName;
		
		public void SetBool(bool value)
		{
			GetComponent<Animator>().SetBool(propertyName, value);
		}

		public void SetTrue()
		{
			GetComponent<Animator>().SetBool(propertyName, true);
		}

		public void SetFalse()
		{
			GetComponent<Animator>().SetBool(propertyName, false);
		}

		public void Toggle()
		{
			GetComponent<Animator>().SetBool(propertyName, !GetComponent<Animator>().GetBool(propertyName));
		}
	}
}