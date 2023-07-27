using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class _SetString : ActionTask<Blackboard>
{
    public BBParameter<string> key;
    public StringTable.StringTableType tableType;
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = {1}", saveAs, key); }
    }

    protected override void OnExecute()
    {
        bool error = true;
        saveAs.value = StringTableUtils.GetString(tableType, key.value, out error);
        EndAction();
    }
}

}
