using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_redeem_free_gem_booster : ActionTask<Blackboard>
{
	public BBParameter<string> credit;
	public BBParameter<string> multiplier;

	private static string BIEventName = "client_free_gem_booster";		// 해당 AE는 서버이벤트로 구현되어 클라에서 보내지 않도록 함

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
		Analytics.CustomEvent(BIEventName, eventData);

		EndAction();
	}
}

}
