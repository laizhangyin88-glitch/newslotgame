using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_click_max_bet : ActionTask<Blackboard>
{
	public BBParameter<long> bet;

    protected override void OnExecute()
    {
    	var gameId = BlackboardUtils.FindVariable<int>(null, "./game/gameId");

        Analytics.CustomEvent("client_click_max_bet", new Dictionary<string, object>
        {
            { "game_id", gameId.value },
            { "bet", bet.value },
        });

        EndAction();
    }
}

}
