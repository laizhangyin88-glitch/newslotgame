using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class GetContextFloatProperty : ActionTask<ContextElement> 
{
    [BlackboardOnly]
    public BBParameter<float> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}.GetFloat()", saveAs, agentInfo); }
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
            saveAs.value = property.GetFloatProperty();
            EndAction();
        }
    }
}

}