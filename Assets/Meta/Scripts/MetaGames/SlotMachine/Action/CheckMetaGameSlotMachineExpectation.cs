using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Task.Condition
{
    [Category("★ BagelCode/Events")]
    public class CheckMetaGameSlotMachineExpectation : ConditionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> reelIndex;

        protected override string info { get { return string.Format("Check MetaGame SlotMachine Expectation({0})", reelIndex); } }

        protected override bool OnCheck()
        {
            return MetaSlotMachineContentCustomData.GetSlotData(slotIndex.value).expectation.expectations[reelIndex.value];
        }
    }
}