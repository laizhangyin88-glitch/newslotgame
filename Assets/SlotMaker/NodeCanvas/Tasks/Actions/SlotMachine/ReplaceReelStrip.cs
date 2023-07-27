using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ReplaceReelStrip : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<GameObject> reelStripObject = null;

    public BBParameter<int> reelIndex;
    public BBParameter<int> beginIndex;
    public BBParameter<List<int>> replaceList;

    protected override string info
    {
        get { return string.Format("Replace ReelStrip({0}, {1}, {2})", reelIndex, beginIndex, replaceList); }
    }

    protected override void OnExecute()
    {
        var strips = GlobalReelStrips.Instance.GetReelStrips();
        BaseReelStrip reelStrip = (reelStripObject.value == null) ? strips.GetReelStrip(reelIndex.value) : reelStripObject.value.GetComponent<ReelStrip>();
        var symbolMask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;

        var newList = new List<SymbolInfo>();

        for (int index = 0; index < replaceList.value.Count; index++)
        {
            int symbolIndex = replaceList.value[index];
            var newSymbolInfo = new SymbolInfo();
            newSymbolInfo.symbol = symbolIndex;
            newSymbolInfo.mask = symbolMask.GetMask(symbolIndex);
            newList.Add(newSymbolInfo);
        }

        reelStrip.ReplaceRange(beginIndex.value % reelStrip.stripCount, newList);
        EndAction();
    }
}

}
