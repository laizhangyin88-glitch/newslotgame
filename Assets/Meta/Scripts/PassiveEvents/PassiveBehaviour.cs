using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PassiveBehaviour : MonoBehaviour
    {
        public EventInfoType passiveEventType = EventInfoType.UNKNOWN;

        public UnityEvent onInitPassive;
        public UnityEvent onEnablePassive;
        public UnityEvent onDisablePassive;

        private MessageDelegates passiveDelegates;

        private int progressEventID = 0;

        private bool isInit = false;

        private void Awake()
        {
            passiveDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "StartPassive", StartPassive },
                    { "RefreshPassive", RefreshPassive }
                }
            );
        }

        private void Start()
        {
            onInitPassive?.Invoke();

            RefreshPassive();

            isInit = true;
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register("OnPassiveEvent", passiveDelegates.Delegate);

            if(!isInit) return;
            RefreshPassive();
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister("OnPassiveEvent", passiveDelegates.Delegate);

            if(!isInit) return;
        }

        private void RefreshPassive()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(passiveEventType);

            CancelInvoke();

            if(eventInfo != null)
            {
                progressEventID = eventInfo.id;
                onEnablePassive?.Invoke();

                if(eventInfo.endTimestamp > 0)
                {
                    long leftTimestamp = eventInfo.endTimestamp - TimeUtils.GetTimeStamp();
                    Invoke(nameof(RefreshPassive), (float)(leftTimestamp/1000L) + 0.33f);
                }
            }
            else
            {
                progressEventID = 0;
                onDisablePassive?.Invoke();
            }
        }

        private void StartPassive(EventData eventData)
        {
            if(progressEventID == 0)
            {
                if(eventData.value != null)
                {
                    EventInfoType eventtype = (EventInfoType)eventData.value;
                    if(passiveEventType == eventtype)
                        RefreshPassive();
                }
            }
        }

        private void RefreshPassive(EventData eventData)
        {
            if(eventData.value != null)
            {
                int eventValueID = (int)eventData.value;
                if(progressEventID == eventValueID)
                    RefreshPassive();
            }
        }
    }
}
