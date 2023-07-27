using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker
{
	public class ContentTurnCredit : MonoBehaviour
	{
		public float minCreditRate = 1f;
		public float maxCreditRate = 10f;
		public float timeX = 0.5f;
		public float timeY = 3f;
		public float extraTimeY = 1.0f;
		public float minTime = 0.1f;
		public float maxTime = 10f;

		private IContextText creditText;
		private long unitCredit;
		private long currentCredit;
		private long targetCredit;

	    private long deltaCredit;

		private const string ON_CREDIT_EVENT = "OnCreditEvent";
		private const string ON_UPDATED_TURN_CREDIT_EVENT = "UpdatedTurnCredit";

		public void SetCredit(IContextText creditText, long targetCredit)
		{
			StopAllCoroutines();

			this.creditText = creditText;
			this.currentCredit = targetCredit;
			this.targetCredit = targetCredit;

			SetText(targetCredit);
		}

		public void UpdateCredit(IContextText creditText, long unitCredit, long targetCredit)
		{
			StopAllCoroutines();

			this.creditText = creditText;
			this.unitCredit = unitCredit;
			this.targetCredit = targetCredit;

			SetText(currentCredit);

			if (currentCredit < targetCredit)
			{
				UpdateCoefficient();
				StartCoroutine(UpdateTurnCredit());
			}
		}

		private void UpdateCoefficient()
		{
			long diffCredit = targetCredit - currentCredit;
			float diffCreditRate = (float)diffCredit / (float)unitCredit;

			float extraTime = diffCreditRate >= 10.0f ? extraTimeY : 0.0f;
			float cA = (timeY + extraTime - timeX) / (maxCreditRate - minCreditRate);
			float cB = timeX - cA * minCreditRate;

			float deltaTime = Mathf.Clamp(cA * diffCreditRate + cB, minTime, maxTime);
			deltaCredit = (long)(diffCreditRate / deltaTime * (float)unitCredit);
		}

		private void SetText(long credit)
		{
			creditText.SetText(FormatUtility.CommaNumberFormat(credit));
		}

		private IEnumerator UpdateTurnCredit()
		{
			while (currentCredit < targetCredit)
			{
				yield return null;

				currentCredit += (long)Mathf.Max((float)deltaCredit * Time.deltaTime, 1f);
				if (currentCredit >= targetCredit)
				{
					currentCredit = targetCredit;
					MessageDispatcher.Dispatch(ON_CREDIT_EVENT, new EventData(ON_UPDATED_TURN_CREDIT_EVENT));
				}
				SetText(currentCredit);
			}
		}
	}
}
