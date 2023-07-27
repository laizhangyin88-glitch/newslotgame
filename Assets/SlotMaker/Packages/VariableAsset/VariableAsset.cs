using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker.Json;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [Serializable]
    public abstract class VariableAsset : ScriptableObject, IVariableRef
    {
        public bool isProtected;

        public event Action<VariableAsset> onValueChanged;

        public object value
        {
            get { return objectValue; }
            set 
            { 
                if (!isProtected && !object.Equals(objectValue, value))
                {
                    objectValue = value; 
                    OnValueChanged();
                }
            }
        }

        protected void OnValueChanged() 
        {
            if (onValueChanged != null) onValueChanged(this);
        }

        [Button]
        [PropertyOrder(-1)]
        public void Dispatch()
        {
            OnValueChanged();
        }

        public void Dispatch(object value)
        {
            objectValue = value;
            Dispatch();
        }

        protected abstract object objectValue { get; set; }
        public abstract Type varType { get; }
    }

    [Serializable]
    public class VariableAsset<T> : VariableAsset, ISerializationCallbackReceiver
    {
        [HideInInspector]
        [SerializeField]
        protected T overridenValue;

        protected T runtimeValue;

#if UNITY_EDITOR
        [ShowInInspector]
        protected bool runtimeEditMode;
#endif

        [ShowInInspector]
        [DisableIf("isProtected")]
        new public T value
        {
            get { return (T)objectValue; }
            set
            {
                if (!isProtected && !object.Equals(objectValue, value))
                {
                    objectValue = value;
                    OnValueChanged();
                }
            }
        }

        protected override object objectValue
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenValue;
#endif
                return runtimeValue;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenValue = (T)value;
#endif
                runtimeValue = (T)value;
            }
        }

        public override Type varType { get { return typeof(T); } }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Deserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() 
        {
            Serialize();
        }

        protected virtual void Deserialize()
        {
            runtimeValue = overridenValue;    
        }

        protected virtual void Serialize() {}
    }

    [Serializable]
    public class VariableList<T> : VariableAsset, ISerializationCallbackReceiver, IEnumerable<T>
    {
        [HideInInspector]
        [SerializeField]
        protected List<T> overridenValue = new List<T>();

        protected List<T> runtimeValue;

#if UNITY_EDITOR
        [ShowInInspector]
        protected bool runtimeEditMode;
#endif

        [ShowInInspector]
        [DisableIf("isProtected")]
        new public List<T> value
        {
            get { return (List<T>)objectValue; }
            set
            {
                if (!isProtected && !object.Equals(objectValue, value))
                {
                    objectValue = value;
                    OnValueChanged();
                }
            }
        }

        public T this[int index]
        {
            get { return value[index]; }
            set 
            { 
                if (!isProtected && !object.Equals(this.value[index], value))
                {
                    this.value[index] = value;
                    OnValueChanged();
                }
            }
        }

        public int Count { get { return value.Count; } }

        public void Clear() { value.Clear(); }

        public void Add(T item)
        {
            value.Add(item);
        }

        protected override object objectValue
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    return overridenValue;
#endif
                return runtimeValue;
            }

            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || runtimeEditMode)
                    overridenValue = (List<T>)value;
#endif
                runtimeValue = (List<T>)value;
            }
        }

        public override Type varType { get { return typeof(List<T>); } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return value.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return value.GetEnumerator();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Deserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() 
        {
            Serialize();
        }

        protected virtual void Deserialize()
        {
            runtimeValue = new List<T>(overridenValue);
        }

        protected virtual void Serialize() {}
    }

    [Serializable]
    public class VariableJson<T> : VariableAsset
    {
        [InlineEditor]
        public TextAsset json;

        protected T runtimeValue;

        new public T value
        {
            get { return (T)objectValue; }
            set
            {
                if (!isProtected && !object.Equals(objectValue, value))
                {
                    objectValue = value;
                    OnValueChanged();
                }
            }
        }

        protected override object objectValue
        {
            get { return runtimeValue; }
            set { runtimeValue = (T)value; }
        }

        public override Type varType { get { return typeof(T); } }

        protected virtual void OnEnable()
        {
            if (json) runtimeValue = SlotSimpleJson.DeserializeObject<T>(json.text);
        }
    }
}