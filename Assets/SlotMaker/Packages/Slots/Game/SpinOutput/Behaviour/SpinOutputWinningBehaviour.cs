using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class SpinOutputWinningBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public SpinOutputWinningSubset output;

        [Serializable]
        public class SpinOutputWinningSubsetEvent : UnityEvent<SpinOutputWinningSubset> {}
        [Serializable]
        public class SpinWinEvent : UnityEvent<SpinWin> {}
        
        public SpinOutputWinningSubsetEvent onTotalWin;
        public SpinWinEvent onSingleWin;
        public SpinOutputWinningSubsetEvent onSkipWin;

        protected void OnEnable()
        {
            output.onTotalWin  += OnTotalWin;
            output.onSingleWin += OnSingleWin;
            output.onSkipWin   += OnSkipWin;
        }

        protected void OnDisable()
        {
            output.onTotalWin  -= OnTotalWin;
            output.onSingleWin -= OnSingleWin;
            output.onSkipWin   -= OnSkipWin;
        }

        protected void OnTotalWin(SpinOutputWinningSubset eventData)
        {
            onTotalWin.Invoke(eventData);
        }

        protected void OnSingleWin(SpinWin eventData)
        {
            onSingleWin.Invoke(eventData);
        }

        protected void OnSkipWin(SpinOutputWinningSubset eventData)
        {
            onSkipWin.Invoke(eventData);
        }
    }
}