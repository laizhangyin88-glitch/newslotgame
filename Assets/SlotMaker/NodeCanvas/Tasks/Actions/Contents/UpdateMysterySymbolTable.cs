using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateMysterySymbolTable : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelCount = 1;
    public BBParameter<string> mysterySymbolPath = "./spin/response/result/mysterySymbols";

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        Dictionary<int,int> mysterySymbols = BlackboardUtils.FindVariable<Dictionary<int,int>>(agent, mysterySymbolPath.value).value;
        var mysterySymbolTable = slotData.mysterySymbolTable;
        var symbolMask = slotData.symbolMask;

        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex) { 
            foreach(int key in mysterySymbols.Keys)
            {
                mysterySymbolTable.SetSymbol(reelIndex, key, mysterySymbols[key], symbolMask);
            }
        }

        EndAction();
    }
}

}
