using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Wheel/LinearWheel")]
public class GetLinearWheelSegment : ActionTask<Transform>
{
    public BBParameter<int> segmentIndex;
    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get 
        {
            return string.Format("{0} = Get Linear Wheel Segment at {1}", saveAs, segmentIndex);
        }
    }

    protected override void OnExecute()
    {
        var linearWheel = agent.GetComponent<AnimationLinearWheel>();
        saveAs.value = linearWheel.GetSegmentGameObject(segmentIndex.value);
        EndAction();
    }
}

}