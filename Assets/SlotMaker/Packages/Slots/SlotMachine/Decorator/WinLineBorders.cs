using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class WinLineBorders : MonoBehaviour
    {
        [InlineEditor]
        public Paylines paylines;

        public List<WinLineBorder> borders;

        public int maxRow;

        protected bool firstSingleWin;

        public void TotalWin()
        {
            firstSingleWin = true;
        }

        public void SingleWin(int lineIndex, int hitCount)
        {
            var line = paylines.value[lineIndex];
            int spotCount = line.Count;
            for (int i = 0, count = line.Count; i < count; ++i)
            {
                borders[i].gameObject.SetActive(true);
                borders[i].ShowLineBorder(spotCount, maxRow, i, line[i], (i < hitCount), firstSingleWin);
            }

            firstSingleWin = false;
        }

        public void SingleWin(SpinWin eventData)
        {
            var win = eventData as SpinLineWin;
            if (win != null)
            {
                SingleWin(win.lineIndex, win.hitCount);
            }
        }

        public void SkipWin()
        {
            foreach (var border in borders)
            {
                border.gameObject.SetActive(false);
            }
        }
    }
}