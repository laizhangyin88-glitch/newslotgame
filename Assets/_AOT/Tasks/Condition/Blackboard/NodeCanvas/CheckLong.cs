using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

using SlotMaker;

namespace NodeCanvas.Tasks.Conditions{

    [Category("✫ Blackboard")]
    public class CheckLong : ConditionTask{

        [BlackboardOnly]
        public BBParameter<long> valueA;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<long> valueB;

        protected override string info{
            get {return valueA + OperationTools.GetCompareString(checkType) + valueB;}
        }

        protected override bool OnCheck(){
            return OperationUtils.Compare(valueA.value, valueB.value, checkType);
        }
    }
}
