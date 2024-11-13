using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bonus")]
public class SetDailySpinCount : ActionTask<Blackboard>
{
    public BBParameter<int> count;
    public BBParameter<MetaJackpotType> type;

    protected override string info
    {
        get { return string.Format("Set Daily Spin Count as {0}", count.value); }
    }

    protected override void OnExecute()
    {
        BlackboardQueryUtils.SetDailySpinCount(count.value, type.value);
        EndAction(true);
    }
}

}
