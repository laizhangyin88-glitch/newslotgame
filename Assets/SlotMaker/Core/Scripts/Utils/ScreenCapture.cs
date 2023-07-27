using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class ScreenCapture : MonoWeakSingleton<ScreenCapture>
	{
		public class CaptureParams
		{
			public string path;
			public int quality;
			public enum ScaleType
			{
				RelativeScale,
				FixedWidth,
				FixedHeight
			};
			public ScaleType scaleType;
			public float scaleFactor;
			public bool destroyTexture;
		}

		private class CameraComparer : IComparer<Camera>
		{
			public int Compare(Camera lhv, Camera rhv)
			{
				float ret = lhv.depth - rhv.depth;
				if (ret > 0)
					return 1;
				else if (ret < 0)
					return -1;
				else
					return 0;
			}
		}

	    public void Capture(CaptureParams captureParams, Action<Texture2D> successCallback)
	    {
	        StartCoroutine(CaptureCo(captureParams, successCallback));
	    }

		public IEnumerator CaptureCo(CaptureParams captureParams, Action<Texture2D> successCallback)
		{
			yield return new WaitForEndOfFrame();

			int width = Screen.width;
			int height = Screen.height;

			if (captureParams.scaleType == CaptureParams.ScaleType.RelativeScale)
			{
				width = (int)((float)width * captureParams.scaleFactor);
				height = (int)((float)height * captureParams.scaleFactor);
			}
			else
			{
				float aspectRatio = (float)width / (float)height;
				if (captureParams.scaleType == CaptureParams.ScaleType.FixedWidth)
				{
					width = (int)captureParams.scaleFactor;
					height = (int)(captureParams.scaleFactor / aspectRatio);
				}
				else if (captureParams.scaleType == CaptureParams.ScaleType.FixedHeight)
				{
					width = (int)(captureParams.scaleFactor * aspectRatio);
					height = (int)captureParams.scaleFactor;
				}
			}

			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24);
			RenderTexture.active = renderTexture;

			List<Camera> cameras = new List<Camera>(Camera.allCameras);
			cameras.Sort(new CameraComparer());
			foreach (Camera camera in cameras)
			{
				if (camera.enabled)
				{
					camera.targetTexture = renderTexture;
					camera.Render();
					camera.targetTexture = null;
				}
			}

			Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
			texture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
			if (!captureParams.destroyTexture)
				texture.Apply();

			yield return null;

			if (!string.IsNullOrEmpty(captureParams.path))
			{
				byte[] bytes = captureParams.quality > 0 ? texture.EncodeToJPG(captureParams.quality) : texture.EncodeToPNG();
				File.WriteAllBytes(captureParams.path, bytes);

				if (ApplicationSettings.LogSystem())
					Debug.Log("[ScreenCapture] " + captureParams.path);
			}

			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(renderTexture);

			if (successCallback != null)
				successCallback(texture);

			if (captureParams.destroyTexture)
				Destroy(texture);
		}
	}
}
