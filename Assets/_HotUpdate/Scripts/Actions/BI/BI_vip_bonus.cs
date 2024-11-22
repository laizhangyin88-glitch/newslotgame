using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_vip_bonus : ActionTask<Blackboard>
{

	protected override string info
	{
		get
		{
			return "BI VIP Bonus";
		}
	}

	protected override void OnExecute() 
	{
		long earnCredit = agent.GetValue<long>("earnCredit");
		Dictionary<string, object> customData = new Dictionary<string, object>();

		customData["earn_coin"] = earnCredit;
        if (VipLounge.VipLounge.Utils.IsEnded)
            customData["vip_lounge_extra_reward"] = null;
        else
            customData["vip_lounge_extra_reward"] = earnCredit - NumberUtils.GetDevideNumeratorValue(earnCredit, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("SHOP_BONUS"));
		BiEventUtils.AppendLevelMultiplierEventData(customData, FreebieLevelUtils.GetFreebieTypeToString(FreebieLevelUtils.FreebieType.VIP_BONUS));

		Analytics.CustomEvent("client_vip_bonus", customData);
		EndAction();
	}
}

}

