using UnityEngine;
using System.Collections;
using System;

namespace SlotMaker.Layout
{
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformCurveTweener : MonoBehaviour, ISkippable
	{
		public bool reverseHorizontal;
		public bool reverseVertical;
		
		public AnimationCurve curve;
		public float          animationTime;

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

		private bool isRunning = false;
		private Tuple<Vector2, Vector2> target = null;
		private IEnumerator coroutine;

		public void Skip()
		{
			if (isRunning)
			{
				StopCoroutine(coroutine);
				isRunning = false;

				rectTransform.anchoredPosition = target.Item1;
				rectTransform.sizeDelta        = target.Item2;
			}
		}

		public void Play(Vector4 newRect)
		{
			Skip();

			coroutine = UpdateRectTransformCoroutine(newRect);
			StartCoroutine(coroutine);
		}

		public void PlayImmediatly(Vector4 newRect)
		{

		}

		private IEnumerator UpdateRectTransformCoroutine(Vector4 newRect)
		{
			isRunning = true;

			var oldPos  = rectTransform.anchoredPosition;
			var oldSize = rectTransform.sizeDelta;

			target = new Tuple<Vector2, Vector2>(
				new Vector2(newRect.x * (reverseHorizontal ? -1f : 1f), newRect.y * (reverseVertical ? -1f : 1f)),
				new Vector2(newRect.z, newRect.w)
			);

			float accTime = 0f;
			while (accTime < animationTime)
			{
				float delta = curve.Evaluate(accTime / animationTime);

				rectTransform.anchoredPosition = oldPos  * (1f - delta) + target.Item1 * delta;
				rectTransform.sizeDelta        = oldSize * (1f - delta) + target.Item2 * delta;

				accTime += Time.deltaTime;

				yield return null;
			}

			rectTransform.anchoredPosition = target.Item1;
			rectTransform.sizeDelta        = target.Item2;

			isRunning = false;
		}
	}
}
