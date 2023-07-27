using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{

[Category("✫ Blackboard")]
public class SetFloatToDouble : ActionTask 
{
    public BBParameter<double> valueA;
    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<float> valueB;

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
