using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Category("★ SlotMaker/Transform")]
    public class MoveFromToTarget : ActionTask<Transform>
    {
        public enum SpeedType
        {
            Lerp,
            Linear
        };
        
        public BBParameter<Transform> from;
        public BBParameter<Transform> to;
        public BBParameter<float> speed;

        public float offset;
        public SpeedType speedType;
        
        private Vector3 objFrom;
        private Vector3 objTo;
        private Vector3 velocity;
        
        protected override string info {
            get { return "GoTo " + to; }
        }

        protected override void OnExecute()
        {
            agent.position = from.value.position;
            objFrom = from.value.position;
            objTo = to.value.position;
            velocity = objTo - objFrom;
            velocity = velocity.normalized * speed.value;
        }

        protected override void OnUpdate()
        {
            // Linear 타입의 경우 speed가 크면 position 변화량이 커지므로 offset을 적절히 설정해주어야 합니다. 
            if (Vector3.Distance(agent.position, objTo) <= offset)
            {
                EndAction();
            }
            else
            {
                if ((int)speedType == 0)
                {
                    agent.position = Vector3.Lerp(agent.position, objTo, Time.deltaTime * speed.value);
                }
                else if ((int)speedType == 1)
                {
                    agent.position = agent.position + Time.deltaTime * velocity;
                }
            }
        }
    }

}
