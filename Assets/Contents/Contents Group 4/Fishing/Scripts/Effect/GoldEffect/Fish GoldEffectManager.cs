using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishGoldEffectManager : MonoSingleton<FishGoldEffectManager>
    {
        FishGameUIManager gameUIManager;
        FishGameData gameData;
        FishSingleCoinAlgorithm SingleCoinAlgorithm;
        Dictionary<int, int> GoldCountMap;
        Dictionary<int, List<FishSingleGoldEffect>> JumpGoldMap;
        Dictionary<int, List<FishSingleGoldEffect>> AllUseEffectInsList;
        Dictionary<int, FishSingleGoldEffect> CurrentUseEffectInsList;
        int UID;
        float width;
        float height;
        float radius;
        float offset;
        float moveToTargetTime;
        public FishGoldEffectManager()
        {
            InitData();
            InitInstance();
        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            AllUseEffectInsList = new Dictionary<int, List<FishSingleGoldEffect>>();
            CurrentUseEffectInsList = new Dictionary<int, FishSingleGoldEffect>();
            UID = 0;
            width = 100;
            height = 100;
            radius = 30;
            offset = 20;
            moveToTargetTime = 0.008f;

            JumpGoldMap = new Dictionary<int, List<FishSingleGoldEffect>>();
            GoldCountMap = new Dictionary<int, int>();
        }

        private void InitInstance()
        {
            SingleCoinAlgorithm = new FishSingleCoinAlgorithm();
        }

        public int GetGoldEffectUID()
        {
            UID += 1;
            return UID;
        }

        public void SetCoinEffectShowMode(Transform fishIns, int chairID, int fishUID, int goldEffectID, int effectCount, Vector3 endPos, int behaviourType)
        {
            List<Vector3> bornPosList = SingleCoinAlgorithm.GetMuiltpleGoldEffect(fishIns, width, height, radius, offset);
            float delayTime = 0;
            int bornPosCount = bornPosList.Count;
            GoldCountMap[fishUID] = effectCount;
            
            float durationTime = moveToTargetTime * Vector3.Distance(new Vector3(fishIns.position.x, fishIns.position.y, 86.4f), endPos) / (Vector3.Distance(fishIns.parent.TransformPoint(Vector3.zero), endPos));
            for (int i = 0; i < effectCount; i++)
            {
                Vector3 beginPos;
                if (i >= bornPosCount)
                {
                    float x = UnityEngine.Random.Range(-radius, radius);
                    float y = UnityEngine.Random.Range(-radius, radius);
                    beginPos = bornPosList[i % bornPosCount] + new Vector3(x, y, 0);
                }
                else
                    beginPos = bornPosList[i];
                beginPos = fishIns.parent.TransformPoint(beginPos);
                ShowGoldEffect(goldEffectID, fishUID, chairID, beginPos, endPos, delayTime, durationTime, behaviourType, () => { });
                delayTime += UnityEngine.Random.Range(0.1f, 0.15f);
            }
        }

        public void ShowGoldEffect(int goldEffectID, int fishUID, int chairID, Vector3 beginPos, Vector3 endPos, float delayTime, float durationTime, int behaviourType, Action callBack)
        {
            EffectVo vo = GetGoldEffectVo(goldEffectID, fishUID, chairID, durationTime, behaviourType);
            FishSingleGoldEffect tempEffectIns = GetGoldEffect(vo);
            if (tempEffectIns != null)
                tempEffectIns.ResetState(beginPos, endPos, delayTime, callBack);
            else
                Debug.LogError("获取GoldEffectIns失败==>" + goldEffectID);
        }

        public EffectVo GetGoldEffectVo(int goldEffectID, int fishUID, int chairID, float durationTime, int behaviourType)
        {
            EffectVo vo = new EffectVo();
            vo.GoldEffectConfig = GetGoldEffectConfig(goldEffectID);
            vo.UID = GetGoldEffectUID();
            vo.fishUID = fishUID;
            vo.chairId = chairID;
            vo.behaviourType = behaviourType;
            vo.isMe = chairID == gameData.playerChairId;
            vo.DurationTime = durationTime;
            if (!JumpGoldMap.ContainsKey(fishUID) || JumpGoldMap[fishUID] == null)
            {
                JumpGoldMap[fishUID] = new List<FishSingleGoldEffect>();
            }
            return vo;
        }

        public FishCoinEffectConfig GetGoldEffectConfig(int goldEffectID)
        {
            return gameData.CoinEffectConfigList[goldEffectID];
        }

        public FishSingleGoldEffect GetGoldEffect(EffectVo goldEffectVo)
        {
            if (AllUseEffectInsList.ContainsKey(goldEffectVo.GoldEffectConfig.id) && AllUseEffectInsList[goldEffectVo.GoldEffectConfig.id] != null && AllUseEffectInsList[goldEffectVo.GoldEffectConfig.id].Count > 0)
            {
                FishSingleGoldEffect tempEffectIns = AllUseEffectInsList[goldEffectVo.GoldEffectConfig.id][0];
                AllUseEffectInsList[goldEffectVo.GoldEffectConfig.id].RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(goldEffectVo);
                    CurrentUseEffectInsList[goldEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    Debug.LogError("创建GoldEffect失败==>" + goldEffectVo.UID);
            }
            else
            {
                GameObject effectItem = FishGameObjectPoolManager.Instance.GetGameObject(goldEffectVo.GoldEffectConfig.coinRes, PoolType.GoldPool);
                int effectType = goldEffectVo.GoldEffectConfig.id;

                FishSingleGoldEffect tempEffectIns = new FishSingleGoldEffect(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(goldEffectVo);
                    CurrentUseEffectInsList[goldEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.GoldPool);
                    Debug.LogError("EffectIns生成失败 ==> " + effectType);
                }
            }
            return null;
        }

        public void GoldCenterToJumpOver(FishSingleGoldEffect goldItem)
        {
            int fishUID = goldItem.EffectVo.fishUID;
            if (!GoldCountMap.ContainsKey(fishUID))
                return;
            JumpGoldMap[fishUID].Add(goldItem);
            GoldCountMap[fishUID]--;
            if (GoldCountMap[fishUID] == 0)
            {
                GoldMoveToEndPoint(fishUID);
                GoldCountMap.Remove(fishUID);
            }
            
        }

        public void GoldMoveToEndPoint(int fishUID)
        {
            float delay = 0;
            int index = 0;
            int count = JumpGoldMap[fishUID].Count;
            while (index < count)
            {
                if (index <= 0)
                {
                    JumpGoldMap[fishUID][index].MoveToEndPoint(delay);
                    index++;
                }
                else
                {
                    int rand = UnityEngine.Random.Range(1, 2);
                    if (rand + index > count)
                        rand = count - index;
                    for (int i = 0; i < rand; i++)
                        JumpGoldMap[fishUID][index + i].MoveToEndPoint(delay);
                    index += rand;
                }
                delay += 0.15f;
            }
            JumpGoldMap[fishUID].Clear();
        }

        public void RecycleGoldEffect(FishSingleGoldEffect goldEffectIns)
        {
            FishSingleGoldEffect tempEffectIns = CurrentUseEffectInsList[goldEffectIns.EffectVo.UID];
            if (tempEffectIns != null)
            {
                if (!AllUseEffectInsList.ContainsKey(goldEffectIns.EffectVo.GoldEffectConfig.id) || AllUseEffectInsList[goldEffectIns.EffectVo.GoldEffectConfig.id] == null)
                    AllUseEffectInsList[goldEffectIns.EffectVo.GoldEffectConfig.id] = new List<FishSingleGoldEffect>();
                AllUseEffectInsList[goldEffectIns.EffectVo.GoldEffectConfig.id].Add(goldEffectIns);
                CurrentUseEffectInsList.Remove(goldEffectIns.EffectVo.UID);
            }
            else
                Debug.LogError("移除的goldEffectIns为null==> UID :" + goldEffectIns.EffectVo.UID);
        }

        public void ClearAllGoldEffect()
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
            {
                for (int i = 0; i < CurrentUseEffectInsList.Count; i++)
                    CurrentUseEffectInsList[i].isCanDestroy = true;
                UpdateRemoveGoldEffect();
            }
            CurrentUseEffectInsList.Clear();
        }

        public void ClearOtherPlayerGoladEffect(int chairId)
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
            {
                for (int i = 0; i < CurrentUseEffectInsList.Count; i++)
                {
                    if (CurrentUseEffectInsList[i].EffectVo.chairId == chairId && CurrentUseEffectInsList[i].EffectVo.behaviourType == 3)
                    {
                        CurrentUseEffectInsList[i].isCanDestroy = true;
                    }
                }
                UpdateRemoveGoldEffect();
            }
        }

        public void UpdateRemoveGoldEffect()
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
            {
                List<int> removeCatchKey = new List<int>();
                foreach (var item in CurrentUseEffectInsList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeCatchKey.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeCatchKey.Count; i++)
                    RecycleGoldEffect(CurrentUseEffectInsList[removeCatchKey[i]]);
            }
        }

        public void Update()
        {
            UpdateRemoveGoldEffect();
        }

        protected override void OnDestroy()
        {

        }
    }

    public class EffectVo
    {
        public FishCoinEffectConfig GoldEffectConfig;
        public FishScoreEffectConfig ScoreEffectConfig;
        public FishPlusTipsEffectConfig PlusTipsEffectConfig;
        public FishSpecialDeclareConfig SpecialDeclareEffectConfig;
        public int UID;
        public int score;
        public int fishUID;
        public int chairId;
        public int AnimType;
        public int multiple;
        public int behaviourType;
        public float lifeTime;
        public float delayTime;
        public float DurationTime;
        public bool isMe;
        public string effectName;
        public string animationName;
        public int partMul;
        public int totalScore;
        public int selfScore;
        public int hurtNum;
        public int hurtScore;
        public int totalMul;
    }
}
