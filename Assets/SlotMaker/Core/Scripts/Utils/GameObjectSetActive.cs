using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/GameObject/Set Active")]
	public class GameObjectSetActive : MonoBehaviour 
	{
		public GameObject target;
		public bool awake;

		private void Awake()
		{
			if (awake && target != null)
				target.SetActive(true);
		}

		public void SetActiveGameObject(bool activate)
		{
			if (target != null)
				target.SetActive(activate);
		}

		public void ActivateGameObject()
		{
			if (target != null)
				target.SetActive(true);	
		}

		public void InActivateGameObject()
		{
			if (target != null)
				target.SetActive(false);	
		}
	}
}