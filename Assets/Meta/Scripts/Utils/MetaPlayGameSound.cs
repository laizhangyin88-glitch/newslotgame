using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.AnimatorBehaviour
{
    public class MetaPlayGameSound : StateMachineBehaviour
    {
        public string id;
        public string checkVariableKey;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool play = false;
            var bb = animator.GetComponent<Blackboard>();
            if(bb != null)
            {
                var variable = BlackboardUtils.FindVariable<bool>(bb, checkVariableKey);
                if(variable != null && variable.value)
                {
                    GSManager.Instance.GetHandler(id).Play();
                    play = true;
                }
            }

            if (!play)
            {
                GSManager.Instance.GetHandler(id).Play();
            }
        }
    }
}
