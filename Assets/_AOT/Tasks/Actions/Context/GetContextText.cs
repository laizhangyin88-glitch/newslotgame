using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class GetContextText : ActionTask<ContextElement> 
{
    [BlackboardOnly]
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}.text", saveAs, agentInfo); }
    }

    protected override void OnExecute()
    {
        IContextText property = agent as IContextText;
        if (property == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextText");
            EndAction(false);
        }
        else
        {
            saveAs.value = property.GetText();
            EndAction();
        }
    }
}

}
