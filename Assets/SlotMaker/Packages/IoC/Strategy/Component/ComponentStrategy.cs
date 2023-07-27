using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy
{
    [Serializable]
    public abstract class ComponentStrategy : ScriptableObject
    {
        public abstract Type componentType { get; }
        
        public Component Get(Component comp) { return comp; }
        public Component GetComponent(GameObject gameObject) { return gameObject.GetComponent(componentType); }
    }

    [Serializable]
    public abstract class ComponentStrategy<T> : ComponentStrategy 
        where T : Component
    {
        public override Type componentType { get { return typeof(T); } }

        public new T Get(Component comp)
        {
            return comp as T;
        }
    }

    [Serializable]
    public abstract class PropertyStrategy : ComponentStrategy
    {
        public virtual float GetSingle(Component comp) { return 0f; }
        public virtual void SetSingle(Component comp, float value) {}
        public virtual Vector2 GetVector2(Component comp) { return Vector2.zero; }
        public virtual void SetVector2(Component comp, Vector2 value) {}
        public virtual Vector3 GetVector3(Component comp) { return Vector3.zero; }
        public virtual void SetVector3(Component comp, Vector3 value) {}
        public virtual Vector4 GetVector4(Component comp) { return Vector4.zero; }
        public virtual void SetVector4(Component comp, Vector4 value) {}
        public virtual Quaternion GetQuaternion(Component comp) { return Quaternion.identity; }
        public virtual void SetQuaternion(Component comp, Quaternion value) {}
        public virtual Color GetColor(Component comp) { return Color.white; }
        public virtual void SetColor(Component comp, Color value) {}
        public virtual String GetString(Component comp) { return string.Empty; }
        public virtual void SetString(Component comp, string value) {}
    }

    [Serializable]
    public abstract class PropertyStrategy<T> : PropertyStrategy
        where T : Component
    {
        public override Type componentType { get { return typeof(T); } }

        public new T Get(Component comp)
        {
            return comp as T;
        }
    }
}