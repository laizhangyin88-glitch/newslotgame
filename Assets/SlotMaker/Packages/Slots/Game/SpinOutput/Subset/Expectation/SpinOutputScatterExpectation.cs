using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Scatter Expectation", menuName="SlotMaker2/Math/Output/Scatter Expectation")]
    public class SpinOutputScatterExpectation : SpinOutputExpectationSubset
    {
        [TabGroup("ScatterExpectation", "Setup")]
        [InlineEditor]
        public SymbolEntity scatter;

        [TabGroup("ScatterExpectation", "Setup")]
        public int winningCount;

        [TabGroup("ScatterExpectation", "Setup")]
        public int minimumCount;

        [TabGroup("ScatterExpectation", "Setup")]
        public bool continuous;

        [TabGroup("ScatterExpectation", "Setup")]
        public List<bool> possibleReels = new List<bool>();

        [TabGroup("ScatterExpectation", "Output")]
        [ShowInInspector]
        [NonSerialized]
        public List<int> possibleMaximumScatterCountsFromReel = new List<int>();

        [TabGroup("ScatterExpectation", "Output")]
        [ShowInInspector]
        [NonSerialized]
        public List<int> foundScatterCounts = new List<int>();

        [TabGroup("ScatterExpectation", "Output")]
        [ShowInInspector]
        [NonSerialized]
        public List<List<Cell3>> foundSpots = new List<List<Cell3>>();

        [TabGroup("ScatterExpectation", "Output")]
        [ShowInInspector]
        [NonSerialized]
        public List<List<Cell3>> expectedSpots = new List<List<Cell3>>();

        [TabGroup("ScatterExpectation", "Output")]
        [ShowInInspector]
        [NonSerialized]
        public List<int> expectedValues = new List<int>();

        [TabGroup("ScatterExpectation", "Output")]
        [ShowInInspector]
        [NonSerialized]
        public List<bool> expectedReels = new List<bool>();

        [TabGroup("ScatterExpectation", "Debug")]
        public bool debug;

        [TabGroup("ScatterExpectation", "Debug")]
        public List<bool> debugExpectedReels = new List<bool>();

        public override List<bool> GetExpectedReels() { return !debug ? expectedReels : debugExpectedReels; }
        public override List<List<Cell3>> GetExpectedSpots() { return expectedSpots; }
        public override List<int> GetExpectedValues() { return expectedValues; }
    }
}