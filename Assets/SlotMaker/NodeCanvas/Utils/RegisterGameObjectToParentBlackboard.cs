using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
	public class RegisterGameObjectToParentBlackboard : MonoBehaviour 
	{
		public string key;

		void OnEnable()
		{
			Register();
		}

		void Start()
		{
			Register();
		}

		void Register()
		{
			if (transform.parent == null) 
				return;

			var bb = transform.parent.GetComponentInParent<Blackboard>();
			if (bb == null)
				return;

			bb.SetValue(key, gameObject);
			Destroy(this);
		}
	}
}