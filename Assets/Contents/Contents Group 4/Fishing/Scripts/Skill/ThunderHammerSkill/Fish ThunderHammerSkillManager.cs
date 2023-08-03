using fishMsg;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishThunderHammerSkillManager : MonoSingleton<FishThunderHammerSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishThunderHammerSkillItem> curUseSkillInsList = new Dictionary<int, FishThunderHammerSkillItem> ();
        List<FishThunderHammerSkillItem> allUseSkillInsList = new List<FishThunderHammerSkillItem>();
        Dictionary<int, int> specialDeclareList = new Dictionary<int, int> ();

        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_THUNDERHAMMER_BOMB_RSP.ToString(), ResponesThunderHammerBombMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_THUNDERHAMMER_BOMB_RSP.ToString(), ResponesThunderHammerBombMsg);
        }

        private void ResponesThunderHammerBombMsg(byte[] bytes)
        {
            ThunderHammer_Bomb_Rsp data = WebSocketTool.Deserialize<ThunderHammer_Bomb_Rsp>(bytes);
            bool isGet = curUseSkillInsList.TryGetValue(data.usThunderHammerId, out FishThunderHammerSkillItem skillItem);
            if (isGet)
            {
                skillItem.PlayBombAnim(data.usTotalScore, data.usTotalMul);
            }
        }

        public void EnterSkillMode(CreateThunderHammerRsp data)
        {
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usThunderHammerId;
            int skillTime = data.usStatusTime;
            int fishId = data.bombFishId;
            int killFishUID = data.usKilledFishId;
            if (!curUseSkillInsList.ContainsKey(data.usThunderHammerId))
                CreateSkill(UID, playerIns, skillTime, killFishUID);
            else
                Debug.LogError("正在播放本雷神锤特效 => " + data.usThunderHammerId);
            if (playerIns.GetOnlineState())
                ShowSpecialDeclareEffect(UID, fishId, playerIns);
        }

        public void CreateSkill(int UID, FishPlayerInfo playerIns, int skillTime, int killFishUID)
        {
            var skillVo = GetSkillVo(UID, playerIns, killFishUID);
            var skillIns = GetSkill(skillVo);
            if (skillIns != null)
                skillIns.ResetSkillState(skillTime);
            else
                Debug.LogError("创建雷神锤失败");
        }

        public SkillVo GetSkillVo(int UID, FishPlayerInfo playerIns, int killFishUID)
        {
            SkillVo vo = new SkillVo()
            {
                UID = UID,
                PlayerIns = playerIns,
                chairId = playerIns.GetPlayerChairId(),
                IsMe = playerIns.GetPlayerChairId() == gameData.playerChairId,
                killFishUID = killFishUID
            };
            return vo;
        }

        public FishThunderHammerSkillItem GetSkill(SkillVo skillVo)
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
                FishThunderHammerSkillItem skillIns = new FishThunderHammerSkillItem();
                if (skillIns != null)
                {
                    skillIns.ResetSkillVo(skillVo);
                    curUseSkillInsList[skillVo.UID] = skillIns;
                    return skillIns;
                }
            }
            return null;
        }

        public void ShowSpecialDeclareEffect(int UID, int fishId, FishPlayerInfo playerIns)
        {
            var fishConfig = gameData.FishConfigList[fishId];
            var damageDieConfig = gameData.DieEffectConfigList[fishConfig.dieEffectId];
            var specialDeclareConfig = FishSpecialDeclareEffectManager.Instance.GetSpecialDeclareEffectConfig(damageDieConfig.specialDeclareID);
            int specialDeclareUID = FishSpecialDeclareEffectManager.Instance.SetSpecialDeclareEffectShowMode(playerIns.GetPlayerChairId(), playerIns.specialDeclarePanel.position, specialDeclareConfig);
            specialDeclareList[UID] = specialDeclareUID;
        }

        public void ShowSpecialDeclareScore(int bombUID, int score, int multiple)
        {
            int specialDeclareUID = specialDeclareList[bombUID];
            if (specialDeclareUID != -1)
            {
                specialDeclareList.Remove(bombUID);
                FishSpecialDeclareEffectManager.Instance.BeginSpecialDeclareEffectChangeScore(specialDeclareUID, score, multiple);
            }
        }

        public void ClearAllSkill()
        {
            if (curUseSkillInsList != null)
            {
                foreach (var kvp in curUseSkillInsList)
                {
                    kvp.Value.isCanDestroy = true;
                }
                UpdateRemoveSkill();
            }
            curUseSkillInsList.Clear();
        }

        public void ClearOtherPlayerSkill(int charidID)
        {
            if (curUseSkillInsList != null)
            {
                foreach (var kvp in curUseSkillInsList)
                {
                    if (kvp.Value.skillVo.chairId == charidID)
                    {
                        kvp.Value.isCanDestroy = true;
                    }
                }
                UpdateRemoveSkill();
            }
        }

        public void RecycleBombSkill(SkillVo vo)
        {
            var tempBombSkillIns = curUseSkillInsList[vo.UID];
            if (tempBombSkillIns != null)
            {
                allUseSkillInsList.Add(tempBombSkillIns);
                curUseSkillInsList.Remove(vo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle BombSkillItem => " + vo.UID);
            }
        }

        public void UpdateRemoveSkill()
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
                    RecycleBombSkill(curUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        private void Update()
        {
            foreach (var item in curUseSkillInsList.Values)
            {
                item.Update();
            }
            UpdateRemoveSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
