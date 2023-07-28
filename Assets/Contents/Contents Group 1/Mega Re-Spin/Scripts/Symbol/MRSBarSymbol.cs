using SlotMaker;

namespace GameStudio.Slot.MRS.Symbol
{
    public class MRSBarSymbol : SymbolBehaviour
    {
        private const int Win = 0;

        public override void OnEntry()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(Win).SetActive(false);
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
            PlayAnimation("Inactive");
            GetCachedObject(Win).SetActive(true);
        }
    }
}