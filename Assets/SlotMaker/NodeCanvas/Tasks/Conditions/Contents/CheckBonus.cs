using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using Debug = UnityEngine.Debug;

namespace BagelCode.Tasks.Conditions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class CheckBonus : ConditionTask
    {
        public BBParameter<int> bonusId;

        protected override string info
        {
            get { return string.Format("{0} {1} bonusId", bonusId, OperationTools.GetCompareString(CompareMethod.EqualTo)); }
        }

        protected override bool OnCheck()
        {
            IBlackboard bb = ContentBlackboard.Get();
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var response = ContentBlackboardUtils.GetBonusResponse(spin, bonusId.value);

            Debug.Log($"【bounsId】 {response}");

            return response != null;
        }
    }
}
