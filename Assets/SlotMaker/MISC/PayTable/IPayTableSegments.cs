using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public interface IPayTableLineSegments
    {
        void SetPayLine(int lineNumber, List<int> payLine, in Color color);
    }
}
