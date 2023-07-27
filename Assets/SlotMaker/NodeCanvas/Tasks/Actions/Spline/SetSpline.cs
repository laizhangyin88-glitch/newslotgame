using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
    [Category("★ SlotMaker/Transform")]
    public class SetSpline : ActionTask<Transform>
    {
        public BBParameter<GameObject> bezierSplineObject;

        protected override string info {
            get { return  string.Format("Set Spline to SplineWalker"); }
        }

        protected override void OnExecute()
        {
            var walker = agent.GetComponent<SplineWalker>();
            var spline = bezierSplineObject.value.GetComponent<BezierSpline>();
            walker.spline = spline;
            EndAction();
        }
    }
}