using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class MergeClipRange : ActionTask<BaseSlotMachine>
{
    public BBParameter<int> begin;
    public BBParameter<int> end;

    protected override string info { get { return string.Format("Merge Clip Range {0} to {1}", begin, end); } }

    protected override void OnExecute()
    {
        ReelClipRangeUtils.MergeClipRange(agent, begin.value, end.value);
        EndAction();
    }
}

}
