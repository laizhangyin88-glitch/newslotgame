using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class LinkReelStripsSuperStack : ActionTask
{
    public BBParameter<int> linkRowCount;
    public BBParameter<int> beginColumn;
    public BBParameter<int> endColumn;
    public BBParameter<int> reelStripsIndex;

    protected override string info
    {
        get { return string.Format("Link Every SuperStack Symbol in ReelStrips[{0}] ({1}, {2})", reelStripsIndex.value, linkRowCount.value, (endColumn.value - beginColumn.value)); }
    }

    protected override void OnExecute()
    {
        //  이 코드는 ReelStrip이 정확한 정보를 가지고 있다고 가정하고 심볼을 Link해줍니다.
        var strips = GlobalReelStrips.Instance.stripsList[reelStripsIndex.value];
        
        for (int columnIndex = beginColumn.value; columnIndex < endColumn.value; ++columnIndex)
        {
            var reelStrip = strips.GetReelStrip(columnIndex);
            
            int columnCount = endColumn.value - beginColumn.value;
            int columnOffset = beginColumn.value - columnIndex;
            
            for (int rowIndex = 0; rowIndex < reelStrip.stripCount; ++rowIndex)
            {
                var symbolInfo = reelStrip.GetSymbol(rowIndex);
                SymbolLink replaceLink = (SymbolLink)symbolInfo.link.Clone();
                replaceLink.rowCount = linkRowCount.value;
                replaceLink.rowOffset = linkRowCount.value - rowIndex % linkRowCount.value - 1;
                replaceLink.columnCount = columnCount;
                replaceLink.columnOffset = columnOffset;
                symbolInfo.link = replaceLink;
            }
        }

        EndAction();
    }
}

}
