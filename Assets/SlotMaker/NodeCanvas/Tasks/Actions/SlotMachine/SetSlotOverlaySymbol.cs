using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSlotOverlaySymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;
    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
	public SymbolAttribute attribute;

	protected override string info
	{
		get { return string.Format("Set Slot OverlaySymbol({0}, {1}, ({2}){3}))", cell, symbolIndex, Operation.ToString(), attribute); }
	}

	protected override void OnExecute()
	{
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		var symbol = sm.GetOverlaySymbol(cell.value.GetHashCode());
		if (symbol == null)
		{
			var symbolInfo = new SymbolInfo
			{
				symbol = symbolIndex.value,
				mask = attribute
			};
			sm.overlay.AddSymbol(cell.value.column, cell.value.row, symbolInfo);
		}
		else
		{
			symbol.symbolIndex = symbolIndex.value;
			symbol.symbolMask = OperationUtils.Operate(symbol.symbolMask, (int)attribute, Operation);
		}

		EndAction();
	}
}

}
