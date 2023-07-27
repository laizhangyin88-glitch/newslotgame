using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

using UnityEngine;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CreateReelSpinOrders : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<bool> direction = true;
    public List<SymbolAttribute> symbolMask;
    public BBParameter<List<int>> saveAs;

    protected override string info { get { return string.Format("CreateReelSpinOrders"); } }

    protected int GetDirectionalColumn(int column)
    {
        return direction.value ? column : (this.column.value - 1) - column;
    }

    private SymbolAttribute GetSymbolMask(int i)
    {
        int column = Mathf.Min(i, symbolMask.Count-1);
        return symbolMask[column];
    }

    protected override void OnExecute()
    {
        var deck  = ContentCustomData.GetSlotData(slotIndex.value).deck;
        var orderList = new List<int>();

        for (int i = 0; i < column.value; ++i)
        {
            int symbolAccumulatedCount = 0;
            int dColumn = GetDirectionalColumn(i);
            for (int j = 0; j < row.value; ++j)
            {
                var symbolInfo = deck.GetSymbol(dColumn, j);
                if (SymbolMask.HasAttribute(symbolInfo, GetSymbolMask(i)))
                {
                    ++symbolAccumulatedCount;
                }
            }

            if (symbolAccumulatedCount < row.value)
            {
                orderList.Add(i);
            }
        }

        saveAs.value = orderList;

        EndAction();
    }
}

}
