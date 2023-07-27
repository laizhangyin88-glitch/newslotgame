using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateMysterySymbolTableAtReel : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelCount = 1;
    public BBParameter<string> mysterySymbolPath = "./spin/response/result/mysterySymbols";

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);

        List<Blackboard> mysterySymbolsList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, mysterySymbolPath.value).value;
        var mysterySymbolTable = slotData.mysterySymbolTable;
        var symbolMask = slotData.symbolMask;

        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
        	Dictionary<int,int> mysterySymbols = mysterySymbolsList[reelIndex].GetValue<Dictionary<int,int>>("value");
        	foreach(int key in mysterySymbols.Keys)
        	{
        		mysterySymbolTable.SetSymbol(reelIndex, key, mysterySymbols[key], symbolMask);
        	}
        }

        EndAction();
    }
}

}
