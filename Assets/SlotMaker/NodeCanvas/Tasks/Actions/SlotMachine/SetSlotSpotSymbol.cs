using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSlotSpotSymbol : ActionTask
{
	public BBParameter<GameObject> slotMachine;
    public BBParameter<int> column;
    public BBParameter<int> row;
	public BBParameter<Cell> cell;

    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
	public SymbolAttribute attribute;

	protected override string info { get { return string.Format("Set Slot Spot Symbol({0}, {1}, ({2}){3}))", cell, symbolIndex, Operation.ToString(), attribute); } }

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        int reelIndex = cell.value.row * column.value + cell.value.column;
		BaseSymbol symbol = sm.GetSymbol(reelIndex, cell.value.row);

		symbol.symbolIndex = symbolIndex.value;
		symbol.symbolMask = OperationUtils.Operate(symbol.symbolMask, (int)attribute, Operation);

        EndAction();
	}
}

}
