using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Cards
{
    [CreateAssetMenu(fileName="New PokerPayTable", menuName="SlotMaker/ScriptableObject/PokerPayTable")]
    public class PokerPayTable : ScriptableObject
    {
        [Serializable]
        public class RankComparer
        {
            public string name;
            public PairComparer pair;
            public bool flush;
            public StraightComparer straight;
        }

        [Serializable]
        public class PairComparer
        {
            public bool enabled;
            public int[] kind;
            public int[] include;
            public int[] with;
        }

        [Serializable]
        public class StraightComparer
        {
            public bool enabled;
            public bool royal;
            public bool natural;
        }

        public List<RankComparer> ranks;
    }
}
