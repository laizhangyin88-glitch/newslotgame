using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	[RequireComponent(typeof(Reel))]
	public class RemoveSymbolsCurveTweener : MonoBehaviour, ISkippable 
	{
		public AnimationCurve curve;
		public float          animationTime;

		private Reel _reel;
		private Reel reel
		{
			get 
			{
				if (_reel == null)
					_reel = GetComponent<Reel>();
				return _reel;
			}
		}

		private bool isRunning = false;
		private List<Tuple<RectTransform, float, float>> target = new List<Tuple<RectTransform, float, float>>();
		private IEnumerator coroutine;

		public void Skip()
		{
			if (isRunning)
			{
				StopCoroutine(coroutine);
				isRunning = false;

				for (int i = 0; i < target.Count; ++i)
				{
					var t = target[i];
					var newPosition = t.Item1.anchoredPosition3D;
					newPosition.y = t.Item3;
					t.Item1.anchoredPosition3D = newPosition;
				}
			}
		}

		public void RemoveReel(float height, int row, int count)
		{
			Skip();

			coroutine = RemoveReelCoroutine(height, row, count);
			StartCoroutine(coroutine);
		}

		private IEnumerator RemoveReelCoroutine(float height, int row, int count)
		{
			isRunning = true;

			target.Clear();

			float displacement = height * count;

			var symbols = reel.GetSymbols();
			for (int i = 0; i < symbols.Count; ++i)
			{
				var symbol = symbols[i];
				if (symbol.row < row + count)
				{
					var symbolPos = reel.CalcSymbolPosition(symbol);
					float y = symbolPos.y;
					target.Add(new Tuple<RectTransform, float, float>(symbol.rectTransform, y + displacement, y));
				}
			}

			float accTime = 0f;
			while (accTime < animationTime)
			{
				float delta = curve.Evaluate(accTime / animationTime);

				for (int i = 0; i < target.Count; ++i)
				{
					var t = target[i];
					var newPosition = t.Item1.anchoredPosition3D;
					newPosition.y = t.Item2 * (1f - delta) + t.Item3 * delta;
					t.Item1.anchoredPosition3D = newPosition;
				}

				accTime += Time.deltaTime;

				yield return null;
			}

			for (int i = 0; i < target.Count; ++i)
			{
				var t = target[i];
				var newPosition = t.Item1.anchoredPosition3D;
				newPosition.y = t.Item3;
				t.Item1.anchoredPosition3D = newPosition;
			}
			
			isRunning = false;
		}
	}
}