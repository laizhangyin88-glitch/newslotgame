using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishLightningEffectManager : MonoSingleton<FishLightningEffectManager>
    {
        FishGameData gameData;
        int UID;
        List<FishLightningEffectItem> AllUseEffectInsList;
        Dictionary<int, FishLightningEffectItem> CurrentUseEffectInsList;
        private void Awake()
        {
            InitData();
        }

        private void InitData()
        {
            gameData = FishGameUIManager.Instance.gameData;
            UID = 1;
            AllUseEffectInsList = new List<FishLightningEffectItem>();
            CurrentUseEffectInsList = new Dictionary<int, FishLightningEffectItem>();
        }

        public int GetLightningEffectUID()
        {
            UID ++;
            return UID;
        }

        public FishDieEffectConfig GetFishEffectConfig(int fishEffectId)
        {
            return gameData.DieEffectConfigList[fishEffectId];
        }

        public void SetLightningEffectShowMode(Transform tempFishTF, Transform subFishTF, int chairId, int fishUID, FishDieEffectConfig dieEffectConfig, Action callBack = null)
        {
            string lineNameRes = dieEffectConfig.childFishLineRes;
            if (!string.IsNullOrEmpty(lineNameRes))
            {
                float lineDelayTime = dieEffectConfig.childFishLineDelyTime;
                float lineLifeTime = dieEffectConfig.childFishLineLifeTime;
                Vector3 beginPos = tempFishTF.position;
                Vector3 targetPos = subFishTF.position;
                float distance = Vector3.Distance(tempFishTF.position, subFishTF.position);
                ShowLightningEffect(chairId, fishUID, beginPos, targetPos, distance, lineNameRes, lineDelayTime, lineLifeTime, callBack);
            }
        }

        public void ShowLightningEffect(int chairId, int fishUID, Vector3 beginPos, Vector3 targetPos, float distance, string lineRes, float delayTime, float lifeTime, Action callBack)
        {
            EffectVo vo = GetLightningEffectVo(fishUID, chairId, lineRes, delayTime, lifeTime);
            FishLightningEffectItem tempEffectIns = GetLightningEffect(vo);
            if (tempEffectIns != null)
                tempEffectIns.ResetEffectVo(vo);
            else
                Debug.LogError("获取effectName失败==>" + vo.effectName);
        }

        public EffectVo GetLightningEffectVo(int fishUID, int chairId, string lineRes, float delayTime, float lifeTime)
        {
            EffectVo vo = new EffectVo();
            vo.UID = GetLightningEffectUID();
            vo.fishUID = fishUID;
            vo.chairId = chairId;
            vo.effectName = lineRes;
            vo.isMe = chairId == gameData.playerChairId;
            vo.delayTime = delayTime;
            vo.lifeTime = lifeTime;
            return vo;
        }

        public FishLightningEffectItem GetLightningEffect(EffectVo lightningEffectVo)
        {
            if (AllUseEffectInsList != null && AllUseEffectInsList.Count > 0)
            {
                FishLightningEffectItem tempEffectIns = AllUseEffectInsList[0];
                AllUseEffectInsList.RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(lightningEffectVo);
                    CurrentUseEffectInsList[lightningEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    Debug.LogError("创建闪电失败==>" + lightningEffectVo.effectName);
            }
            else
            {
                var effectItem = FishGameObjectPoolManager.Instance.GetGameObject(lightningEffectVo.effectName, PoolType.EffectPool);
                var tempEffectIns = new FishLightningEffectItem(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(lightningEffectVo);
                    CurrentUseEffectInsList[lightningEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.EffectPool);
                    Debug.LogError("创建闪电失败==>" + lightningEffectVo.effectName);
                }
            }
            return null;
        }

        public void RecycleLightningEffect(EffectVo vo)
        {
            var tempEffect = CurrentUseEffectInsList[vo.UID];
            if (tempEffect != null)
            {
                if (AllUseEffectInsList == null)
                    AllUseEffectInsList = new List<FishLightningEffectItem>();
                AllUseEffectInsList.Add(tempEffect);
                CurrentUseEffectInsList.Remove(vo.UID);
            }
            else
                Debug.LogError("移除的闪电为nil==> UID " + vo.UID);
        }

        public void ClearLightningEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                    item.isCanDestroy = true;
                UpdateRemoveLightningEffect();
            }
            CurrentUseEffectInsList.Clear();
        }

        public void UpdateRemoveLightningEffect()
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
                    RecycleLightningEffect(CurrentUseEffectInsList[removeKeyCatch[i]].lightningVo);
            }
        }

        private void Update()
        {
            foreach(var item in CurrentUseEffectInsList.Values)
                item.Update();
            UpdateRemoveLightningEffect();
        }

        protected override void OnDestroy()
        {
            
        }
    }
}

