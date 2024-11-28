using System;
using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/EarlyAccess")]
public class GetBonusSpinCount : ActionTask<Blackboard>
{
    public BBParameter<string> gameId;

    public BBParameter<long> endTimestamp;

    [BlackboardOnly]
    public BBParameter<int> bonusSpinCount;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Get Bonus Spin Count({1})", bonusSpinCount, gameId);
        }
    }

    protected override void OnExecute()
    {
        int gameID = BlackboardUtils.FindVariable<int>(agent, gameId.value).value;


        bonusSpinCount.value = BlackboardQueryUtils.GetBonusSpinTotalCount(gameID);
        endTimestamp.value = BlackboardQueryUtils.GetBonusSpinEndTimestamp(gameID);
        
        if(endTimestamp.value == 0)
        {
            endTimestamp.value = TimeUtils.GetNextDayTimestamp(true);
        }
        else
        {
            endTimestamp.value = TimeUtils.ApplyTimeZoneOffset( endTimestamp.value );
        }
            
        EndAction();
    }
}

}
