using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    // List는 앞부분에 inactive, 뒷부분에 active 한 오브젝트가 놓인 상태가 유지된다.
    // Inactive 상태로 미리 잔뜩 만들어둔 다음 필요할 때 마다 active 해서 가져오는 식으로 사용 가능
    // ex) □□□□□□□■■■ : 7 inactived objects, 3 actived objects

    // 사용이 끝난 오브젝트는 destroy 하거나 deactive 하는 경우 재사용이 불가능함
    // 대신 ReleaseInstance 를 호출하여 반환시켜야 재사용 가능. 대신 동작이 조금 느린 문제(IndexOf 사용으로 N(O))가 있어서 고민 중
    public class MetaObjectPooler
    {
        private GameObject prefab;

        public int InstanceCount => instanceList.Count;
        public int InactivedCount => InstanceCount - ActivedCount;
        public int ActivedCount { get; private set; }

        private List<GameObject> instanceList = new List<GameObject>();
        
        public MetaObjectPooler(GameObject _prefab)
        {
            if(_prefab is null)
            {
                Debug.LogWarning("MetaObjectPooler.AssignPrefab failure. The prefab is null.");
                return;
            }

            prefab = _prefab;
            prefab.SetActive(false);

            var controller = GetOrAddController(prefab);
            controller.InitPrefab(this);
        }

        public GameObject GetInstance()
        {
            return GetInstance(1)?.Pop();
        }

        public List<GameObject> GetInstance(int count)
        {
            if (count < 1) return null;

            if (prefab is null)
            {
                Debug.LogWarning("MetaObjectPooler.Instantiate failure. The prefab is null.");
                return null;
            }

            // Create New
            int newCount = System.Math.Max(count - InactivedCount, 0);
            var newInstanceList = AddInstance(newCount, true);

            // Recycle
            int recycleCount = count - newCount;
            if (recycleCount > 0)
            {
                var targets = instanceList.GetRange(InactivedCount - recycleCount, recycleCount);
                for (int i = 0; i < recycleCount; ++i)
                    InitMetaObject(targets[i], true);

                newInstanceList.AddRange(targets);
            }

            return newInstanceList;
        }

        public void AddInactiveInstance(int count)
        {
            AddInstance(count, false);
        }

        public void ReleaseInstance(List<GameObject> list)
        {
            list.ForEach(o => ReleaseInstance(o));
        }

        public void ReleaseInstance(GameObject obj)
        {
            var controller = GetOrAddController(obj);
            if (!controller.Actived) return;

            int objIdx = instanceList.IndexOf(obj);

            if (!instanceList.IsValidIndex(objIdx))
                return;

            instanceList.Swap(objIdx, InactivedCount);
            obj.SetActive(false);

            controller.Deactivate();
            --ActivedCount;
        }

        public void OnDestroyInstance(GameObject obj)
        {
            bool removed = instanceList.Remove(obj);
            if (!removed) return;

            var controller = GetOrAddController(obj);
            if (controller.IsPrefab)
            {
                Debug.LogWarning("MetaObjectPooler warning. prefab is destroyed. name: " + obj.name);
                return;
            }

            if (controller.Actived) --ActivedCount;
        }

        //

        private MetaObjectController GetOrAddController(GameObject obj)
        {
            var controller = obj.GetComponent<MetaObjectController>();
            if (controller == null)
                controller = obj.AddComponent<MetaObjectController>();
            return controller;
        }

        private List<GameObject> AddInstance(int count, bool doActive)
        {
            if (count < 1) return new List<GameObject>();

            if (doActive) ActivedCount += count;

            var newInstanceList = new List<GameObject>();
            for (int i = 0; i < count; ++i)
            {
                var obj = Object.Instantiate(prefab);
                InitMetaObject(obj, doActive);
                newInstanceList.Add(obj);

#if UNITY_EDITOR
                obj.name += string.Format(" {0}", instanceList.Count + newInstanceList.Count);
#endif
            }

            if (doActive)
            {
                instanceList.AddRange(newInstanceList);
            }
            else
            {
                instanceList.InsertRange(InactivedCount, newInstanceList);
            }

            return newInstanceList;
        }

        private void InitMetaObject(GameObject target, bool doActive)
        {
            if (target is null) return;

            var controller = GetOrAddController(target);
            controller.InitPooledObject(this, doActive);

            target.SetActive(doActive);
        }
    }
}