using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.FHL
{
    public class FHLBlankBehaviour : SymbolBehaviour
    {
        private const int Win = 0;
        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            GetCachedObject(0);
        }

        public override void OnWin()
        {
            PlayAnimation("Win");
        }
        public override void OnEntry()
        {
            animator.gameObject.SetActive(true);
        }
    }
}
