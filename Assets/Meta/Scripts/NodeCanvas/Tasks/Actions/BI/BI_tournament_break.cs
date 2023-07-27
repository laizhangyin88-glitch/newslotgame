using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_tournament_break : ActionTask<Blackboard>
{
    public BBParameter<string>  idValue;
    public BBParameter<string>  roundValue;
    public BBParameter<string>  rankValue;
    public BBParameter<string>  rewardValue;
    public BBParameter<string>  prizeMultiplierValue;

    protected override void OnExecute()
    {
        var id = BlackboardUtils.FindValue(agent, idValue.value);
        var round = BlackboardUtils.FindValue(agent, roundValue.value);
        var rank = BlackboardUtils.FindValue(agent, rankValue.value);
        var reward = BlackboardUtils.FindValue(agent, rewardValue.value);
        var prizeMultiplier = BlackboardUtils.FindValue(agent, prizeMultiplierValue.value);

        Analytics.CustomEvent("client_tournament", new Dictionary<string, object>
        {
            { "type", "break" },
            { "tournament_id", id },
            { "round", round },
            { "rank", rank },
            { "earn_coin", reward },
            { "prize_multiplier", prizeMultiplier }
        });

        EndAction();
    }
}

}
