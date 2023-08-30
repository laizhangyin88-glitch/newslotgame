using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public abstract class FishSkillManagerBase<T> : MonoSingleton<T> where T : MonoSingleton<T>
    {
        protected FishGameData gameData;
        protected Dictionary<int, FishSkillItemBase> curUseSkillInsList = new Dictionary<int, FishSkillItemBase>();
        protected List<FishSkillItemBase> allUseSkillInsList = new List<FishSkillItemBase>();

        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        protected abstract void AddEventListener();

        protected abstract void RemoveEventListener();

        protected FishSkillItemBase CreateSkill(int uid, Vector3 beginPos, Vector3 nextPos, int bombCount, FishPlayerInfo playerIns, int skillStatus, float skillTime, int killFishUid, int fishId)
        {
            SkillVo skillVo = GetSkillVo(uid, nextPos, bombCount, playerIns, killFishUid);
            var skillIns = GetSkill(skillVo, fishId);
            return skillIns;
        }

        private SkillVo GetSkillVo(int uid, Vector3 nextPos, int bombCount, FishPlayerInfo playerIns, int killFishUid)
        {
            SkillVo vo = new SkillVo
            {
                UID = uid,
                NextPos = nextPos,
                BombCount = bombCount,
                PlayerIns = playerIns,
                chairId = playerIns.GetPlayerChairId(),
                IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId),
                killFishUID = killFishUid
            };
            return vo;
        }

        private FishSkillItemBase GetSkill(SkillVo skillVo, int fishId)
        {
            if (allUseSkillInsList != null && allUseSkillInsList.Count > 0)
            {
                var skillIns = allUseSkillInsList[0];
                allUseSkillInsList.RemoveAt(0);
                if (skillIns != null)
                {
                    skillIns.ResetSkillVo(skillVo);
                    curUseSkillInsList[skillVo.UID] = skillIns;
                    return skillIns;
                }
            }
            else
            {
                FishSkillItemBase skillIns = null;
                switch (fishId)
                {
                    case 47:
                        skillIns = new FishDragonTurtleSkillItem();
                        break;
                }
                if (skillIns != null)
                {
                    skillIns.ResetSkillVo(skillVo);
                    curUseSkillInsList[skillVo.UID] = skillIns;
                    return skillIns;
                }
            }
            return null;
        }

        public void ClearAllSkill()
        {
            if (curUseSkillInsList != null)
            {
                foreach (var item in curUseSkillInsList.Values)
                    item.isCanDestroy = true;
                UpdateRemoveSkill();
            }
            curUseSkillInsList.Clear();
        }

        public void ClearOtherSkill(int chairId)
        {
            if (curUseSkillInsList != null)
            {
                foreach (var item in curUseSkillInsList.Values)
                {
                    if (item.skillVo.chairId == chairId)
                        item.isCanDestroy = true;
                }
            }
        }

        private void UpdateRemoveSkill()
        {
            if (curUseSkillInsList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in curUseSkillInsList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleSkill(curUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        private void Update()
        {
            UpdateRemoveSkill();
        }

        public void RecycleSkill(SkillVo skillVo)
        {
            var tempLaternFishSkillIns = curUseSkillInsList[skillVo.UID];
            if (tempLaternFishSkillIns != null)
            {
                allUseSkillInsList.Add(tempLaternFishSkillIns);
                curUseSkillInsList.Remove(skillVo.UID);
            }
            else
                Debug.LogError("Failed to recycle skillItem => " + skillVo.UID);
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
