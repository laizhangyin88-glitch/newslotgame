using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_freespin_trigger : ActionTask<Blackboard>
    {
        public BBParameter<bool> isAdd;

        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
            long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");

            var bonus = BlackboardUtils.FindValue<Blackboard>("./spin/parent");
            if (bonus.GetValue<ContentNodeType>("type") == ContentNodeType.Turn)
                bonus = BlackboardUtils.FindValue<Blackboard>("./bonus");

        	var totalSpinCount = bonus.GetValue<int>("totalSpinCount");
        	var spinCount = bonus.GetValue<int>("spinCount");
            var bonusId = bonus.GetValue<int>("bonusId");

        	var addedSpinCount = totalSpinCount;
            if (isAdd.value)
            	addedSpinCount = BlackboardUtils.FindValue<int>(agent, "response/addedSpinCount");

            Analytics.freespin_trigger(gameId, betCredit, bonusId, totalSpinCount, spinCount, addedSpinCount);

            EndAction();
        }
    }
}
