using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSlotOverlaySubSymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;
    public BBParameter<int> subSymbolIndex;

    protected override string info
    {
        get { return string.Format("Set Sub Symbol of Slot OverlaySymbol({0}, {1})", cell, subSymbolIndex); }
    }

    protected override void OnExecute()
    {
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        var symbol = sm.GetOverlaySymbol(cell.value.GetHashCode());
        if (symbol == null)
        {
            Debug.LogError("[SlotMachine] Cannot found the slot overlay symbol at (" + cell.value.column + ", " + cell.value.row + ")");
            EndAction(false);
        }
        else
        {
            if (symbol.symbolInfo.subSymbol == null) 
            {
                symbol.symbolInfo.subSymbol = new SubSymbolInfo();
            }
            symbol.symbolInfo.subSymbol.symbol = subSymbolIndex.value;
            EndAction();
        }
    }
}

}
