using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishObjectPool
    {
        private Dictionary<string, GameObject> PrototypeObjectList;
        private Dictionary<string, List<GameObject>> ObjectPoolList;
        private int InitCount;
        public FishObjectPool()
        {
            InitData();
        }

        private void InitData()
        {
            PrototypeObjectList = new Dictionary<string, GameObject>();
            ObjectPoolList = new Dictionary<string, List<GameObject>>();
            InitCount = 10;
        }

        public void RegisterPrototype(GameObject gameObj, string keyName, PoolType poolType)
        {
            if (!FindIsHaveKey(keyName, PrototypeObjectList))
            {
                GameObject tempObj = GameObject.Instantiate(gameObj);
                tempObj.name = keyName;
                tempObj.SetActive(false);
                SetPoolParent(tempObj, poolType);
                PrototypeObjectList[keyName] = tempObj;
            }
            else
            {
                //Debug.Log("已经存在原型对象" + keyName);
            }
        }

        public void AddObjectPool(GameObject gameObj, int number, string keyName, PoolType poolType)
        {
            if (keyName == null)
            {
                keyName = gameObj.name;
            }
            if (!FindIsHaveKey(keyName, ObjectPoolList))
            {
                ObjectPoolList[keyName] = new List<GameObject>();
            }

            RegisterPrototype(gameObj, keyName, poolType);

            for (int i = 0; i < number; i++)
            {
                GameObject tempObj = Object.Instantiate(gameObj);
                tempObj.name = keyName;
                tempObj.SetActive(false);
                SetPoolParent(tempObj, poolType);
                ObjectPoolList[keyName].Add(tempObj);
            }
        }

        public void RemoveKeyAtObjectPool(string keyName)
        {
            ObjectPoolList.Remove(keyName);
        }

        public void SetPoolParent(GameObject gameObj, PoolType poolType)
        {
            GameObject parentObj = FishGameObjectPoolManager.Instance.PoolList[(int)poolType];
            if (parentObj != null)
            {
                if (gameObj.transform.parent != null)
                {
                    if (gameObj.transform.parent.name == parentObj.name)
                    {
                        return;
                    }
                }
                gameObj.transform.SetParent(parentObj.transform, false);
                gameObj.transform.localPosition = new Vector3(0, 5000, 0);
                gameObj.transform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogError("创建Pool失败poolType==>" + poolType);
            }
        }

        public GameObject GetGameObject(string gameObjName, PoolType poolType)
        {
            if (FindIsHaveKey(gameObjName, ObjectPoolList))
            {
                GameObject tempObj = GetActiveGameObject(gameObjName);
                if (tempObj != null)
                {
                    return tempObj;
                }
                else
                {
                    //Debug.LogError("keyName值不够使用,继续生成==>" + gameObjName);
                    GameObject tempPoolObj = PrototypeObjectList[gameObjName];
                    if (tempPoolObj == null)
                    {
                        Debug.LogError(gameObjName + "对象池原型列表中并不存在");
                        return null;
                    }
                    else
                    {
                        AddObjectPool(tempPoolObj, InitCount, gameObjName, poolType);
                        return GetActiveGameObject(gameObjName);
                    }
                }
            }
            return null;
        }

        private bool FindIsHaveKey(string keyName, Dictionary<string, GameObject> targetPool)
        {
            return targetPool.ContainsKey(keyName);
        }

        private bool FindIsHaveKey(string keyName, Dictionary<string, List<GameObject>> targetPool)
        {
            return targetPool.ContainsKey(keyName);
        }

        public void ReCycleToGameObject(GameObject gameObj, PoolType poolType)
        {
            gameObj.SetActive(false);
            SetPoolParent(gameObj, poolType);
            ObjectPoolList[gameObj.name].Add(gameObj);
        }

        private GameObject GetActiveGameObject(string keyName)
        {
            for (int i = 0; i < ObjectPoolList[keyName].Count; i++)
            {
                if (ObjectPoolList[keyName][i].activeSelf == false)
                {
                    ObjectPoolList[keyName][i].SetActive(true);
                    GameObject go = ObjectPoolList[keyName][i];
                    ObjectPoolList[keyName].RemoveAt(i);
                    return go;
                }
            }
            return null;
        }
    }
}

