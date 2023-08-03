using BagelCode;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFishOutTipsEffectManager : MonoSingleton<FishFishOutTipsEffectManager>
    {
        public int UID;
        Dictionary<int, FishFishOutTipsEffecItem> CurrentUseEffectInsList;

        private void Awake()
        {
            UID = 1;
            CurrentUseEffectInsList = new Dictionary<int, FishFishOutTipsEffecItem>();
        }

        public int GetGoldEffectUID()
        {
            UID += 1;
            return UID;
        }

        public FishOutTipsEffectVo GetFishOutTipsEffectVo(string resourceName, string animationName, float lifeTime)
        {
            FishOutTipsEffectVo vo = new FishOutTipsEffectVo();
            vo.UID = UID;
            vo.ResourceName = resourceName;
            vo.animationName = animationName;
            vo.LifeTime = lifeTime;
            return vo;
        }

        public void SetFishOutTipsEffectShowMode(string resourceName, string animationName, float lifeTime)
        {
            FishOutTipsEffectVo FishOutTipsEffectVo = GetFishOutTipsEffectVo(resourceName, animationName, lifeTime);
            FishFishOutTipsEffecItem tempFishOutTipsEffectIns = GetFishOutTipsEffect(FishOutTipsEffectVo);
            if (tempFishOutTipsEffectIns != null)
            {
                tempFishOutTipsEffectIns.ResetState(() => { });
            }
        }

        public FishFishOutTipsEffecItem GetFishOutTipsEffect(FishOutTipsEffectVo fishOutTipsEffectVo)
        {
            GameObject effecItem = FishGameObjectPoolManager.Instance.GetGameObject(fishOutTipsEffectVo.ResourceName, PoolType.FishOutTipsPool);
            FishFishOutTipsEffecItem tempEffectIns = new FishFishOutTipsEffecItem(effecItem);
            if (tempEffectIns != null)
            {
                tempEffectIns.ResetEffectVo(fishOutTipsEffectVo);
                CurrentUseEffectInsList[fishOutTipsEffectVo.UID] = tempEffectIns;
                return tempEffectIns;
            }
            else
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(effecItem, PoolType.FishOutTipsPool);
                Debug.LogError("EffectIns生成失败==> " + fishOutTipsEffectVo.ResourceName);
            }
            return null;
        }

        public void RecycleFishOutTipsEffect(FishFishOutTipsEffecItem FishOutTipsEffectIns)
        {
            FishFishOutTipsEffecItem tempEffectIns = CurrentUseEffectInsList[FishOutTipsEffectIns.EffectVo.UID];
            if (tempEffectIns != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(FishOutTipsEffectIns.gameObject, PoolType.FishOutTipsPool);
                CurrentUseEffectInsList.Remove(FishOutTipsEffectIns.EffectVo.UID);
            }
            else
                Debug.LogError("移除的FishOutTipsEffect为nil==>UID " + FishOutTipsEffectIns.EffectVo.UID);
        }

        public void ClearAllFishOutTipsEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                {
                    item.isCanDestroy = true;
                }
                UpdateRemoveFishOutTipsEffect();
            }
            CurrentUseEffectInsList.Clear();
        }

        public void UpdateRemoveFishOutTipsEffect()
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                {
                    if (item.isCanDestroy)
                    {
                        RecycleFishOutTipsEffect(item);
                    }
                }
            }
        }

        public void Update()
        {
            UpdateRemoveFishOutTipsEffect();
        }

        protected override void OnDestroy()
        {
            
        }
    }

    public class FishOutTipsEffectVo
    {
        public int UID;
        public string ResourceName;
        public string animationName;
        public float LifeTime;
        public int EffectType;
        public string effectName;
        public float effectDelayTime;
        public float effectLifeTime;
        public int effectPositionFlag;
        public int effectAudio;
    }
}

