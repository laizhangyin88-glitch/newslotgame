using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SimpleJSON;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckJSONNodeInt32 : ConditionTask<Blackboard> 
{
	public BBParameter<string> nodePath;
    public BBParameter<string> keyPath; //"xxx/xxx/xxx"
    public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<int> valueB;

	protected override string info
	{
		//get { return nodePath + OperationUtils.GetCompareString(checkType) + valueB; }
        get { return $"({nodePath} as JSON)[{keyPath}]" + OperationUtils.GetCompareString(checkType) + valueB; }
    }

	protected override bool OnCheck() 
	{
		var variableA = BlackboardUtils.FindVariable<string>(agent, nodePath.value);
        if (variableA == null || variableA.value == null)
            return false;

        JSONNode node = JSONNode.Parse(variableA.value);

        string[] itemsStrs = keyPath.value.Split('/') ?? new string[] { };

        if (itemsStrs.Length == 0)
            return false;

        JSONNode target = node;
        foreach (string itemStr in itemsStrs)
        {
            if (target.HasKey(itemStr))
            {
                target = target[itemStr];
            }
            else
            {
                return false;
            }
        }

        var res = OperationUtils.Compare((int)target, valueB.value, checkType);

        return res;

    }
}

}
