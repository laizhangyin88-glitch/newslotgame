using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/JackpotChase")]
public class UpdateJackpotCreditInfo : ActionTask
{
    public BBParameter<int>    jackpotIndex;
    public BBParameter<string> jackpotList;

    // Save As
    public BBParameter<long> prev;
    public BBParameter<long> current;
    public BBParameter<int>  deltaMs;
    public BBParameter<long> min;
    public BBParameter<long> max;
    public BBParameter<bool> alarm;
    public BBParameter<float> endTime;

    protected override void OnExecute()
    {
        var src = BlackboardUtils.FindVariable<List<Blackboard>>(null, jackpotList.value).value;
        var jackpot = src[jackpotIndex.value];

        prev.value         = jackpot.GetValue<long>("prev");
        current.value      = jackpot.GetValue<long>("current");
        deltaMs.value      = jackpot.GetValue<int>("deltaMs");
        min.value          = jackpot.GetValue<long>("min");
        max.value          = jackpot.GetValue<long>("max");
        alarm.value        = jackpot.GetValue<bool>("alarm");

        if (deltaMs.value == 0)
            deltaMs.value = 60000;
            
        endTime.value = (float)deltaMs.value * 0.001f;
        EndAction();
    }
}

}
