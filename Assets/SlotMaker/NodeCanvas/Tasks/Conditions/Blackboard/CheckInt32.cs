using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckInt32 : ConditionTask<Blackboard> 
{
	public BBParameter<string> valueA;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<int> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{

        var variableA = BlackboardUtils.FindVariable<int>(agent, valueA.value);

#if NEW_NET000
        Debug.Log("@ 问题待解决");
        if (valueA.value  == "/me/loginCount")
        {
            if (variableA == null)
            {
                Debug.LogError($"valueA.name = {valueA.name} ; valueA.value =  {valueA.value}");
                Debug.LogError($"valueB.name = {valueB.name} ; valueB.value =  {valueB.value}");
                return true;
            }
        }
#endif

		return OperationUtils.Compare(variableA.value, valueB.value, checkType);
	}
}

}
