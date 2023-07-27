using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Condition
{
    [Category("★ BagelCode/Utility")]
    public class CheckSlotSpin : ConditionTask<GraphOwner>
    {
        public BBParameter<bool> checkValue = true;

        private const string variableName = "isSpin";

        protected override string info
        {
            get { return "Is Spin == " + checkValue; }
        }

        protected override bool OnCheck()
        {
            var variableA = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), variableName);
            if (variableA == null)
            {
                return false;
            }
            else
            {
                return variableA.value == checkValue.value;
            }
        }
    }

}