using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFishEffectManager : MonoSingleton<FishFishEffectManager>
    {
        FishGameUIManager gameUIManager;
        FishGameData gameData;
        public int UID;
        public Animator animator;
        public Dictionary<string, List<FishEffectItemBase>> AllUseEffectInsList;
        public Dictionary<int, FishEffectItemBase> CurrentUseEffectInsList;

        public enum FishEffectType
        {
            AnimatorType = 1,
            SpineType = 2,
            ParticleType = 3,
        }

        FishFishEffectManager()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            UID = 1;
            AllUseEffectInsList = new Dictionary<string, List<FishEffectItemBase>>();
            CurrentUseEffectInsList = new Dictionary<int, FishEffectItemBase>();
        }

        public int GetFishEffectUID()
        {
            UID++;
            return UID;
        }

        public FishDieEffectConfig GetFishEffectConfig(int fishEffectID)
        {
            return gameData.DieEffectConfigList[fishEffectID];
        }

        public void ShowFishEffect(Vector3 beginPos, int type, string name, float delayTime, float lifeTime, string effectAudio, Action callBack = null)
        {
            FishEffectVo vo = GetFishEffectVo(type, name, delayTime, lifeTime, effectAudio);
            FishEffectItemBase tempEffectIns = GetFishEffect(vo);
            if (tempEffectIns != null)
            {
                tempEffectIns.ResetState(beginPos, callBack);
            }
            else
                Debug.LogError("获取effectName失败==> " + name);
        }

        public FishEffectVo GetFishEffectVo(int effectType, string effectName, float effectDelayTime, float effectLifeTime, string effectAudio)
        {
            FishEffectVo vo = new FishEffectVo();
            vo.UID = GetFishEffectUID();
            vo.EffectType = effectType;
            vo.EffectName = effectName;
            vo.EffectDelayTime = effectDelayTime;
            vo.EffectLifeTime = effectLifeTime;
            vo.EffectAudio = effectAudio;
            return vo;
        }

        public FishEffectItemBase GetFishEffect(FishEffectVo fishEffectVo)
        {
            if (AllUseEffectInsList.ContainsKey(fishEffectVo.EffectName) && AllUseEffectInsList[fishEffectVo.EffectName].Count > 0)
            {
                FishEffectItemBase tempEffectIns = AllUseEffectInsList[fishEffectVo.EffectName][0];
                AllUseEffectInsList[fishEffectVo.EffectName].RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(fishEffectVo);
                    CurrentUseEffectInsList[fishEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    Debug.LogError("创建特效失败==> " + fishEffectVo.EffectName);
            }
            else
            {
                GameObject effectItem = FishGameObjectPoolManager.Instance.GetGameObject(fishEffectVo.EffectName, PoolType.EffectPool);
                FishEffectItemBase tempEffectIns = BuildFishEffectInstance(fishEffectVo.EffectType, effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(fishEffectVo);
                    CurrentUseEffectInsList[fishEffectVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.EffectPool);

            }
            return null;
        }

        public FishEffectItemBase BuildFishEffectInstance(int effectType, GameObject go)
        {
            switch (effectType)
            {
                case 1:
                    return new FishAnimatorEffectItem(go);
                case 2:
                    return new FishSpineEffectItem(go);
                case 3:
                    return new FishParticaleEffectItem(go);
                default:
                    Debug.LogError("未定义类型EffectType==> " + effectType);
                    break;
            }
            return null;
        }

        public void RecycleFishEffect(FishEffectVo fishEffectVo)
        {
            FishEffectItemBase tempEffectIns = CurrentUseEffectInsList[fishEffectVo.UID];
            if (tempEffectIns != null)
            {
                if (!AllUseEffectInsList.ContainsKey(fishEffectVo.EffectName) ||  AllUseEffectInsList[fishEffectVo.EffectName] == null)
                {
                    AllUseEffectInsList[fishEffectVo.EffectName] = new List<FishEffectItemBase>();
                }
                AllUseEffectInsList[fishEffectVo.EffectName].Add(tempEffectIns);
                CurrentUseEffectInsList.Remove(fishEffectVo.UID);
            }
            else
                Debug.LogError("移除的FishEffectItem为nil==>UID " + fishEffectVo.UID);
        }

        public void ClearAllFishEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                {
                    item.isCanDestroy = true;
                }
                UpdateRemoveFishEffect();
            }
            CurrentUseEffectInsList.Clear();
        }

        public void UpdateRemoveFishEffect()
        {
            if (CurrentUseEffectInsList != null && CurrentUseEffectInsList.Count > 0)
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
                {
                    RecycleFishEffect(CurrentUseEffectInsList[removeKeyCatch[i]].EffectVo);
                }
            }
        }

        protected override void OnDestroy()
        {
            
        }

        public void Update()
        {
            //临时这么弄
            List<int> tempKeyList = new List<int>();
            foreach (var item in CurrentUseEffectInsList.Keys)
                tempKeyList.Add(item);
            for (int i = 0; i < tempKeyList.Count; i++)
                CurrentUseEffectInsList[tempKeyList[i]].Update();
            
            UpdateRemoveFishEffect();
        }
    }
    public class FishEffectVo
    {
        public int UID;
        public int EffectType;
        public string EffectName;
        public float EffectDelayTime;
        public float EffectLifeTime;
        public int EffectPositionFlag;
        public string EffectAudio;
    }
}
