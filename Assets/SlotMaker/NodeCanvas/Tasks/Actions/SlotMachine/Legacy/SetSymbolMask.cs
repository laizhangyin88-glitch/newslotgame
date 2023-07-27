using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class SetSymbolMask : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;

    public SymbolAttribute mask;

    protected override string info
    {
        get { return string.Format("[Legacy]Set Symbol({0},{1}) mask", column, row); }
    }

    protected override void OnExecute()
    {
        var deck  = ContentCustomData.GetSlotData(slotIndex.value).deck;

        SymbolInfo symbolInfo = deck.GetSymbol(column.value, row.value);
        symbolInfo.mask = mask;

        EndAction();
    }
}

}
