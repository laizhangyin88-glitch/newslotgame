using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker
{
	public class ContentTurnCreditController : MonoBehaviour
	{
		public long unitCredit = 0;
		public long currentCredit = 0;
		public long targetCredit = 0;
		private ContextElement creditText;
		private ContentTurnCredit contentTurnCredit;

		private const string turnPath = "./turn";
		private const string betCreditPath = "./turn/totalBetCredit";
		private const string turnCreditPath = "./turn/earnCredit";
		private const string ON_CREDIT_EVENT = "OnCreditEvent";
		private const string ON_UPDATE_TURN_CREDIT_EVENT = "UpdateTurnCredit";

		public void Awake()
		{
			if (BlackboardUtils.FindVariable(null, turnPath) != null)
			{
				currentCredit = BlackboardUtils.GetOrCreateVariable<long>(null, turnCreditPath).value;
				targetCredit = currentCredit;
			}

			creditText = GetComponent<ContextTextMeshProUGUI>();
			contentTurnCredit = GetComponent<ContentTurnCredit>();
		}

		private void OnEnable()
	    {
	        MessageDispatcher.Register(ON_CREDIT_EVENT, OnCreditEvent);
	    }

	    private void OnDisable()
	    {
	        MessageDispatcher.UnRegister(ON_CREDIT_EVENT, OnCreditEvent);
	    }

		private void OnCreditEvent(EventData eventData)
		{
			if (eventData.name.Equals(ON_UPDATE_TURN_CREDIT_EVENT, StringComparison.Ordinal))
	        {
				if (BlackboardUtils.FindVariable(null, turnPath) == null)
					return;

				unitCredit = BlackboardUtils.GetOrCreateVariable<long>(null, betCreditPath).value;
				targetCredit = BlackboardUtils.GetOrCreateVariable<long>(null, turnCreditPath).value;
				if (eventData.value is bool && (bool)eventData.value)
				{ // Force Update
					currentCredit = targetCredit;
					contentTurnCredit.SetCredit(creditText as IContextText, targetCredit);
				}
				else if (eventData.value is bool && !(bool)eventData.value)
				{
					contentTurnCredit.UpdateCredit(creditText as IContextText, unitCredit, targetCredit);
				}
			}
		}
	}
}