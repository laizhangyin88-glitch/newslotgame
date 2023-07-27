using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.HOC
{
    public class HOCScatterSymbolBehaviour : SymbolBehaviour
    {
        public override void OnEntry()
        {
        }

        public override void OnSkip()
        {
            animator.Play("Idle");
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);
        }

        public override void OnStopEffect()
        {
            animator.Play("Invisible");
            GetCachedObject(0).SetActive(true);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            animator.Play("Invisible");
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(true);
        }
    }
}
