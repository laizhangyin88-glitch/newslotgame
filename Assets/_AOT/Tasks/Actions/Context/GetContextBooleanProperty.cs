using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class GetContextBooleanProperty : ActionTask<ContextElement> 
{
    [BlackboardOnly]
    public BBParameter<bool> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}.GetBool()", saveAs, agentInfo); }
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
            saveAs.value = property.GetBooleanProperty();
            EndAction();
        }
    }
}

}