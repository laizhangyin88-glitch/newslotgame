using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot
{
    public abstract class FeatureController : FeatureModule
    {
        protected abstract string ON_FEATURE_BEGIN_EVENT { get; }
        protected abstract string ON_FEATURE_END_EVENT { get; }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(ON_FEATURE_BEGIN_EVENT, (EventData eventData) => StartCoroutine(Play()));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(ON_FEATURE_BEGIN_EVENT);
        }

        private IEnumerator Play()
        {
            OnStart();
            yield return StartCoroutine(OnPlayCoroutine());
            OnFinish();

            ContentEvent.SendEvent(ON_FEATURE_END_EVENT);
        }

        protected virtual void OnStart() {}
        protected virtual void OnFinish() {}
        protected abstract IEnumerator OnPlayCoroutine();
    }
}