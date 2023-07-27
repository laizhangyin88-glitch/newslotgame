using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BagelCode
{
    public class MetaObjectPoolerManager : MonoBehaviour
    {
        [SerializeField]
        private List<PrefabInfo> prefabInfoList = new List<PrefabInfo>();

        [Serializable]
        public class PrefabInfo
        {
            public PrefabInfo(GameObject _prefab, string _key, int _prePoolingCount = 0)
            {
                prefab = _prefab;
                key = _key;
                prePoolingCount = _prePoolingCount;
            }
            public GameObject prefab;
            public string key;
            public int prePoolingCount; // 실행 시점에 count 만큼 생성
        }

        private static Dictionary<string, MetaObjectPooler> poolerDict = new Dictionary<string, MetaObjectPooler>();

        private void Start()
        {
            InitializePoolerManager();
        }

        public bool AssignPrefab(GameObject prefab, string key, int count = 0)
        {
            if (!IsValid(prefab) || !IsValidToAssign(key))
                return false;

            var info = new PrefabInfo(prefab, key, count);
            prefabInfoList.Add(info);

            var pooler = new MetaObjectPooler(prefab);
            poolerDict.Add(key, pooler);

            if(count > 0)
            {
                pooler.AddInactiveInstance(count);
            }

            return true;
        }

        public GameObject GetInstanceRequest(string key)
        {
            if (IsValidToIndexing(key))
                return poolerDict[key]?.GetInstance();

            return null;
        }

        public List<GameObject> GetInstanceRequest(string key, int count)
        {
            if(IsValidToIndexing(key) && count > 0)
                return poolerDict[key]?.GetInstance(count);

            return null;
        }

        //

        private void InitializePoolerManager()
        {
            foreach (var info in prefabInfoList)
            {
                if (IsValid(info.prefab) && IsValidToAssign(info.key))
                {
                    // Make Pooler
                    var pooler = new MetaObjectPooler(info.prefab);
                    poolerDict.Add(info.key, pooler);

                    // Instantiate PrePooling Objects
                    if (info.prePoolingCount > 0)
                    {
                        pooler.AddInactiveInstance(info.prePoolingCount);
                    }
                }
            }
        }

        //

        private static bool IsValid(GameObject prefab)
        {
            if(prefab is null)
            {
                Debug.LogWarning("MetaObjectPoolerManager warning. prefab is null.");
                return false;
            }

            return true;
        }

        private static bool IsValidToAssign(string key)
        {
            if(string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("MetaObjectPoolerManager warning. key is null or empty.");
                return false;
            }
            else if(poolerDict.ContainsKey(key))
            {
                Debug.LogWarning("MetaObjectPoolerManager warning. key is already used.");
                return false;
            }

            return true;
        }

        private static bool IsValidToIndexing(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("MetaObjectPoolerManager warning. key is null or empty.");
                return false;
            }
            else if (!poolerDict.ContainsKey(key))
            {
                Debug.LogWarning("MetaObjectPoolerManager warning. key is not contained in poolerDict.");
                return false;
            }
            else if(poolerDict[key] == null)
            {
                Debug.LogWarning("MetaObjectPoolerManager warning. poolerDict[key] is null.");
                return false;
            }

            return true;
        }
    }
}