using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_big_win : ActionTask<Blackboard>
    {
        public BBParameter<int> bigWinType;
        public BBParameter<string> spinType;

        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
            long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");
            long earnCredit = BlackboardUtils.FindValue<long>("./spin/earnCredit");

            var spinTypeValue = BlackboardUtils.FindVariable<int>(agent, spinType.value);
            bool freeSpin = false;
            if (spinTypeValue != null)
                freeSpin = System.Convert.ToBoolean(spinTypeValue.value);

            Analytics.big_win(gameId, betCredit, earnCredit, bigWinType.value, freeSpin);

            EndAction();
        }
    }
}
