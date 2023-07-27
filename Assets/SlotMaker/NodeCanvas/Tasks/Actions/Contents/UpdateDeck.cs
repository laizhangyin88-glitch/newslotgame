using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateDeck : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<List<int>> indices;
    public BBParameter<string> targetBB = "./spin/deck";

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        var deck = (Deck)slotData.deck.Clone();
        deck.stripIndices = indices.value;

        ProcessUpdateDeck(deck);

        var variable = BlackboardUtils.GetOrCreateVariable<Deck>(null, targetBB.value);
        variable.value = deck;

        slotData.deck = deck;

        EndAction();
    }

    private void ProcessUpdateDeck(Deck deck)
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        int totalColumn = slotData.column;
        int totalRow = slotData.row;

        deck.deck = new List<List<SymbolInfo>>();
        deck.hitMap = new List<List<bool>>();
        var strips = GlobalReelStrips.Instance.GetReelStrips();
        for (int column = 0; column < totalColumn; ++column)
        {
            var hitReel = new List<bool>();
            var reel = new List<SymbolInfo>();
            var strip = strips.GetReelStrip(column);
            for (int row = 0; row < totalRow; ++row)
            {
                int idx = strip.CalcIndex(deck.stripIndices[column] + row);
                reel.Add(SlotUtils.GetSymbol(slotIndex.value, column, strip, idx));
                hitReel.Add(false);
            }
            deck.deck.Add(reel);
            deck.hitMap.Add(hitReel);
        }
    }
}

}
