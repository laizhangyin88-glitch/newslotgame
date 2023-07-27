namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMHighBehaviour : SlotMaker.SymbolBehaviour
    {
        const int WIN = 0;
        public override void OnEntry()
        {
            animator.SetBool("Text", false);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
            GetCachedObject(WIN).SetActive(false);
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
            PlayAnimation("InActive");
            GetCachedObject(WIN).SetActive(true);
        }
    }
}