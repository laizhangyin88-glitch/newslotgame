using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextBooleanProperty1 : ActionTask<ContextElement>
{
    public BBParameter<bool> arg;

    protected override string info
    {
        get
        {
            return string.Format("Set {0} to {1}", agent, arg);
        }
    }

    protected override void OnExecute()
    {
		IContextBooleanProperty property = agent as IContextBooleanProperty;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextBooleanProperty");
            EndAction(false);
        }
        else
        {
            property.SetBooleanProperty(arg.value);

            EndAction();
        }
    }
}

}
