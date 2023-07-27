using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class ContextElement : MonoBehaviour, IContextElement
    {
    	public ContextElement Root
    	{
    		get
    		{
    			var parent = Parent;
    			if (Parent == null)
    				return this;

    			while (parent != null)
    				parent = parent.Parent;

    			return parent;
    		}
    	}

    	public ContextElement Parent { get; set; }

    	public string ContextName { get { return gameObject.name; } }

        public virtual bool IsRoot { get { return false;  } }

        public virtual bool IsLeaf { get { return false; } }

        public virtual void AddContextElement(ContextElement element) {}

        public virtual void RemoveContextElement(ContextElement element) {}

    	public virtual void DestroyElement()
    	{
    		Destroy(gameObject);
    	}

        public virtual ContextElement Find(string name, bool deepSearch = false) { return null; }

        public virtual ContextElement FindWithFullName(string fullName) { return null; }

        public virtual int ChildCount { get { return 0; } }

        public virtual ContextElement GetChildElement(int index) { return null; }

        public virtual IEnumerator GetEnumerator() { return null; }

    	protected bool dirty = true;

    	public virtual void SetDirty()
    	{
    		dirty = true;
    	}

    	public bool IsDirty() { return dirty; }

        public virtual void UpdateContext(bool forceUpdate = false) {}

    	protected virtual void UpdateContext(ContextElement parentElement, Transform searchingTransform, bool forceUpdate = false)
    	{
    		if (parentElement.IsLeaf) return;

    		int count = searchingTransform.childCount;
    		for (int i = 0; i < count; ++i)
    		{
    			var child = searchingTransform.GetChild(i);
    			var childElement = child.GetComponent<ContextElement>();
    			if (childElement != null)
    			{
    				if (!childElement.IsRoot && !(childElement is IContextIgnoreGroup))
    				{
    					parentElement.AddContextElement(childElement);
    					childElement.UpdateContext(forceUpdate);
    				}
    			}
    			else
    			{
    				UpdateContext(parentElement, child, forceUpdate);
    			}
    		}
    	}

    	protected void OnTransformChildrenChanged()
    	{
    		SetDirty();
    	}

    	public override string ToString()
    	{
    		return ContextName;
    	}

#if UNITY_EDITOR
        private void Reset()
        {
            if (this.GetType() == typeof(ContextElement))
            {
                Debug.LogError("ContextElement Can't add itself on GameObject!");
                DestroyImmediate( this );
            }
        }
#endif

    }
}
