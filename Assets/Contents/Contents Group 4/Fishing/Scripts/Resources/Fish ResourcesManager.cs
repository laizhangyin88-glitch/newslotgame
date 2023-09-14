using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace BagelCode
{
    public class FishResourcesManager : MonoSingleton<FishResourcesManager>
    {
        private FishGameData gameData;
        private int ResourcesTotalCount = 0;
        private int CurrentLoadedResourcesTotalCount = 0;
        private Dictionary<string,GameObject> FishPackResList = new Dictionary<string, GameObject>();
        Queue<IEnumerator> enumerators = new Queue<IEnumerator>();


        /// <summary>
        /// 资源的设置信息
        /// </summary>
        private FishResourcePackerInfoSet m_resourcePackerInfoSet;
        /// <summary>
        /// 缓存资源映射
        /// </summary>
        private Dictionary<int, FishResourceBase> m_cachedResourceMap;

        private void Awake()
        {
            InitInstance();
        }

        public void InitInstance()
        {
            InitData();
            CalculateTotalResourcesCount();
        }

        private void InitData()
        {
            gameData = FishGameManager.Instance.gameData;
        }

        public void CalculateTotalResourcesCount()
        {
            int totalCount = 0;
            var gameConfig = gameData.GameConfig;
            totalCount += 1;
            totalCount += gameConfig.FishConfig.Length;
            totalCount += gameConfig.FishConfigJson.Length;
            totalCount += gameConfig.FishPackRes.Length;
            totalCount += gameConfig.FishRes.Length;
            totalCount += gameConfig.BulletRes.Length;
            totalCount += gameConfig.NetRes.Length;
            totalCount += gameConfig.PlayerRes.Length;
            totalCount += gameConfig.GoldRes.Length;
            totalCount += gameConfig.ScoreRes.Length;
            totalCount += gameConfig.PlusTipsRes.Length;
            totalCount += gameConfig.SpecialDeclareRes.Length;
            totalCount += gameConfig.LightningRes.Length;
            totalCount += gameConfig.EffectRes.Length;
            totalCount += gameConfig.SkillRes.Length;
            totalCount += gameConfig.FishOutTipsRes.Length;
            totalCount += gameConfig.AudioRes.Length;
            totalCount += gameConfig.TipsContentRes.Length;
            //totalCount += gameConfig.GameSetRes.Count;
            //atlasCount += gameConfig.GameSetRes.Count;

            ResourcesTotalCount = totalCount;
        }

        public void LoadProgressBarEvent()
        {
            CurrentLoadedResourcesTotalCount++;
            string slider = string.Format("%.3f", CurrentLoadedResourcesTotalCount / ResourcesTotalCount);
            if (CurrentLoadedResourcesTotalCount == ResourcesTotalCount)
            {
                LoadResCompleteCallBack();
            }
        }

        private void LoadResCompleteCallBack()
        {
            FishGameManager.Instance.LoadResourcesCompleteCallBack();
        }

        public void CorotineLoadResources()
        {
            enumerators.Enqueue(InitAddProtoBuffer());
            enumerators.Enqueue(InitGameConfig());
            enumerators.Enqueue(InitGameGroup());
            enumerators.Enqueue(InitFishResources());
            enumerators.Enqueue(CreateFishPool());
            enumerators.Enqueue(CreateBulletPool());
            enumerators.Enqueue(CreatePlayerPrefab());
            enumerators.Enqueue(CreateNetPool());
            enumerators.Enqueue(CreateGoldPool());
            enumerators.Enqueue(CreateScorePool());
            enumerators.Enqueue(CreatePlusTipsPool());
            //enumerators.Enqueue(CreateGameSetPanel());
            enumerators.Enqueue(CreateAudio());
            enumerators.Enqueue(CreateLightningPool());
            enumerators.Enqueue(CreateEffectPool());
            enumerators.Enqueue(CreateSkillPool());
            enumerators.Enqueue(CreateFishOutTipsPool());
            enumerators.Enqueue(CreateSpecialDeclarePool());
            enumerators.Enqueue(CreateTipsContentPool());

            ExcuteLoadResourcesQueue();

        }

        private void ExcuteLoadResourcesQueue()
        {
            if (enumerators.Count > 0)
            {
                IEnumerator action = enumerators.Dequeue();
                StartCoroutine(action);
            }
        }

        private IEnumerator InitAddProtoBuffer()
        {
            yield return null;
            for (int i = 0; i < gameData.GameConfig.FishConfig.Length; i++)
            {
                FishGameManager.Instance.LoadGameProtoBufferFile(gameData.GameConfig.FishConfig[i]);
                LoadProgressBarEvent();
            }
            ExcuteLoadResourcesQueue();
        }

        private IEnumerator InitGameConfig()
        {
            yield return 0;
            ConfigFile[] fishConfig = gameData.GameConfig.FishConfigJson;

            for (int i = 0; i < fishConfig.Length; i++)
            {
                string path = Application.streamingAssetsPath + "/fishConfig" + fishConfig[i].path;

                string json = null;

#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("文件不存在: " + path);
                Debug.LogError(request.error);
            }
            else
            {
                Debug.LogError("json读取成功: " + path);
                json = request.downloadHandler.text;
            }
#else
                json = File.ReadAllText(path);
#endif

                json = json.Replace("\n", "");
                switch (i)
                {
                    case 0:
                        FishFishConfigList temp0 = JsonUtility.FromJson<FishFishConfigList>(json);
                        gameData.FishConfigList = AlignGameConfig(temp0.configList);
                        break;
                    case 1:
                        FishCoinEffectConfigList temp1 = JsonUtility.FromJson<FishCoinEffectConfigList>(json);
                        gameData.CoinEffectConfigList = AlignGameConfig(temp1.configList);
                        break;
                    case 2:
                        FishDieEffectConfigList temp2 = JsonUtility.FromJson<FishDieEffectConfigList>(json);
                        gameData.DieEffectConfigList = AlignGameConfig(temp2.configList);
                        break;
                    case 3:
                        FishGunConfigList temp3 = JsonUtility.FromJson<FishGunConfigList>(json);
                        gameData.GunConfigList = temp3.configList;
                        break;
                    case 4:
                        FishRoomConfigList temp4 = JsonUtility.FromJson<FishRoomConfigList>(json);
                        gameData.RommConfigList = temp4.configList;
                        break;
                    case 5:
                        FishScoreEffectConfigList temp5 = JsonUtility.FromJson<FishScoreEffectConfigList>(json);
                        gameData.ScoreEffectConfigList = AlignGameConfig(temp5.configList);
                        break;
                    case 6:
                        FishSoundConfigList temp6 = JsonUtility.FromJson<FishSoundConfigList>(json);
                        gameData.SoundConfigList = temp6.configList;
                        break;
                    case 7:
                        FishPlusTipsEffectConfigList temp7 = JsonUtility.FromJson<FishPlusTipsEffectConfigList>(json);
                        gameData.PlusTipsEffectConfigList = AlignGameConfig(temp7.configList);
                        break;
                    case 8:
                        FishSpecialDeclareConfigList temp8 = JsonUtility.FromJson<FishSpecialDeclareConfigList>(json);
                        gameData.SpecialDeclareConfigList = AlignGameConfig(temp8.configList);
                        break;
                    default:
                        break;
                }
                LoadProgressBarEvent();
            }
            ExcuteLoadResourcesQueue();
        }

        private Dictionary<int, FishFishConfig> AlignGameConfig(List<FishFishConfig> gameConfig)
        {
            Dictionary<int, FishFishConfig> tempList = new Dictionary<int, FishFishConfig>();
            for (int i = 0; i < gameConfig.Count; i++)
            {
                int id = gameConfig[i].id;
                tempList[id] = gameConfig[i];
            }
            return tempList;
        }
        private Dictionary<int, FishCoinEffectConfig> AlignGameConfig(List<FishCoinEffectConfig> gameConfig)
        {
            Dictionary<int, FishCoinEffectConfig> tempList = new Dictionary<int, FishCoinEffectConfig>();
            for (int i = 0; i < gameConfig.Count; i++)
            {
                int id = gameConfig[i].id;
                tempList[id] = gameConfig[i];
            }
            return tempList;
        }
        private Dictionary<int, FishDieEffectConfig> AlignGameConfig(List<FishDieEffectConfig> gameConfig)
        {
            Dictionary<int, FishDieEffectConfig> tempList = new Dictionary<int, FishDieEffectConfig>();
            for (int i = 0; i < gameConfig.Count; i++)
            {
                int id = gameConfig[i].id;
                tempList[id] = gameConfig[i];
            }
            return tempList;
        }
        private Dictionary<int, FishScoreEffectConfig> AlignGameConfig(List<FishScoreEffectConfig> gameConfig)
        {
            Dictionary<int, FishScoreEffectConfig> tempList = new Dictionary<int, FishScoreEffectConfig>();
            for (int i = 0; i < gameConfig.Count; i++)
            {
                int id = gameConfig[i].id;
                tempList[id] = gameConfig[i];
            }
            return tempList;
        }

        private Dictionary<int, FishPlusTipsEffectConfig> AlignGameConfig(List<FishPlusTipsEffectConfig> gameConfig)
        {
            Dictionary<int, FishPlusTipsEffectConfig> tempList = new Dictionary<int, FishPlusTipsEffectConfig>();
            for (int i = 0; i < gameConfig.Count; i++)
            {
                int id = gameConfig[i].id;
                tempList[id] = gameConfig[i];
            }
            return tempList;
        }

        private Dictionary<int, FishSpecialDeclareConfig> AlignGameConfig(List<FishSpecialDeclareConfig> gameConfig)
        {
            Dictionary<int, FishSpecialDeclareConfig> tempList = new Dictionary<int, FishSpecialDeclareConfig>();
            for (int i = 0; i < gameConfig.Count; i++)
            {
                int id = gameConfig[i].id;
                tempList[id] = gameConfig[i];
            }
            return tempList;
        }

        private IEnumerator InitGameGroup()
        {
            yield return 0;
            FishGameManager.Instance.InitGameGroup();
            LoadProgressBarEvent();
            ExcuteLoadResourcesQueue();
        }

        private IEnumerator InitFishResources()
        {
            Debug.LogError("call InitFishResources");
            int totalCount = gameData.GameConfig.FishPackRes.Length;
            int count = 0;
            if (totalCount > 0)
            {
                for (int i = 0; i < gameData.GameConfig.FishPackRes.Length; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.FishPackRes[i].name);
                    if (gameObject != null)
                    {
                        if (gameObject != null)
                        {
                            FishPackResList[gameData.GameConfig.FishPackRes[i].name] = Instantiate(gameObject);
                            count++;
                            LoadProgressBarEvent();
                            if (count == totalCount)
                                ExcuteLoadResourcesQueue();
                        }
                        else
                            Debug.LogError("资源加载失败==> " + gameData.GameConfig.FishPackRes[i].name);
                    }
                }
            }
        }

        private IEnumerator CreateFishPool()
        {
            int totalCount = gameData.GameConfig.FishRes.Length;
            if (totalCount > 0)
            {
                for (int i = 0; i < gameData.GameConfig.FishRes.Length; i++)
                {
                    yield return 0;

                    Fish v = gameData.GameConfig.FishRes[i];
                    GameObject parentPrefab = FishPackResList[v.ParentName];
                    GameObject prefab = parentPrefab.transform.Find(v.name).gameObject;
                    FishGameObjectPoolManager.Instance.AddGameObjectPool(prefab, v.amout, v.name, PoolType.FishPool);
                    LoadProgressBarEvent();
                }
            }
            ExcuteLoadResourcesQueue();
        }

        private IEnumerator CreateBulletPool()
        {
            int totalCount = gameData.GameConfig.BulletRes.Length;
            if (totalCount > 0)
            {
                foreach (var v in gameData.GameConfig.BulletRes)
                {
                    yield return null;
                    var parentPrefab = FishPackResList[v.ParentName];
                    var prefab = parentPrefab.transform.Find(v.name).gameObject;
                    FishGameObjectPoolManager.Instance.AddGameObjectPool(prefab, v.amout, v.name, PoolType.BulletPool);
                    LoadProgressBarEvent();
                }
            }
            DeleteAllFishPackResources();
            ExcuteLoadResourcesQueue();
        }

        private void DeleteAllFishPackResources()
        {
            foreach (var item in FishPackResList)
                Destroy(item.Value);
            FishPackResList = null;
        }

        public IEnumerator CreatePlayerPrefab()
        {
            int totalCount = gameData.GameConfig.PlayerRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.PlayerRes[i].name);
                    if (gameObject != null)
                    {
                        if (gameObject != null)
                        {
                            LoadProgressBarEvent();
                            GameObject tempPlayer = Instantiate(gameObject);
                            tempPlayer.transform.localPosition = Vector3.zero;
                            tempPlayer.transform.localScale = Vector3.one;
                            FishPlayerManager.Instance.InitPlayerInfo(i, tempPlayer);
                            count--;
                            if (count <= 0)
                                ExcuteLoadResourcesQueue();
                        }
                        else
                            Debug.LogError("资源加载失败==> " + gameData.GameConfig.PlayerRes[i].name);
                    }
                }
            }
        }

        public IEnumerator CreateNetPool()
        {
            int totalCount = gameData.GameConfig.NetRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.NetRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempNet = Instantiate(gameObject);
                        tempNet.transform.localPosition = Vector3.zero;
                        tempNet.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempNet, gameData.GameConfig.NetRes[i].amout, gameData.GameConfig.NetRes[i].name, PoolType.NetPool);
                        Destroy(tempNet);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.NetRes[i].path);
                }
            }
        }

        public IEnumerator CreateGoldPool()
        {
            int totalCount = gameData.GameConfig.GoldRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.GoldRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempGold = Instantiate(gameObject);
                        tempGold.transform.localPosition = Vector3.zero;
                        tempGold.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempGold, gameData.GameConfig.GoldRes[i].amout, gameData.GameConfig.GoldRes[i].name, PoolType.GoldPool);
                        Destroy(tempGold);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.GoldRes[i].name);
                }
            }
        }
        
        public IEnumerator CreateScorePool()
        {
            int totalCount = gameData.GameConfig.ScoreRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.ScoreRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempScore = Instantiate(gameObject);
                        tempScore.transform.localPosition = Vector3.zero;
                        tempScore.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempScore, gameData.GameConfig.ScoreRes[i].amout, gameData.GameConfig.ScoreRes[i].name, PoolType.ScorePool);
                        Destroy(tempScore);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.ScoreRes[i].name);
                }
            }
        }
        
        public IEnumerator CreatePlusTipsPool()
        {
            int totalCount = gameData.GameConfig.PlusTipsRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.PlusTipsRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempScore = Instantiate(gameObject);
                        tempScore.transform.localPosition = Vector3.zero;
                        tempScore.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempScore, gameData.GameConfig.PlusTipsRes[i].amout, gameData.GameConfig.PlusTipsRes[i].name, PoolType.ScorePool);
                        Destroy(tempScore);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.PlusTipsRes[i].name);
                }
            }
        }

        //public IEnumerator CreateGameSetPanel()
        //{
        //    yield return 0;
        //}

        //todo
        public IEnumerator CreateAudio()
        {
            int totalCount = gameData.GameConfig.AudioRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.AudioRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject prefab = Instantiate(gameObject);
                        FishAudioManager.Instance.Init(prefab);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 1)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.AudioRes[i].name);
                }
            }
        }

        public IEnumerator CreateLightningPool()
        {
            int totalCount = gameData.GameConfig.LightningRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.LightningRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempLightning = Instantiate(gameObject);
                        tempLightning.transform.localPosition = Vector3.zero;
                        tempLightning.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempLightning, gameData.GameConfig.LightningRes[i].amout, gameData.GameConfig.LightningRes[i].name, PoolType.LightningPool);
                        Destroy(tempLightning);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.LightningRes[i].name);


                    //Action<FishResourceBase, float> CreateEffectPoolCallBackFunc = (gameObj, time) =>
                    //{
                    //    if (gameObj != null && gameObj.content != null)
                    //    {
                    //        GameObject tempLightning = (GameObject)Instantiate(gameObj.content);
                    //        tempLightning.transform.localPosition = Vector3.zero;
                    //        tempLightning.transform.localScale = Vector3.one;
                    //        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempLightning, gameData.GameConfig.LightningRes[i].amout, gameData.GameConfig.LightningRes[i].name, PoolType.LightningPool);
                    //        Destroy(tempLightning);
                    //        LoadProgressBarEvent();
                    //        count--;
                    //        if (count <= 0)
                    //            ExcuteLoadResourcesQueue();
                    //    }
                    //    else
                    //        Debug.LogError("资源加载失败==> " + gameData.GameConfig.LightningRes[i].path);
                    //};
                    //yield return 0;
                    //FishGameManager.Instance.AsyncLoadResource(gameData.GameConfig.LightningRes[i].path, typeof(GameObject), CreateEffectPoolCallBackFunc);
                }
            }
        }

        public IEnumerator CreateEffectPool()
        {
            int totalCount = gameData.GameConfig.EffectRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.EffectRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempEffect = Instantiate(gameObject);
                        tempEffect.transform.localPosition = Vector3.zero;
                        tempEffect.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempEffect, gameData.GameConfig.EffectRes[i].amout, gameData.GameConfig.EffectRes[i].name, PoolType.EffectPool);
                        Destroy(tempEffect);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.EffectRes[i].name);


                    //Action<FishResourceBase, float> CreateEffectPoolCallBackFunc = (gameObj, time) =>
                    //{
                    //    if (gameObj != null && gameObj.content != null)
                    //    {
                    //        GameObject tempEffect = (GameObject)Instantiate(gameObj.content);
                    //        tempEffect.transform.localPosition = Vector3.zero;
                    //        tempEffect.transform.localScale = Vector3.one;
                    //        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempEffect, gameData.GameConfig.EffectRes[i].amout, gameData.GameConfig.EffectRes[i].name, PoolType.EffectPool);
                    //        Destroy(tempEffect);
                    //        LoadProgressBarEvent();
                    //        count--;
                    //        if (count <= 0)
                    //            ExcuteLoadResourcesQueue();
                    //    }
                    //    else
                    //        Debug.LogError("资源加载失败==> " + gameData.GameConfig.EffectRes[i].path);
                    //};
                    //yield return 0;
                    //FishGameManager.Instance.AsyncLoadResource(gameData.GameConfig.EffectRes[i].path, typeof(GameObject), CreateEffectPoolCallBackFunc);
                }
            }
        }

        public IEnumerator CreateSkillPool()
        {
            int totalCount = gameData.GameConfig.SkillRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.SkillRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempSkill = Instantiate(gameObject);
                        tempSkill.transform.localPosition = Vector3.zero;
                        tempSkill.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempSkill, gameData.GameConfig.SkillRes[i].amout, gameData.GameConfig.SkillRes[i].name, PoolType.EffectPool);
                        Destroy(tempSkill);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.SkillRes[i].name);

                    //Action<FishResourceBase, float> CreateSkillPoolCallBackFunc = (gameObj, time) =>
                    //{
                    //    if (gameObj != null && gameObj.content != null)
                    //    {
                    //        GameObject tempSkill = (GameObject)Instantiate(gameObj.content);
                    //        tempSkill.transform.localPosition = Vector3.zero;
                    //        tempSkill.transform.localScale = Vector3.one;
                    //        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempSkill, gameData.GameConfig.SkillRes[i].amout, gameData.GameConfig.SkillRes[i].name, PoolType.EffectPool);
                    //        Destroy(tempSkill);
                    //        LoadProgressBarEvent();
                    //        count--;
                    //        if (count <= 0)
                    //            ExcuteLoadResourcesQueue();
                    //    }
                    //    else
                    //        Debug.LogError("资源加载失败==> " + gameData.GameConfig.SkillRes[i].path);
                    //};
                    //yield return 0;
                    //FishGameManager.Instance.AsyncLoadResource(gameData.GameConfig.SkillRes[i].path, typeof(GameObject), CreateSkillPoolCallBackFunc);
                }
            }
        }

        public IEnumerator CreateFishOutTipsPool()
        {
            int totalCount = gameData.GameConfig.FishOutTipsRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.FishOutTipsRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempFishOutTips = Instantiate(gameObject);
                        tempFishOutTips.transform.localPosition = Vector3.zero;
                        tempFishOutTips.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempFishOutTips, gameData.GameConfig.FishOutTipsRes[i].amout, gameData.GameConfig.FishOutTipsRes[i].name, PoolType.FishOutTipsPool);
                        Destroy(tempFishOutTips);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.FishOutTipsRes[i].name);


                    //Action<FishResourceBase, float> CreateFishOutTipsPoolCallBackFunc = (gameObj, time) =>
                    //{
                    //    if (gameObj != null && gameObj.content != null)
                    //    {
                    //        GameObject tempFishOutTips = (GameObject)Instantiate(gameObj.content);
                    //        tempFishOutTips.transform.localPosition = Vector3.zero;
                    //        tempFishOutTips.transform.localScale = Vector3.one;
                    //        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempFishOutTips, gameData.GameConfig.FishOutTipsRes[i].amout, gameData.GameConfig.FishOutTipsRes[i].name, PoolType.FishOutTipsPool);
                    //        Destroy(tempFishOutTips);
                    //        LoadProgressBarEvent();
                    //        count--;
                    //        if (count <= 0)
                    //            ExcuteLoadResourcesQueue();
                    //    }
                    //    else
                    //        Debug.LogError("资源加载失败==> " + gameData.GameConfig.FishOutTipsRes[i].path);
                    //};
                    //yield return 0;
                    //FishGameManager.Instance.AsyncLoadResource(gameData.GameConfig.FishOutTipsRes[i].path, typeof(GameObject), CreateFishOutTipsPoolCallBackFunc);
                }
            }
        }

        public IEnumerator CreateSpecialDeclarePool()
        {
            int totalCount = gameData.GameConfig.SpecialDeclareRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.SpecialDeclareRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempBigAwardTips = Instantiate(gameObject);
                        tempBigAwardTips.transform.localPosition = Vector3.zero;
                        tempBigAwardTips.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempBigAwardTips, gameData.GameConfig.SpecialDeclareRes[i].amout, gameData.GameConfig.SpecialDeclareRes[i].name, PoolType.FishOutTipsPool);
                        Destroy(tempBigAwardTips);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.SpecialDeclareRes[i].name);

                    //Action<FishResourceBase, float> CreateSpecialDeclarePoolCallBackFunc = (gameObj, time) =>
                    //{
                    //    if (gameObj != null && gameObj.content != null)
                    //    {
                    //        GameObject tempBigAwardTips = (GameObject)Instantiate(gameObj.content);
                    //        tempBigAwardTips.transform.localPosition = Vector3.zero;
                    //        tempBigAwardTips.transform.localScale = Vector3.one;
                    //        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempBigAwardTips, gameData.GameConfig.SpecialDeclareRes[i].amout, gameData.GameConfig.SpecialDeclareRes[i].name, PoolType.FishOutTipsPool);
                    //        Destroy(tempBigAwardTips);
                    //        LoadProgressBarEvent();
                    //        count--;
                    //        if (count <= 0)
                    //            ExcuteLoadResourcesQueue();
                    //    }
                    //    else
                    //        Debug.LogError("资源加载失败==> " + gameData.GameConfig.SpecialDeclareRes[i].path);
                    //};
                    //yield return 0;
                    //FishGameManager.Instance.AsyncLoadResource(gameData.GameConfig.SpecialDeclareRes[i].path, typeof(GameObject), CreateSpecialDeclarePoolCallBackFunc);
                }
            }
        }

        public IEnumerator CreateTipsContentPool()
        {
            int totalCount = gameData.GameConfig.TipsContentRes.Length;
            int count = totalCount;
            if (totalCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return 0;
                    GameObject gameObject = AssetBundleManager.LoadAsset<GameObject>("fishing", gameData.GameConfig.TipsContentRes[i].name);
                    if (gameObject != null)
                    {
                        GameObject tempLightning = Instantiate(gameObject);
                        tempLightning.transform.localPosition = Vector3.zero;
                        tempLightning.transform.localScale = Vector3.one;
                        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempLightning, gameData.GameConfig.TipsContentRes[i].amout, gameData.GameConfig.TipsContentRes[i].name, PoolType.TipsContent);
                        Destroy(tempLightning);
                        LoadProgressBarEvent();
                        count--;
                        if (count <= 0)
                            ExcuteLoadResourcesQueue();
                    }
                    else
                        Debug.LogError("资源加载失败==> " + gameData.GameConfig.TipsContentRes[i].name);

                    //Action<FishResourceBase, float> CreateTipsContentPoolCallBackFunc = (gameObj, time) =>
                    //{
                    //    if (gameObj != null && gameObj.content != null)
                    //    {
                    //        GameObject tempLightning = (GameObject)Instantiate(gameObj.content);
                    //        tempLightning.transform.localPosition = Vector3.zero;
                    //        tempLightning.transform.localScale = Vector3.one;
                    //        FishGameObjectPoolManager.Instance.AddGameObjectPool(tempLightning, gameData.GameConfig.TipsContentRes[i].amout, gameData.GameConfig.TipsContentRes[i].name, PoolType.TipsContent);
                    //        Destroy(tempLightning);
                    //        LoadProgressBarEvent();
                    //        count--;
                    //        if (count <= 0)
                    //            ExcuteLoadResourcesQueue();
                    //    }
                    //    else
                    //        Debug.LogError("资源加载失败==> " + gameData.GameConfig.TipsContentRes[i].path);
                    //};
                    //yield return 0;
                    //FishGameManager.Instance.AsyncLoadResource(gameData.GameConfig.TipsContentRes[i].path, typeof(GameObject), CreateTipsContentPoolCallBackFunc);
                }
            }
        }

        public Dictionary<string, GameObject> GetFishPackResList()
        {
            return FishPackResList;
        }

        protected override void OnDestroy()
        {
            
        }
    }
}
