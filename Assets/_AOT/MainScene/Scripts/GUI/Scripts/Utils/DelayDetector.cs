using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class DelayDetector : MonoBehaviour 
	{
		public float delay;

		public UnityBoolEvent onValueChanged;

		bool detect;
		Coroutine coroutine;

		void OnEnabled()
		{
			detect = false;
			coroutine = null;
		}

		public void BeginJob()
		{
			EndJob();

			coroutine = StartCoroutine(CheckOverTime(delay));
		}

		public void EndJob()
		{
			if (coroutine != null)
				StopCoroutine(coroutine);

			Detect(false);
		}

		IEnumerator CheckOverTime(float time)
		{
			yield return new WaitForSeconds(time);

			Detect(true);
		}

		void Detect(bool value)
		{
			if (detect != value)
			{
				detect = value;
				onValueChanged.Invoke(value);
			}
		}
	}
}