using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSBlankBehaviour : SymbolBehaviour
    {
        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
        }

        public override void OnStopEffect()
        {
        }

        public override void OnWin()
        {
        }
        public override void OnEntry()
        {
            animator.gameObject.SetActive(true);
        }
    }
}
