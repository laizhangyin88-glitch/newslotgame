using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using System;
using UnityEngine.Events;

namespace SlotMaker
{
    public abstract partial class SymbolBehaviour : MonoBehaviour
    {
        protected Symbol symbol { get { return eventHandler.symbol; } }
        protected Animator animator { get { return eventHandler.animator; } }

        public bool IsCached(int prefabId) => eventHandler.IsCached(prefabId);

        public GameObject GetCachedObject(int prefabId)
        {
            return eventHandler.GetCachedObject(prefabId);
        }

        public void ClearCachedObjects()
        {
            eventHandler.ClearCachedObjects();
        }

        public void DisableAllCachedObjects()
        {
            eventHandler.DisableAllCachedObjects();
        }

        public void PlayAnimation(string name)
        {
            animator.Play(Animator.StringToHash(name), -1, 0f);
        }

        public void PlayAnimation(int hash)
        {
            animator.Play(hash, -1, 0f);
        }
    }

}