using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishScoreEffectManager : MonoSingleton<FishScoreEffectManager>
    {
        public enum ScoreType
        {
            AllShow = 1,
            OnlyShow = 2,
        }
        FishGameUIManager gameUIManager;
        FishGameData gameData;
        public int UID;
        Dictionary<int, FishScoreItem> CurrentUseEffectInsList;
        Dictionary<int, List<FishScoreItem>> AllUseEffectInsList;
        public FishScoreEffectManager()
        {
            InitData();

        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            UID = 1;
            CurrentUseEffectInsList = new Dictionary<int, FishScoreItem>();
            AllUseEffectInsList = new Dictionary<int, List<FishScoreItem>>();
        }

        public int GetGoldEffectUID()
        {
            UID += 1;
            return UID;
        }

        public FishScoreEffectConfig GetScoreEffectConfig(int scoreEffectID)
        {
            FishScoreEffectConfig tempConfig = gameData.ScoreEffectConfigList[scoreEffectID];
            if (tempConfig != null)
            {
                return tempConfig;
            }
            else
                Debug.LogError("获取得分效果配置失败==> " + scoreEffectID);
            return null;
        }

        public EffectVo GetScoreEffectVo(FishScoreEffectConfig scoreEffectConfig, int score, int chairID, int behaviourType)
        {
            EffectVo vo = new EffectVo();
            vo.ScoreEffectConfig = scoreEffectConfig;
            vo.score = score;
            vo.behaviourType = behaviourType;
            vo.chairId = chairID;
            vo.UID = GetGoldEffectUID();
            vo.isMe = (chairID == gameData.playerChairId);
            return vo;
        }

        public void SetScoreEffectShowMode(Vector3 startPos, int chairID, int showScore, int scoreEffectID, int behaviourType)
        {
            FishScoreEffectConfig scoreEffectConfig = GetScoreEffectConfig(scoreEffectID);
            if (scoreEffectConfig != null)
            {
                EffectVo scoreEffectVo = GetScoreEffectVo(scoreEffectConfig, showScore, chairID, behaviourType);
                FishScoreItem tempScoreEffectIns = GetScoreEffect(scoreEffectVo);
                if (tempScoreEffectIns != null)
                    tempScoreEffectIns.ResetState(startPos, scoreEffectConfig.scoreDelayTime, scoreEffectConfig.scoreShowTime, () => { });
            }
        }

        public FishScoreItem GetScoreEffect(EffectVo scoreEffectVo)
        {
            if (AllUseEffectInsList.ContainsKey(scoreEffectVo.ScoreEffectConfig.id) && AllUseEffectInsList[scoreEffectVo.ScoreEffectConfig.id] != null && AllUseEffectInsList[scoreEffectVo.ScoreEffectConfig.id].Count > 0)
            {
                FishScoreItem tempEffectIns = AllUseEffectInsList[scoreEffectVo.ScoreEffectConfig.id][0];
                AllUseEffectInsList[scoreEffectVo.ScoreEffectConfig.id].RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(scoreEffectVo);
                    CurrentUseEffectInsList[scoreEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    Debug.LogError("Failed to get ScoreEffect from object pool: " + scoreEffectVo.ScoreEffectConfig.scoreResource);
                }
            }
            else
            {
                GameObject effectItem = FishGameObjectPoolManager.Instance.GetGameObject(scoreEffectVo.ScoreEffectConfig.scoreResource, PoolType.ScorePool);
                FishScoreItem tempEffectIns = new FishScoreItem(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(scoreEffectVo);
                    CurrentUseEffectInsList[scoreEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.ScorePool);
                    Debug.LogError("Failed to create ScoreEffect instance: " + scoreEffectVo.ScoreEffectConfig.scoreResource);
                }
            }
            return null;
        }

        public void RecycleScoreEffect(FishScoreItem scoreEffectIns)
        {
            FishScoreItem tempEffectIns = CurrentUseEffectInsList[scoreEffectIns.EffectVo.UID];
            if (tempEffectIns != null)
            {
                if (!AllUseEffectInsList.ContainsKey(scoreEffectIns.EffectVo.ScoreEffectConfig.id) || AllUseEffectInsList[scoreEffectIns.EffectVo.ScoreEffectConfig.id] == null)
                {
                    AllUseEffectInsList[scoreEffectIns.EffectVo.ScoreEffectConfig.id] = new List<FishScoreItem>();
                }
                AllUseEffectInsList[scoreEffectIns.EffectVo.ScoreEffectConfig.id].Add(scoreEffectIns);
                CurrentUseEffectInsList.Remove(scoreEffectIns.EffectVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle ScoreEffecItem instance: UID " + scoreEffectIns.EffectVo.UID);
            }
        }

        public void ClearAllScoreEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (FishScoreItem scoreEffect in CurrentUseEffectInsList.Values)
                {
                    scoreEffect.isCanDestory = true;
                }
                UpdateRemoveScoreEffect();
            }
            CurrentUseEffectInsList = new Dictionary<int, FishScoreItem>();
        }

        public void UpdateRemoveScoreEffect()
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentUseEffectInsList)
                {
                    if (item.Value.isCanDestory)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }

                for (int i = 0; i < removeKeyCatch.Count; i++)
                    RecycleScoreEffect(CurrentUseEffectInsList[removeKeyCatch[i]]);
            }
        }

        private void Update()
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
                foreach (var item in CurrentUseEffectInsList.Values)
                    item.Update();
            UpdateRemoveScoreEffect();
        }

        protected override void OnDestroy()
        {
            
        }
    }
}
