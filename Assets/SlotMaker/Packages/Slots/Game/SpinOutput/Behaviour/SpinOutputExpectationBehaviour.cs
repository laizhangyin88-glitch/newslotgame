using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class SpinOutputExpectationBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public SpinOutputExpectationSubset output;

        public UnityIntEvent onBeginExpectation;
        public UnityIntEvent onBeginFirstExpectation;
        
        public UnityIntEvent onEndExpectation;
        public UnityIntEvent onEndLastExpectation;

        [Serializable]
        public class UnityCell3ListEvent : UnityEvent<List<List<Cell3>>> {}

        public UnityCell3ListEvent onExpectationSpots;

        protected void OnEnable()
        {
            output.onBeginExpectation      += OnBeginExpectation;
            output.onBeginFirstExpectation += OnBeginFirstExpectation;
            output.onEndExpectation        += OnEndExpectation;
            output.onEndLastExpectation    += OnEndLastExpectation;
            output.onExpectationSpots      += OnExpectationSpots;
        }

        protected void OnDisable()
        {
            output.onBeginExpectation      -= OnBeginExpectation;
            output.onBeginFirstExpectation -= OnBeginFirstExpectation;
            output.onEndExpectation        -= OnEndExpectation;
            output.onEndLastExpectation    -= OnEndLastExpectation;
            output.onExpectationSpots      -= OnExpectationSpots;
        }

        protected void OnBeginExpectation(int index)
        {
            onBeginExpectation.Invoke(index);
        }

        protected void OnBeginFirstExpectation(int index)
        {
            onBeginFirstExpectation.Invoke(index);
        }

        protected void OnEndExpectation(int index)
        {
            onEndExpectation.Invoke(index);
        }

        protected void OnEndLastExpectation(int index)
        {
            onEndLastExpectation.Invoke(index);
        }

        protected void OnExpectationSpots(List<List<Cell3>> spots)
        {
            onExpectationSpots.Invoke(spots);
        }
    }
}