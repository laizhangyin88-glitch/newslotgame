using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetCalcBetCredit : ActionTask
{
    public BBParameter<long> bet;
    public BBParameter<long> saveAsCalcBet;
    public BBParameter<long> saveAsExtraBet;

    protected override string info
    {
        get { return string.Format("{0} = Get Auto Calc Bet", saveAsCalcBet); }
    }

    protected override void OnExecute()
    {
        var cb = ContentBlackboard.Get();
        var game = cb.GetValue<Blackboard>("game");
        
        // Temp: Get first extra bet info
        var extraBetRatio = game.GetValue<List<Blackboard>>("extraBetRatioList")[0];

        int betRatioExtraBet = extraBetRatio.GetValue<int>("numerator");

        if (betRatioExtraBet > 0)
        {
            int betRatioBaseBet = extraBetRatio.GetValue<int>("denominator");

            saveAsCalcBet.value = (bet.value * betRatioBaseBet) / (betRatioBaseBet + betRatioExtraBet);
            saveAsExtraBet.value = bet.value - saveAsCalcBet.value;
        }
        else
            saveAsExtraBet.value = 0;

        EndAction();
    }
}

}
