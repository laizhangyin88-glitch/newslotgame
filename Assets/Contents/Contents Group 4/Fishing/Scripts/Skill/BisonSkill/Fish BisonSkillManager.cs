using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishBisonSkillManager : MonoSingleton<FishBisonSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishBisonSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishBisonSkillItem>();
        List<FishBisonSkillItem> AllUseSkillInsList = new List<FishBisonSkillItem>();
        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_MADCOWSTATUS_RSP.ToString(), ResponesBisonShootMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_MADCOWSCORE_RSP.ToString(), ResponesBisonScoreMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYMADCOW_RSP.ToString(), ResponesBisonDestroyMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_MADCOWSTATUS_RSP.ToString(), ResponesBisonShootMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_MADCOWSCORE_RSP.ToString(), ResponesBisonScoreMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYMADCOW_RSP.ToString(), ResponesBisonDestroyMsg);
        }

        private void ResponesBisonShootMsg(byte[] bytes)
        {
            MadCowStatusRsp data = WebSocketTool.Deserialize<MadCowStatusRsp>(bytes);
            if (CurrentUseSkillInsList.TryGetValue(data.usMadCowIdId, out FishBisonSkillItem bisonSkillItem) && data.usStatus == 2)
            {
                bisonSkillItem.BeginBisonShoot(0);
            }
        }

        private void ResponesBisonScoreMsg(byte[] bytes)
        {
            MadCowScoreRsp data = WebSocketTool.Deserialize<MadCowScoreRsp>(bytes);
            if (CurrentUseSkillInsList.TryGetValue(data.usMadCowIdId, out FishBisonSkillItem bisonSkillItem))
            {
                bisonSkillItem.RefreshBisonInfo(data.usTotalScore, data.usTotalMul);
            }
        }

        private void ResponesBisonDestroyMsg(byte[] bytes)
        {
            DestoryMadCowRsp data = WebSocketTool.Deserialize<DestoryMadCowRsp>(bytes);
            if(CurrentUseSkillInsList.TryGetValue(data.usMadCowId, out FishBisonSkillItem bisonSkillItem))
            if (bisonSkillItem != null)
            {
                bisonSkillItem.EndBisonSkill(data.usTotalScore, data.usTotalMul);
            }
        }

        public void EnterBisonSkillMode(CreateMadCowRsp data)
        {
            var fishIns = FishFishManager.Instance.GetUsingFishByFishUID(data.usKilledFishId);
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            var UID = data.usMadCowId;
            var skillStatus = data.usStatus;
            var skillTime = data.usStatusTime;
            var skillScore = data.usTotalScore;
            var skillMul = data.usTotalMul;
            var direction = data.usRunDirection;

            if (!CurrentUseSkillInsList.ContainsKey(data.usMadCowId) || CurrentUseSkillInsList[data.usMadCowId] == null)
            {
                CreateBisonSkill(UID, playerIns, skillStatus, skillTime, skillScore, skillMul, direction);
            }
            else
            {
                Debug.LogError("The bison is already in the collision state");
            }
        }

        public void CreateBisonSkill(int UID, FishPlayerInfo playerIns, int skillStatus, float skillTime, int skillScore, int skillMul, int direction)
        {
            SkillVo bisonSkillVo = GetBisonSkillVo(UID, playerIns, direction);
            var tempBisonSkillIns = GetBisonSkill(bisonSkillVo);
            if (tempBisonSkillIns != null)
            {
                tempBisonSkillIns.ResetSkillState(skillStatus, skillTime, skillScore, skillMul);
            }
            else
            {
                Debug.LogError("Failed to get BisonSkill");
            }
        }

        public SkillVo GetBisonSkillVo(int UID, FishPlayerInfo playerIns, int direction)
        {
            SkillVo vo = new SkillVo();
            vo.UID = UID;
            vo.PlayerIns = playerIns;
            vo.Direction = direction;
            vo.chairId = playerIns.GetPlayerChairId();
            vo.IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId);
            return vo;
        }

        public FishBisonSkillItem GetBisonSkill(SkillVo bisonSkillVo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                FishBisonSkillItem tempBisonSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempBisonSkillIns != null)
                {
                    tempBisonSkillIns.ResetSkillVo(bisonSkillVo);
                    CurrentUseSkillInsList[bisonSkillVo.UID] = tempBisonSkillIns;
                    return tempBisonSkillIns;
                }
            }
            else
            {
                FishBisonSkillItem tempBisonSkillIns = new FishBisonSkillItem();
                if (tempBisonSkillIns != null)
                {
                    tempBisonSkillIns.ResetSkillVo(bisonSkillVo);
                    CurrentUseSkillInsList[bisonSkillVo.UID] = tempBisonSkillIns;
                    return tempBisonSkillIns;
                }
            }
            return null;
        }

        public void ClearAllBisonSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    kvp.Value.isCanDestory = true;
                }
                UpdateRemoveBisonSkill();
            }
            CurrentUseSkillInsList.Clear();
        }

        public void ClearOtherPlayerBisonSkill(int charidID)
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    if (kvp.Value.SkillVo.chairId == charidID)
                    {
                        kvp.Value.isCanDestory = true;
                    }
                }
                UpdateRemoveBisonSkill();
            }
        }

        public void RecycleBisonSkill(SkillVo bisonSkillVo)
        {
            var tempBisonSkillIns = CurrentUseSkillInsList[bisonSkillVo.UID];
            if (tempBisonSkillIns != null)
            {
                AllUseSkillInsList.Add(tempBisonSkillIns);
                CurrentUseSkillInsList.Remove(bisonSkillVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle BisonSkillItem => " + bisonSkillVo.UID);
            }
        }

        public void UpdateRemoveBisonSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentUseSkillInsList)
                {
                    if (item.Value.isCanDestory)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleBisonSkill(CurrentUseSkillInsList[removeKeyCatch[i]].SkillVo);
                }
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseSkillInsList.Values)
            {
                item.Update();
            }
            UpdateRemoveBisonSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
