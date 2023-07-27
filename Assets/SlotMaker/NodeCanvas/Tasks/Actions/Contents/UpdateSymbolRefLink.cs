using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateSymbolRefLink : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> linkTableIndex;
    public BBParameter<int> linkCount;

    protected override string info
    {
        get { return string.Format("Change Reference SymbolLink"); }
    }

    protected override void OnExecute()
    {
        var symbolRefLinkTable = ContentCustomData.GetSlotData(slotIndex.value).symbolRefLinkTable;

        int prevLinkCount = symbolRefLinkTable.GetSymbolRefLinkCount(linkTableIndex.value);

        for (int i = 0; i < prevLinkCount; ++i)
        {
            SymbolLink symbolLink = symbolRefLinkTable.GetSymbolRefLink(linkTableIndex.value, i);
            symbolLink.rowCount = linkCount.value;
            symbolLink.rowOffset = i % linkCount.value;
        }

        EndAction();
    }
}

}
