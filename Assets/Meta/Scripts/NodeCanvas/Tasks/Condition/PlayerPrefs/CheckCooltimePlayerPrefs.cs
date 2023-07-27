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
    public class CheckCooltimePlayerPrefs : ConditionTask<Blackboard>
    {
        public BBParameter<string> prefsKey;
        public BBParameter<long> defaultValue;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<long> cooltimeLong;
        public BBParameter<string> cooltimeBB;

        protected override string info
        {
            get { return prefsKey + "+" + cooltimeLong + "\n+" + cooltimeBB + OperationUtils.GetCompareString(checkType) + "Current Time"; }
        }

        protected override bool OnCheck()
        {
            string cooltimePrefs = PlayerPrefs.GetString(prefsKey.value, defaultValue.value.ToString());
            long nowCooltime = System.Convert.ToInt64(cooltimePrefs);
            var variableValue = BlackboardUtils.FindVariable<long>(agent, cooltimeBB.value);

            if (variableValue == null)
                return OperationUtils.Compare(nowCooltime + cooltimeLong.value, TimeUtils.GetCurrentTime(), checkType);
            else
                return OperationUtils.Compare(nowCooltime + cooltimeLong.value + variableValue.value, TimeUtils.GetCurrentTime(), checkType);
        }
    }
}