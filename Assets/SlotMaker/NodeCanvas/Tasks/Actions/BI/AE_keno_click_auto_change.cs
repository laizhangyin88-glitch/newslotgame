using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class AE_keno_click_auto_change : ActionTask<Blackboard>
{
    public BBParameter<string> isAutoQuickPick;

    protected override string info { get { return $"AE Keno AuotQuickPick = {isAutoQuickPick}"; } }

    protected override void OnExecute()
    {
        int gameId = BlackboardUtils.FindValue<int>(agent, "./game/gameId");
        long baseBet = BlackboardUtils.FindValue<long>(agent, "./betCredit");
        long extraBet = BlackboardUtils.FindValue<long>(agent, "./extraBetCredit");
        int ticketCount = BlackboardUtils.FindValue<int>(agent, "./game/ticketCount");
        bool autoQuickPickType = BlackboardUtils.FindValue<bool>(agent, isAutoQuickPick.value);

        Analytics.keno_click_auto_change(gameId, baseBet, extraBet, ticketCount, autoQuickPickType);

        EndAction();
    }
}

}
