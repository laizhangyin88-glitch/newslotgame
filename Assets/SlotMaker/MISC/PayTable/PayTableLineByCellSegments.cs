using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker
{
    public class PayTableLineByCellSegments : BasePayTableLineSegments
    {
        public int column;
        public int row;

        public override void SetPayLine(int lineNumber, List<int> payLine, in Color color)
        {
            number.text = lineNumber.ToString();
            for (int lineIndex = 0; lineIndex < payLine.Count; ++lineIndex)
            {
                int cellIndex   = payLine[lineIndex];
                int columnIndex = cellIndex % column;
                int rowIndex    = cellIndex / column;
                segments[columnIndex * row + rowIndex].color = color;
            }
        }
    }
}
