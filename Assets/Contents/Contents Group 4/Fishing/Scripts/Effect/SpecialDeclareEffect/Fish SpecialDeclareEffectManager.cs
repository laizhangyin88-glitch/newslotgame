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
        private Dictionary<int, List<FishBossDeclareEffectItem>> AllUseBossEffectList;
        private Dictionary<int, FishBossDeclareEffectItem> CurrentUseBossEffectList;

        private void Awake()
        {
            InitData();
            
        }

        private void InitData()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AllUseEffectInsList = new Dictionary<int, List<FishSpecialDeclareEffectItem>>();
            CurrentUseEffectInsList = new Dictionary<int, FishSpecialDeclareEffectItem>();
            AllUseBossEffectList = new Dictionary<int, List<FishBossDeclareEffectItem>>();
            CurrentUseBossEffectList = new Dictionary<int, FishBossDeclareEffectItem>();
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

        public int SetSpecialDeclareEffectShowMode(int chairId, Vector3 specialDeclarePos, FishSpecialDeclareConfig specialDeclareConfig, bool isBoss = false)
        {
            EffectVo vo = GetSpecialDeclareEffectVo(specialDeclareConfig, chairId);
            Vector3 targetPos = specialDeclarePos;
            if (isBoss)
            {
                FishBossDeclareEffectItem temp = GetBossDeclareEffect(vo);
                temp?.ResetState(targetPos);
            }
            else
            {
                FishSpecialDeclareEffectItem tempSpecialDeclareIns = GetSpecialDeclareEffect(vo);
                tempSpecialDeclareIns?.ResetState(targetPos);
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

        public void BeginBossDeclare(int UID, int score, int mul)
        {
            var obj = CurrentUseBossEffectList[UID];
            if (obj != null)
            {
                obj.effectVo.score = score;
                obj.effectVo.multiple = mul;
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

        private FishBossDeclareEffectItem GetBossDeclareEffect(EffectVo vo)
        {
            if (AllUseBossEffectList.ContainsKey(vo.SpecialDeclareEffectConfig.id) && AllUseBossEffectList[vo.SpecialDeclareEffectConfig.id].Count > 0)
            {
                var tempEffectIns = AllUseBossEffectList[vo.SpecialDeclareEffectConfig.id][0];
                AllUseBossEffectList[vo.SpecialDeclareEffectConfig.id].RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(vo);
                    CurrentUseBossEffectList[vo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
            }
            else
            {
                var effectItem = FishGameObjectPoolManager.Instance.GetGameObject(vo.SpecialDeclareEffectConfig.specialDeclareRes, PoolType.SpecialDeclarePool);
                var tempEffectIns = new FishBossDeclareEffectItem(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(vo);
                    CurrentUseBossEffectList[vo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.SpecialDeclarePool);
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

        public void RecycleBossDeclare(FishBossDeclareEffectItem bossDeclareItem)
        {
            var tempEfffectIns = CurrentUseBossEffectList[bossDeclareItem.effectVo.UID];
            if (tempEfffectIns != null)
            {
                if (!AllUseBossEffectList.ContainsKey(bossDeclareItem.effectVo.SpecialDeclareEffectConfig.id)
                    || AllUseBossEffectList[bossDeclareItem.effectVo.SpecialDeclareEffectConfig.id] == null)
                    AllUseBossEffectList[bossDeclareItem.effectVo.SpecialDeclareEffectConfig.id] = new List<FishBossDeclareEffectItem>();
                AllUseBossEffectList[bossDeclareItem.effectVo.SpecialDeclareEffectConfig.id].Add(bossDeclareItem);
                CurrentUseBossEffectList.Remove(bossDeclareItem.effectVo.UID);
            }
            else
                Debug.LogError("移除的BossDeclareIns为nil==>UID" + bossDeclareItem.effectVo.UID);
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

        public void ClearAllBossDeclare()
        {
            if (CurrentUseBossEffectList != null)
            {
                foreach (var item in CurrentUseBossEffectList.Values)
                    item.isCanDestroy = true;
                UpdateRemoveBossDeclare();
                CurrentUseBossEffectList.Clear();
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

        public void ClearOtherPlayerBossDeclare(int chairId)
        {
            if (CurrentUseBossEffectList != null)
            {
                foreach (var item in CurrentUseBossEffectList.Values)
                    if (item.effectVo.chairId == chairId)
                        item.isCanDestroy = true;
                UpdateRemoveBossDeclare();
                CurrentUseBossEffectList.Clear();
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

        private void UpdateRemoveBossDeclare()
        {
            if (CurrentUseBossEffectList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentUseBossEffectList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                    RecycleBossDeclare(CurrentUseBossEffectList[removeKeyCatch[i]]);
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseBossEffectList.Values)
                item.Update();
            UpdateRemoveBossDeclare();
            foreach (var item in CurrentUseEffectInsList.Values)
                item.Update();
            UpdateRemoveSpecialDeclareEffect();

        }

        protected override void OnDestroy()
        {
            
        }
    }
}


