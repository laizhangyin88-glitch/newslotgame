using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextBooleanProperty : ActionTask<ContextElement> 
{
    public BoolSetModes setTo = BoolSetModes.True;

    protected override string info
    {
        get
        {
            if (setTo == BoolSetModes.Toggle)
                return "Toggle " + agent;
            else 
                return "Set " + agent + " to " + setTo;
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
            if (setTo == BoolSetModes.Toggle)
                property.SetBooleanProperty(!property.GetBooleanProperty());
            else 
                property.SetBooleanProperty((int)setTo == 1);

            EndAction();
        }
    }
}

}