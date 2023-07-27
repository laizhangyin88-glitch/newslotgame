namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMLowBehaviour : SlotMaker.SymbolBehaviour
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