
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Transform")]
public class UpdateFromToScale : ActionTask<Transform>
{
    public BBParameter<Vector3> from;
    public BBParameter<Vector3> to;

    public BBParameter<float> weight = 0.0f;

    protected override string info
    {
        get
        {
            return string.Format("UpdateFromToScale {0} to {1}", from,to);
        }
    }

    protected override void OnUpdate()
    {
        Vector3 direction = to.value - from.value;
        agent.localScale  = from.value + direction * weight.value;
        EndAction();
    }
}

}
