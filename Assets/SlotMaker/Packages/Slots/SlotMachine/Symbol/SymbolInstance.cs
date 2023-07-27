using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using NodeCanvas.StateMachines;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
	[ExecuteAlways]
	public class SymbolInstance : MonoBehaviour
	{
		public GameObject origin;
        public List<SymbolInstance> subSymbols = new List<SymbolInstance>();

        [TabGroup("SymbolInstance", "Setup")]
        public RectTransform rectTransform;
        
        [TabGroup("SymbolInstance", "Setup")]
		public Animator animator;

        [TabGroup("SymbolInstance", "Setup")]
        public FSMOwner fsmOwner;

        [TabGroup("SymbolInstance", "Setup")]
		public OverridenSymbolEntity symbol;

		public int value { get { return symbol.value; } set { symbol.value = value; } }
        public SymbolEntity.SymbolAttribute attribute { get { return symbol.attribute; } set { symbol.attribute = value; } }
        public long multiplier { get { return symbol.multiplier; } set { symbol.multiplier = value; } }
        public int columnCount { get { return symbol.columnCount; } set { symbol.columnCount = value; } }
        public int rowCount { get { return symbol.rowCount; } set { symbol.rowCount = value; } }
        public int columnOffset { get { return symbol.columnOffset; } set { symbol.columnOffset = value; } }
        public int rowOffset { get { return symbol.rowOffset; } set { symbol.rowOffset = value; } }

        protected int _x;
        [TabGroup("SymbolInstance", "Setup")]
        [ShowInInspector]
        public int x
        {
            get { return _x; }
            set
            {
                if (_x != value)
                {
                    _x = value;
                }
            }
        }

        protected int _y;
        [TabGroup("SymbolInstance", "Setup")]
        [ShowInInspector]
        public int y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                }
            }
        }

	    [HideInInspector]
		[SerializeField]
		protected int _sortingOrder;
        [TabGroup("SymbolInstance", "Setup")]
		[ShowInInspector]
		public int sortingOrder
		{
			get { return _sortingOrder; }
			set
			{
				if (_sortingOrder != value)
				{
					_sortingOrder = value;
					ChangeSortingOrder();
				}
			}
		}

		[HideInInspector]
		[SerializeField]
		protected int _sortingOffset;
        [TabGroup("SymbolInstance", "Setup")]
		[ShowInInspector]
		public int sortingOffset
		{
			get { return _sortingOffset; }
			set
			{
				if (_sortingOffset != value)
				{
					_sortingOffset = value;
					ChangeSortingOrder();
				}
			}
		}

        public void AddSymbol(SymbolInstance symbolInstance)
        {
            symbolInstance.rectTransform.SetParent(rectTransform, true);
            symbolInstance.rectTransform.SetAsLastSibling();
            subSymbols.Add(symbolInstance);
        }

        [TabGroup("SymbolInstance", "Events")]
        [PropertyOrder(200)]
        public bool optimizeEvents = true;

        [TabGroup("SymbolInstance", "Events")]
        [ShowIf("optimizeEvents")]
        [PropertyOrder(201)]
        public List<string> events = new List<string>{ "Idle", "Win" };

        protected HashSet<string> hashEvents = new HashSet<string>();

        protected virtual void Awake()
        {
            if (optimizeEvents)
            {
                foreach (var evt in events)
                {
                    hashEvents.Add(evt);
                }
            }
        }

        [Button]
        public void Initialize()
        {
            onInitialize.Invoke(this);
        }

        public void SendEvent(string eventName)
		{
            if (optimizeEvents && !hashEvents.Contains(eventName)) return;
			onEvent.Invoke(eventName);
		}

        [Button]
		public void ReturnToPool()
		{
            foreach (var subSymbol in subSymbols)
            {
                subSymbol.ReturnToPool();
            }
            subSymbols.Clear();

            onReturnToPool.Invoke();

			if (origin)
				SymbolInstancePool.ReturnToPool(origin, this);
			else
				gameObject.DestroyThis();
		}

        private void ChangeSortingOrder()
        {
            onChangedSortingOrder.Invoke(_sortingOrder + _sortingOffset);
        }

        [Serializable]
        public class SymbolInstanceEvent : UnityEvent<SymbolInstance> {}
		
        [Space]
        [TabGroup("SymbolInstance", "Events")]
        [PropertyOrder(300)]
        public SymbolInstanceEvent onInitialize;

        [TabGroup("SymbolInstance", "Events")]
		[PropertyOrder(301)]
	    public UnityIntEvent onChangedSortingOrder;

        [TabGroup("SymbolInstance", "Events")]
        [PropertyOrder(302)]
        public UnityStringEvent onEvent;

        [TabGroup("SymbolInstance", "Events")]
        [PropertyOrder(303)]
        public UnityEvent onReturnToPool;

        [Button]
        public void Idle()
        {
            SendEvent("Idle");
        }

        [Button]
        public void Win()
        {
            SendEvent("Win");
        }

        [Button]
        public void Expectation()
        {
            SendEvent("Expectation");
        }

        ////////////////////////////////////////////////////////////////////////////
        /// Animation
        ////////////////////////////////////////////////////////////////////////////
        public void PlayAnimation(string stateName)
        {
            animator.Play(stateName);
        }

        public void PlayAnimation0(string stateName)
        {
            animator.Play(stateName, -1, 0f);
        }

        public void PlayAnimation1(string stateName)
        {
            animator.Play(stateName, -1, 1f);
        }

        public void ResetAndSetTriggerAnimation(string triggerName)
        {
            animator.ResetTrigger(triggerName);
            animator.SetTrigger(triggerName);
        }

        ////////////////////////////////////////////////////////////////////////////
        /// FSM
        ////////////////////////////////////////////////////////////////////////////
        public void EnterState(string stateName)
        {
            fsmOwner.TriggerState(stateName);
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
			if (animator == null) animator = GetComponent<Animator>();
            if (fsmOwner == null) fsmOwner = GetComponent<FSMOwner>();
        }
#endif
    }
}