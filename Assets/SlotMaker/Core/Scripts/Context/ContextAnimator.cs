using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using BagelCode;

namespace SlotMaker
{
    public class ContextAnimator : ContextCompositor, IContextBooleanProperty, IContextIntProperty, IContextFloatProperty
    {
        public Animator animator;
        public string propertyName;

        public bool isPreserve = false;

        private AnimationPreserve preserveController = null;

        private void OnEnable()
        {
            if (isPreserve)
            {
                if (preserveController != null)
                {
                    preserveController.Preserve();
                }
            }
        }

        //private void OnSaveAniState(object value)
        private void OnSaveAniState()
        {
            if (isPreserve)
            {
                if (preserveController == null)
                {
                    preserveController = new AnimationPreserve();
                    preserveController.Init(animator);
                }
                //preserveController.SaveState(propertyName, value);
                preserveController.SaveState();
            }
        }
        
        public void SetBooleanProperty(bool value)
        {
            if (!animator.isActiveAndEnabled) return;
            animator.SetBool(propertyName, value);
            OnSaveAniState();
            //OnSaveAniState(value);
        }

        public bool GetBooleanProperty()
        {
            if (!animator.isActiveAndEnabled) return false;
            return animator.GetBool(propertyName);
        }

        public void SetIntProperty(int value)
        {
            if (!animator.isActiveAndEnabled) return;
            animator.SetInteger(propertyName, value);
            OnSaveAniState();
            //OnSaveAniState(value);
        }

        public int  GetIntProperty()
        {
            if (!animator.isActiveAndEnabled) return 0;
            return animator.GetInteger(propertyName);
        }

        public void SetFloatProperty(float value)
        {
            if (!animator.isActiveAndEnabled) return;
            animator.SetFloat(propertyName, value);
            OnSaveAniState();
            //OnSaveAniState(value);
        }

        public float GetFloatProperty()
        {
            if (!animator.isActiveAndEnabled) return 0f;
            return animator.GetFloat(propertyName);
        }
    }
}
