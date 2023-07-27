using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [System.Serializable]
    public class MixedLineWinInfo
    {
        public int payIndex;
        #if UNITY_EDITOR
            [EnumFlags]
        #endif
        public List<SymbolAttribute> masks;
    }
}
