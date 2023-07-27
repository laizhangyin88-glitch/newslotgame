using BagelCode;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("★ BagelCode")]
    public class CheckPopupClubResultCooltime : ConditionTask<Blackboard>
    {
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<long> cooltimeLong;
        public BBParameter<string> baseCooltimeBB;

        protected override string info
        {
            get
            {
                return cooltimeLong + "+" + baseCooltimeBB + "\n" + OperationUtils.GetCompareString(checkType) + "Current Time";
            }
        }

        protected override bool OnCheck()
        {
            var variableValue = BlackboardUtils.FindVariable<long>(agent, baseCooltimeBB.value);
            if (variableValue == null)
                return OperationUtils.Compare(cooltimeLong.value, TimeUtils.GetTimeStamp(), checkType);
            else
                return OperationUtils.Compare(cooltimeLong.value + variableValue.value, TimeUtils.GetTimeStamp(), checkType); ;
        }
    }
}