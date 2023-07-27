using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public abstract class BaseReelStrip : MonoBehaviour, IReelStrip
    {
        public abstract int stripIndex { get; set; }
        public abstract int stripCount { get; }
        public abstract int stripSubSymbolOffset { get; set; }

        public abstract int GetRandomIndex();
        public abstract int CalcIndex(int idx);
        public abstract SymbolInfo GetSymbol(int index);
        public virtual void CopyFrom(IReelStrip value) {}
        public virtual void InsertRange(int beginIndex, List<SymbolInfo> insertRange) {}
        public virtual void ReplaceRange(int beginIndex, List<SymbolInfo> replaceRange) {}
        public virtual void RemoveRange(int beginIndex, int count) {}
        public virtual bool UnDo() { return false; }
        public virtual bool ReDo() { return false; }
        public virtual void ClearHistory() {}
    }
}
