using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/JackpotChase")]
public class UpdateJackpotInfo : ActionTask
{
    public BBParameter<string> jackpotList;
    public BBParameter<bool>   forceUpdate;

    protected override string info
    {
        get
        {
            return string.Format("Update 'game/jackpotList' by {0}", jackpotList);
        }
    }

    protected override void OnExecute()
    {
        Blackboard bb = ContentBlackboard.Get();
        var jackpotBB = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "jackpotList");
        var src  = BlackboardUtils.FindVariable<List<Blackboard>>(null, jackpotList.value).value;

        if (jackpotBB == null)
        {
            BlackboardUtils.GetOrCreateBlackboardList(bb, "jackpotList");
            for (int i = 0; i < src.Count; ++i)
            {
                var jackpotInfo = BlackboardUtils.CreateBlackboard("jackpotInfo");

                BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "current", src[i].GetValue<long>("current"));
                BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "prev", src[i].GetValue<long>("prev"));
                BlackboardUtils.SetOrCreateValue<int>(jackpotInfo, "deltaMs", src[i].GetValue<int>("deltaMs"));
                BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "min", src[i].GetValue<long>("min"));
                BlackboardUtils.SetOrCreateValue<long>(jackpotInfo, "max", src[i].GetValue<long>("max"));
                BlackboardUtils.SetOrCreateValue<bool>(jackpotInfo, "alarm", src[i].GetValue<bool>("alarm"));

                BlackboardUtils.AddToBlackboardList(bb, "jackpotList", jackpotInfo);
            }
        }
        else
        {
            var dest = jackpotBB.value;
                int destCount = dest.Count;
                int minCount = Mathf.Min(destCount, src.Count);
                //for (int i = 0; i < src.Count; ++i)
                for (int i = 0; i < minCount; ++i)
                {
                if (dest[i].GetValue<long>("prev")    != src[i].GetValue<long>("prev") ||
                    dest[i].GetValue<long>("current") != src[i].GetValue<long>("current") || forceUpdate.value)
                {
                    dest[i].SetValue("current", src[i].GetValue<long>("current"));
                    dest[i].SetValue("prev", src[i].GetValue<long>("prev"));
                    dest[i].SetValue("deltaMs", src[i].GetValue<int>("deltaMs"));
                    dest[i].SetValue("min", src[i].GetValue<long>("min"));
                    dest[i].SetValue("max", src[i].GetValue<long>("max"));
                    dest[i].SetValue("alarm", src[i].GetValue<bool>("alarm"));
                }
            }
        }

        EndAction();
    }
}

}
