using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextFloatProperty : ActionTask<ContextElement> 
{
    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<float> valueB;

    protected override string info
    {
        get { return agentInfo + OperationUtils.GetOperationString(Operation) + valueB; }
    }

    protected override void OnExecute()
    {
        IContextFloatProperty property = agent as IContextFloatProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextFloatProperty");
            EndAction(false);
        }
        else 
        {
            property.SetFloatProperty( OperationUtils.Operate(property.GetFloatProperty(), valueB.value, Operation) );
            EndAction();
        }
    }
}

}