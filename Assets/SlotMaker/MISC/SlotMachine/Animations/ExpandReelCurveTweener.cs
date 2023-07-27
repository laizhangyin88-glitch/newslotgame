using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	public class ExpandReelCurveTweener : MonoBehaviour, ISkippable
	{
		public AnimationCurve curve;
		public float animationTime;

		public SpritePanel panel;
		public RectTransform rectTransform;

	    public bool needTranslate = true;

		public bool reverseHorizontal;
		public bool reverseVertical;

		private bool isRunning = false;
		private Vector4 target;
		private IEnumerator coroutine;

		public void Skip()
		{
			if (isRunning)
			{
				StopCoroutine(coroutine);
				PlayImmediatly(target);
				isRunning = false;
			}
		}

		public void Play(Vector4 newRect)
		{
			Skip();

			coroutine = PlayCoroutine(newRect);
			StartCoroutine(coroutine);
		}

		public void PlayImmediatly(Vector4 newRect)
		{
			if (panel != null)
				panel.clipRange = newRect;

			if (rectTransform != null)
			{
				rectTransform.anchoredPosition = needTranslate ? new Vector2(newRect.x * (reverseHorizontal ? -1f : 1f), newRect.y * (reverseVertical ? -1f : 1f)) : rectTransform.anchoredPosition;
				rectTransform.sizeDelta = new Vector2(newRect.z, newRect.w);
			}
		}

		private IEnumerator PlayCoroutine(Vector4 newRect)
		{
			isRunning = true;

	        target = newRect;

			Vector4 oldRect     = Vector4.zero;
			Vector3 oldPosition = Vector3.zero;
			Vector3 newPosition = Vector3.zero;
			Vector2 oldSize     = Vector2.zero;
			Vector2 newSize     = Vector2.zero;

			if (panel != null)
				oldRect = panel.clipRange;

			if (rectTransform != null)
			{
				newPosition = new Vector2(newRect.x * (reverseHorizontal ? -1f : 1f), newRect.y * (reverseVertical ? -1f : 1f));
				newSize = new Vector2(newRect.z, newRect.w);
				oldPosition = rectTransform.anchoredPosition;
				oldSize = rectTransform.sizeDelta;
			}

			float accTime = 0f;
			while (accTime < animationTime)
			{
				float delta = curve.Evaluate(accTime / animationTime);

				if (panel != null)
					panel.clipRange = oldRect * (1f - delta) + newRect * delta;

				if (rectTransform != null)
				{
	    			rectTransform.anchoredPosition = needTranslate ? (oldPosition * (1f - delta) + newPosition * delta) : oldPosition;
					rectTransform.sizeDelta = oldSize * (1f - delta) + newSize * delta;
				}

				accTime += Time.deltaTime;

				yield return null;
			}

			if (panel != null)
				panel.clipRange = newRect;

			if (rectTransform != null)
			{
	            rectTransform.anchoredPosition = needTranslate ? newPosition : oldPosition;
				rectTransform.sizeDelta = newSize;
			}

			isRunning = false;
		}
	}
}
