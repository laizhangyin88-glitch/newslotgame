using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class InGameMessageDispatcher : MonoBehaviour
    {
        public static readonly string ON_META_UI_EVENT = "OnMetaUIEvent";
        public static readonly string ON_CREDIT_EVENT = "OnCreditEvent";
        public static readonly string ON_CONTENT_EVENT = "OnContentEvent";
        public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        public static readonly string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";

        private MessageRouter _router;
        protected MessageRouter router 
        { 
            get 
            { 
                if (_router == null)
                {
                    _router = GetComponent<MessageRouter>();
                    if (_router == null)
                        _router = gameObject.AddComponent<MessageRouter>();
                }
                return _router;
            } 
        }

        protected void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.Register(ON_CREDIT_EVENT, OnCreditEvent);
            MessageDispatcher.Register(ON_CONTENT_EVENT, OnContentEvent);
            MessageDispatcher.Register(ON_CONTENT_UI_EVENT, OnContentUIEvent);
            MessageDispatcher.Register(ON_SPINBUTTON_EVENT, OnSpinButtonEvent);
        }

        protected void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.UnRegister(ON_CREDIT_EVENT, OnCreditEvent);
            MessageDispatcher.UnRegister(ON_CONTENT_EVENT, OnContentEvent);
            MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, OnContentUIEvent);
            MessageDispatcher.UnRegister(ON_SPINBUTTON_EVENT, OnSpinButtonEvent);
        }

        protected void OnMetaUIEvent(EventData eventData)
        {
            router.Dispatch(ON_META_UI_EVENT, eventData);
        }

        protected void OnCreditEvent(EventData eventData)
        {
            router.Dispatch(ON_CREDIT_EVENT, eventData);
        }

        protected void OnContentEvent(EventData eventData)
        {
            router.Dispatch(ON_CONTENT_EVENT, eventData);
        }

        protected void OnContentUIEvent(EventData eventData)
        {
            router.Dispatch(ON_CONTENT_UI_EVENT, eventData);
        }

        protected void OnSpinButtonEvent(EventData eventData)
        {
            router.Dispatch(ON_SPINBUTTON_EVENT, eventData);
        }
    }
}