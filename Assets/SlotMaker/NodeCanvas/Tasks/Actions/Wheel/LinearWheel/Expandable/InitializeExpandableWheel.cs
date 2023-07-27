using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Wheel/LinearWheel/Expandable")]
    public class InitializeExpandableWheel : ActionTask<Transform>
    {
        private const string InfoString = "Set segment and jackpot values for Expandable Wheel";
        
        public BBParameter<List<int>> SegmentValuesList;
        public BBParameter<List<int>> JackpotIndexesList;
        
        public BBParameter<int> StopSegmentIndex;
        public BBParameter<int> JackpotOffsetPosition;
        
        protected override string info
        {
            get 
            {
                return InfoString;
            }
        }
        
        protected override void OnExecute()
        {
            var expandableWheel = agent.GetComponent<ExpandableAnimationLinearWheel>();

            if (expandableWheel != null)
            {
                expandableWheel.Initialize(new ExpandableWheelData
                {
                    SegmentValuesList = SegmentValuesList.value,
                    JackpotIndexesList = JackpotIndexesList.value,
                    StopSegmentIndex = StopSegmentIndex.value,
                    JackpotOffsetPosition = JackpotOffsetPosition.value
                });
            }
        
            EndAction();
        }
    }
}