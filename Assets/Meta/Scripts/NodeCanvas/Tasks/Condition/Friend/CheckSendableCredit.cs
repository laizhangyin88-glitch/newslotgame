using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Condition
{

[Category("★ BagelCode/Friend")]
public class CheckSendableCredit : ConditionTask<Blackboard>
{
    protected override string info
    {
        get { return "Check sendable credit"; }
    }

    protected override bool OnCheck()
    {
        var bb = agent.GetValue<Blackboard>("cellInfo");
        if (bb == null) { return false; }

        var lastSendGiftTimestamp = bb.GetVariable("lastSendGiftTimestamp");
        if (lastSendGiftTimestamp == null) { return false; }

        int sendGiftTimeInterval  = SlotMaker.BlackboardUtils.FindVariable<int>(agent, "/values/misc/FRIEND_GIFT_SEND_INTERVAL_SEC").value;

        if (BagelCode.TimeUtils.GetTimeStamp() - (long)lastSendGiftTimestamp.value > (long)sendGiftTimeInterval * 1000)
        {
            return true;
        }

        return false;        
    }
}

}
