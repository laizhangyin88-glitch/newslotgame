using SlotMaker;

namespace GameStudio.Slot.IIP.Symbol
{
    public class IIPMiddleBehaviour : SymbolBehaviour
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
            GetCachedObject(0).SetActive(false);
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
            PlayAnimation("Deactive");
            GetCachedObject(0).SetActive(true);
        }
    }
}
