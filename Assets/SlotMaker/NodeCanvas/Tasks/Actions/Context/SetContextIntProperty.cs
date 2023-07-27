using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextIntProperty : ActionTask<ContextElement>
{
    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<int> valueB;

    protected override string info
    {
        get { return string.Format("{0}.value{1}{2}", agentInfo, OperationUtils.GetOperationString(Operation), valueB); }
    }

    protected override void OnExecute()
    {
        IContextIntProperty property = agent as IContextIntProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextIntProperty");
            EndAction(false);
        }
        else 
        {
            property.SetIntProperty( OperationUtils.Operate(property.GetIntProperty(), valueB.value, Operation) );
            EndAction();
        }
    }
}

}
