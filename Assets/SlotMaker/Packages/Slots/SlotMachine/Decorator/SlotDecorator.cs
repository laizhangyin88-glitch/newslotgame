using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class SlotDecorator : MonoBehaviour
    {
        [InlineEditor]
        public SlotMediator slot;
        public Transform content;
        public ObjectPool pool;

        [NonSerialized]
        [ShowInInspector]
        public List<PooledObject> caching = new List<PooledObject>();

        public void AddToSlot()
        {
            var pooledObject = pool.GetObject();
            pooledObject.transform.SetParent(content, false);
            pooledObject.transform.position = slot.GetSlotPosition();
            caching.Add(pooledObject);
        }

        public void AddToReelByIndex(int index)
        {
            var pooledObject = pool.GetObject();
            pooledObject.transform.SetParent(content, false);
            pooledObject.transform.position = slot.GetReelPositionByIndex(index, 0);
            caching.Add(pooledObject);
        }

        public void AddToReel(Cell3 spot)
        {
            var pooledObject = pool.GetObject();
            pooledObject.transform.SetParent(content, false);
            pooledObject.transform.position = slot.GetReelPosition(spot.x, spot.y, 0);
            caching.Add(pooledObject);
        }

        public void AddToSymbol(Cell3 spot)
        {
            var pooledObject = pool.GetObject();
            pooledObject.transform.SetParent(content, false);
            pooledObject.transform.position = slot.GetSymbolPosition(spot.x, spot.y, 0);
            caching.Add(pooledObject);
        }

        public void AddToSymbols(List<Cell3> spots)
        {
            foreach (var spot in spots)
            {
                AddToSymbol(spot);
            }
        }

        public void AddToSymbols(List<List<Cell3>> spots)
        {
            foreach (var spot in spots)
            {
                AddToSymbols(spot);
            }
        }

        [Button]
        public void Clear()
        {
            foreach (var cache in caching)
            {
                cache.ReturnToPool();
            }
            caching.Clear();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (content == null) content = GetComponent<RectTransform>();
            if (pool == null) pool = GetComponent<ObjectPool>();
        }
#endif
    }
}