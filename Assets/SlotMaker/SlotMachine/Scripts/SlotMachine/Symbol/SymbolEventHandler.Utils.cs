using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public partial class SymbolEventHandler : MonoBehaviour
    {
        private Dictionary<int, GameObject> cachedObjects = new Dictionary<int, GameObject>();

        public bool IsCached(int prefabId) => cachedObjects.ContainsKey(prefabId);

        public GameObject GetCachedObject(int prefabId)
        {
            if (IsCached(prefabId))
            {
                var cache = cachedObjects[prefabId];
                return cache;
            }

            var original = symbol.symbolAssets.GetPrefab(symbol.symbolIndex, prefabId);
            GameObject instance = Instantiate(original, Vector3.zero, Quaternion.identity);
            instance.transform.SetParent(cachedObjectsParent, false);
            instance.gameObject.name = original.name;
            cachedObjects.Add(prefabId, instance);
            return instance;
        }

        public void ClearCachedObjects()
        {
            foreach (var cache in cachedObjects)
                Destroy(cache.Value);
            cachedObjects.Clear();
        }

        public void DisableAllCachedObjects()
        {
            foreach (var cache in cachedObjects)
                cache.Value.SetActive(false);
        }
    }
}
