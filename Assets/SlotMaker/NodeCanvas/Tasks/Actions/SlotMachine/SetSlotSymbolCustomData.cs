using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSlotSymbolCustomData<T> : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;

    public BBParameter<string> key;
    public BBParameter<T> valueA;

	protected override string info
	{
		get { return string.Format("Set Slot Symbol CustomData({0}, {1}, {2})", cell, key, valueA); }
	}

	protected override void OnExecute()
	{
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        var symbol = sm.GetPivotSymbol(cell.value.column, cell.value.row);

        if (symbol.symbolInfo.customData == null)
        {
            symbol.symbolInfo.customData = new Dictionary<string, object>();
        }

        if (symbol.symbolInfo.customData.ContainsKey(key.value))
        {
            symbol.symbolInfo.customData[key.value] = valueA.value;
        }
        else
        {
            symbol.symbolInfo.customData.Add(key.value, valueA.value);
        }

		EndAction();
	}
}

}
