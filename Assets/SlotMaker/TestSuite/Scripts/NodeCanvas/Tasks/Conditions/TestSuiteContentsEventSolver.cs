using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.TestSuite;

namespace SlotMaker.Task.Conditions.TestSuite
{
    [Category("★ SlotMaker/TestSuite")]
    public class TestSuiteContentsEventSolver : ConditionTask
    {
        public int weight = 1;
        protected override string info { get { return string.Format("TEST SUITE Contents Weight {0}", weight); } }

#if DEV
        protected override void OnEnable()
        {
            TestSuiteEventSolver.Register("Contents", Solve, weight);
        }

        protected override void OnDisable()
        {
            TestSuiteEventSolver.UnRegister("Contents", Solve);
        }

        private void Solve()
        {
            YieldReturn(true);
        }
#endif

        protected override bool OnCheck(){ return false; }
    }
}
