using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class RemainingTimerGaugeController : MonoBehaviour
    {
        public IContextFloatProperty gaugeElement;
        public Animator gaugeRateAnimator;

        public long startTimestamp;
        public long endTimestamp;
        public bool isInvertGauge;

        public string gaugeRateKey;

        public Action callback;

        public bool IsInit
        {
            get { return gaugeElement != null  && startTimestamp > 0 && endTimestamp > 0; }
        }

        private void OnEnable()
        {
            if(IsInit)
                StartCoroutine(StartRemainingTimer());
        }

        private void OnDisable()
        {
            StopTimer();
        }

        public void Init(  ContextElement gaugeElement,
                           Animator gaugeRateAnimator,
                           bool isInvertGauge,
                           string gaugeRateAnimatorKey,
                           Action callback )
        {
            this.gaugeElement      = gaugeElement as IContextFloatProperty;
            this.gaugeRateAnimator = gaugeRateAnimator;
            this.isInvertGauge     = isInvertGauge;
            this.callback          = callback;
            this.gaugeRateKey      = gaugeRateAnimatorKey;
            startTimestamp = 0;
            endTimestamp = 0;
        }

        public void StartTimer(long startTimestamp, long endTimestamp)
        {
            this.startTimestamp  = startTimestamp;
            this.endTimestamp = endTimestamp;

            StopAllCoroutines();

            if(gameObject.activeInHierarchy && IsInit)
                StartCoroutine(StartRemainingTimer());
        }

        public void StopTimer()
        {
            StopAllCoroutines();
        }

        private IEnumerator StartRemainingTimer()
        {
            long totalLeftTimestamp = endTimestamp - startTimestamp;
            long currentLeftTimestamp = TimeUtils.GetTimeStamp() - startTimestamp;

            while(currentLeftTimestamp < totalLeftTimestamp)
            {
                totalLeftTimestamp = endTimestamp - startTimestamp;
                currentLeftTimestamp = TimeUtils.GetTimeStamp() - startTimestamp;

                float rate = 0f;

                if(isInvertGauge)
                    rate = 1f - (float)((double)currentLeftTimestamp/(double)totalLeftTimestamp);
                else
                    rate = (float)((double)currentLeftTimestamp/(double)totalLeftTimestamp);

                if(rate < 0f)
                    rate = 0f;
                else if(rate > 1f)
                    rate = 1f;

                if(gaugeRateAnimator != null)
                    gaugeRateAnimator.SetFloat(gaugeRateKey, rate);

                if(gaugeElement != null)
                    gaugeElement.SetFloatProperty(rate);

                yield return new WaitForSeconds(0.33f);
            }

            if(callback != null)
            {
                callback.Invoke();
            }
        }
    }
}
