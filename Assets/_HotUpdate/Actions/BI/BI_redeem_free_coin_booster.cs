using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_redeem_free_coin_booster : ActionTask<Blackboard>
{
	public BBParameter<string> credit;
	public BBParameter<string> multiplier;

	private static string BIEventName = "client_free_coin_booster";

	protected override void OnExecute()
	{
		var creditVariable = BlackboardUtils.FindVariable<long>(agent, credit.value);
		var multiplierVariable = BlackboardUtils.FindVariable<double>(agent, multiplier.value);

		if(creditVariable == null || multiplierVariable == null)
		{
			EndAction(false);
			return;
		}

		var eventData = new Dictionary<string, object>();
		eventData["earn_coin"] = creditVariable.value;
		eventData["multiplier"] = multiplierVariable.value;
		BiEventUtils.AppendLevelMultiplierEventData(eventData, "coin");
		Analytics.CustomEvent(BIEventName, eventData);

		EndAction();
	}
}

}
