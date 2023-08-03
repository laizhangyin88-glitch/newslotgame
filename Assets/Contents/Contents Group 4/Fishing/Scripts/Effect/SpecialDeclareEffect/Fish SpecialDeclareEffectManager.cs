using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BagelCode
{
    public class FishSpecialDeclareEffectManager : MonoSingleton<FishSpecialDeclareEffectManager>
    {
        private FishGameData gameData;
        private int UID;
        private Dictionary<int, List<FishSpecialDeclareEffectItem>> AllUseEffectInsList;
        private Dictionary<int, FishSpecialDeclareEffectItem> CurrentUseEffectInsList;

        private void Awake()
        {
            InitData();
            
        }

        private void InitData()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AllUseEffectInsList = new Dictionary<int, List<FishSpecialDeclareEffectItem>>();
            CurrentUseEffectInsList = new Dictionary<int, FishSpecialDeclareEffectItem>();
        }

        public int GetSpecialDeclareEffectUID()
        {
            UID++;
            return UID;
        }

        public FishSpecialDeclareConfig GetSpecialDeclareEffectConfig(int specialDeclareId)
        {
            FishSpecialDeclareConfig tempCfg = gameData.SpecialDeclareConfigList[specialDeclareId];
            if (tempCfg == null)
            {
                Debug.LogError("获取得分提示配置失败==>" + specialDeclareId);
                return null;
            }
            return tempCfg;
        }

        public EffectVo GetSpecialDeclareEffectVo(FishSpecialDeclareConfig specialDeclareConfig, int chairId)
        {
            EffectVo vo = new EffectVo();
            vo.SpecialDeclareEffectConfig = specialDeclareConfig;
            vo.UID = GetSpecialDeclareEffectUID();
            vo.chairId = chairId;
            vo.AnimType = vo.SpecialDeclareEffectConfig.specialDeclareAnimType;
            vo.isMe = chairId == gameData.playerChairId;
            return vo;
        }

        public int SetSpecialDeclareEffectShowMode(int chairId, Vector3 specialDeclarePos, FishSpecialDeclareConfig specialDeclareConfig)
        {
            EffectVo vo = GetSpecialDeclareEffectVo(specialDeclareConfig, chairId);
            FishSpecialDeclareEffectItem tempSpecialDeclareIns = GetSpecialDeclareEffect(vo);
            Vector3 targetPos = specialDeclarePos;
            if (tempSpecialDeclareIns != null)
            {
                tempSpecialDeclareIns.ResetState(targetPos);
            }
            return vo.UID;
        }

        public void BeginSpecialDeclareEffectChangeScore(int UID, int score, int mul)
        {
            var specialDelareEffect = CurrentUseEffectInsList[UID];
            if (specialDelareEffect != null)
            {
                specialDelareEffect.effectVo.score = score;
                specialDelareEffect.effectVo.multiple = mul;
                specialDelareEffect.BeginShowScore();
            }
        }

        public FishSpecialDeclareEffectItem GetSpecialDeclareEffect(EffectVo vo)
        {
            if (AllUseEffectInsList.ContainsKey(vo.SpecialDeclareEffectConfig.id) && AllUseEffectInsList[vo.SpecialDeclareEffectConfig.id].Count > 0)
            {
                var tempEffectIns = AllUseEffectInsList[vo.SpecialDeclareEffectConfig.id][0];
                AllUseEffectInsList[vo.SpecialDeclareEffectConfig.id].RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(vo);
                    CurrentUseEffectInsList[vo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    Debug.LogError("创建得分提示失败==>" + vo.SpecialDeclareEffectConfig.id);
            }
            else
            {
                var effectItem = FishGameObjectPoolManager.Instance.GetGameObject(vo.SpecialDeclareEffectConfig.specialDeclareRes, PoolType.SpecialDeclarePool);
                var tempEffectIns = new FishSpecialDeclareEffectItem(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(vo);
                    CurrentUseEffectInsList[vo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.SpecialDeclarePool);
                    Debug.LogError("创建得分提示失败==>" + vo.SpecialDeclareEffectConfig.id);
                }
            }
            return null;
        }

        public void RecycleSpecialDeclareEffect(FishSpecialDeclareEffectItem specialDeclareEffectItem)
        {
            var tempEfffectIns = CurrentUseEffectInsList[specialDeclareEffectItem.effectVo.UID];
            if (tempEfffectIns != null)
            {
                if (!AllUseEffectInsList.ContainsKey(specialDeclareEffectItem.effectVo.SpecialDeclareEffectConfig.id)
                    || AllUseEffectInsList[specialDeclareEffectItem.effectVo.SpecialDeclareEffectConfig.id] == null)
                    AllUseEffectInsList[specialDeclareEffectItem.effectVo.SpecialDeclareEffectConfig.id] = new List<FishSpecialDeclareEffectItem>();
                AllUseEffectInsList[specialDeclareEffectItem.effectVo.SpecialDeclareEffectConfig.id].Add(specialDeclareEffectItem);
                CurrentUseEffectInsList.Remove(specialDeclareEffectItem.effectVo.UID);
            }
            else
                Debug.LogError("移除的specialDeclareEffectIns为nil==>UID" + specialDeclareEffectItem.effectVo.UID);
        }

        public void ClearAllSpecialDeclareEffect()
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                    item.isCanDestroy = true;
                UpdateRemoveSpecialDeclareEffect();
                CurrentUseEffectInsList.Clear();
            }
        }
        
        public void ClearOtherPlayerSpecialDeclareEffect(int chairId)
        {
            if (CurrentUseEffectInsList != null)
            {
                foreach (var item in CurrentUseEffectInsList.Values)
                    if (item.effectVo.chairId == chairId)
                        item.isCanDestroy = true;
                UpdateRemoveSpecialDeclareEffect();
                CurrentUseEffectInsList.Clear();
            }
        }

        public void UpdateRemoveSpecialDeclareEffect()
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
                    RecycleSpecialDeclareEffect(CurrentUseEffectInsList[removeKeyCatch[i]]);
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseEffectInsList.Values)
                item.Update();
            UpdateRemoveSpecialDeclareEffect();
        }

        protected override void OnDestroy()
        {
            
        }
    }
}


