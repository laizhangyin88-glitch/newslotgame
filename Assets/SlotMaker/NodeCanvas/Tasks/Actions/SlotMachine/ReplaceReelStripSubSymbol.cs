using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ReplaceReelStripSubSymbol : ActionTask
{
    public BBParameter<int> reelIndex;
    public BBParameter<int> beginIndex;
    public BBParameter<List<int>> replaceList;

    protected override string info
    {
        get { return string.Format("Replace ReelStrip SubSymbol({0}, {1}, {2})", reelIndex, beginIndex, replaceList); }
    }

    protected override void OnExecute()
    {
        var strips = GlobalReelStrips.Instance.GetReelStrips();
        var reelStrip = strips.GetReelStrip(reelIndex.value);
        var newList = new List<SymbolInfo>();

        for (int index = 0; index < replaceList.value.Count; index++)
        {
            int subSymbolIndex = replaceList.value[index];
            int indexOnReelStrip = (beginIndex.value + index) % reelStrip.stripCount;
            var newSymbolInfo = (SymbolInfo)reelStrip.GetSymbol(indexOnReelStrip).Clone();
            newSymbolInfo.subSymbol.symbol = subSymbolIndex;
            newList.Add(newSymbolInfo);
        }

        reelStrip.ReplaceRange(beginIndex.value % reelStrip.stripCount, newList);
        EndAction();
    }
}

}
