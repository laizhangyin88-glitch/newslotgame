using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	public class ReelClipRangeAndRectTransform : MonoBehaviour 
	{
		public SpritePanel panel;
		public RectTransform rectTransform;
		public bool reverseHorizontal;
		public bool reverseVertical;

		public void PlayImmediatly(Vector4 newRect)
		{
			if (panel != null)
				panel.clipRange = newRect;

			if (rectTransform != null)
			{
				rectTransform.anchoredPosition = new Vector2(newRect.x * (reverseHorizontal ? -1f : 1f), newRect.y * (reverseVertical ? -1f : 1f));
				rectTransform.sizeDelta = new Vector2(newRect.z, newRect.w);
			}
		}
	}
}