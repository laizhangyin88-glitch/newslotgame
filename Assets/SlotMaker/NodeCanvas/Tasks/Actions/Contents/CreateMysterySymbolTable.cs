using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CreateMysterySymbolTable : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelCount = 1;

    public BBParameter<bool> useSingleMysteryTable = true;

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        slotData.mysterySymbolTable.Initialize(reelCount.value, slotData.symbolMask, useSingleMysteryTable.value);
        EndAction();
    }
}


}
