using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{

    [Name("Set Long Integer")]
    [Category("✫ Blackboard")]
    public class SetLong : ActionTask{

        [BlackboardOnly]
        public BBParameter<long> valueA;
        public OperationMethod Operation = OperationMethod.Set;
        public BBParameter<long> valueB;

        protected override string info{
            get {return valueA + OperationUtils.GetOperationString(Operation) + valueB;}
        }

        protected override void OnExecute(){
            valueA.value = OperationUtils.Operate(valueA.value, valueB.value, Operation);
            EndAction();
        }
    }
}
