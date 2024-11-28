using UnityEngine;
using System.Collections;

namespace SlotMaker.Layout
{
	public class SetRectTransform : MonoBehaviour 
	{
		public bool reverseHorizontal;
		public bool reverseVertical;

		private RectTransform _rectTransform;
		private RectTransform rectTransform
		{
			get 
			{
				if (_rectTransform == null)
					_rectTransform = GetComponent<RectTransform>();
				return _rectTransform;
			}
		}

		public void UpdateRectTransform(Vector4 rect)
		{
			rectTransform.anchoredPosition = new Vector2(rect.x * (reverseHorizontal ? -1f : 1f), rect.y * (reverseVertical ? -1f : 1f));
			rectTransform.sizeDelta        = new Vector2(rect.z, rect.w);
		}
	}
}