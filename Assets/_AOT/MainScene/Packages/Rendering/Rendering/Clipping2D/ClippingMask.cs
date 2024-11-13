using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Rendering.Clipping2D
{
	[RequireComponent(typeof(RectTransform))]
	public class ClippingMask : MonoBehaviour
	{
		[HideInInspector]
		public RectTransform rectTransform;

		public enum ClippingType
		{
			Rect,
			Texture
		}
		public ClippingType clippingType = ClippingType.Rect;

		[ShowIf("clippingType", ClippingType.Rect)]
		public Vector2 clipSoftness = new Vector2(1000f, 1000f);
		[ShowIf("clippingType", ClippingType.Texture)]
		[Space]
		[PreviewField(ObjectFieldAlignment.Left, Height = 105)]
		public Texture2D clipTexture;
		[ShowIf("clippingType", ClippingType.Texture)]
		public float cutoff = 0f;
		[ShowIf("clippingType", ClippingType.Texture)]
		public float cutoffSoftness = 1f;

		private Vector3[] worldCorners = new Vector3[4];

		private Matrix4x4 _clipMatrix;
		public Matrix4x4 clipMatrix { get { return _clipMatrix; } }

		private Vector4 _clipArgs;
		public Vector4 clipArgs { get { return _clipArgs; } }

		public bool IsActive()
	    {
	    	return isActiveAndEnabled;
	    }

	    public bool IsDestroyed()
	    {
	    	return this == null;
	    }

	    public bool IsRectMask()
	    {
	    	return clippingType == ClippingType.Rect;
	    }

	    public bool IsTextureMask()
	    {
	    	return clippingType == ClippingType.Texture;
	    }

		public void Calculate()
		{
			rectTransform.GetWorldCorners(worldCorners);
			Vector3 position = rectTransform.position;	
			Vector2 halfSize = new Vector2(Vector3.Distance(worldCorners[1], worldCorners[2]), Vector3.Distance(worldCorners[0], worldCorners[1])) * 0.5f;

			Matrix4x4 rotateMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(rectTransform.rotation), Vector3.one);
			Matrix4x4 scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f / halfSize.x, 1f / halfSize.y, 0f));
			_clipMatrix = scaleMatrix * rotateMatrix;

			_clipArgs.x = position.x;
			_clipArgs.y = position.y;
			if (clippingType == ClippingType.Rect)
			{
				_clipArgs.z = clipSoftness.x;
				_clipArgs.w = clipSoftness.y;
			}
			else if (clippingType == ClippingType.Texture)
			{
				_clipArgs.z = cutoff;
				_clipArgs.w = cutoffSoftness;	
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
        	if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        }
#endif
	}
}