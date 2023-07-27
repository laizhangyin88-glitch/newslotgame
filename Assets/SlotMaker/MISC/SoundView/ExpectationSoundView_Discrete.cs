using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class ExpectationSoundView_Discrete : MonoBehaviour
    {
        // TODO: Test this thoroughly and if this works well, then replace original ExpectationSoundView to this.
        // Assumption: Expectation occurs in ascending order of reel indices
        public int slotIndex;
        public string expectationSound;
        public string expectationSnapshot = "Content_Expectation";
        public float transitionTimeToReach = 1f;
        private int consecutiveExpectationCount = -1;

        private bool needEndExpectation = false;

        public static readonly string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        public static readonly string ON_BEGIN_EXPECTATION = "BeginExpectation";
        public static readonly string ON_END_EXPECTATION = "EndExpectation";

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnSlotDetailEvent(EventData receivedEvent)
        {
            if (receivedEvent.name.Equals(ON_BEGIN_EXPECTATION, StringComparison.Ordinal))
            {
                OnBeginExpectation((int)receivedEvent.value);
            }
            else if (receivedEvent.name.Equals(ON_END_EXPECTATION, StringComparison.Ordinal))
            {
                OnEndExpectation();
            }
        }

        private void OnBeginExpectation(int reelIndex)
        {
            if (consecutiveExpectationCount < 0)
            {
                var expectation  = ContentCustomData.GetSlotData(slotIndex).expectation;
                consecutiveExpectationCount = GetConsecutiveExpectationCount(expectation, reelIndex);

                var snapshot = GSManager.Instance.GetAudioMixerSnapshot(expectationSnapshot);
                snapshot.TransitionTo(transitionTimeToReach);
            }
            GSManager.Instance.GetHandler(expectationSound).Play();
            needEndExpectation = true;
        }

        private void OnEndExpectation()
        {
            if (!needEndExpectation) return;

            --consecutiveExpectationCount;
            if (consecutiveExpectationCount == 0)
            {
                GSManager.Instance.GetHandler(expectationSound).Stop();
                consecutiveExpectationCount = -1;
            }
            needEndExpectation = false;
        }

        private int GetConsecutiveExpectationCount(Expectation expectation, int startReelIndex) {
            int consecutiveCount = 0;
            var expectationList = expectation.expectations;
            for (int i = startReelIndex; i < expectationList.Count; ++i) 
            {
                if (!expectationList[i]) break;

                consecutiveCount++;
            }

            return consecutiveCount;
        }
    }
}
