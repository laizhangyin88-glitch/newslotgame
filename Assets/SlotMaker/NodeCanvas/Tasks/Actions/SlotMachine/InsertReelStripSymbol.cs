using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class InsertReelStripSymbol : ActionTask
{
    public BBParameter<int> slotIndex;
    public BBParameter<int> reelIndex;
    public BBParameter<int> beginRowIndex;
    public BBParameter<int> symbolCount;
    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
    public SymbolAttribute extraAttribute;

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        var symbolMask = slotData.symbolMask;
        var strips = GlobalReelStrips.Instance.GetReelStrips();
        var strip = strips.GetReelStrip(reelIndex.value);

        List<SymbolInfo> insertStrip = new List<SymbolInfo>();
        for (int i = 0; i < symbolCount.value; ++i)
        {
            SymbolInfo symbol = SlotUtils.CreateSymbolInfo(symbolIndex.value, symbolMask);
            symbol.mask = (SymbolAttribute)OperationUtils.Operate((int)symbol.mask, (int)extraAttribute, Operation);
            insertStrip.Add(symbol);
        }
        strip.InsertRange(beginRowIndex.value, insertStrip);

        EndAction();
    }
}

}
