using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_welcome_back_bonus : ActionTask
{
	private long credit;
	private long prevLoginTimestamp;
	private long currentLoginTimestamp;

    protected override void OnExecute()
    {
        var welcomeBackBonus = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "welcomeBackRewardInfo");

        if (welcomeBackBonus != null && welcomeBackBonus.value != null)
        {
        	credit = welcomeBackBonus.value.GetValue<long>("totalEarnCredit");
        	prevLoginTimestamp = welcomeBackBonus.value.GetValue<long>("prevLoginTimestamp");
        	currentLoginTimestamp = welcomeBackBonus.value.GetValue<long>("currentLoginTimestamp");

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["earn_coin"] = credit;
            customData["distance_from_last_login"] = currentLoginTimestamp - prevLoginTimestamp;
            BiEventUtils.AppendFreebieLevelMultiplierEventData(customData, FreebieLevelUtils.FreebieType.WELCOME_BACK_BONUS);

            Analytics.CustomEvent("client_welcome_back_bonus", customData);
        }
        EndAction();
    }
}

}
