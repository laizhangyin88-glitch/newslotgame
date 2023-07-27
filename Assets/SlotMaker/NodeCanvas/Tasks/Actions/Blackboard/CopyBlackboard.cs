using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class CopyBlackboard : ActionTask<Blackboard>
{
    public BBParameter<IBlackboard> source;
    public BBParameter<string>      targetName;
    public BBParameter<Blackboard> saveAs;

    protected override string info
    { get { return string.Format("copy {0} Blackboard to {1}", source, targetName); } }

    protected override void OnExecute()
    {
        var bb = BlackboardUtils.CreateBlackboard(targetName.value);
        ((Blackboard)bb).transform.parent = ((Blackboard)source.value).transform.parent; // make sibiling

        // string sourceInfo = ((Blackboard)source.value).Serialize();
        // ((Blackboard)bb).Deserialize(sourceInfo);
        BlackboardUtils.CopyBlackboard(source.value, bb);

        saveAs.value = (Blackboard)bb;
        
        EndAction();
    }
}

}
