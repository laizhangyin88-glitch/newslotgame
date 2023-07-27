using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Transform")]
public class SetFromToScaleController : ActionTask<Transform>
{
    public BBParameter<Vector3> from;
    public BBParameter<Vector3> to;
    public BBParameter<float>     endPoint;

    protected override string info
    {
        get
        {
            return "Set DirectionalWeightScaleController";
        }
    }

    protected override void OnExecute()
    {
        DirectionalWeightScaleController controller = agent.GetComponent<DirectionalWeightScaleController>();

        if (controller != null)
        {
            controller.from = from.value;
            controller.to   = to.value;
        }

        if (endPoint.value == 0)
        {
            EndAction();
        }
    }

    protected override void OnUpdate()
    {
        if (Vector3.Distance(agent.localScale, to.value) <= endPoint.value)
        {
            EndAction();
        }
    }
}

}
