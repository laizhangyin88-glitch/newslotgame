using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetString1 : ActionTask<Blackboard>
{
	public BBParameter<string> key;
    public StringTable.StringTableType tableType;
	public BBParameter<string> arg1;
	public BBParameter<string> saveAs;

	protected override string info
    {
        get { return string.Format("{0} = ({1}){2}({3})", saveAs, tableType, key, arg1); }
    }

	protected override void OnExecute()
	{
		object a1 = BlackboardUtils.FindValue(agent, arg1.value);
    	if (a1 == null)
    	{
    		EndAction(false);
    		return;
    	}

    	bool error = true;
    	saveAs.value = StringTableUtils.GetString(tableType, key.value, a1, out error);
        EndAction(!error);
	}
}

}