using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New Paylines", menuName = "SlotMaker2/Math/Paylines")]
    public class Paylines : VariableJson<List<List<int>>>
    {
        [InlineEditor]
        public VariableInt min;
        [InlineEditor]
        public VariableInt max;

        [ShowInInspector]
        public int begin { get { return Mathf.Max(0, min.value); } }
        [ShowInInspector]
        public int end { get { return Mathf.Min(value.Count, max.value); } }
    }
}