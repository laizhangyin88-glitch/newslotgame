using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using SlotMaker.IoC;
using Sirenix.OdinInspector;

namespace SlotMaker.Rendering
{
    [ExecuteAlways]
    public class IndexableSortingLayer : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        protected SortingGroup sortingGroup;
        
        [HideInInspector]
        [SerializeField]
        new protected Renderer renderer;

        [InlineEditor]
        public SortingLayerNameIDs sortingLayerIDs;

        [HideInInspector]
        [SerializeField]
        protected int _sortingLayerIndex;
        [ShowInInspector]
        public int sortingLayerIndex
        {
            get { return _sortingLayerIndex; }
            set
            {
                if (_sortingLayerIndex != value)
                {
                    _sortingLayerIndex = value;
                    SetSortingLayerIndexDirty();
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _sortingOrder;
        [ShowInInspector]
        public int sortingOrder
        {
            get { return _sortingOrder; }
            set 
            { 
                if (_sortingOrder != value)
                {
                    _sortingOrder = value;
                    SetSortingOrderDirty();
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _sortingOffset;
        [ShowInInspector]
        public int sortingOffset
        {
            get { return _sortingOffset; }
            set 
            { 
                if (_sortingOffset != value)
                {
                    lastSortingOffset = _sortingOffset = value;
                    SetSortingOrderDirty();
                }
            }
        }
        private int lastSortingOffset;

        [PropertyOrder(100)]
        public EnableAction enableAction = EnableAction.EnableBehaviour; 

        protected bool startCalled;

        protected virtual void Start()
        {
            startCalled = true;
            if (enableAction == EnableAction.EnableBehaviour)
                SetAllDirty();
        }

        protected virtual void OnEnable()
        {
            if (startCalled && enableAction == EnableAction.EnableBehaviour)
                SetAllDirty();
        }

        protected void OnDidApplyAnimationProperties()
        {
            SetAllDirty();
        }

        public void SetAllDirty()
        {
            SetSortingLayerIndexDirty();
            SetSortingOrderDirty();
        }

        public void SetSortingLayerIndexDirty()
        {
            if (sortingLayerIDs != null)
            {
                _sortingLayerIndex = sortingLayerIDs.ValidateIndex(_sortingLayerIndex);

                if (sortingGroup != null)
                    sortingGroup.sortingLayerID = sortingLayerIDs.GetID(_sortingLayerIndex);
                else if (renderer != null)
                    renderer.sortingLayerID = sortingLayerIDs.GetID(_sortingLayerIndex);
            }
        }

        public void SetSortingOrderDirty()
        {
            if (sortingGroup != null)
                sortingGroup.sortingOrder = _sortingOrder + _sortingOffset;
            else if (renderer != null)
                renderer.sortingOrder = _sortingOrder + _sortingOffset;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sortingGroup == null) 
                sortingGroup = GetComponent<SortingGroup>();
            if (renderer == null)
                renderer = GetComponent<Renderer>();

            SetAllDirty();
        }
#endif
    }
}