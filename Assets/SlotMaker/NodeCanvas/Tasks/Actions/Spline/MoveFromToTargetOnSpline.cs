using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{
    [Category("★ SlotMaker/Transform")]
    public class MoveFromToTargetOnSpline : ActionTask<Transform>
    {
    	public BBParameter<float> from;
        public BBParameter<float> to;
        public BBParameter<float> speed;

        public AnimationCurve curve;

        private float dist;
        private SplineWalker walker;

        protected override string info {
            get { return  string.Format("{0} To {1}", from, to); }
        }

        protected override void OnExecute()
        {
        	walker = agent.GetComponent<SplineWalker>();
        	dist = to.value - from.value;
        	walker.position = from.value;
        }

        protected override void OnUpdate()
        {
        	float nomalizedDist = 1.0f - (to.value - walker.position) / dist;
        	float curveValue = curve.Evaluate(nomalizedDist);
        	float velocity = Time.deltaTime * speed.value * curveValue;

        	walker.position += velocity;

        	if (nomalizedDist >= 1.0f)
        	{
                walker.position = to.value - (int)to.value;
        		EndAction();
        	}
        	else
        	{
        		
        	}
        }
    }
}