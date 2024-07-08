using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System;

namespace SlotMaker.Tasks.Condition
{

    [Category("★ BagelCode/Utility")]
   // [EventReceiver("OnSymbolEvent")]
    public class CheckSymbolRowIndex : ConditionTask<Transform>
    {

        public CompareMethod checkType = CompareMethod.EqualTo;

        [RequiredField]
        public BBParameter<int> rowIndex;
        protected override string info
        {
            get { return "symbol row index" + OperationUtils.GetCompareString(checkType) + rowIndex; }
        }

        protected override bool OnCheck()
        {

            int row = agent.GetSiblingIndex();

            bool res = OperationUtils.Compare(row, rowIndex.value, checkType);

            return res;

        }
    }
}
