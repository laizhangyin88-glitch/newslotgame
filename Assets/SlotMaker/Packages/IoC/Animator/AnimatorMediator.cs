using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorMediator : MonoBehaviour
    {
        public Animator animator;
        [Serializable]
        public class Listener
        {
            [HideInInspector]
            public Animator animator;
            public EnableAction enableAction = EnableAction.DoNothing; 
            public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;
            [ValueDropdown("GetListenerNames")]
            public string key;
            [InlineEditor]
            public VariableAsset value;
            public ValueType valueType = ValueType.ByAsset;

            [ShowIf("ActiveTrigger")]
            public bool resetTrigger;
            [ShowIf("ActiveBoolValue")]
            public bool boolValue;
            [ShowIf("ActiveIntValue")]
            public int intValue;
            [ShowIf("ActiveFloatValue")]
            public float floatValue;

            private bool startCalled;

            public void Start()
            {
                startCalled = true;
                if (enableAction == EnableAction.EnableBehaviour)
                    OnValueChanged(value);
            }

            public void OnEnable()
            {
                if (startCalled && enableAction == EnableAction.EnableBehaviour)
                    OnValueChanged(value);

                value.onValueChanged += OnValueChanged;
            }

            public void OnDisable()
            {
                value.onValueChanged -= OnValueChanged;
            }

            public void OnValueChanged(VariableAsset val)
            {
                switch (parameterType)
                {
                case AnimatorControllerParameterType.Trigger:
                    if (resetTrigger) animator.ResetTrigger(key);
                    animator.SetTrigger(key);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(key, valueType == ValueType.ByAsset ? (bool)value.value : boolValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(key, valueType == ValueType.ByAsset ? (int)value.value : intValue);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(key, valueType == ValueType.ByAsset ? (float)value.value : floatValue);
                    break;
                }
            }

            private IEnumerable GetListenerNames()
            {
                return animator.parameters.Where(p => p.type == parameterType).Select(p => p.name).ToArray();
            }

            private bool ActiveTrigger() { return parameterType == AnimatorControllerParameterType.Trigger; }
            private bool ActiveBoolValue() { return valueType == ValueType.ByValue && parameterType == AnimatorControllerParameterType.Bool; }
            private bool ActiveIntValue() { return valueType == ValueType.ByValue && parameterType == AnimatorControllerParameterType.Int; }
            private bool ActiveFloatValue() { return valueType == ValueType.ByValue && parameterType == AnimatorControllerParameterType.Float; }
        }
        public List<Listener> listeners = new List<Listener>();

        protected virtual void Start()
        {
            foreach (var listener in listeners)
            {
                listener.Start();
            }
        }

        protected virtual void OnEnable()
        {
            foreach (var listener in listeners)
            {
                listener.OnEnable();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var listener in listeners)
            {
                listener.OnDisable();
            }
        }

        public void Dispatch(VariableAsset evt)
        {
            if (evt) evt.Dispatch();
        }

        ////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        ////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            Validate();
        }

        private void Validate()
        {
            if (!animator)
                animator = GetComponent<Animator>();

            foreach (var listener in listeners)
            {
                if (listener.animator == null) 
                    listener.animator = animator;
            }
        }
#endif
    }
}