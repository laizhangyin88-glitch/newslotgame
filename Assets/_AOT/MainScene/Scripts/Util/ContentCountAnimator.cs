using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker
{
	public class ContentCountAnimator : MonoBehaviour
	{
	    public enum CountFormat
	    {
	        SimpleNumberFormat,
	        CommaNumberFormat,
	        NumberFormat,
	    };
	    public CountFormat countNumberFormat = CountFormat.SimpleNumberFormat;

		public float minCreditRate = 1f;
		public float maxCreditRate = 10f;
		public float timeX = 0.5f;
		public float timeY = 3f;
		public float minTime = 0.1f;
		public float maxTime = 10f;

		private IContextText creditText;

		private long unitCount;
		private long currentCount;
		private long targetCount;

	    private long deltaCount;

	    public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";

	    public static readonly string ON_FINISH_COUNT_ANIMATOR = "OnFinishContentCountAnimator";

		public void SetCount(IContextText creditText, long targetCount)
		{
			StopAllCoroutines();

			this.creditText = creditText;
			this.currentCount = targetCount;
			this.targetCount = targetCount;

			SetText(targetCount);
		}

		public void UpdateCount(IContextText creditText, long unitCount, long targetCount)
		{
			StopAllCoroutines();

			this.creditText = creditText;
			this.unitCount = unitCount;
			this.targetCount = targetCount;

			SetText(currentCount);

			if (currentCount < targetCount)
			{
				UpdateCoefficient();
				StartCoroutine(UpdateTurnCount());
			}
		}

		private void UpdateCoefficient()
		{
			float cA = (timeY - timeX) / (maxCreditRate - minCreditRate);
			float cB = timeX - cA * minCreditRate;

			long diffCredit = targetCount - currentCount;
			float diffCreditRate = (float)diffCredit / (float)unitCount;
			float deltaTime = Mathf.Clamp(cA * diffCreditRate + cB, minTime, maxTime);
			deltaCount = (long)(diffCreditRate / deltaTime * (float)unitCount);
		}

		private void SetText(long credit)
		{
	        string outputText = "";
	        if (countNumberFormat == CountFormat.SimpleNumberFormat)
	            outputText = FormatUtility.SimpleNumberFormat(credit);
	        else if (countNumberFormat == CountFormat.CommaNumberFormat)
	            outputText = FormatUtility.CommaNumberFormat(credit);
	        else
	            outputText = credit.ToString();

	        creditText.SetText(outputText);
		}

		private IEnumerator UpdateTurnCount()
		{
			while (currentCount < targetCount)
			{
				yield return null;

				currentCount += (long)Mathf.Max((float)deltaCount * Time.deltaTime, 1f);
				if (currentCount >= targetCount)
				{
					currentCount = targetCount;
	                MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData(ON_FINISH_COUNT_ANIMATOR));
				}
				SetText(currentCount);
			}
		}
	}
}
