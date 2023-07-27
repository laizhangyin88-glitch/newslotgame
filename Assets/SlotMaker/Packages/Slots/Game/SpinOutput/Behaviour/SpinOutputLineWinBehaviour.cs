using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class SpinOutputLineWinBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public SpinOutputLineWin output;

        [Serializable]
        public class SpinOutputLineWinEvent : UnityEvent<SpinOutputLineWin> {}
        
        public SpinOutputLineWinEvent onTotalLineWin;

        protected virtual void OnEnable()
        {
            output.onTotalLineWin += OnTotalLineWin;
        }

        protected virtual void OnDisable()
        {
            output.onTotalLineWin -= OnTotalLineWin;
        }

        protected void OnTotalLineWin(SpinOutputLineWin eventData)
        {
            onTotalLineWin.Invoke(eventData);
        }
    }
}