using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class RemoveAtContextElement : ActionTask<ContextElement>
{
    public BBParameter<int> index;
    public BBParameter<bool> fromLast;

    protected override string info
    {
        get 
        { 
        	if (!fromLast.value)
        		return string.Format("{0}.RemoveAt({1})", agentInfo, index); 
        	else 
        		return string.Format("{0}.RemoveAt(last - {1})", agentInfo, index); 
        }
    }

    protected override void OnExecute()
    {
    	ContextElement element = agent.GetChildElement(GetIndex(agent.ChildCount));
		agent.RemoveContextElement(element);
		EndAction();
    }

    private int GetIndex(int childCount)
    {
        return !fromLast.value ? index.value : (childCount - 1 - index.value);
    }
}

}