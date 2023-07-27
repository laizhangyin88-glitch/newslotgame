using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class TweenContextFloatProperty : ActionTask<ContextElement> 
{
    public BBParameter<float> speed;
    public BBParameter<float> valueB;

    protected IContextFloatProperty property = null;

    protected override string info
    {
        get { return string.Format("{0}.Tween({1})", agentInfo, valueB); }
    }

    protected override void OnExecute()
    {
        property = agent as IContextFloatProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextFloatProperty");
            EndAction(false);
        }
    }

    protected override void OnUpdate()
    {
        float fromValue = property.GetFloatProperty();
        float toValue = valueB.value;
        if (fromValue == toValue)
        {
            EndAction();
            return;
        }

        float displacement = speed.value * elapsedTime;
        float direction = (toValue > fromValue) ? 1f : -1f;
        float targetValue = fromValue + displacement * direction;
        targetValue = (direction > 0f) ? Mathf.Min(targetValue, toValue) : Mathf.Max(targetValue, toValue);
        property.SetFloatProperty(targetValue);
    }
}

}