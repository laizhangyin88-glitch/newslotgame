using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PassiveEventsBehaviour : MonoBehaviour
    {
        public List<EventInfoType> passiveEventTypeList = new List<EventInfoType>();

        public UnityEvent onRefreshPassive;
        public UnityEvent onEnablePassive;
        public UnityEvent onDisablePassive;

        private List<int> progressEventIdList = new List<int>();

        private MessageDelegates passiveDelegates;

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

            if (!isInit) return;
            RefreshPassive();
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister("OnPassiveEvent", passiveDelegates.Delegate);

            if (!isInit) return;
        }

        private void RefreshPassive()
        {
            progressEventIdList.Clear();

            bool enabled = false;
            foreach(var type in passiveEventTypeList)
            {
                EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(type);

                CancelInvoke();

                if (eventInfo != null)
                {
                    progressEventIdList.Add(eventInfo.id);

                    if(!enabled)
                    {
                        enabled = true;
                        onEnablePassive?.Invoke();
                    }

                    if (eventInfo.endTimestamp > 0)
                    {
                        long leftTimestamp = eventInfo.endTimestamp - TimeUtils.GetTimeStamp();
                        Invoke(nameof(RefreshPassive), (float)(leftTimestamp / 1000L) + 0.33f);
                    }
                }
            }

            if (!enabled)
            {
                onDisablePassive?.Invoke();
            }

            onRefreshPassive?.Invoke();
        }

        private void StartPassive(EventData eventData)
        {
            if (eventData.value != null)
            {
                foreach (var type in passiveEventTypeList)
                {
                    EventInfoType eventtype = (EventInfoType)eventData.value;
                    if (type == eventtype)
                    {
                        RefreshPassive();
                        return;
                    }
                }
            }
        }

        private void RefreshPassive(EventData eventData)
        {
            if (eventData.value != null)
            {
                int eventValueID = (int)eventData.value;
                if (progressEventIdList.Contains(eventValueID))
                    RefreshPassive();
            }
        }
    }
}
