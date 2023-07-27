using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/GameObject/Destroy Root")]
	public class DestroyRoot : MonoBehaviour 
	{
		public void DestroyRootGameObject()
		{
			Destroy(gameObject.GetRoot());
		}
	}
}