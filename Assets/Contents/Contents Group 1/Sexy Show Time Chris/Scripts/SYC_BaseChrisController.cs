using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using ParadoxNotion;
using TMPro;

namespace GS.Slot.SYC {
    public class SYC_BaseChrisController : MonoBehaviour
    {
        [SerializeField]
        private Animator chrisAnimator;
        public float randomAnimationProbablity = 0.5f;
        public float idleStateChangeProbability = 0.5f;

        private void OnEnable()
        {
            MessageDispatcher.Register("OnContentUIEvent", FSTriggerAnimation);
            MessageDispatcher.Register("OnSlotEvent", RandomAnimation);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnSlotEvent", RandomAnimation);
            MessageDispatcher.UnRegister("OnContentUIEvent", FSTriggerAnimation);
        }

        private void RandomAnimation(EventData eventData)
        {
            if (eventData.name == "SpinSlotMachine" && eventData.id == 0)
            {
                var stateChangeProb = Random.Range(0, 1f);
                if (stateChangeProb <= randomAnimationProbablity) chrisAnimator.SetTrigger("Idle Swap");
                var randomAnimProb = Random.Range(0, 1f);
                if (randomAnimProb <= randomAnimationProbablity) chrisAnimator.SetTrigger("Random");
            }
        }

        private void FSTriggerAnimation(EventData eventData)
        {
            if (eventData.name == "ChrisFSTrigger")
            {
                chrisAnimator.SetTrigger("Scatter Win");
            }
        }
    }
}
