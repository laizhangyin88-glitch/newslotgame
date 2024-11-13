using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class CheckContextFloatProperty : ConditionTask 
{
	public BBParameter<ContextElement> valueA;
    public CompareMethod checkType = CompareMethod.EqualTo;
    public BBParameter<float> valueB;

    [SliderField(0,0.1f)]
	public float differenceThreshold = 0.05f;

    protected override string info
    {
        get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
    }

    protected override bool OnCheck()
    {
        IContextFloatProperty property = valueA.value as IContextFloatProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + valueA.value.ContextName + " is not IContextFloatProperty");
            return false;
        }
        else 
        {
        	return OperationUtils.Compare(property.GetFloatProperty(), valueB.value, checkType, differenceThreshold);
        }
    }
}

}