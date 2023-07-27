using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Wheel/LinearWheel/Expandable")]
    public class GetExpandableWheelSegment : ActionTask<Transform>
    {
        public BBParameter<float> WonPosition;
        public BBParameter<GameObject> saveAs;

        protected override string info
        {
            get 
            {
                return string.Format("{0} = Get Expandable Wheel Segment at {1}", saveAs, WonPosition);
            }
        }

        protected override void OnExecute()
        {
            var expandableWheel = agent.GetComponent<ExpandableAnimationLinearWheel>();

            if (expandableWheel != null)
            {
                saveAs.value = expandableWheel.GetSegmentGameObject(WonPosition.value);
            }
            
            EndAction();
        }
    }
}