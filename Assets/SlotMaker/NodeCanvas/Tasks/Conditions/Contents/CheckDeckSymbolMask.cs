using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions.Contents
{

[Category("★ BagelCode/Contents")]
public class CheckDeckSymbolMask : ConditionTask
{
    public BBParameter<int> slotIndex = 0;
	public BBParameter<int> column;
	public BBParameter<int> row;
    public SymbolAttribute symbolMask;

	protected override string info { get { return string.Format("CheckDeckSymbolMask({0})", symbolMask); } }

    protected override bool OnCheck()
    {
        var deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        var symbolInfo = deck.GetSymbol(column.value, row.value);
		return SymbolMask.HasAttribute(symbolInfo, symbolMask);
    }
}

}
