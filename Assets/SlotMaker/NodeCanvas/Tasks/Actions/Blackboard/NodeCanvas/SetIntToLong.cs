using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{

[Category("✫ Blackboard")]
public class SetIntToLong : ActionTask 
{
    public BBParameter<long> valueA;
    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<int> valueB;

    protected override string info {
        get {return valueA + OperationUtils.GetOperationString(Operation) + valueB;}
    }

    protected override void OnExecute()
    {
         valueA.value = OperationUtils.Operate(valueA.value, System.Convert.ToInt64(valueB.value), Operation);
         EndAction();
    }
}

}
