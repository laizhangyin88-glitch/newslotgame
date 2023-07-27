using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class SimpleReserveTimer : MonoBehaviour
    {
        public long targetTimeStamp;
        public Action callback;

        public bool existReserve { get; private set; }

        private void OnEnable()
        {
            if(existReserve)
            {
                SetReserveCallback(targetTimeStamp, callback);
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void SetReserveCallback(long targetTimeStamp, Action callback)
        {
            Stop();

            this.targetTimeStamp = targetTimeStamp;
            this.callback = callback;

            long currentTimestamp = TimeUtils.GetTimeStamp();

            if(targetTimeStamp > currentTimestamp)
            {
                existReserve = true;

                if(gameObject.activeInHierarchy)
                {
                    StartCoroutine("CheckTime");
                }
            }
        }

        private IEnumerator CheckTime()
        {
            while(true)
            {
                if(TimeUtils.GetTimeStamp() >= targetTimeStamp)
                {
                    ReserveTimeCall();
                    break;
                }

                yield return new WaitForSeconds(0.33f);
            }
        }

        public void Stop()
        {
            callback = null;
            targetTimeStamp = 0;
            StopAllCoroutines();
        }

        private void ReserveTimeCall()
        {
            existReserve = false;

            if(callback != null)
            {
                callback.Invoke();
                callback = null;
            }
        }
    }
}
