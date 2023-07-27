using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class UpdateMetaGameSlotMachineDeck : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<List<int>> indices;
        public BBParameter<string> targetBB = "deck";

        protected override void OnExecute()
        {
            var slotData = MetaSlotMachineContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();
            deck.stripIndices = indices.value;

            ProcessUpdateDeck(deck);

            var variable = BlackboardUtils.GetOrCreateVariable<Deck>(agent, targetBB.value);
            variable.value = deck;

            slotData.deck = deck;

            EndAction();
        }

        private void ProcessUpdateDeck(Deck deck)
        {
            var slotData = MetaSlotMachineContentCustomData.GetSlotData(slotIndex.value);
            int totalColumn = slotData.column;
            int totalRow = slotData.row;

            deck.deck = new List<List<SymbolInfo>>();
            deck.hitMap = new List<List<bool>>();

            int backupIndex = MetaSlotMachineGlobalReelStrips.Instance.index;
            MetaSlotMachineGlobalReelStrips.Instance.index = slotIndex.value;
            var strips = MetaSlotMachineGlobalReelStrips.Instance.GetReelStrips();
            MetaSlotMachineGlobalReelStrips.Instance.index = backupIndex;
            for (int column = 0; column < totalColumn; ++column)
            {
                var hitReel = new List<bool>();
                var reel = new List<SymbolInfo>();
                var strip = strips.GetReelStrip(column);
                for (int row = 0; row < totalRow; ++row)
                {
                    int idx = strip.CalcIndex(deck.stripIndices[column] + row);
                    reel.Add(MetaGameUtils.GetSymbol(slotIndex.value, column, strip, idx));
                    hitReel.Add(false);
                }
                deck.deck.Add(reel);
                deck.hitMap.Add(hitReel);
            }
        }
    }
}