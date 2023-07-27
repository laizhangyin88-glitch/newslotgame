using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace GameStudio.Slot.HOC
{
    public class HOCWheelController : FeatureController
    {
        [SerializeField] private BigWheel wheel;
        [SerializeField] private Animator animator;
        [SerializeField] private int wheelSegmentCount;
        [SerializeField] private float intialSpinTorque;
        [SerializeField] private int additionalSpinCount;
        [SerializeField] private float spinWinAnimWaitTime = 1.5f;

        private bool isWheelStopped;

        public const int WHEEL_BONUS_ID = 21100;

        protected override string ON_FEATURE_BEGIN_EVENT => "SpinWheel";

        protected override string ON_FEATURE_END_EVENT => "SpinWheelDone";

        protected override IEnumerator OnPlayCoroutine()
        {

            var bonusResponse = ContentBlackboardUtils.GetBonusResponse( BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value, WHEEL_BONUS_ID);
            int spotIndex = bonusResponse.GetValue<int>("wheelSpotIndex");
            bool isJackpot = bonusResponse.GetValue<int>("jackpotIndex") >= 0;
            animator.SetBool("IsJackpot",isJackpot);

            float spinAngle = (360f / (float)wheelSegmentCount) * (float)spotIndex;
            isWheelStopped = false;
            wheel.Simulation(intialSpinTorque, spinAngle, additionalSpinCount);
            animator.SetTrigger("Spin");
            GSManager.Instance.GetHandler("Wheel Start").Play();

            yield return new WaitUntil(()=> isWheelStopped);
            animator.SetTrigger("Spin End");

            var spinWaitDelay = new WaitForSeconds(spinWinAnimWaitTime);
            yield return spinWaitDelay;
            if (isJackpot) yield return spinWaitDelay;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            wheel.onStoppedBigWheel.AddListener(OnWheelStop);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            wheel.onStoppedBigWheel.RemoveListener(OnWheelStop);
        }

        private void OnWheelStop()
        {
            isWheelStopped = true;
        }
    }
}
