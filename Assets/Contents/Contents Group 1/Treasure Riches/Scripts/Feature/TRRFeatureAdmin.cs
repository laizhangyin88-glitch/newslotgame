using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Popup;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace BagelCode.Slots.TRR.FeatureAdmin
{
    public abstract class TRRFeatureAdmin : MonoBehaviour
    {
        private MessageDelegates eventDelegates;
        private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        public abstract string CONTENT_EVENT_PLAY_FEATURE { get; }
        public abstract string CONTENT_EVENT_ON_FINISH_FEATURE { get; }

        protected Coroutine ShowDefualtPopupCoroutine(TRRPopup popup, Action initalize = null) => StartCoroutine(_ShowDefaultPopupCoroutine(popup, initalize));
        private IEnumerator _ShowDefaultPopupCoroutine(TRRPopup popup, Action initalize)
        {
            popup.gameObject.SetActive(true);
            yield return popup.PlayDefaultPopup(initalize);
            popup.gameObject.SetActive(false);
        }

        protected Coroutine ShowCustomPopupCoroutine(TRRPopup popup, Action initalize = null) => StartCoroutine(_ShowCustomPopupCoroutine(popup, initalize));
        private IEnumerator _ShowCustomPopupCoroutine(TRRPopup popup, Action initalize)
        {
            popup.gameObject.SetActive(true);
            yield return popup.PlayCustomPopup(initalize);
            popup.gameObject.SetActive(false);
        }

        private IEnumerator _PlayFeatureCoroutine()
        {
            yield return StartCoroutine(OnFeaturePlayCoroutine());
            MessageDispatcher.Dispatch(SendEvent.ON_CONTENT_EVENT, new EventData(CONTENT_EVENT_ON_FINISH_FEATURE));
        }
        protected abstract IEnumerator OnFeaturePlayCoroutine();

        protected void AddEventDelegate(string eventName, MessageDispatcher.EventDelegate eventDelegate)
        {
            delegates.Add(eventName, eventDelegate);
        }

        protected abstract void RegisterEventDelegates();

        protected virtual void OnEnable()
        {
            AddEventDelegate(CONTENT_EVENT_PLAY_FEATURE, (EventData eventData) => StartCoroutine(_PlayFeatureCoroutine()));
            RegisterEventDelegates();
            eventDelegates = new MessageDelegates(delegates);

            MessageDispatcher.Register(SendEvent.ON_CONTENT_EVENT, eventDelegates.Delegate);
        }
        protected virtual void OnDisable()
        {
            delegates.Clear();

            MessageDispatcher.UnRegister(SendEvent.ON_CONTENT_EVENT, eventDelegates.Delegate);
        }
    }
}