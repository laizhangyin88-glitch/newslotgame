using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSubSymbolInReelStrips : ActionTask
{

    protected override string info
    {
        get { return string.Format("Set SubSymbolInfo in ReelStrips");}
    }

    protected override void OnExecute()
    {
        var reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;
        for (int reelSetIndex = 0; reelSetIndex < reelSetList.Count; ++reelSetIndex)
        {
            var reelSet = reelSetList[reelSetIndex];
            var subSymbolReelSequenceList = reelSet.GetValue<List<Blackboard>>("subSymbolReelSequenceList");
            var strips = GlobalReelStrips.Instance.stripsList[reelSetIndex];
            
            for (int reelIndex = 0; reelIndex < strips.reelCount; ++reelIndex)
            {
                var reelStrip = strips.GetReelStrip(reelIndex);
                var indexList = subSymbolReelSequenceList[reelIndex].GetValue<List<int>>("value");
                for (int i = 0; i < reelStrip.stripCount; ++i)
                {
                    int subIndex = indexList[i];
                    var symbolInfo = reelStrip.GetSymbol(i);
                    var newSubSymbol = new SubSymbolInfo();
                    newSubSymbol.symbol = subIndex;
                    symbolInfo.subSymbol = newSubSymbol;
                }
            }
        }
        EndAction();
    }
}

}
