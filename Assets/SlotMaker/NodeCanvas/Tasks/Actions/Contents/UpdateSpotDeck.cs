using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateSpotDeck : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelCount;
    public BBParameter<List<int>> indices;

    public BBParameter<string> targetBB = "./spin/deck";

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        var symbolMask = slotData.symbolMask;
        var strips = GlobalReelStrips.Instance.GetReelStrips();

        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
            var strip = strips.GetReelStrip(reelIndex);
            int stripIndex = strip.GetRandomIndex();

            var symbolInfo = strip.GetSymbol(stripIndex);
            int symbolIndex = indices.value[reelIndex];
            symbolInfo.symbol = symbolIndex;
            symbolInfo.mask = symbolMask.GetMask(symbolIndex);
            indices.value[reelIndex] = stripIndex;
        }

        var deck = (Deck)slotData.deck.Clone();
        deck.stripIndices = indices.value;

        ProcessUpdateSpotDeck(deck, reelCount.value);

        var variable = BlackboardUtils.GetOrCreateVariable<Deck>(null, targetBB.value);
        variable.value = deck;

        slotData.deck = deck;

        EndAction();
    }

    private void ProcessUpdateSpotDeck(Deck deck, int reelCount)
    {
        int totalColumn = ContentCustomData.GetSlotData(slotIndex.value).column;

        deck.deck   = new List<List<SymbolInfo>>();
        deck.hitMap = new List<List<bool>>();
        var strips = GlobalReelStrips.Instance.GetReelStrips();

        for (int reelIndex = 0; reelIndex < reelCount; ++reelIndex)
        {
            var strip = strips.GetReelStrip(reelIndex);
            int idx   = strip.CalcIndex(deck.stripIndices[reelIndex]);
            int column = reelIndex % totalColumn;
            int row    = reelIndex / totalColumn;

            if (row == 0)
            {
                deck.deck.Add(new List<SymbolInfo>());
                deck.hitMap.Add(new List<bool>());
            }

            deck.deck[column].Add(SlotUtils.GetSymbol(slotIndex.value, reelIndex, strip, idx));
            deck.hitMap[column].Add(false);
        }
    }
}

}
