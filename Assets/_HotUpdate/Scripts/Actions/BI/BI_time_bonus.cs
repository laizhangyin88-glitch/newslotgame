using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_time_bonus : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        var earn_coin = BlackboardUtils.FindVariable<long>(agent, "timeBonusCredit");

        Dictionary<string, object> customData = new Dictionary<string, object>();
        customData["earn_coin"] = earn_coin.value;
        customData["multiplier"] = 1;
        if (VipLounge.VipLounge.Utils.IsEnded)
            customData["vip_lounge_extra_reward"] = null;
        else
            customData["vip_lounge_extra_reward"] = earn_coin.value - NumberUtils.GetDevideNumeratorValue(earn_coin.value, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("LOBBY_BONUS"));
        BiEventUtils.AppendFreebieLevelMultiplierEventData(customData, FreebieLevelUtils.FreebieType.TIME_BONUS);
        Analytics.CustomEvent("client_time_bonus", customData);

        EndAction();
    }
}

}
