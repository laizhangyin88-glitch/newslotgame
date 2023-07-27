using UnityEngine;
using BagelCode;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using NodeCanvas.Framework.Internal;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("★ BagelCode")]
    public class CheckLongPlayerPrefsBB : ConditionTask<Blackboard>
    {
        public BBParameter<string> prefsKey;
        public BBParameter<long> defaultValue;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<long> valueLong;
        public BBParameter<string> valueBB;

        protected override string info
        {
            get { return prefsKey + OperationUtils.GetCompareString(checkType) + "\n" + valueLong + "+" + valueBB; }
        }

        protected override bool OnCheck()
        {
            long checkPrefsValue = PlayerPrefsUtils.GetInt64(prefsKey.value, defaultValue.value);
            var variableValue = BlackboardUtils.FindVariable<long>(agent, valueBB.value);

            if (variableValue == null)
                return OperationUtils.Compare(checkPrefsValue, valueLong.value, checkType);
            else
                return OperationUtils.Compare(checkPrefsValue, valueLong.value + variableValue.value, checkType);
        }
    }
}