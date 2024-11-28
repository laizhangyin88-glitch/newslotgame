using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[RequireComponent(typeof(CanvasRenderer))]
	[ExecuteInEditMode]
	public class CanvasRendererProperty : MonoBehaviour
	{
		protected CanvasRenderer _canvasRenderer;
		protected CanvasRenderer canvasRenderer
		{
			get
			{
				if (_canvasRenderer == null)
					_canvasRenderer = GetComponent<CanvasRenderer>();
				return _canvasRenderer;
			}
		}

		public Color color = Color.white;
		public float alpha = 1f;

		private void LateUpdate()
		{
			Color currentColor = canvasRenderer.GetColor();
			if (!currentColor.Equals(color))
				canvasRenderer.SetColor(color);

			float currentAlpha = canvasRenderer.GetAlpha();
			if (currentAlpha != alpha)
				canvasRenderer.SetAlpha(alpha);
		}
	}
}
