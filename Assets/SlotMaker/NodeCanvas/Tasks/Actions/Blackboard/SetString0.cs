using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetString0 : ActionTask<Blackboard>
{
	public BBParameter<string> arg0;
	public BBParameter<string> saveAs;

	protected override string info
	{
		get { return string.Format("{0} = {1}", saveAs, arg0); }
	}

	protected override void OnExecute()
	{
		object a0 = BlackboardUtils.FindValue(agent, arg0.value);
    	if (a0 == null)
    	{
    		EndAction(false);
    		return;
    	}

    	saveAs.value = a0.ToString();
        EndAction();
	}
}

}
