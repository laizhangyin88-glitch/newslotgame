using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Line Win", menuName="SlotMaker2/Math/Output/Line Win")]
    public class SpinOutputLineWin : SpinOutputWinningSubset
    {
        public event Action<SpinOutputLineWin> onTotalLineWin;

        [TabGroup("LineWin", "Setup")]
        [InlineEditor]
        public Paytable paytable;

        [TabGroup("LineWin", "Setup")]
        [InlineEditor]
        public Paylines paylines;

        [TabGroup("LineWin", "Setup")]
        [InlineEditor]
        public SpinOutputLineWinStrategy winningStrategy;

        [TabGroup("LineWin", "Setup")]
        public OperationMethod symbolMultiplierOperationMethod = OperationMethod.Multiply;

        [TabGroup("LineWin", "Output")]
        [NonSerialized]
        [ShowInInspector]
        public long betPerLine;

        [TabGroup("LineWin", "Output")]
        [NonSerialized]
        [ShowInInspector]
        public List<SpinLineWin> totalWin = new List<SpinLineWin>();

        public override IEnumerable<SpinWin> GetTotalWin() { return totalWin; }
        public override SpinWin GetSingleWin(int winningIndex) { return totalWin[winningIndex]; }
        public override int winCount { get { return totalWin.Count; } }

        public virtual void TotalLineWin()
        {
            OnTotalLineWin();
            if (onTotalLineWin != null) onTotalLineWin(this);
        }

        protected virtual void OnTotalLineWin() {}

        public virtual void CalcLineWin(SpinOutputLineWin output, int lineIndex, WinningCombination.CombinationRule combinationRule, WinningCombination.CombinationMask combinationMask, bool includeFullLineWin)
        {
            winningStrategy.CalcLineWin(output, lineIndex, combinationRule, combinationMask, includeFullLineWin);
        }
    }
}