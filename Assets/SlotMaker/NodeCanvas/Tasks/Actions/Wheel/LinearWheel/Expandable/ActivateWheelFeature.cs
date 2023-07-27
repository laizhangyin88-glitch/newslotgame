using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Wheel/LinearWheel/Expandable")]
    public class ActivateWheelFeature : ActionTask<Transform>
    {
        public ExpandableWheelFeatureType FeatureType;
        
        protected override string info
        {
            get 
            {
                return FeatureType.ToString();
            }
        }
        
        protected override void OnExecute()
        {
            var expandableWheel = agent.gameObject.GetComponent<ExpandableAnimationLinearWheel>();

            if (expandableWheel != null)
            {
                expandableWheel.ActivateFeature(FeatureType);
            }
        
            EndAction();
        }
    }
}