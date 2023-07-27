using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/GameObject/Dont Destroy")]
	public class DontDestroyOnLoad : MonoBehaviour 
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}