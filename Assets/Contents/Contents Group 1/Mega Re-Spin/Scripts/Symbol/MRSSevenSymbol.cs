using SlotMaker;

namespace GameStudio.Slot.MRS.Symbol
{
    public class MRSSevenSymbol : SymbolBehaviour
    {
        private const int Win = 0;
        public override void OnEntry()
        {
        }
        public override void StopBehaviour()
        {
            base.StopBehaviour();
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            GetCachedObject(Win).SetActive(false);
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
            GetCachedObject(Win).SetActive(true);
            PlayAnimation("Inactive");
        }
    }
}
