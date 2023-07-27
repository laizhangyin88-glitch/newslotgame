using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Transform")]
public class SetFromToPositionController : ActionTask<Transform>
{
    public BBParameter<GameObject> from;
    public BBParameter<GameObject> to;
    public BBParameter<float>     endPoint;

    protected override string info
    {
        get
        {
            return "Set DirectionalWeightPositionController";
        }
    }

    protected override void OnExecute()
    {
        DirectionalWeightPositionController controller = agent.GetComponent<DirectionalWeightPositionController>();

        if (controller != null)
        {
            controller.from = from.value.transform;
            controller.to   = to.value.transform;
        }

        if (endPoint.value == 0)
        {
            EndAction();
        }
    }

    protected override void OnUpdate()
    {
        if (Vector3.Distance(agent.position, to.value.transform.position) <= endPoint.value)
        {
            EndAction();
        }
    }
}

}
