using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class AutoUpdateCheckNull : ConditionTask<Blackboard>
{
    public BBParameter<string> valueA;

    private Variable variableA;

    protected override string info
    {
        get { return valueA + " == NULL"; }
    }

    override protected void OnEnable()
	{
		variableA = BlackboardUtils.FindVariable(agent, valueA.value);
		variableA.onValueChanged += OnValueChanged;
	}

	override protected void OnDisable()
	{
		variableA.onValueChanged -= OnValueChanged;
	}

    protected override bool OnCheck()
    {
        return variableA.value == null;
    }

    private void OnValueChanged(string name, object value)
	{
		ownerAgent.GetComponent<ManualGraphOwner>().OnDispatch();
	}
}

}
