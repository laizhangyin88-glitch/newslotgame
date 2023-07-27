using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class InsertSymbolToSlotMachine : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;
    public BBParameter<int> symbolCount;
    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
    public SymbolAttribute extraAttribute;

    protected override string info
    {
        get { return string.Format("Insert SlotMachine Symbol({0}, {1}, ({2}){3}))", cell, symbolIndex, Operation.ToString(), extraAttribute); }
    }

    protected override void OnExecute()
    {
        var sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        var reels = sm.GetReels();
        var symbolMask = ContentCustomData.GetSlotData(sm.slotIndex).symbolMask;

        SymbolInfo symbol = SlotUtils.CreateSymbolInfo(symbolIndex.value, symbolMask);
        symbol.mask = (SymbolAttribute)OperationUtils.Operate((int)symbol.mask, (int)extraAttribute, Operation);
        reels[cell.value.column].Insert(cell.value.row, symbolCount.value, symbol);

        EndAction();
    }
}

}
