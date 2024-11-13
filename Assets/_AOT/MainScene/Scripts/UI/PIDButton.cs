using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace SlotMaker
{
    public class PIDButton : Button
    { 
        [SerializeField]
        public bool onDisable = true;
        public float scaleFactor = 0.8f;
        public float frequency   = 10f;
        public float damping     = 1f;
        public Transform scaleTarget;
        public GameObject disableCover;
        public UnityBoolEvent onInteractableChanged;
        private Vector3 toScale = Vector3.one;
        private Vector3 scaleVelocity = Vector3.one;

        protected override void Awake()
        {
            if (disableCover == null)
            {
                Transform t = transform.Find("Anchor/Cover");

                if (t != null)
                    disableCover = t.gameObject;
            }

            if (scaleTarget == null)
            {
                scaleTarget = transform;
            }
        }

        private void ScaleButton(float toFactor, bool instant)
        {
            StopCoroutine("ScaleCoroutine");        
            toScale = new Vector3(toFactor, toFactor, 1f);
            if (instant)
            {
                scaleTarget.localScale = toScale;
            }
            else
            {
                StartCoroutine("ScaleCoroutine");
            }
        }

        private IEnumerator ScaleCoroutine()
        {
            while (!Mathf.Approximately(toScale.x, scaleTarget.localScale.x))
            {
                Vector3 delta = PIDUtils.CalcDisplacement(scaleTarget.localScale, toScale, ref scaleVelocity, Vector3.zero, frequency, damping, Time.deltaTime);       
                scaleTarget.localScale += delta;
                yield return null;
            }
        }

        protected override void InstantClearState()
        {
            base.InstantClearState();

            ScaleButton(1f, true);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (gameObject.activeInHierarchy)
            {
                float toFactor = 1f;
                if (state == SelectionState.Normal || state == SelectionState.Highlighted)
                {
                    if (disableCover != null) disableCover.SetActive(false);
                    onInteractableChanged.Invoke(true);
                }
                else if (state == SelectionState.Pressed)
                {
                    toFactor = scaleFactor;
                }
                else if (state == SelectionState.Disabled)
                {
                    if (onDisable && (disableCover != null)) disableCover.SetActive(true);
                    onInteractableChanged.Invoke(false);
                }

                ScaleButton(toFactor, instant);
            }

            base.DoStateTransition(state, instant);        
        }
    }
}
