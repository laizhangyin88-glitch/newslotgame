using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New Scatter Win", menuName = "SlotMaker2/Math/Output/Scatter Win")]
    public class SpinOutputScatterWin : SpinOutputWinningSubset
    {
        [TabGroup("ScatterWin", "Setup")]
        [InlineEditor]
        public WinningCombination paytable;

        [TabGroup("ScatterWin", "Setup")]
        [InlineEditor]
        public SpinOutputScatterWinStrategy winningStrategy;

        [TabGroup("ScatterWin", "Setup")]
        public OperationMethod symbolMultiplierOperationMethod = OperationMethod.Multiply;

        [TabGroup("ScatterWin", "Output")]
        [NonSerialized]
        [ShowInInspector]
        public SpinWin win;

        public override IEnumerable<SpinWin> GetTotalWin() { return null; }
        public override SpinWin GetSingleWin(int winningIndex) { return win; }
        public override int winCount { get { return 1; } }

        public void CalcScatterWin()
        {
            winningStrategy.CalcScatterWin(this);
        }
    }
}