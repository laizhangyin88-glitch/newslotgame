using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BagelCode
{
    public class FishSpiderCrabEffectManager : MonoSingleton<FishSpiderCrabEffectManager>
    {
        public enum KingEffectType
        {
            Hurt = 1,
            Kill = 2,
            Board = 3,
        }

        FishGameData gameData;
        List<FishSpiderCrabBossHurt> AllUseCrabBossHurtList = new List<FishSpiderCrabBossHurt>();
        Dictionary<int, FishSpiderCrabBossHurt> CurrentCrabBossHurtList = new Dictionary<int, FishSpiderCrabBossHurt>();

        List<FishSpiderCrabBoardScore> AllUseCrabBossScoreList = new List<FishSpiderCrabBoardScore>();
        Dictionary<int, FishSpiderCrabBoardScore> CurrentCrabBossScoreList = new Dictionary<int, FishSpiderCrabBoardScore>();

        int UID = 1;

        string SpiderCrabBossHurt_Res = "SpiderCrabBossHurt";
        string SpiderCrabBossScore_Res = "SpiderCrabBoardScore";

        private void Awake()
        {
            InitData();
        }

        private void InitData()
        {
            gameData = FishGameUIManager.Instance.gameData;
        }

        public int GetSpiderCrabEffectUID()
        {
            UID++;
            return UID;
        }

        public int CreateSpiderCrabBoardScore(int chairId, int partMul, int selfScore, int totalScore, int totalMul, Vector3 pos)
        {
            EffectVo spiderCrabBossScoreVo = GetSpiderCrabBossScoreVo(chairId, partMul, selfScore, totalScore, totalMul);
            var tempSpiderCrabBoardScoreEffectIns = GetSpiderCrabBossScoreEffect(spiderCrabBossScoreVo);
            if (tempSpiderCrabBoardScoreEffectIns != null)
                tempSpiderCrabBoardScoreEffectIns.ResetState(pos);
            return spiderCrabBossScoreVo.UID;
        }

        public EffectVo GetSpiderCrabBossScoreVo(int chairId, int partMul, int selfScore, int totalScore, int totalMul)
        {
            EffectVo vo = new EffectVo();
            vo.chairId = chairId;
            vo.partMul = partMul;
            vo.selfScore = selfScore;
            vo.totalScore = totalScore;
            vo.totalMul = totalMul;
            vo.isMe = chairId == gameData.playerChairId;
            return vo;
        }

        public FishSpiderCrabBoardScore GetSpiderCrabBossScoreEffect(EffectVo spiderCrabBossScoreVo)
        {
            if (AllUseCrabBossScoreList.Count > 0)
            {
                var tempEffectIns = AllUseCrabBossScoreList[0];
                AllUseCrabBossScoreList.RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(spiderCrabBossScoreVo);
                    CurrentCrabBossScoreList[spiderCrabBossScoreVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    Debug.LogError("创建螃蟹死亡得分提示失败==>" + spiderCrabBossScoreVo.UID);
            }
            else
            {
                var effectItem = FishGameObjectPoolManager.Instance.GetGameObject(SpiderCrabBossScore_Res, PoolType.SpecialDeclarePool);
                var tempEffectIns = new FishSpiderCrabBoardScore(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(spiderCrabBossScoreVo);
                    CurrentCrabBossScoreList[spiderCrabBossScoreVo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.SpecialDeclarePool);
                    Debug.LogError("创建螃蟹死亡得分提示失败==>" + spiderCrabBossScoreVo.UID);
                }
            }
            return null;
        }

        public void RecycleSpiderCrabBossScoreEffect(FishSpiderCrabBoardScore spiderCrabBossScoreEffectIns)
        {
            if (spiderCrabBossScoreEffectIns != null)
            {
                if (AllUseCrabBossScoreList == null)
                {
                    AllUseCrabBossScoreList = new List<FishSpiderCrabBoardScore>();
                }
                AllUseCrabBossScoreList.Add(spiderCrabBossScoreEffectIns);
                CurrentCrabBossScoreList.Remove(spiderCrabBossScoreEffectIns.effectVo.UID);
            }
            else
                Debug.LogError("移除的spiderCrabBossHurtEffectIns为nil==>UID" + spiderCrabBossScoreEffectIns.effectVo.UID);
        }

        public void ClearSpiderCrabBossScoreEffect()
        {
            if (CurrentCrabBossScoreList != null)
            {
                foreach (var item in CurrentCrabBossScoreList.Values)
                {
                    item.isCanDestroy = true;
                }
                UpdateRemoveSpiderCrabBossScoreEffect();
            }
            CurrentCrabBossScoreList?.Clear();
        }

        public void UpdateRemoveSpiderCrabBossScoreEffect()
        {
            if (CurrentCrabBossScoreList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentCrabBossScoreList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleSpiderCrabBossScoreEffect(CurrentCrabBossScoreList[removeKeyCatch[i]]);
                }
            }
        }

        public int CreateSpiderCrabBosshurt(int charId, int hurtNum, int hurtScore, Vector3 pos)
        {
            var spiderCrabBossHurtVo = GetSpiderCrabBossHurtVo(charId, hurtNum, hurtScore);
            var tempSpiderCrabBossHurtEffectIns = GetSpiderCrabBossHurtEffect(spiderCrabBossHurtVo);
            if (tempSpiderCrabBossHurtEffectIns != null)
            {
                tempSpiderCrabBossHurtEffectIns.ResetState(pos);
            }
            return spiderCrabBossHurtVo.UID;
        }

        public EffectVo GetSpiderCrabBossHurtVo(int charId, int hurtNum, int hurtScore)
        {
            EffectVo vo = new EffectVo();
            vo.UID = GetSpiderCrabEffectUID();
            vo.chairId = charId;
            vo.hurtNum = hurtNum;
            vo.score = hurtScore;
            vo.isMe = charId == gameData.playerChairId;
            return vo;
        }

        public FishSpiderCrabBossHurt GetSpiderCrabBossHurtEffect(EffectVo vo)
        {
            if (AllUseCrabBossHurtList.Count > 0)
            {
                var tempEffectIns = AllUseCrabBossHurtList[0];
                AllUseCrabBossHurtList.RemoveAt(0);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(vo);
                    CurrentCrabBossHurtList[vo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                    Debug.LogError("创建螃蟹得分提示失败==>" + vo.UID);
            }
            else
            {
                var effectItem = FishGameObjectPoolManager.Instance.GetGameObject(SpiderCrabBossHurt_Res, PoolType.SpecialDeclarePool);
                var tempEffectIns = new FishSpiderCrabBossHurt(effectItem);
                if (tempEffectIns != null)
                {
                    tempEffectIns.ResetEffectVo(vo);
                    CurrentCrabBossHurtList[vo.UID] = tempEffectIns;
                    return tempEffectIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(effectItem, PoolType.SpecialDeclarePool);
                    Debug.LogError("创建螃蟹得分提示失败==>" + vo.UID);
                }
            }
            return null;
        }

        public void RecycleSpiderCrabBossHurtEffect(FishSpiderCrabBossHurt effectIns)
        {
            if (effectIns != null)
            {
                if (AllUseCrabBossHurtList == null)
                {
                    AllUseCrabBossHurtList = new List<FishSpiderCrabBossHurt>();
                }
                AllUseCrabBossHurtList.Add(effectIns);
                CurrentCrabBossHurtList.Remove(effectIns.effectVo.UID);
            }
            else
                Debug.LogError("移除的spiderCrabBossHurtEffectIns为nil==>UID" + effectIns.effectVo.UID);
        }

        public void ClearSpiderCrabBossHurtEffect()
        {
            if (CurrentCrabBossHurtList != null)
            {
                foreach (var item in CurrentCrabBossHurtList.Values)
                {
                    item.isCanDestroy = true;
                }
                UpdateRemoveSpiderCrabBossHurtEffect();
            }
            CurrentCrabBossHurtList?.Clear();
        }

        public void UpdateRemoveSpiderCrabBossHurtEffect()
        {
            if (CurrentCrabBossHurtList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentCrabBossHurtList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleSpiderCrabBossHurtEffect(CurrentCrabBossHurtList[removeKeyCatch[i]]);
                }
            }
        }

        private void Update()
        {
            foreach (var item in CurrentCrabBossHurtList.Values)
            {
                item.Update();
            }
            foreach (var item in CurrentCrabBossScoreList.Values)
            {
                item.Update();
            }
            UpdateRemoveSpiderCrabBossHurtEffect();
            UpdateRemoveSpiderCrabBossScoreEffect();
        }
    }
}
