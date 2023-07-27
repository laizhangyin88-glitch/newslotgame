using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/SlotMachine/ReelMovement")]
    public class CurveVelocity: ActionTask
    {
        public BBParameter<Vector3> velocity;
        public BBParameter<float> duration;
        public BBParameter<AnimationCurve> curve;

        private Vector3 startAcceleration;
        protected override void OnExecute()
        {
            startAcceleration = velocity.value;
        }

        protected override void OnUpdate()
        {
            float normalizedTime = elapsedTime / duration.value;
            velocity.value = Vector3.LerpUnclamped(startAcceleration, Vector3.zero, curve.value.Evaluate(normalizedTime));

            if (normalizedTime >= 1f)
                EndAction();
        }
    }
}
