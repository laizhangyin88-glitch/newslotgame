using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishPlusTipsEffectManager : MonoSingleton<FishPlusTipsEffectManager>
    {
        private FishGameData gameData;
        private int UID;
        private Dictionary<int, List<FishPlusTipsEffectItem>> AllUseEffectInsList;
        private Dictionary<int, FishPlusTipsEffectItem> CurrentUseEffectInsList;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AllUseEffectInsList = new Dictionary<int, List<FishPlusTipsEffectItem>>();
            CurrentUseEffectInsList = new Dictionary<int, FishPlusTipsEffectItem>();
            UID = 1;
        }

        public int GetPlusTipsEffectUID()
        {
            UID++;
            return UID;
        }

        public FishPlusTipsEffectConfig GetPlusTipsEffectConfig(int plusTipsEffectId)
        {
            FishPlusTipsEffectConfig tempConfig = gameData.PlusTipsEffectConfigList[plusTipsEffectId];
            if (tempConfig == null)
                Debug.LogError("获取加分配置失败==> " + plusTipsEffectId);
            return tempConfig;
        }

        public EffectVo GetPlusTipsEffectVo(FishPlusTipsEffectConfig plusTipsEffectConfig)
        {
            EffectVo vo = new EffectVo();
            vo.PlusTipsEffectConfig = plusTipsEffectConfig;
            vo.UID = GetPlusTipsEffectUID();
            return vo;
        }

        public void SetPlusTipsEffectShowMode(int chairId, Transform flyObj, int showScore, FishPlusTipsEffectConfig plusTipsEffectConfig)
        {
            if (chairId != gameData.playerChairId)
                return;
            EffectVo plusTipsEffectVo = GetPlusTipsEffectVo(plusTipsEffectConfig);
            FishPlusTipsEffectItem tempPlusTipsEffectIns = GetPlusTipsEffect(plusTipsEffectVo);
            Vector3 startPos = flyObj.position;
            if (tempPlusTipsEffectIns != null)
            {
                tempPlusTipsEffectIns.SetShowScoreText(showScore.ToString());
                tempPlusTipsEffectIns.ResetState(startPos, plusTipsEffectConfig.plusTipsDelayTime, plusTipsEffectConfig.plusTipsShowTime);
            }
        }

        public FishPlusTipsEffectItem GetPlusTipsEffect(EffectVo plusTipsEffectVo)
        {
            if (AllUseEffectInsList.ContainsKey(plusTipsEffectVo.PlusTipsEffectConfig.id)
                && AllUseEffectInsList[plusTipsEffectVo.PlusTipsEffectConfig.id] != null
                && AllUseEffectInsList[plusTipsEffectVo.PlusTipsEffectConfig.id].Count > 0)
            {
                FishPlusTipsEffectItem tempEffectIns = AllUseEffectInsList[plusTipsEffectVo.PlusTipsEffectConfig.id][0];
                AllUseEffectInsList[plusTipsEffectVo.PlusTipsEffectConfig.id].RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(plusTipsEffectVo);
                    CurrentUseEffectInsList[plusTipsEffectVo.UID] = tempEffectIns;
                }
                else
                    Debug.LogError("创建PlusTipsEffect失败==>" + plusTipsEffectVo.PlusTipsEffectConfig.id);
            }
            else
            {
                var effectItem = FishGameObjectPoolManager.Instance.GetGameObject(plusTipsEffectVo.PlusTipsEffectConfig.plusTipsResource, PoolType.PlusTips);
                var temEffectIns = new FishPlusTipsEffectItem(effectItem);
                if (temEffectIns != null)
                {
                    temEffectIns.ResetEffectVo(plusTipsEffectVo);
                    CurrentUseEffectInsList[plusTipsEffectVo.UID] = temEffectIns;
                    return temEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.PlusTips);
                    Debug.LogError("创建PlusTipsEffect失败==>" + plusTipsEffectVo.PlusTipsEffectConfig.id);
                }
            }
            return null;
        }

        public void RecyclePlusTipsEffect(FishPlusTipsEffectItem plusTipsEffectIns)
        {
            FishPlusTipsEffectItem tempEffectIns = CurrentUseEffectInsList[plusTipsEffectIns.effectVo.UID];
            if (tempEffectIns != null)
            {
                if (!AllUseEffectInsList.ContainsKey(plusTipsEffectIns.effectVo.PlusTipsEffectConfig.id)
                    || AllUseEffectInsList[plusTipsEffectIns.effectVo.PlusTipsEffectConfig.id] == null)
                {
                    AllUseEffectInsList[plusTipsEffectIns.effectVo.PlusTipsEffectConfig.id] = new List<FishPlusTipsEffectItem>();
                }
                AllUseEffectInsList[plusTipsEffectIns.effectVo.PlusTipsEffectConfig.id].Add(plusTipsEffectIns);
                CurrentUseEffectInsList.Remove(plusTipsEffectIns.effectVo.UID);
            }
            else
                Debug.LogError("移除的goldEffectIns为nil==>UID " + plusTipsEffectIns.effectVo.UID);
        }

        public void ClearAllPlusTipsEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                    item.isCanDestroy = true;
                UpdateRemovePlusTipsEffect();
            }
            CurrentUseEffectInsList.Clear();
        }

        public void UpdateRemovePlusTipsEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentUseEffectInsList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                    RecyclePlusTipsEffect(CurrentUseEffectInsList[removeKeyCatch[i]]);
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseEffectInsList.Values)
                item.Update();
            UpdateRemovePlusTipsEffect();
        }

        protected override void OnDestroy()
        {
            
        }
    }
}
