using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Tournament")]
public class GetTournamentRoundInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  pathBB;

    public BBParameter<long>    totalPrize;
    public BBParameter<int>     round;
    public BBParameter<double>  roundMultiplier;

    protected override string info
    {
        get { return "Get Round Info BB"; }
    }

    protected override void OnExecute()
    {
        var roundBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(agent, pathBB.value);

        round.value = BlackboardUtils.GetOrCreateVariable<int>(roundBB.value, "serialWinCount").value;
        roundMultiplier.value = BlackboardUtils.GetOrCreateVariable<double>(roundBB.value, "serialWinBonus").value;

        var basePrize = BlackboardUtils.GetOrCreateVariable<long>(roundBB.value, "baseTotalPrize");
        totalPrize.value = System.Convert.ToInt64(System.Convert.ToDouble(basePrize.value) * roundMultiplier.value);

        EndAction();
    }
}

}
