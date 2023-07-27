using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetString2 : ActionTask<Blackboard>
{
	public BBParameter<string> key;
    public StringTable.StringTableType tableType;
	public BBParameter<string> arg1;
    public BBParameter<string> arg2;
	public BBParameter<string> saveAs;

	protected override string info
    {
        get { return string.Format("{0} = ({1}){2}({3}, {4})", saveAs, tableType, key, arg1, arg2); }
    }

	protected override void OnExecute()
	{
		object a1 = BlackboardUtils.FindValue(agent, arg1.value);
        object a2 = BlackboardUtils.FindValue(agent, arg2.value);
        if (a1 == null || a2 == null)
        {
            EndAction(false);
            return;
        }

    	bool error = true;
    	saveAs.value = StringTableUtils.GetString(tableType, key.value, a1, a2, out error);
        EndAction(!error);
	}
}

}