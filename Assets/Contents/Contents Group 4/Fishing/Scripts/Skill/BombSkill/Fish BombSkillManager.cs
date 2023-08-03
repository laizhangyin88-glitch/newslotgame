using fishMsg;
using ParadoxNotion.Serialization.FullSerializer;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishBombSkillManager : MonoSingleton<FishBombSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishBombSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishBombSkillItem>();
        List<FishBombSkillItem> AllUseSkillInsList = new List<FishBombSkillItem>();
        Dictionary<int, int> SpecialDeclareList = new Dictionary<int, int>();
        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }
        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DELAYBOMB_BOMB_RSP.ToString(), ResponesBombCrabBombMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DELAYBOMB_BOMB_RSP.ToString(), ResponesBombCrabBombMsg);
        }

        private void ResponesBombCrabBombMsg(byte[] bytes)
        {
            DelayBomb_Bomb_Rsp data = WebSocketTool.Deserialize<DelayBomb_Bomb_Rsp>(bytes);
            FishBombSkillItem bombSkillItem;
            bool isGet = CurrentUseSkillInsList.TryGetValue(data.usDelayBombId, out bombSkillItem);
            if (isGet)
            {
                bombSkillItem.BombCrabExplosion(data.usTotalScore, data.usTotalMul);
            }
        }

        public void EnterBombSkillMode(CreateDelayBombRsp data)
        {
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usDelayBombId;
            int killFishUID = data.usKilledFishId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int bombFishId = data.bombFishId;
            Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usPosX, data.usPoxY));
            //Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(1110, 432));

            if (CurrentUseSkillInsList.ContainsKey(data.usDelayBombId) == false)
            {
                CreateBombSkill(UID, beginPos, playerIns, skillStatus, skillTime, killFishUID);
            }
            else
            {
                Debug.LogError("The bomb is currently in the exploding state");
            }

            if (playerIns.GetOnlineState() == true)
            {
                ShowSpecialDeclareEffect(UID, bombFishId, playerIns);
            }
        }

        public void CreateBombSkill(int UID, Vector3 beginPos, FishPlayerInfo playerIns, int skillStatus, int skillTime, int killFishUID)
        {
            SkillVo bombSkillVo = GetBombSkillVo(UID, playerIns, killFishUID);
            var tempBombSkillIns = GetBombSkill(bombSkillVo);
            if (tempBombSkillIns != null)
            {
                tempBombSkillIns.ResetSkillState(beginPos, skillStatus, skillTime);
            }
            else
            {
                Debug.LogError("Failed to get BombSkill");
            }
        }

        public SkillVo GetBombSkillVo(int UID, FishPlayerInfo playerIns, int killFishUID)
        {
            SkillVo vo = new SkillVo();
            vo.killFishUID = killFishUID;
            vo.UID = UID;
            vo.PlayerIns = playerIns;
            vo.chairId = playerIns.GetPlayerChairId();
            vo.IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId);
            return vo;
        }

        public FishBombSkillItem GetBombSkill(SkillVo bombSkillVo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                FishBombSkillItem tempBombSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempBombSkillIns != null)
                {
                    tempBombSkillIns.ResetSkillVo(bombSkillVo);
                    CurrentUseSkillInsList[bombSkillVo.UID] = tempBombSkillIns;
                    return tempBombSkillIns;
                }
            }
            else
            {
                FishBombSkillItem tempBombSkillIns = new FishBombSkillItem();
                if (tempBombSkillIns != null)
                {
                    tempBombSkillIns.ResetSkillVo(bombSkillVo);
                    CurrentUseSkillInsList[bombSkillVo.UID] = tempBombSkillIns;
                    return tempBombSkillIns;
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
            SpecialDeclareList[UID] = specialDeclareUID;
        }

        public void ShowSpecialDeclareScore(int bombUID, int score, int multiple)
        {
            int specialDeclareUID = SpecialDeclareList[bombUID];
            if (specialDeclareUID != -1)
            {
                SpecialDeclareList.Remove(bombUID);
                FishSpecialDeclareEffectManager.Instance.BeginSpecialDeclareEffectChangeScore(specialDeclareUID, score, multiple);
            }
        }

        public void ClearAllBombSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    kvp.Value.isCanDestory = true;
                }
                UpdateRemoveBombSkill();
            }
            CurrentUseSkillInsList.Clear();
        }

        public void ClearOtherPlayerBombSkill(int charidID)
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
                UpdateRemoveBombSkill();
            }
        }

        public void RecycleBombSkill(SkillVo bombSkillVo)
        {
            var tempBombSkillIns = CurrentUseSkillInsList[bombSkillVo.UID];
            if (tempBombSkillIns != null)
            {
                AllUseSkillInsList.Add(tempBombSkillIns);
                CurrentUseSkillInsList.Remove(bombSkillVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle BombSkillItem => " + bombSkillVo.UID);
            }
        }

        public void UpdateRemoveBombSkill()
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
                    RecycleBombSkill(CurrentUseSkillInsList[removeKeyCatch[i]].SkillVo);
                }
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseSkillInsList.Values)
            {
                item.Update();
            }
            UpdateRemoveBombSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
