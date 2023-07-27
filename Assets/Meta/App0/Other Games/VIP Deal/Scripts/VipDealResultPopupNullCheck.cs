using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Vip Deal")]
    public class VipDealResultPopupNullCheck : ConditionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        protected override string info
        {
            get { return valueA + " == NULL + fake Null Check"; }
        }

        protected override bool OnCheck()
        {
            var variableA = BlackboardUtils.FindVariable(agent, valueA.value);
            return (variableA == null) || (variableA.value == null) || string.Equals("null", variableA.value.ToString());
        }
    }
}