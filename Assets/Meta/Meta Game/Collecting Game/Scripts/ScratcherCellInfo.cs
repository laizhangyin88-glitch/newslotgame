using NodeCanvas.Framework;

namespace BagelCode.Scratcher
{
    [System.Serializable]
    public class ScratcherCellInfo
    {
        public ScratcherCellSymbol symbol;
        public ScratcherCellCover cover;
        public ScratcherCellHighlight highlightInfo;
        
        public ScratcherCellInfo(ScratcherCellSymbol cellSymbol, ScratcherCellCover cellCover, Blackboard symbolBB)
        {
            symbol = cellSymbol;
            cover = cellCover;
            highlightInfo = new ScratcherCellHighlight(symbolBB);
        }
    }
}