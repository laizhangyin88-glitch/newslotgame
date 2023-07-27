using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.SymbolBehaviour
{
    public class EDMBlankBehaviour : SlotMaker.SymbolBehaviour
    {
        int blankSymbolIndex = 0;

        public override void OnEntry()
        {
            PlayAnimation("Idle");
            animator.SetBool("Text", false);
            blankSymbolIndex = Random.Range(0, 9);

            (eventHandler as DefaultSymbolEventHandler).symbolPresets[symbol.symbolIndex].value[0].spriteIndex = blankSymbolIndex;
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
        }
    }
}