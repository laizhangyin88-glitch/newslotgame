using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    partial class SymbolEventHandler : MonoBehaviour
    {
        public Symbol symbol;
        public Animator animator;
        public Transform behavioursParent;
        public Transform cachedObjectsParent;

        private SymbolBehaviour currentBehaviour;
        protected SymbolBehaviour CurrentBehaviour
        {
            get
            {
                if (!currentBehaviour)
                {
                    SymbolBehaviour original = symbol.symbolAssets.GetSymbolBehaviour(symbol.symbolInfo.symbol);
                    if (original)
                    {
                        currentBehaviour = InstantiateBehaviour(original);
                        currentBehaviour.gameObject.SetActive(true);
                        currentBehaviour.StartBehaviour(this);
                    }
                }
                return currentBehaviour;
            }
        }

        [Serializable]
        public class DictionarySymbolBehaviour : SerializedDictionary<SymbolBehaviour, SymbolBehaviour>{}

        public DictionarySymbolBehaviour behaviours = new DictionarySymbolBehaviour();
        protected SymbolBehaviour InstantiateBehaviour(SymbolBehaviour original)
        {
            SymbolBehaviour instance = null;
            if (!behaviours.TryGetValue(original, out instance))
            {
                instance = Instantiate(original, Vector3.zero, Quaternion.identity);
                instance.transform.SetParent(behavioursParent, false);
                instance.gameObject.name = original.name;
                instance.gameObject.SetActive(false);
                behaviours.Add(original, instance);
            }
            return instance;
        }

        protected const string SKIP_ANIMATION_NAME = "Skip";
        protected const string ENTRY_ANIMATION_NAME = "Entry";

        protected void EnterState(string stateName)
        {
            CurrentBehaviour?.ExecuteStateMethod(stateName);
        }

        public void Play(BaseSymbol symbol, string animationName)
        {
            EnterState(animationName);
        }

        public void Play(string animationName)
        {
            EnterState(animationName);
        }

        public void Skip(BaseSymbol symbol)
        {
            if (currentBehaviour == null)
                return;

            EnterState(SKIP_ANIMATION_NAME);
        }

        public virtual void Apply(BaseSymbol symbol)
        {
            EnterState(ENTRY_ANIMATION_NAME);
        }

        public virtual void Clear(BaseSymbol symbol)
        {
            if (currentBehaviour != null)
            {
                ClearCachedObjects();
                currentBehaviour.StopBehaviour();
                currentBehaviour.gameObject.SetActive(false);
                currentBehaviour = null;
            }
        }
        public virtual void Change(BaseSymbol symbol) { }
        public virtual void Restore(BaseSymbol target, BaseSymbol source) { }
    }
}
