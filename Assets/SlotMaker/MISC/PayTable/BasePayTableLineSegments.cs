using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotMaker
{
    public abstract class BasePayTableLineSegments : MonoBehaviour, IPayTableLineSegments
    {
        public TextMeshProUGUI number;
        public List<Image> segments;

        public abstract void SetPayLine(int lineNumber, List<int> payLine, in Color color);
    }
}
