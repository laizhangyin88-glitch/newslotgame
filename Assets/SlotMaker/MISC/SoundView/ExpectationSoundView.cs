using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class ExpectationSoundView : MonoBehaviour
    {
        public int slotIndex;
        public string expectationSound;
        public string expectationSnapshot = "Content_Expectation";
        public float transitionTimeToReach = 1f;
        private int expectationCount = -1;

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
            if (receivedEvent.id != slotIndex) return;
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
            if (expectationCount < 0)
            {
                var expectation  = ContentCustomData.GetSlotData(slotIndex).expectation;
                expectationCount = expectation.expectationCount;

                var snapshot = GSManager.Instance.GetAudioMixerSnapshot(expectationSnapshot);
                snapshot.TransitionTo(transitionTimeToReach);
            }
            GSManager.Instance.GetHandler(expectationSound).Play();
        }

        private void OnEndExpectation()
        {
            --expectationCount;

            if (expectationCount == 0)
            {
                GSManager.Instance.GetHandler(expectationSound).Stop();
                expectationCount = -1;
            }
        }
    }
}
