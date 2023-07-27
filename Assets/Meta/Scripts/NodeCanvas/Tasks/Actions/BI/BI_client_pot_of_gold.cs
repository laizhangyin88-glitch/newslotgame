using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_pot_of_gold : ActionTask<Blackboard>
{
    public BBParameter<string> type;
    public BBParameter<string> coinValue;

    protected override void OnExecute()
    {
        var earnCoin = BlackboardUtils.FindVariable<long>(agent, coinValue.value);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["type"] = type.value;
        customData["earn_coin"] = earnCoin.value;
        BiEventUtils.AppendLevelMultiplierEventData(customData, "pog");

        Analytics.CustomEvent("client_pot_of_gold", customData);

        EndAction();
    }
}

}
