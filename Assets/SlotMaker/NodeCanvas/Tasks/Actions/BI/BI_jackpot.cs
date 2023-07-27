using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_jackpot : ActionTask
    {
        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
            long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");
            long earnCredit = BlackboardUtils.FindValue<long>("./bonus/response/earnCredit");
            int jackpotType = BlackboardUtils.FindValue<int>("./bonus/response/jackpotIndex");

            Analytics.jackpot(gameId, betCredit, earnCredit, jackpotType);

            EndAction();
        }
    }
}
