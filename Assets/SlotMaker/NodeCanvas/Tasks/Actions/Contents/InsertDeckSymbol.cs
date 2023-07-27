using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class InsertDeckSymbol : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;
    public BBParameter<int> symbolCount;
    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
    public SymbolAttribute extraAttribute;

    protected override string info
    {
        get { return string.Format("Insert Deck Symbol({0}, {1}, ({2}){3}))", cell, symbolIndex, Operation.ToString(), extraAttribute); }
    }

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        Deck deck = slotData.deck;
        var symbolMask = slotData.symbolMask;

        for (int i = 0; i < symbolCount.value; ++i)
        {
            SymbolInfo symbol = SlotUtils.CreateSymbolInfo(symbolIndex.value, symbolMask);
            symbol.mask = (SymbolAttribute)OperationUtils.Operate((int)symbol.mask, (int)extraAttribute, Operation);
            deck.deck[cell.value.column].Insert(cell.value.row, symbol);
            deck.deck[cell.value.column].RemoveAt(0);
        }

        EndAction();
    }
}

}
