using System;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    [RequireComponent(typeof(PooledObject))]
    public class Symbol : BaseSymbol
    {
        public RectTransform _anchor;

        public override RectTransform anchor
        { get { return _anchor; } }

        [Serializable]
        public class UnitySymbolEvent : UnityEvent<BaseSymbol>
        { }

        [Serializable]
        public class UnitySymbolAnimationEvent : UnityEvent<BaseSymbol, string>
        { }

        [Serializable]
        public class UnitySymbolRestoreEvent : UnityEvent<BaseSymbol, BaseSymbol>
        { }

        public UnitySymbolEvent onClear;
        public UnitySymbolEvent onChange;
        public UnitySymbolEvent onApply;
        public UnitySymbolRestoreEvent onRestore;
        public UnitySymbolAnimationEvent onPlay;
        public UnitySymbolEvent onSkip;

        private PooledObject _pooledObject;

        protected PooledObject pooledObject
        { get { return _pooledObject ?? (_pooledObject = GetComponent<PooledObject>()); } }

        public override void Clear()
        {
            base.Clear();

            pooledObject.ReturnToPool();
        }

        protected override void OnClear()
        {
            onClear.Invoke(this);
        }

        protected override void OnChange()
        {
            onChange.Invoke(this);
        }

        protected override void OnApply()
        {
            onApply.Invoke(this);
        }

        protected override void OnRestore(BaseSymbol src)
        {
            onRestore.Invoke(this, src);
        }

        protected override void OnPlay(string animationName)
        {
            onPlay.Invoke(this, animationName);
        }

        protected override void OnSkip()
        {
            onSkip.Invoke(this);
        }
    }
}
