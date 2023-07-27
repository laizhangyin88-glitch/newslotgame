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
public class BeginGamble : ActionTask
{
    public BBParameter<long> gambleCredit = 0L;

    protected override void OnExecute()
    {
        var cb = ContentBlackboard.Get();
        var turn = cb.GetValue<Blackboard>("turn");
        Blackboard gamble = null;

        gamble = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(turn, "gamble");
        BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "gamble", gamble);

        string guid = Guid.NewGuid().ToString();
        long timestamp = MetaSystem.GetTimeStamp();
        string ticketId = BlackboardUtils.FindValue<string>(turn, "gambleTicket/ticketId");

        ContentBlackboardUtils.AddSpentCredit(turn, gambleCredit.value);
        
        BlackboardUtils.SetOrCreateValue<long>(turn, "earnCredit", 0L);
        BlackboardUtils.SetOrCreateValue<string>(gamble, "ticketId", ticketId);
        BlackboardUtils.SetOrCreateValue(gamble, "uid", guid);
        BlackboardUtils.SetOrCreateValue<long>(gamble, "betCredit", gambleCredit.value);
        BlackboardUtils.SetOrCreateValue<long>(gamble, "earnCredit", 0L);
        BlackboardUtils.SetOrCreateValue<long>(gamble, "singleCredit", 0L);
        BlackboardUtils.SetOrCreateValue<long>(gamble, "multiplier", 1L);
        BlackboardUtils.SetOrCreateValue<int>(gamble, "spinCount", 0);
        BlackboardUtils.SetOrCreateValue<long>(gamble, "beginTime", timestamp);
        BlackboardUtils.SetOrCreateValue<Blackboard>(gamble, "parent", turn);

        EndAction();
    }
}

}
