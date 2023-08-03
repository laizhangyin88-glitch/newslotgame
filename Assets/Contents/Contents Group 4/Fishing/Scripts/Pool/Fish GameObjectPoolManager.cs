using BagelCode.Tasks.Actions.ClientAPI;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public enum PoolType
    {
        FishPool = 0,
        BulletPool = 1,
        NetPool = 2,
        GoldPool = 3,
        ScorePool = 4,
        PlusTips = 5,
        EffectPool = 6,
        FishOutTipsPool = 7,
        SpecialDeclarePool = 8,
        LightningPool = 9,
        TipsContent = 10,
    }
    public class FishGameObjectPoolManager : MonoSingleton<FishGameObjectPoolManager>
    {
        FishGameData gameData;
        GameObject _gameObject;
        FishPoolPanel PoolPanel;
        FishObjectPool ObjectPool;
        public List<GameObject> PoolList;
        private void Awake()
        {
            InitData();
            InitView();
            InitInstance();
            InitViewData();
        }

        private void InitData()
        {
            gameData = FishGameManager.Instance.gameData;
        }
        private void InitView()
        {
            _gameObject = FishGameManager.Instance.contentGameObject.transform.Find("FishPanel/GameObjectPool").gameObject;
        }

        private void InitInstance()
        {
            PoolPanel = new FishPoolPanel(_gameObject);
            ObjectPool = new FishObjectPool();
        }

        private void InitViewData()
        {
            PoolList = PoolPanel.PoolList;
        }

        public GameObject GetPoolParent(PoolType poolType)
        {
            if ((int)poolType > 0 && (int)poolType < PoolList.Count)
                return PoolList[(int)poolType];
            return null;
        }

        public GameObject GetPoolFishRootObj()
        {
            return PoolPanel.ShakeCamera.gameObject;
        }

        public void AddGameObjectPool(GameObject gameObj, int number, string keyName, PoolType poolType)
        {
            ObjectPool.AddObjectPool(gameObj, number, keyName, poolType);
        }

        public void SetPoolParent(GameObject gameObj, PoolType poolType)
        {
            ObjectPool.SetPoolParent(gameObj, poolType);
        }

        public GameObject GetGameObject(string gameObjNmae, PoolType poolType)
        {
            return ObjectPool.GetGameObject(gameObjNmae, poolType);
        }

        public void ReCycleToGameObject(GameObject gameObj, PoolType poolType)
        {
            ObjectPool.ReCycleToGameObject(gameObj, poolType);
        }

        protected override void OnDestroy()
        {

        }
    }
}
