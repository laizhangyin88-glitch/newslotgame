using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;

namespace BagelCode
{
    public class OrientationChangeListener : MonoBehaviour
    {
        public UnityScreenOrientationEvent changeOrientation;

        private SlotMaker.MessageDelegates metaUIEventDelegates;

        private void Awake()
        {
            metaUIEventDelegates = new SlotMaker.MessageDelegates
            (
                new Dictionary<string, SlotMaker.MessageDispatcher.EventDelegate>
                {
                    { "OnChangeOrientation",   ChangeOrientation }
                }
            );
        }

        private void OnEnable()
        {
            SlotMaker.MessageDispatcher.Register("OnMetaUIEvent", metaUIEventDelegates.Delegate);
        }

        private void OnDisable()
        {
            SlotMaker.MessageDispatcher.UnRegister("OnMetaUIEvent", metaUIEventDelegates.Delegate);
        }

        public void ChangeOrientation(EventData eventData)
        {
            var targetOriendation = ((EventData<ScreenOrientation>)eventData).value;
            if(changeOrientation != null)
                changeOrientation.Invoke(targetOriendation);
        }
    }

    [Serializable]
    public class UnityScreenOrientationEvent : UnityEvent<ScreenOrientation> { }
}
