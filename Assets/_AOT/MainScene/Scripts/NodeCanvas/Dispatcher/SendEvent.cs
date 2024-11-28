using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public class SendEvent : MonoBehaviour
    {
        public GraphOwner owner;
        public bool sendGlobal;

        public int eventId = 0;

        public static readonly string ON_CONTENT_EVENT = "OnContentEvent";
        public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        public static readonly string ON_CONTENT_UI_DETAIL_EVENT = "OnContentUIDetailEvent";
        
        private const string ON_SOUND_EVENT = "OnSoundEvent";
        public static readonly string ON_CREDIT_EVENT = "OnCreditEvent";
        public static readonly string ON_SLOT_EVENT = "OnSlotEvent";
        public static readonly string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        public static readonly string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";
        public static readonly string ON_WIN_EVENT = "OnWinEvent";
        public static readonly string ON_META_UI_EVENT = "OnMetaUIEvent";

        public void SendNow(string eventName)
        {
            var e = new EventData(eventName, eventId);
            if (sendGlobal)
            {
                Graph.SendGlobalEvent(e, this);
            }
            else
            {
                owner.SendEvent(e, this);
            }
        }

        public void DispatchContentEvent(string eventName)
        {
            var e = new EventData(eventName);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_CONTENT_EVENT, e);
            else
                GetComponent<ContentEventDispatcher>().Dispatch(e);
        }

        public void DispatchContentUIEvent(string eventName)
        {
            var e = new EventData(eventName);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, e);
            else
                GetComponent<ContentUIEventDispatcher>().Dispatch(e);
        }

        public void DispatchContentUIDetailEvent(string eventName)
        {
            if(eventName == "demonessBreathUnlockReelsFromAnimation") 
            {
                Debug.LogError(eventName);
            }
            var e = new EventData(eventName);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_CONTENT_UI_DETAIL_EVENT, e);
            else
                GetComponent<ContentUIDetailEventDispatcher>().Dispatch(e);
        }

        public void DispatchSoundEvent(string eventName)
        {
            var e = new EventData(eventName);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_SOUND_EVENT, e);
            else
                GetComponent<SoundEventDispatcher>().Dispatch(e);
        }

        public void DispatchCreditEvent(string eventName)
        {
            var e = new EventData(eventName);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_CREDIT_EVENT, e);
            else
                GetComponent<CreditEventDispatcher>().Dispatch(e);
        }

        public void DispatchSlotEvent(string eventName)
        {
            var e = new EventData(eventName, eventId);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_SLOT_EVENT, e);
            else
                GetComponent<SlotEventDispatcher>().Dispatch(e);
        }

        public void DispatchSlotDetailEvent(string eventName)
        {
            var e = new EventData(eventName, eventId);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, e);
            else
                GetComponent<SlotDetailEventDispatcher>().Dispatch(e);
        }

        public void DispatchSpinButtonEvent(string eventName)
        {
            var e = new EventData(eventName);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, e);
            else
                GetComponent<SpinButtonEventDispatcher>().Dispatch(e);
        }

        public void DispatchWinEvent(string eventName)
        {
            var e = new EventData(eventName, eventId);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_WIN_EVENT, e);
            else
                GetComponent<WinEventDispatcher>().Dispatch(e);
        }

        public void DispatchMetaUIEvent(string eventName)
        {
            var e = new EventData(eventName, eventId);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_META_UI_EVENT, e);
            else
                GetComponent<MetaUIEventDispatcher>().Dispatch(e); 
        }
    }
}
