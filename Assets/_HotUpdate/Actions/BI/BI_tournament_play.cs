using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_tournament_play : ActionTask<Blackboard>
{
    public BBParameter<string>  idValue;
    public BBParameter<string>  roundValue;

    protected override string info
    {
        get { return string.Format("BI Tournament Play ({0}, {1})", idValue, roundValue); }
    }

    protected override void OnExecute()
    {
        var id = BlackboardUtils.FindValue(agent, idValue.value);
        var round = BlackboardUtils.FindValue(agent, roundValue.value);

        Analytics.CustomEvent("client_tournament", new Dictionary<string, object>
        {
            { "type", "play" },
            { "tournament_id", id },
            { "round", round }
        });

        EndAction();
    }
}

}
