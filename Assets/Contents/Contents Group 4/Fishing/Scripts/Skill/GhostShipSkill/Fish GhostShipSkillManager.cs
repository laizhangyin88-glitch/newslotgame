using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishGhostShipSkillManager : MonoSingleton<FishGhostShipSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishGhostShipSkillItem> curUseSkillInsList = new Dictionary<int, FishGhostShipSkillItem>();
        List<FishGhostShipSkillItem> allUseSkillInsList = new List<FishGhostShipSkillItem>();

        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_GHOSTSHIPSTATUS_RSP.ToString(), ResponesGhostShipPlayMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_GHOSTSHIPSCORE_RSP.ToString(), ResponesGhostShipScoreMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DISTORYGHOSTSHIP_RSP.ToString(), ResponesGhostShipDestroyMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_GHOSTSHIPSTATUS_RSP.ToString(), ResponesGhostShipPlayMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_GHOSTSHIPSCORE_RSP.ToString(), ResponesGhostShipScoreMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DISTORYGHOSTSHIP_RSP.ToString(), ResponesGhostShipDestroyMsg);
        }

        private void ResponesGhostShipPlayMsg(byte[] bytes)
        {
            GhostShipStatusRsp data = WebSocketTool.Deserialize<GhostShipStatusRsp>(bytes);
            if (curUseSkillInsList.ContainsKey(data.usGhostShipId))
            {
                FishGhostShipSkillItem ghostShipSkillItem = curUseSkillInsList[data.usGhostShipId];
                if (ghostShipSkillItem != null && data.usStatus == 2)
                    ghostShipSkillItem.GhostShipPlay();
            }
        }

        private void ResponesGhostShipScoreMsg(byte[] bytes)
        {
            GhostShipScoreRsp data = WebSocketTool.Deserialize<GhostShipScoreRsp>(bytes);
            var ghostShipSkill = curUseSkillInsList[data.usGhostShipId];
            if (ghostShipSkill != null)
            {
                ghostShipSkill.RefreshGhostShip(data.usTotalScore, data.usTotalMul);
            }
        }

        private void ResponesGhostShipDestroyMsg(byte[] bytes)
        {
            DestoryGhostShipRsp data = WebSocketTool.Deserialize<DestoryGhostShipRsp>(bytes);
            var ghostShipSkill = curUseSkillInsList[data.usGhostShipId];
            if (ghostShipSkill != null)
            {
                ghostShipSkill.EndGhostShipSkill(data.usTotalScore, data.usTotalMul);
            }
        }

        public void EnterGhostShipSkillMode(CreateGhostShipRsp data)
        {
            var fishIns = FishFishManager.Instance.GetUsingFishByFishUID(data.usKilledFishId);
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            var UID = data.usGhostShipId;
            var skillStatus = data.usStatus;
            var skillTime = data.usStatusTime;
            var skillScore = data.usTotalScore;
            var skillMul = data.usTotalMul;
            var direction = data.usRunDirection;

            if (!curUseSkillInsList.ContainsKey(data.usGhostShipId) || curUseSkillInsList[data.usGhostShipId] == null)
                CreateGhostShipSkill(UID, playerIns, skillStatus, skillTime, skillScore, skillMul, direction);
            else
                Debug.LogError("The GhostShip is already in the collision state");
        }

        public void CreateGhostShipSkill(int UID, FishPlayerInfo playerIns, int skillStatus, float skillTime, int skillScore, int skillMul, int direction)
        {
            SkillVo skillVo = GetGhostShipSkillVo(UID, playerIns, direction);
            var tempBisonSkillIns = GetGhostShipSkill(skillVo);
            if (tempBisonSkillIns != null)
                tempBisonSkillIns.ResetSkillState(skillStatus, skillTime, skillScore, skillMul);
            else
                Debug.LogError("Failed to get GhostShip");
        }

        public SkillVo GetGhostShipSkillVo(int UID, FishPlayerInfo playerIns, int direction)
        {
            SkillVo vo = new SkillVo();
            vo.UID = UID;
            vo.PlayerIns = playerIns;
            vo.Direction = direction;
            vo.chairId = playerIns.GetPlayerChairId();
            vo.IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId);
            return vo;
        }

        public FishGhostShipSkillItem GetGhostShipSkill(SkillVo skillVo)
        {
            if (allUseSkillInsList != null && allUseSkillInsList.Count > 0)
            {
                FishGhostShipSkillItem tempBisonSkillIns = allUseSkillInsList[0];
                allUseSkillInsList.RemoveAt(0);
                if (tempBisonSkillIns != null)
                {
                    tempBisonSkillIns.ResetSkillVo(skillVo);
                    curUseSkillInsList[skillVo.UID] = tempBisonSkillIns;
                    return tempBisonSkillIns;
                }
            }
            else
            {
                FishGhostShipSkillItem tempBisonSkillIns = new FishGhostShipSkillItem();
                if (tempBisonSkillIns != null)
                {
                    tempBisonSkillIns.ResetSkillVo(skillVo);
                    curUseSkillInsList[skillVo.UID] = tempBisonSkillIns;
                    return tempBisonSkillIns;
                }
            }
            return null;
        }

        public void ClearAllGhostShipSkill()
        {
            if (curUseSkillInsList != null)
            {
                foreach (var kvp in curUseSkillInsList)
                {
                    kvp.Value.isCanDestroy = true;
                }
                UpdateRemoveGhostShipSkill();
            }
            curUseSkillInsList.Clear();
        }

        public void ClearOtherPlayerBisonSkill(int charidID)
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
                UpdateRemoveGhostShipSkill();
            }
        }

        public void RecycleGhostShipSkill(SkillVo skillVo)
        {
            var tempBisonSkillIns = curUseSkillInsList[skillVo.UID];
            if (tempBisonSkillIns != null)
            {
                allUseSkillInsList.Add(tempBisonSkillIns);
                curUseSkillInsList.Remove(skillVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle BisonSkillItem => " + skillVo.UID);
            }
        }

        public void UpdateRemoveGhostShipSkill()
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
                    RecycleGhostShipSkill(curUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        private void Update()
        {
            foreach (var item in curUseSkillInsList.Values)
            {
                item.Update();
            }
            UpdateRemoveGhostShipSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
