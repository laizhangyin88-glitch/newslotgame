using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Wheel/LinearWheel/Expandable")]
    public class StopSpinExpandableWheel : ActionTask<Transform>
    {
        protected override string info
        {
            get 
            {
                return "Stop Spinning Wheel";
            }
        }
        
        protected override void OnExecute()
        {
            var expandableWheel = agent.GetComponent<ExpandableAnimationLinearWheel>();

            if (expandableWheel != null)
            {
                expandableWheel.StopSpin();
            }
        
            EndAction();
        }
    }
}