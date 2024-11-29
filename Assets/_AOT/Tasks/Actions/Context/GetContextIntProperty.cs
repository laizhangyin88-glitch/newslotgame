using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class GetContextIntProperty : ActionTask<ContextElement>
{
    [BlackboardOnly]
    public BBParameter<int> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}.GetInt()", saveAs, agentInfo); }
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
            saveAs.value = property.GetIntProperty();
            EndAction();
        }
    }
}

}