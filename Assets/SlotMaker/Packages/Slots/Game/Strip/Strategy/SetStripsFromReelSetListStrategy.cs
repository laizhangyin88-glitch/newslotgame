using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Strategy
{
    [CreateAssetMenu(fileName = "New SetStrips Strategy", menuName = "SlotMaker2/Math/Output/Strategy/Strips/SetStrips")]
    public class SetStripsFromReelSetListStrategy : SetStripsStrategy
    {
        public string source = "./game/reelSetList";

        private const string REEL_SEQUENCE_LIST = "reelSequenceList";

        public override void Set(List<SymbolStrips> strips, List<int> remap)
        {
            var found = BlackboardUtils.FindVariable<List<Blackboard>>(null, source);
            if (found != null)
            {
                var bbList3 = found.value;
                for (int i = 0, count3 = remap.Count; i < count3; ++i)
                {
                    int index = remap[i];
                    var bbList2 = bbList3[index].GetValue<List<Blackboard>>(REEL_SEQUENCE_LIST);
                    for (int j = 0, count2 = bbList2.Count; j < count2; ++j)
                    {
                        List<int> strip = bbList2[j].GetValue<List<int>>(BlackboardUtils.LIST_WRAPPER_KEY);
                        strips[i][j].SetStrip(strip);
                    }
                }
            }
        }
    }
}