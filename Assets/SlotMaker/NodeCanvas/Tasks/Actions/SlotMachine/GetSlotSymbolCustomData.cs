using System.Collections;
using UnityEngine;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetSlotSymbolCustomData<T> : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;

    public BBParameter<string> key = "";
    [BlackboardOnly]
    public BBParameter<T> saveAs;

	protected override string info
	{
		get { return string.Format("Get Slot Symbol CustomData({0}, {1})", cell, key); }
	}

	protected override void OnExecute()
	{
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        var symbol = sm.GetPivotSymbol(cell.value.column, cell.value.row);

        if (symbol.symbolInfo.customData.ContainsKey(key.value))
        {
            saveAs.value = (T)symbol.symbolInfo.customData[key.value];
        }
        else
        {
            return;
        }

		EndAction();
	}
}

}
