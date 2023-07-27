using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher
{
    public class StopParticleScratch : StateMachineBehaviour
    {
        public int stopFrame;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetInteger("StopFrame") == stopFrame)
            {
                var root = animator.GetComponent<ContextElement>();
                var particleElement = ContextUtils.FindElement(root, "Particle/Particle Scratch", ContextSearchingType.FullNameSearch);
                if(particleElement != null)
                {
                    var emission = particleElement.GetComponent<ParticleSystem>().emission;
                    emission.rateOverTime = 0.0f;
                }
            }
        }
    }

}
