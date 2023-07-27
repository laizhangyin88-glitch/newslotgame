using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class BeginTurn : ActionTask
    {
        public BBParameter<int> historyLimit;

        protected override void OnExecute()
        {
            var cb = ContentBlackboard.Get();
            var turnVal = cb.GetVariable<Blackboard>("turn");
            if (turnVal != null)
            {
                var history = BlackboardUtils.AddToBlackboardList(cb, "history", turnVal.value);
                while (history.Count > historyLimit.value)
                    BlackboardUtils.RemoveAtBlackboardList(cb, "history", 0);
                cb.RemoveVariable("turn");
            }
            var turn = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(cb, "turn");
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "current", turn);

            string guid = Guid.NewGuid().ToString();
            long totalBetCredit = cb.GetValue<long>("totalBetCredit");
            bool isGameSpin = cb.GetValue<bool>("isGameSpin");
            long initialCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit").value;
            long timestamp = MetaSystem.GetTimeStamp();

            BlackboardUtils.SetOrCreateValue<ContentNodeType>(turn, "type", ContentNodeType.Turn);
            BlackboardUtils.SetOrCreateValue(turn, "uid", guid);
            ContentBlackboardUtils.AddTurnCount(turn);
            BlackboardUtils.SetOrCreateValue<long>(turn, "totalBetCredit", totalBetCredit);
            ContentBlackboardUtils.AddSpentCredit(turn, isGameSpin ? 0 : totalBetCredit);
            BlackboardUtils.SetOrCreateValue<long>(turn, "earnCredit", 0L);
            BlackboardUtils.SetOrCreateValue<long>(turn, "beginCredit", initialCredit);
            BlackboardUtils.SetOrCreateValue<long>(turn, "beginTime", timestamp);

            ContentEvent.BeginTurn(turn);

            EndAction();
        }
    }
}
