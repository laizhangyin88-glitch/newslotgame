using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker.IoC;

namespace SlotMaker.Slots.Tasks.Actions.Signboard
{
    [Category("✶ Slots/Signboard")]
    public class ExtractSpinWin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public BBParameter<int> index;

        public OperationMethod earnCreditOperation = OperationMethod.Set;

        [BlackboardOnly]
        public BBParameter<Dictionary<string, long>> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Extract({1}[{2}])", saveAs, target, index); }
        }

        private static readonly string EARN_CREDIT = "earnCredit";
        private static readonly string MULTIPLIER = "multiplier";
        private static readonly string WINNING_SYMBOL = "winningSymbol";
        private static readonly string HIT_COUNT = "hitCount";
        private static readonly string LINE_INDEX = "lineIndex";
        private static readonly string WAY_COUNT = "wayCount";

        protected override void OnExecute()
        {
            SpinOutputWinningSubset win = target.value as SpinOutputWinningSubset;
            var spinWin = win.GetSingleWin(index.value);

            long earnCredit;
            saveAs.value.TryGetValue(EARN_CREDIT, out earnCredit);
            saveAs.value[EARN_CREDIT] = OperationUtils.Operate((long)earnCredit, spinWin.earnCredit, earnCreditOperation);

            saveAs.value[MULTIPLIER] = spinWin.multiplier;
            saveAs.value[WINNING_SYMBOL] = spinWin.winningCombination.symbol.value;
            saveAs.value[HIT_COUNT] = spinWin.hitCount;

            var lineWin = spinWin as SpinLineWin;
            if (lineWin != null)
                saveAs.value[LINE_INDEX] = lineWin.lineIndex + 1;
            
            var wayWin = spinWin as SpinWayWin;
            if (wayWin != null)
                saveAs.value[WAY_COUNT] = wayWin.wayCount;

            EndAction();
        }
    }
}