using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Conditions
{

[Category("★ SlotMaker/Blackboard")]
public class CheckStringIsNullOrEmpty : ConditionTask<Blackboard>
{
    public BBParameter<string> valueA;

    protected override string info
    {
        get { return valueA + " Is Null Or Empty"; }
    }

    protected override bool OnCheck()
    {
        var variableA = BlackboardUtils.FindVariable<string>(agent, valueA.value);
        return string.IsNullOrEmpty(variableA.value);
    }
}

}
