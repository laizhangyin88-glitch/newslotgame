using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ReplaceReelStripWithCustomData<T> : ActionTask
{
    public BBParameter<GameObject> reelStripsObject = null;

    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelIndex;
    public BBParameter<int> beginIndex;
    public BBParameter<List<int>> replaceList;
    public BBParameter<string> customDataKey;
    public BBParameter<T> customDataValue;
    public BBParameter<bool> clearCustomData;

    protected override string info
    {
        get { return string.Format("Replace ReelStrip({0}, {1}, {2})", reelIndex, beginIndex, replaceList); }
    }

    protected override void OnExecute()
    {
        var strips = (reelStripsObject.isNull || reelStripsObject.isNone) ? GlobalReelStrips.Instance.GetReelStrips() : reelStripsObject.value.GetComponent<ReelStrips>();
        var reelStrip = strips.GetReelStrip(reelIndex.value);
        var symbolMask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;
        var newList = new List<SymbolInfo>();

        for (int index = 0; index < replaceList.value.Count; index++)
        {
            int symbolIndex = replaceList.value[index];
            var newSymbolInfo = (SymbolInfo)reelStrip.GetSymbol(reelStrip.CalcIndex(beginIndex.value)).Clone();
            if (newSymbolInfo.customData == null)
            {
               newSymbolInfo.customData = new Dictionary<string, object>();
            }
            else if (clearCustomData.value)
            {
               newSymbolInfo.customData.Clear();
            }
            newSymbolInfo.customData[customDataKey.value] = customDataValue.value;

            newSymbolInfo.symbol = symbolIndex;
            newSymbolInfo.mask = symbolMask.GetMask(symbolIndex);
            newList.Add(newSymbolInfo);
        }

        reelStrip.ReplaceRange(beginIndex.value % reelStrip.stripCount, newList);
        EndAction();
    }
}

}
