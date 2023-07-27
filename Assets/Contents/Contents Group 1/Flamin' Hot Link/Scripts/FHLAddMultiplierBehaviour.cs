using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.FHL
{
    public class FHLAddMultiplierBehaviour : SymbolBehaviour
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
            GSManager.Instance.GetHandler("Plus Symbol Land").Play();
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
