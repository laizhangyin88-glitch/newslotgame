using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class OverridenLayoutElement : UIBehaviour, ILayoutElement
    {
    	public RectTransform target;

        public float _minWidth = -1;
        public float _minHeight = -1;
        public float _preferredWidth = -1;
        public float _preferredHeight = -1;
        public float _flexibleWidth = -1;
        public float _flexibleHeight = -1;

        protected RectTransform targetRect
        {
        	get	{ return (target == null) ? (transform as RectTransform) : target; }
        }

        public virtual void CalculateLayoutInputHorizontal() {}
        public virtual void CalculateLayoutInputVertical() {}
        
        public virtual float minWidth 
        { 
        	get { return _minWidth < 0 ? targetRect.rect.width : _minWidth; } 
        }
        
        public virtual float minHeight 
        { 
        	get { return _minHeight < 0 ? targetRect.rect.height : _minHeight; } 
        }

        public virtual float preferredWidth { get { return _preferredWidth; } }
        public virtual float preferredHeight { get { return _preferredHeight; } }
        public virtual float flexibleWidth { get { return _flexibleWidth; } }
        public virtual float flexibleHeight { get { return _flexibleHeight; } }
        public virtual int layoutPriority { get { return 1; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnDisable()
        {
            SetDirty();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
    #endif
    }
}