using SlotMaker;

namespace GameStudio.Slot.TRL.Symbol
{
    public class TRLLowSymbol : SymbolBehaviour
    {
        public override void OnEntry()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
            PlayAnimation("Win");
        }
    }
}
