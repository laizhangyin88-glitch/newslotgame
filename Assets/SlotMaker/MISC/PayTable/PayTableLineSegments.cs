using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker
{
    public class PayTableLineSegments : BasePayTableLineSegments
    {
        public override void SetPayLine(int lineNumber, List<int> payLine, in Color color)
        {
            number.text = lineNumber.ToString();
            int totalColumn = payLine.Count;
            int totalRow = segments.Count / totalColumn;
            for (int column = 0; column < totalColumn; ++column)
            {
                int row = payLine[column];
                segments[column * totalRow + row].color = color;
            }
        }
    }
}
