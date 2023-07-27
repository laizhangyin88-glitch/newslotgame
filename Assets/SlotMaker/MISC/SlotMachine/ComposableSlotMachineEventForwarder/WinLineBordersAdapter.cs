using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;
using SlotMaker.Slots;

namespace SlotMaker
{
    /// <summary>
    /// Adaptor class for connecting <see cref="WinLineBorders"> to <see cref="SlotWinEventForwarder"/>.
    /// </summary>
    [RequireComponent(typeof(WinLineBorders))]
    public class WinLineBordersAdapter : MonoBehaviour
    {
        private WinLineBorders _winLineBorders;
        protected WinLineBorders winLineBorders { get { return _winLineBorders ?? (_winLineBorders = GetComponent<WinLineBorders>()); } }

        public virtual void TotalWin()
        {
            winLineBorders.TotalWin();
        }

        public virtual void SingleWin(SymbolWin win)
        {
            int lineIndex = win.lineIndex ?? 0;
            if (lineIndex > 0)
            {
                winLineBorders.SingleWin(lineIndex - 1, win.hitCount);
            }
        }

        public virtual void SkipWin()
        {
            winLineBorders.SkipWin();
        }
    }
}
