using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class GetMaxOrMinInt64 : ActionTask<Blackboard>
{
	public BBParameter<string> valueA;
    public BBParameter<string> valueB;
    public enum MathfOperation
    {
        Max = 0,
        Min = 1
    }
	public MathfOperation operation = MathfOperation.Max;
	public BBParameter<long> saveAs;

	protected override string info
	{
		get { return string.Format("{0} = Mathf.{1}({2}, {3})", saveAs, operation, valueA, valueB); }
	}

	protected override void OnExecute()
	{
		var variableA = BlackboardUtils.GetOrCreateVariable<long>(agent, valueA.value);
        var variableB = BlackboardUtils.GetOrCreateVariable<long>(agent, valueB.value);
		if (variableA == null)
		{
			Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
			EndAction(false);
		}
        else if (variableB == null)
		{
			Debug.LogError("[Blackboard] Null variableB founded in " + valueB.value + "in " + agent.name);
			EndAction(false);
		}
		else
		{
            if (operation == MathfOperation.Max)
            {
                saveAs.value = (long)Mathf.Max(variableA.value, variableB.value);
            }
            else
            {
                saveAs.value = (long)Mathf.Min(variableA.value, variableB.value);
            }
			
			EndAction();
		}
	}
}

}
