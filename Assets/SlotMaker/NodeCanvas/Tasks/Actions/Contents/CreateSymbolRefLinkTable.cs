using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CreateSymbolRefLinkTable : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> linkTableCount;

    protected override void OnExecute()
    {
        ContentCustomData.GetSlotData(slotIndex.value).symbolRefLinkTable.Initialize(linkTableCount.value);
        EndAction();
    }
}

}
