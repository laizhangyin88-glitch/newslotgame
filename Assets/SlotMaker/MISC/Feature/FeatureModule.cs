using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot
{
    public abstract class FeatureModule : MonoBehaviour
    {
        private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
        private void Dispatch(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        protected void RegisterEvent(string eventName, MessageDispatcher.EventDelegate eventDelegate)
        {
            delegates.Add(eventName, eventDelegate);
        }

        protected void UnRegisterEvent(string eventName)
        {
            delegates.Remove(eventName);
        }

        protected virtual void OnEnable()
        {
            ContentEvent.Register(Dispatch);
        }

        protected virtual void OnDisable()
        {
            ContentEvent.UnRegister(Dispatch);
        }
    }
}