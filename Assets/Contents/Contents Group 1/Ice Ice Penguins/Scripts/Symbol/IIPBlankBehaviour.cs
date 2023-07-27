using SlotMaker;

namespace GameStudio.Slot.IIP.Symbol
{
    public class IIPBlankBehaviour : SymbolBehaviour
    {
        public override void OnEntry()
        {
            animator.SetBool("Text", false);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
        }
    }
}
