using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC.UI
{
    public class TextMediator : MonoBehaviour
    {
        public Component target;
        
        [InlineEditor]
        public PropertyStrategy propertyInjector;
        [InlineEditor]
        public StringSources sourcesInjector;
        
        public EnableAction enableAction = EnableAction.EnableBehaviour; 
        
        [ValueDropdown("GetStringSourcesNames")]
        public string key;

        [InlineEditor]
        public List<VariableAsset> args = new List<VariableAsset>();
        
        [HideLabel]
        [ShowInInspector]
        [MultiLineProperty(3)]
        public string value
        {
            get 
            {
                return sourcesInjector ? sourcesInjector.GetString(key) : string.Format("<color=red>${0}</color>", key);        
            }

            set
            {
                if (sourcesInjector)
                {
                    sourcesInjector.SetString(key, value);
                    UpdateText();
                }
            }
        }

        protected bool startCalled;

        protected virtual void Start()
        {
            startCalled = true;
            if (enableAction == EnableAction.EnableBehaviour)
                UpdateText();
        }

        protected virtual void OnEnable()
        {
            if (startCalled && enableAction == EnableAction.EnableBehaviour)
                UpdateText();

            foreach (var arg in args)
                arg.onValueChanged += OnArgumentChanged;
        }

        protected virtual void OnDisable()
        {
            foreach (var arg in args)
                arg.onValueChanged -= OnArgumentChanged;
        }

        [Button]
        private void UpdateText()
        {
            if (target && propertyInjector)
                propertyInjector.SetString(target, ToString());
        }

        private object[] GetArgs()
        {
            int count = args.Count;
            if (count == 0) 
                return null;

            List<object> list = new List<object>();
            foreach (var arg in args)
            {
                if (arg == null)
                    list.Add(arg);
                else if (arg is IEnumerable)
                {
                    foreach (var item in (IEnumerable)arg)
                    {
                        list.Add(item);
                    }
                }
                else
                    list.Add(arg.value);
            }
            return list.ToArray();
        }

        private void OnArgumentChanged(VariableAsset arg)
        {
            UpdateText();
        }

        public override string ToString()
        {
            var objArgs = GetArgs();
            if (objArgs == null) return value;
#if UNITY_EDITOR
            try 
            {
                return string.Format(StringTableUtils.customProvider, value, objArgs);
            }
            catch (System.FormatException e)
            {
                return value;
            }
#else
            return string.Format(StringTableUtils.customProvider, value, objArgs);
#endif
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
            if (!target && propertyInjector)
                target = propertyInjector.GetComponent(gameObject);

            UpdateText();
        }

        private IEnumerable GetStringSourcesNames()
        {
            return sourcesInjector ? sourcesInjector.GetNames() : null;
        }
#endif
    }
}