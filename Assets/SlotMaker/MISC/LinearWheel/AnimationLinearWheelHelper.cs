using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

namespace SlotMaker
{
    public class AnimationLinearWheelHelper : MonoBehaviour
    {
        [Serializable]
        public enum SegmentAnimatorEventType
        {
            Trigger,
            Boolean,
            GotoState
        }
    	public AnimationLinearWheel linearWheel;

        [BoxGroup("Segment List")]
        public Transform segmentsParent;

        [BoxGroup("Segment Animator Event")]
        public SegmentAnimatorEventType onEnterEventType;
        [BoxGroup("Segment Animator Event")]
        public List<string> onEnterSelection;
        [BoxGroup("Segment Animator Event")]
        public SegmentAnimatorEventType onExitEventType;
        [BoxGroup("Segment Animator Event")]
        public List<string> onExitSelection;
        [BoxGroup("Segment Animator Event")]
        public SegmentAnimatorEventType onResetEventType;
        [BoxGroup("Segment Animator Event")]
        public List<string> onResetWheel;

#if UNITY_EDITOR
        [BoxGroup("Segment List")]
        [Button]
        private void SetSegmentsList()
        {
            if (segmentsParent == null) return;

            linearWheel = gameObject.GetComponent<AnimationLinearWheel>();
            if (linearWheel.segmentList == null)
                linearWheel.segmentList = new List<LinearWheelSegment>();
            linearWheel.segmentList.Clear();

            foreach (Transform child in segmentsParent)
            {
                var segment = child.GetComponent<LinearWheelSegment>();
                if (segment != null)
                    linearWheel.segmentList.Add(segment);
            }
        }

        [BoxGroup("Segment Animator Event")]
        [Button]
        private void SetAllSegmentsAnimatorEvents()
        {
            linearWheel = gameObject.GetComponent<AnimationLinearWheel>();

            foreach (LinearWheelSegment segment in linearWheel.segmentList)
            {
                SetSegmentEvents(segment);
            }
        }

        private void SetSegmentEvents(LinearWheelSegment segment)
        {
            Animator segmentAnim = segment.gameObject.GetComponent<Animator>();

            AddSegmentEventListeners(segment.onEnterSelection, segmentAnim, onEnterEventType, onEnterSelection, false);
            AddSegmentEventListeners(segment.onExitSelection, segmentAnim, onExitEventType, onExitSelection, true);
            AddSegmentEventListeners(segment.onResetWheel, segmentAnim, onResetEventType, onResetWheel, true);
        }

        private void AddSegmentEventListeners(UnityEvent segmentEvent, Animator segmentAnim, SegmentAnimatorEventType eventType, List<string> paramList, bool resetOnBoolean)
        {
            UnityAction<string> listenerAction;
            switch (eventType)
            {
                case SegmentAnimatorEventType.GotoState:
                    listenerAction = segmentAnim.Play;
                    break;
                
                case SegmentAnimatorEventType.Boolean:
                    if (resetOnBoolean)
                    {
                        listenerAction = segmentAnim.ResetTrigger; // Equals to Animator.SetBool(..., false);
                    }
                    else
                    {
                        listenerAction = segmentAnim.SetTrigger;  // Equals to Animator.SetBool(..., true);
                    }
                    break;

                case SegmentAnimatorEventType.Trigger:
                    listenerAction = segmentAnim.SetTrigger;
                    break;

                default:
                    listenerAction = null;
                    break;
            }

            foreach (string paramName in paramList)
            {
                UnityEventTools.AddStringPersistentListener(segmentEvent, listenerAction, paramName);
            }
        }
#endif
    }
}
