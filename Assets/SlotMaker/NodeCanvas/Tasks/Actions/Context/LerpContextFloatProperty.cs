using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class LerpContextFloatProperty : ActionTask<ContextElement> 
{
    public BBParameter<float> elapsedSpeed;
    public BBParameter<float> valueA;
    public BBParameter<float> valueB;

    protected IContextFloatProperty property = null;

    private float fromValue = 0f;
    private float targetValue = 1f;

    protected override string info
    {
        get { return string.Format("{0}.Lerp({1},{2},{3})", agentInfo, valueA, valueB, elapsedSpeed); }
    }

    protected override void OnExecute()
    {
        property = agent as IContextFloatProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextFloatProperty");
            EndAction(false);
        }

        if(valueA == null)
            fromValue = property.GetFloatProperty();
        else
            fromValue = valueA.value;
            
        targetValue = valueB.value;
    }

    protected override void OnUpdate()
    {
        if (property.GetFloatProperty() == targetValue)
        {
            property.SetFloatProperty(targetValue);
            EndAction();
            return;
        }
        else
        {
            property.SetFloatProperty(Mathf.Lerp(fromValue, targetValue, elapsedTime/elapsedSpeed.value));
        }
    }
}

}
