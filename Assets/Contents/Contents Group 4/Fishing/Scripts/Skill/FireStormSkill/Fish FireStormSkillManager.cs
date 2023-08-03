using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFireStormSkillManager : MonoSingleton<FishFireStormSkillManager>
    {
        FishGameData gameData;
        List<FishFireStormSkillItem> AllUseSkillInsList = new List<FishFireStormSkillItem> ();
        Dictionary<int, FishFireStormSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishFireStormSkillItem>();
        int UID = 1;
        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYFIRESTORM_RSP.ToString(), ResponesFireStormDestroyMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_FIRESTORMSTATUS_RSP.ToString(), ResponesFireStormShootMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.F_FISH_CMD_FIRESTORMSCORE_RSP.ToString(), ResponesFireStormScoreMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYFIRESTORM_RSP.ToString(), ResponesFireStormDestroyMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_FIRESTORMSTATUS_RSP.ToString(), ResponesFireStormShootMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.F_FISH_CMD_FIRESTORMSCORE_RSP.ToString(), ResponesFireStormScoreMsg);
        }

        private void ResponesFireStormShootMsg(byte[] bytes)
        {
            FireStormStatusShootRsp data = WebSocketTool.Deserialize<FireStormStatusShootRsp>(bytes);
            var fireStormSkillItem = CurrentUseSkillInsList[data.usFireStormId];
            if (fireStormSkillItem != null && data.usStatus == 2)
            {
                fireStormSkillItem.BeginFireStormShoot(0);
            }
        }

        private void ResponesFireStormScoreMsg(byte[] bytes)
        {
            FireStormScoreRsp data = WebSocketTool.Deserialize<FireStormScoreRsp>(bytes);
            var fireStormSkillItem = CurrentUseSkillInsList[data.usFireStormId];
            if (fireStormSkillItem != null)
            {
                fireStormSkillItem.RefreshFireStormInfo(data.usTotalScore, data.usTotalMul);
            }
        }

        private void ResponesFireStormDestroyMsg(byte[] bytes)
        {
            DestoryFireStormRsp data = WebSocketTool.Deserialize<DestoryFireStormRsp>(bytes);
            var fireStormSkillItem = CurrentUseSkillInsList[data.usFireStormId];
            if (fireStormSkillItem != null)
            {
                fireStormSkillItem.EndFireStorm(data.usTotalScore);
            }
        }

        public void EnterFireStormSkillMode(CreateFireStormRsp data)
        {
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usFireStormId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int skillScore = data.usTotalScore;
            int skillMul = data.usTotalMul;

            if (CurrentUseSkillInsList.ContainsKey(data.usFireStormId))
            {
                CurrentUseSkillInsList[data.usFireStormId].isCanDestory = true;
                UpdateRemoveFireStormSkill();
            }

            CreateFireStormSkill(UID, playerIns, skillStatus, skillTime, skillScore, skillMul);
        }

        public void CreateFireStormSkill(int UID, FishPlayerInfo playerIns, int skillStatus, int skillTime, int skillScore, int skillMul)
        {
            SkillVo fireStormSkillVo = GetFireStormSkillVo(UID, playerIns);
            FishFireStormSkillItem tempFireStormSkillVoSkillIns = GetFireStormSkill(fireStormSkillVo);
            if (tempFireStormSkillVoSkillIns != null)
            {
                tempFireStormSkillVoSkillIns.ResetSkillState(skillStatus, skillTime, skillScore, skillMul);
            }
            else
            {
                Debug.LogError("Failed to get FireStormSkill");
            }
        }

        public SkillVo GetFireStormSkillVo(int UID, FishPlayerInfo playerIns)
        {
            SkillVo vo = new SkillVo {
                UID = UID,
                PlayerIns = playerIns,
                IsLockFish = playerIns.IsLockFish,
                chairId = playerIns.GetPlayerChairId(),
                IsMe = playerIns.GetPlayerChairId() == gameData.playerChairId,
            };
            return vo;   
        }

        public FishFireStormSkillItem GetFireStormSkill(SkillVo fireStormSkillVo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                var tempFireStormSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempFireStormSkillIns != null)
                {
                    tempFireStormSkillIns.ResetSkillVo(fireStormSkillVo);
                    CurrentUseSkillInsList[fireStormSkillVo.UID] = tempFireStormSkillIns;
                    return tempFireStormSkillIns;
                }
            }
            else
            {
                var tempFireStormSkillIns = new FishFireStormSkillItem();
                if (tempFireStormSkillIns != null)
                {
                    tempFireStormSkillIns.ResetSkillVo(fireStormSkillVo);
                    CurrentUseSkillInsList[fireStormSkillVo.UID] = tempFireStormSkillIns;
                    return tempFireStormSkillIns;
                }
            }
            return null;
        }

        public void ClearAllFireStormSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    kvp.Value.isCanDestory = true;
                }
                UpdateRemoveFireStormSkill();
            }
            CurrentUseSkillInsList.Clear();
        }

        public void ClearOtherFireStormSkill(int charidID)
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
                UpdateRemoveFireStormSkill();
            }
        }

        public void RecycleFireStormSkill(SkillVo fireStormSkillVo)
        {
            var tempFireStormSkillIns = CurrentUseSkillInsList[fireStormSkillVo.UID];
            if (tempFireStormSkillIns != null)
            {
                AllUseSkillInsList.Add(tempFireStormSkillIns);
                CurrentUseSkillInsList.Remove(fireStormSkillVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle FireStormSkillItem => " + fireStormSkillVo.UID);
            }
        }

        public void UpdateRemoveFireStormSkill()
        {
            if (CurrentUseSkillInsList != null && CurrentUseSkillInsList.Count > 0)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    if (kvp.Value.isCanDestory)
                    {
                        kvp.Value.Destroy();
                        removeKeyCatch.Add(kvp.Key);
                    }
                }

                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleFireStormSkill(CurrentUseSkillInsList[removeKeyCatch[i]].SkillVo);
                }
            }
        }

        public void OnUpdate()
        {
            if (CurrentUseSkillInsList != null && CurrentUseSkillInsList.Count > 0)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    if (!kvp.Value.isCanDestory)
                    {
                        kvp.Value.Update();
                    }
                }
            }
        }

        private void Update()
        {
            OnUpdate();
            UpdateRemoveFireStormSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
