using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateSpotWin : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelCount;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<int> symbolCount;

    public BBParameter<int> minimumSymbolCount;

    public BBParameter<List<int>> symbolIndices;

    public bool ignoreOveray;

    /* !!! Reference */
    public BBParameter<List<List<Cell>>> saveAsAddedSymbolList;
    public BBParameter<List<int>>  saveAsSymbolIndices;

    private SymbolMask mask;

    protected SymbolInfo GetSymbolInfo(int column, int row)
    {
        return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
    }

    protected bool IsHit(ref SymbolInfo winSymbolInfo, SymbolInfo symbolInfo)
    {
        if (SymbolMask.HasReject(symbolInfo))
        {
            return false;
        }
        else if (winSymbolInfo.Equals(symbolInfo))
        {
            winSymbolInfo = symbolInfo;
            return true;
        }
        else if (SymbolMask.HasWild(symbolInfo))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected List<Cell> CalcOneSymbol(int symbolIndex)
    {
        var cells = new List<Cell>();
        var winSymbolInfo = SlotUtils.CreateSymbolInfo(symbolIndex, mask);

        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
            int column = reelIndex % this.column.value;
            int row    = reelIndex / this.column.value;

            var symbolInfo  = GetSymbolInfo(column, row);

            if (ignoreOveray && SymbolMask.HasAttribute(symbolInfo, SymbolAttribute.Overlay))
                continue;

            if (SymbolMask.HasWild(symbolInfo) || IsHit(ref winSymbolInfo, symbolInfo))
            {
                cells.Add(new Cell(column, row));
            }
        }

        return cells;
    }

    private bool HasSymbol(int symbolIndex)
    {
        if (symbolIndices.isNone)
            return true;

        for (int i = 0; i < symbolIndices.value.Count; ++i)
        {
            if (symbolIndices.value[i] == symbolIndex)
                return true;
        }
        return false;
    }

    private List<int> symbolIndexList;
    private List<List<Cell>> symbolList;
    protected override void OnExecute()
    {
        mask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;
        List<int> addedSymbolIndices;
        List<List<Cell>> symbolSpots = new List<List<Cell>>();

        if (!symbolIndices.isNone)
        {
            addedSymbolIndices = symbolIndices.value;
        }
        else
        {
            addedSymbolIndices = new List<int>();
        }

        for (int count = 0; count < symbolCount.value; ++count)
        {
            int symbolIndex = count;
            if (HasSymbol(symbolIndex))
            {
                List<Cell> spots = CalcOneSymbol(symbolIndex);
                if (spots.Count >= minimumSymbolCount.value)
                {
                    symbolSpots.Add(spots);
                    if (!addedSymbolIndices.Contains(symbolIndex))
                        addedSymbolIndices.Add(symbolIndex);
                }
            }
        }

        saveAsAddedSymbolList.value = symbolSpots;
        saveAsSymbolIndices.value = addedSymbolIndices;
        EndAction();
    }
}

}
