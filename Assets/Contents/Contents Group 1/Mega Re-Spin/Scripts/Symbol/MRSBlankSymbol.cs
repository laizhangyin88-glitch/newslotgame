using GameStudio.Slot.MRS.Utility;
using SlotMaker;

namespace GameStudio.Slot.MRS.Symbol
{
    public class MRSBlankSymbol : SymbolBehaviour
    {
        public override void OnEntry()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {

        }

        public override void OnStopEffect()
        {
            if (this.symbol.column == 1)
                MRSUtility.SendEvent("OnContentUIDetailEvent", "WILD_STACK_STOP", 1);
        }

        public override void OnWin()
        {

        }
    }
}
