using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class InitializeSlotSymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;
    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
    public SymbolAttribute attribute;

    protected override string info
    {
        get { return string.Format("Initialize Slot Symbol({0}, {1}, ({2}){3}))", cell, symbolIndex, Operation.ToString(), attribute); }
    }

    protected override void OnExecute()
    {
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        BaseSymbol symbol = sm.GetSymbol(cell.value.column, cell.value.row);
        symbol.symbolIndex = symbolIndex.value;
        symbol.symbolMask = OperationUtils.Operate(symbol.symbolMask, (int)attribute, Operation);
        symbol.Change(symbol.symbolInfo);
        symbol.Apply();
        EndAction();
    }
}

}
