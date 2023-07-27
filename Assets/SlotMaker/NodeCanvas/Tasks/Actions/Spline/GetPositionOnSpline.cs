using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{
    [Category("★ SlotMaker/Transform")]
	public class GetPositionOnSpline : ActionTask<Transform> 
	{
		public BBParameter<float> time;

		public BBParameter<Vector3> saveAs;

        protected override string info {
            get { return  string.Format("GetPoint({0}) on spline", time); }
        }

        protected override void OnExecute()
        {
        	saveAs.value = agent.GetComponent<BezierSpline>().GetPoint(time.value);

        	EndAction();
        }

	}
}