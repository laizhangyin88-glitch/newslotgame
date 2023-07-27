using SlotMaker;

namespace GameStudio.Slot.IIP.Symbol
{
    public class IIPLowBehaviour : SymbolBehaviour
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
            if (gameObject.activeInHierarchy)
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
