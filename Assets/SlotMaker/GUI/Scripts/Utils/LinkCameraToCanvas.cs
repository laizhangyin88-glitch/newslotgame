using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[RequireComponent(typeof(Canvas))]
	public class LinkCameraToCanvas : MonoBehaviour 
	{
		public CameraManager.CameraType cameraType;

		private void Awake()
		{
			Link();
		}

		[ContextMenu("Link")]
		public void Link()
		{
			GetComponent<Canvas>().worldCamera = CameraManager.Get(cameraType);
		}
	}
}
