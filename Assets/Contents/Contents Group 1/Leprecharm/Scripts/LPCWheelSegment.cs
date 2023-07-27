using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Leprecharm
{
    public class LPCWheelSegment : MonoBehaviour
    {
        public Animator animator;
        public ContextTextMeshProUGUI label;
        public ExpandableHorizontalWheelSegment segment;
        public GameObject[] jackpotObjects;

        private int _oldSegmentValue = 0;
        private int _oldAddSegmentValue = 0;
        private int _oldJackpotValue = 1;

        private const string EVENT_CONTENT_UI = "OnContentUIEvent";
        private const string RESET_IMAGE = "ResetImage";
        private const string VALUE_UPGRADE = "Value Upgarde";
        private const string JACKPOT_FRAME = "Jackpot Frame";
        private const string SECTOR_VALUE = "SectorValue";
        private const string SIZE = "Size";
        private const string FORMAT_SECTOR_VALUE = "LPC_WHEEL_SECTOR_VALUE";
        
        private MessageDelegates eventDelegates;

        private void Awake()
        {
            eventDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "ResetSegments",           ResetSegment            },
                    { "UpdateSegmentMultiplier", UpdateSegmentMultiplier }
                }
            );
        }

        private void ResetSegment()
        {
            _oldSegmentValue = 0;
            _oldAddSegmentValue = 0;
            _oldJackpotValue = 0;
        }

        private void OnEnable()
        {
            ResetSegment();
            
            MessageDispatcher.Register(EVENT_CONTENT_UI, eventDelegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(EVENT_CONTENT_UI, eventDelegates.Delegate);
        }

        private void ResetSegment(EventData eventData)
        {
            StartCoroutine(_ResetSegment());
        }

        private IEnumerator _ResetSegment()
        {
            yield return new WaitForSeconds(5f);
            
            animator.SetTrigger(RESET_IMAGE);
            ToggleJackpotObjects(-1);
            label.gameObject.SetActive(false);
        }

        private void UpdateSegmentMultiplier(EventData eventData)
        {
            animator.SetTrigger(VALUE_UPGRADE);
        }

        private void Update()
        {
            if (segment.segmentValue != _oldSegmentValue || segment.additionalSegmentValue != _oldAddSegmentValue)
            {
                _oldSegmentValue = segment.segmentValue;
                _oldAddSegmentValue = segment.additionalSegmentValue;

                int segmentValue = segment.segmentValue + segment.additionalSegmentValue;
                
                int segmentType = -1;
                if (segmentValue < 25) segmentType = -1;
                else if (segmentValue < 50) segmentType = 0;
                else if (segmentValue < 100) segmentType = 1;
                else segmentType = 2;

                if (segmentType == -1)
                {
                    bool error;
                    label.SetText(StringTableUtils.GetString(StringTable.StringTableType.Content, FORMAT_SECTOR_VALUE, segmentValue, out error));
                }

                label.gameObject.SetActive(segmentType == -1);
                ToggleJackpotObjects(segmentType);
                animator.SetInteger(JACKPOT_FRAME, segmentType + 1);
                
                animator.SetInteger(SECTOR_VALUE, segment.segmentValue);
            }

            if (segment.jackpotSizeValue != _oldJackpotValue && segment.jackpotSizeValue >= 1)
            {
                _oldJackpotValue = segment.jackpotSizeValue;
                animator.SetInteger(SIZE, segment.jackpotSizeValue);
            }
        }

        private void ToggleJackpotObjects(int index)
        {
            for (int i = 0; i < jackpotObjects.Length; ++i)
            {
                jackpotObjects[i].SetActive(i == index);
            }
        }
    }
}
