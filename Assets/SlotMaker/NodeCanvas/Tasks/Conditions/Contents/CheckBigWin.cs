using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions.Contents
{

[Category("★ BagelCode/Contents")]
public class CheckBigWin : ConditionTask<Blackboard>
{
    public BBParameter<long> betCredit;
    public BBParameter<int>  multiple;
    public CompareMethod compareOperator  = CompareMethod.LessOrEqualTo;
    public BBParameter<string> earnCredit = "./spin/earnCredit";

    protected override string info
    {
        get
        {
            return string.Format("{0}x{1} {2} {3}",multiple, betCredit, OperationTools.GetCompareString(compareOperator) ,earnCredit);
        }
    }

    protected override bool OnCheck()
    {
        var credit = BlackboardUtils.FindVariable<long>(agent, earnCredit.value);
        return OperationUtils.Compare(betCredit.value * (long)(multiple.value), credit.value, compareOperator);
    }
}

}
