using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_bonus1 : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> bonusResponse;

        protected override string info
        {
            get { return string.Format("BI Bonus {0}", bonusResponse); }
        }

        protected override void OnExecute()
        {
            if (bonusResponse == null || bonusResponse.value == null)
            {
                EndAction();
                return;
            }

            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
            long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");
            int bonusId = bonusResponse.value.GetValue<int>("bonusId");
            long earnCredit = bonusResponse.value.GetValue<long>("earnCredit");

            Analytics.bonus(gameId, betCredit, bonusId, earnCredit);

            EndAction();
        }
    }
}
