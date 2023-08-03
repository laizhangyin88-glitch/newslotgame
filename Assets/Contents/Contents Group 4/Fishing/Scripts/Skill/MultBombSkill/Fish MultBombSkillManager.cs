using fishMsg;
using ParadoxNotion.Serialization.FullSerializer;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishMultBombSkillManager : MonoSingleton<FishMultBombSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishMultBombSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishMultBombSkillItem>();
        List<FishMultBombSkillItem> AllUseSkillInsList = new List<FishMultBombSkillItem>();
        Dictionary<int, int> SpecialDeclareList = new Dictionary<int, int>();

        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_SERIALBOMBCRAB_BOMB_RSP.ToString(), ResponesMultBombCrabBombMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYSERIALBOMBCRAB_RSP.ToString(), ResponesMultBombCrabDestroyMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_SERIALBOMBCRAB_BOMB_RSP.ToString(), ResponesMultBombCrabBombMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DESTORYSERIALBOMBCRAB_RSP.ToString(), ResponesMultBombCrabDestroyMsg);
        }

        private void ResponesMultBombCrabBombMsg(byte[] bytes)
        {
            SerialBombCrabBombRsp data = WebSocketTool.Deserialize<SerialBombCrabBombRsp>(bytes);
            var multBombSkillItem = CurrentUseSkillInsList[data.usSerialBombCrabId];
            if (multBombSkillItem != null)
            {
                multBombSkillItem.SkillVo.BombCount = data.usBombCount;
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                {
                    multBombSkillItem.SkillVo.haveNextPos = false;
                }
                else
                {
                    multBombSkillItem.SkillVo.haveNextPos = true;
                    multBombSkillItem.SkillVo.NextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                }
                multBombSkillItem.MultBombExplose();
            }
        }

        public void ResponesMultBombCrabDestroyMsg(byte[] bytes)
        {
            SerialBombCrabBombRsp data = WebSocketTool.Deserialize<SerialBombCrabBombRsp>(bytes);
            var multBombSkillItem = CurrentUseSkillInsList[data.usSerialBombCrabId];
            if (multBombSkillItem != null)
            {
                multBombSkillItem.DelayMultBombDestroy(data.usTotalScore, data.usTotalMul);
            }
        }

        public void EnterMultBombSkillMode(CreateSerialBombCrabRsp data)
        {
            var fishIns = FishFishManager.Instance.GetUsingFishByFishUID(data.usKilledFishId);
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usSerialBombCrabId;
            int skillStatus = data.usStatus;
            int skillTime = data.usStatusTime;
            int bombCount = data.usBombCount;
            int bombFishId = data.bombFishId;
            int killFishUID = data.usKilledFishId;
            if (!CurrentUseSkillInsList.ContainsKey(data.usSerialBombCrabId) || CurrentUseSkillInsList[data.usSerialBombCrabId] == null)
            {
                if (data.usNextBombPosX == 0 && data.usNextBombPosy == 0)
                {
                    return;
                }
                else
                {
                    Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usBombPosX, data.usBombPosY));
                    Vector3 nextPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.TransformPoint(FishCsharpManager.ScreenPointToRealPoint(data.usNextBombPosX, data.usNextBombPosy));
                    CreateMultBombSkill(UID, beginPos, nextPos, bombCount, playerIns, skillStatus, skillTime, killFishUID);
                }
            }
            else
            {
                Debug.LogError("MultBombSkill is currently in the exploding state");
            }

            if (playerIns.GetOnlineState() == true)
            {
                ShowSpecialDeclareEffect(UID, bombFishId, playerIns);
            }
        }

        public void CreateMultBombSkill(int UID, Vector3 beginPos, Vector3 nextPos, int bombCount, FishPlayerInfo playerIns, int skillStatus, float skillTime, int killFishUID)
        {
            SkillVo multBombSkillVo = GetMultBombSkillVo(UID, nextPos, bombCount, playerIns, killFishUID);
            var tempMultBombSkillIns = GetMultBomSkill(multBombSkillVo);
            if (tempMultBombSkillIns != null)
            {
                tempMultBombSkillIns.ResetSkillState(beginPos, skillStatus, skillTime);
            }
            else
            {
                Debug.LogError("Failed to get MultBombSkill instance");
            }
        }

        public SkillVo GetMultBombSkillVo(int UID, Vector3 nextPos, int bombCount, FishPlayerInfo playerIns, int killFishUID)
        {
            SkillVo vo = new SkillVo();
            vo.UID = UID;
            vo.NextPos = nextPos;
            vo.BombCount = bombCount;
            vo.PlayerIns = playerIns;
            vo.chairId = playerIns.GetPlayerChairId();
            vo.IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId);
            vo.killFishUID = killFishUID;
            return vo;
        }

        public FishMultBombSkillItem GetMultBomSkill(SkillVo multBombSkillVo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                var tempMultBombSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempMultBombSkillIns != null)
                {
                    tempMultBombSkillIns.ResetSkillVo(multBombSkillVo);
                    CurrentUseSkillInsList[multBombSkillVo.UID] = tempMultBombSkillIns;
                    return tempMultBombSkillIns;
                }
            }
            else
            {
                var tempMultBombSkillIns = new FishMultBombSkillItem();
                if (tempMultBombSkillIns != null)
                {
                    tempMultBombSkillIns.ResetSkillVo(multBombSkillVo);
                    CurrentUseSkillInsList[multBombSkillVo.UID] = tempMultBombSkillIns;
                    return tempMultBombSkillIns;
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

        public void ClearAllMultBombSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    kvp.Value.isCanDestory = true;
                }
                UpdateRemoveMultBombSkill();
            }
            CurrentUseSkillInsList.Clear();
        }

        public void ClearOtherPlayerMultBombSkill(int charidID)
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
                UpdateRemoveMultBombSkill();
            }
        }

        public void RecycleMultBombSkill(SkillVo multBombSkillVo)
        {
            var tempMultBombSkillIns = CurrentUseSkillInsList[multBombSkillVo.UID];
            if (tempMultBombSkillIns != null)
            {
                AllUseSkillInsList.Add(tempMultBombSkillIns);
                CurrentUseSkillInsList.Remove(multBombSkillVo.UID);
            }
            else
            {
                Debug.LogError("Failed to recycle MultBombSkillItem => " + multBombSkillVo.UID);
            }
        }

        public void UpdateRemoveMultBombSkill()
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
                    RecycleMultBombSkill(CurrentUseSkillInsList[removeKeyCatch[i]].SkillVo);
                }
            }
        }

        private void Update()
        {
            UpdateRemoveMultBombSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}

