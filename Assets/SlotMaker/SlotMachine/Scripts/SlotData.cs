using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class SlotData : MonoBehaviour
    {
        public int column;
        public int row;
        public List<int> visibleCounts;
        public SymbolMask symbolMask;
        public List<MixedLineWinInfo> mixedLineWinInfos;

        public Deck deck;
        public Expectation expectation;
        public MysterySymbolTable mysterySymbolTable;
        public GameObject slotMachine;
        public SymbolRefLinkTable symbolRefLinkTable;


        [Button]
        void test_ShowMysterySymbolTable()
        {
            if (mysterySymbolTable == null || mysterySymbolTable.mysterySymbolReels == null)
                return;
            foreach (List<SymbolInfo> item in mysterySymbolTable.mysterySymbolReels)
            {
                string res = "==@";
                foreach (SymbolInfo sf in item)
                {
                    res += $"{sf.symbol},";
                }
                Debug.Log(res);
            }
        }
    }

}
