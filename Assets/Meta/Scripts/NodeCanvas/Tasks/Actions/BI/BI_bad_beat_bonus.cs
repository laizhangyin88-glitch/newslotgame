using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_bad_beat_bonus : ActionTask
{
	private long credit;
    private int bbbId;
	private List<string> typeList = new List<string>{"default", "coolTime"};

    protected override void OnExecute()
    {
        var bbbInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "bbbRewardInfo");

        if(bbbInfo != null && bbbInfo.value != null)
        {
        	credit = bbbInfo.value.GetValue<long>("credit");
            bbbId = bbbInfo.value.GetValue<int>("bbbId");

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = typeList[0];
            customData["earn_coin"] = credit;
            customData["bbb_id"] = bbbId;
            BiEventUtils.AppendLevelMultiplierEventData(customData, "bbb");

            Analytics.CustomEvent("client_bbb", customData);
        }

        EndAction();
    }
}

}
