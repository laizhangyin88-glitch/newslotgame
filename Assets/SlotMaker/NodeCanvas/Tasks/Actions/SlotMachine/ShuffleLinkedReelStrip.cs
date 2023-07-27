using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions.Contents
{
    [Category("★ SlotMaker/SlotMachine")]
    public class ShuffleLinkedReelStrip : ActionTask
    {
        public BBParameter<int> column;
        public BBParameter<int> offset;
        [BlackboardOnly]
        public BBParameter<List<int>> reelStripIndices = new List<int>();

        protected override void OnExecute()
        {
            List<int> reelOutputList = new List<int>();
            var reelStrips = GlobalReelStrips.Instance.GetReelStrips();

            int prevIndex = -1;
            int currentIndex = 0;

            for (int columnIndex = 0; columnIndex < column.value; ++columnIndex)
            {
                var reelStrip = reelStrips.GetReelStrip(columnIndex);
                if (prevIndex != -1)
                {
                    currentIndex = prevIndex;

                    var symbolLink = (SymbolLink)reelStrip.GetSymbol(currentIndex).link;
                    if (1 - symbolLink.columnOffset >= symbolLink.columnCount)
                    {
                        prevIndex = -1;
                    }
                }
                else
                {
                    currentIndex = reelStrip.GetRandomIndex();

                    var symbolLink = (SymbolLink)reelStrip.GetSymbol(currentIndex).link;
                    if (symbolLink.rowCount > 1)
                    {
                        var rowOffset = symbolLink.rowCount - 1 - symbolLink.rowOffset;
                        currentIndex = currentIndex - rowOffset + offset.value;
                    }
                    if (symbolLink.columnCount > 1)
                    {
                        prevIndex = currentIndex;
                    }
                }

                reelOutputList.Add(reelStrip.CalcIndex(currentIndex));
            }

            reelStripIndices.value = reelOutputList;
            EndAction();
        }
    }
}
