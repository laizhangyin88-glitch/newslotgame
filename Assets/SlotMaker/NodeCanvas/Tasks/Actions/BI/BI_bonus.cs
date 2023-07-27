using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_bonus : ActionTask<Blackboard>
    {
        public BBParameter<string> bonusId;

        protected override string info
        {
            get { return string.Format("BI Bonus {0}", bonusId); }
        }

        protected override void OnExecute()
        {
            var spin = BlackboardUtils.FindValue<Blackboard>("./spin");
            var bonusResponse = ContentBlackboardUtils.GetBonusResponse(spin, System.Convert.ToInt32(bonusId.value));
            var result = ContentBlackboardUtils.GetSpinResponseValue<Blackboard>("result");

            if (bonusResponse == null || result == null)
            {
                EndAction();
                return;
            }

            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
            long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");
            long earnCredit = bonusResponse.GetValue<long>("earnCredit");

            Analytics.bonus(gameId, betCredit, int.Parse(bonusId.value), earnCredit);

            EndAction();
        }
    }
}
