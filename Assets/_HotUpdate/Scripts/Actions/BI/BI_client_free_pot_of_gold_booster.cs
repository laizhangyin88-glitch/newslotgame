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
public class BI_client_free_pot_of_gold_booster : ActionTask<Blackboard>
{
    public BBParameter<long> earnCoin;
    public BBParameter<double> multiplier;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["earn_coin"] = earnCoin.value;
        customData["multiplier"] = multiplier.value;

        BiEventUtils.AppendLevelMultiplierEventData(customData, "pog");

        Analytics.CustomEvent("client_free_pot_of_gold_booster", customData);

        EndAction();
    }
}

}
