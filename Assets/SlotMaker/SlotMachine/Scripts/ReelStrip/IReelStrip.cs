using System.Collections.Generic;

namespace SlotMaker
{
    public interface IReelStrip
    {
        int stripIndex { get; set; }
        int stripCount { get; }
        int stripSubSymbolOffset { get; set; }

        int GetRandomIndex();

        int CalcIndex(int index);

        SymbolInfo GetSymbol(int index);

        void CopyFrom(IReelStrip value);

        void InsertRange(int index, List<SymbolInfo> insertList);

        void ReplaceRange(int index, List<SymbolInfo> replaceList);

        void RemoveRange(int index, int count);

        bool UnDo();

        bool ReDo();

        void ClearHistory();
    }

    public interface IReelStrips
    {
        int reelCount { get; }

        BaseReelStrip GetReelStrip(int reelIndex);

        void SetReelStrip(int reelIndex, IReelStrip reelStrip);
    }

    public interface IGlobalReelStrips
    {
        int index { get; set; }

        int stripsCount { get; }

        ReelStrips GetReelStrips();
    }
}
