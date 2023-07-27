using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.HOC
{
    public class HOCLowSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry()
        {
        }

        public override void OnSkip()
        {
            animator.Play("Idle");
            var spriteRenderer = animator.GetComponentInChildren<SpriteRenderer>(true);
            spriteRenderer.sortingLayerName = "Base";
            spriteRenderer.sortingOrder = 0;
        }

        public override void OnStopEffect()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            animator.Play("Win");
            var spriteRenderer = animator.GetComponentInChildren<SpriteRenderer>(true);
            spriteRenderer.sortingLayerName = "Foreground";
            spriteRenderer.sortingOrder = 0;
        }
    }
}
