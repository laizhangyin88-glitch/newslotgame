using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace SlotMaker
{
	public class ScreenScaler : MonoBehaviour
	{
		public Vector2 referenceResolution;

		private void Start()
		{
			bool fullScreen = Screen.fullScreen;
			int width = Screen.width;
			int height = Screen.height;

			int magnification = 1;
			while (width > ((int)(referenceResolution.x * 2f)) &&
				height > ((int)(referenceResolution.y * 2f)))
			{
				magnification *= 2;
				width /= 2;
				height /= 2;
			}

			if (magnification > 1)
			{
				Screen.SetResolution(width, height, fullScreen);
			}

			Destroy(this);
		}
	}
}
