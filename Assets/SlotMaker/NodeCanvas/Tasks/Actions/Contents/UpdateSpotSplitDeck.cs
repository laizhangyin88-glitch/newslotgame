using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateSpotSplitDeck : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> reelCount;
        public BBParameter<List<int>> indices;
        public BBParameter<int> totalColumn;
        public BBParameter<int> totalRow;

        public BBParameter<string> targetBB = "./spin/deck";

        protected override void OnExecute()
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
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
            deck.deck = new List<List<SymbolInfo>>();
            deck.hitMap = new List<List<bool>>();
            var strips = GlobalReelStrips.Instance.GetReelStrips();

            for (int reelIndex = 0; reelIndex < reelCount; ++reelIndex)
            {
                var strip = strips.GetReelStrip(reelIndex);
                int idx = strip.CalcIndex(deck.stripIndices[reelIndex]);
                int column = reelIndex % totalColumn.value;
                int row = reelIndex / totalColumn.value;

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
