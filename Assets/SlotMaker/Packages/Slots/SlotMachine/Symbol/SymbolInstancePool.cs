using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [ExecuteAlways]
    public class SymbolInstancePool : MonoWeakSingleton<SymbolInstancePool>
    {
        private Dictionary<GameObject, List<SymbolInstance>> caching = new Dictionary<GameObject, List<SymbolInstance>>();

        [Button]
        public static void Clear()
        {
            if (Instance)
            {
                Instance.gameObject.DestroyChildren();
                Instance.caching.Clear();
            }
        }

        public static SymbolInstance GetSymbolInstance(GameObject prefab)
        {
            if (Instance)
            {
                List<SymbolInstance> availiables;
                if (Instance.caching.TryGetValue(prefab, out availiables))
                {
                    int lastAvailiableIndex = availiables.Count - 1;
                    if (lastAvailiableIndex >= 0)
                    {
                        var symbolInstance = availiables[lastAvailiableIndex];
                        availiables.RemoveAt(lastAvailiableIndex);
                        return symbolInstance;
                    }
                }
            }
            return CreateSymbolInstance(prefab);
        }

        public static SymbolInstance CreateSymbolInstance(GameObject prefab)
        {
            var go = Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            go.SetActive(false);
            var symbolInstance = go.GetComponent<SymbolInstance>();
            symbolInstance.origin = prefab;
            return symbolInstance;
        }

        public static void ReturnToPool(GameObject origin, SymbolInstance symbolInstance)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                symbolInstance.gameObject.DestroyThis();
                return;    
            }
#endif
            if (Instance)
            {
                symbolInstance.transform.SetParent(Instance.transform, false);
                symbolInstance.gameObject.SetActive(false);

                List<SymbolInstance> availiables;
                if (!Instance.caching.TryGetValue(origin, out availiables))
                {
                    availiables = new List<SymbolInstance>();
                    Instance.caching[origin] = availiables;
                }
                availiables.Add(symbolInstance);
            }
            else
            {
                symbolInstance.gameObject.DestroyThis();
            }
        }
    }
}