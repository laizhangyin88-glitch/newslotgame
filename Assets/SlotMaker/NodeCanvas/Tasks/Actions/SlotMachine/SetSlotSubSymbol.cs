using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSlotSubSymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;
    public BBParameter<int> subSymbolIndex;

    protected override string info
    {
        get { return string.Format("Set Sub Symbol of Slot Symbol({0}, {1})", cell, subSymbolIndex); }
    }

    protected override void OnExecute()
    {
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        BaseSymbol symbol = sm.GetSymbol(cell.value.column, cell.value.row);
        if (symbol.symbolInfo.subSymbol == null)
        {
            symbol.symbolInfo.subSymbol = new SubSymbolInfo();
        }
        symbol.symbolInfo.subSymbol.symbol = subSymbolIndex.value;
        EndAction();
    }
}

}
