using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

public enum BIPickType
{
    Mark = 0,
    Erase = 1,
    QuickPick = 2,
}

[Category("★ BagelCode/BI")]
public class AE_keno_pick : ActionTask<Blackboard>
{
    public BBParameter<string> pickedNumber;
    public BBParameter<string> markedCount;
    public BBParameter<BIPickType> pickType;

    private static string[] BIPickTypeDict = {"mark", "erase", "quick_pick"};

    protected override string info { get { return $"AE Keno Pick [{pickType}]"; } }

    protected override void OnExecute()
    {
        int gameId = BlackboardUtils.FindValue<int>(agent, "./game/gameId");
        long baseBet = BlackboardUtils.FindValue<long>(agent, "./betCredit");
        long extraBet = BlackboardUtils.FindValue<long>(agent, "./extraBetCredit");
        int ticketCount = BlackboardUtils.FindValue<int>(agent, "./game/ticketCount");
        int markedCountValue = BlackboardUtils.FindValue<int>(agent, markedCount.value);

        string pickedNumberString = null;
        if (pickType.value != BIPickType.QuickPick)
        {
            var pickedNumberVariable = BlackboardUtils.FindVariable<int>(agent, pickedNumber.value);
            if (pickedNumberVariable != null)
                pickedNumberString = pickedNumberVariable.value.ToString();
        }

        Analytics.keno_pick(gameId, baseBet, extraBet, ticketCount, (int)pickType.value, pickedNumberString, markedCountValue);

        EndAction();
    }
}

}
