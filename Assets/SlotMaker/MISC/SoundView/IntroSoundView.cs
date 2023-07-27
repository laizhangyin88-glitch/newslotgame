using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class IntroSoundView : MonoBehaviour
    {
        public string introSound;

        public static readonly string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        public static readonly string ON_INTRO_EVENT = "Intro";

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register(ON_CONTENT_UI_EVENT, OnContentUIEvent);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, OnContentUIEvent);
        }

        protected virtual void OnContentUIEvent(EventData receivedEvent)
        {
            if (receivedEvent.name.Equals(ON_INTRO_EVENT, StringComparison.Ordinal))
                OnIntro();
        }

        private void OnIntro()
        {
            GSManager.Instance.GetHandler(introSound).Play();
        }
    }
}
