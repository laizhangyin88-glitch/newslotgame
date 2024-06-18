using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

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
            //var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            //if (spinBB == null || spinBB.value == null) return;
            //var responseBB = BlackboardUtils.GetOrCreateBlackboard(spinBB.value, "response");
            //var new_indices = responseBB.GetValue<List<List<int>>>("new_reel_output_list");
            //var gameBB = ContentBlackboard.Get().GetValue<Blackboard>("game");
            //var gameId = gameBB.GetValue<int>("gameId");

            deck.deck = new List<List<SymbolInfo>>();
            deck.hitMap = new List<List<bool>>();

            //if (globalStore.IsNewGame(gameId))
            //{
            //    var count = new_indices[0].Count;
            //    for (int i = 0; i < count; i++)
            //    {
            //        var hitReel = new List<bool>();
            //        var reel = new List<SymbolInfo>();
            //        string str = "";
            //        for (int j = 0; j < new_indices.Count; j++)
            //        {
            //            var list = new_indices[j];

            //            for (int k = 0; k < list.Count; k++)
            //            {
            //                if (k == i)
            //                {
            //                    var symbolInfo = new SymbolInfo();
            //                    symbolInfo.link = new SymbolLink()
            //                    {
            //                        columnCount = 1,
            //                        columnOffset = 0,
            //                        rowCount = 1,
            //                        rowOffset = 0,
            //                    };
            //                    symbolInfo.symbol = list[k];
            //                    str += " " + list[k];
            //                    reel.Add(symbolInfo);
            //                    hitReel.Add(false);
            //                    break;
            //                }
            //            }
            //        }
            //        Debug.LogError(str);
            //        deck.deck.Add(reel);
            //        deck.hitMap.Add(hitReel);
            //    }
            //}
            //else
            {  
                Variable<bool> variable = BlackboardUtils.GetOrCreateVariable<bool>(BlackboardUtils.GetContentFSMBlackboard(), "_hasWild");
                variable.value = false;
                var slotData = ContentCustomData.GetSlotData(slotIndex.value);
                int totalColumn = slotData.column;
                int totalRow = slotData.row;
                var strips = GlobalReelStrips.Instance.GetReelStrips();
                for (int column = 0; column < totalColumn; ++column)
                {
                    var hitReel = new List<bool>();
                    var reel = new List<SymbolInfo>();
                    var strip = strips.GetReelStrip(column);
                    for (int row = 0; row < totalRow; ++row)
                    {
                        int idx = strip.CalcIndex(deck.stripIndices[column] + row);
                        var temp = SlotUtils.GetSymbol(slotIndex.value, column, strip, idx);
                        if (temp != null && temp.symbol == 0) ///水果派对，wild牌的值为 0
                        {
                            variable.value = true;
                        }
                        reel.Add(temp);
                        hitReel.Add(false);
                    } 
                    deck.deck.Add(reel);
                    deck.hitMap.Add(hitReel);
                } 
                //BlackboardUtils.SetOrCreateValue<bool>(null, "./customData/_hasWild", variable.value);
                BlackboardUtils.GetOrCreateVariable<bool>(BlackboardUtils.GetContentFSMBlackboard(), "_hasWild").value = variable.value;
            }
        }
    }
}
