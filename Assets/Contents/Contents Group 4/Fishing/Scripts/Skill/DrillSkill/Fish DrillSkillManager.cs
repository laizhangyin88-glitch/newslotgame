using fishMsg;
using ParadoxNotion.Serialization.FullSerializer;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishDrillSkillManager : MonoSingleton<FishDrillSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishDrillSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishDrillSkillItem>();
        List<FishDrillSkillItem> AllUseSkillInsList = new List<FishDrillSkillItem>();
        Dictionary<int, int> SpecialDeclareList = new Dictionary<int, int>();
        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUAIM_RSP.ToString(), ResponesDrillAimMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUSHOOT_RSP.ToString(), ResponesDrillShootMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUBOMB_RSP.ToString(), ResponesDrillBombMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUAIM_RSP.ToString(), ResponesDrillAimMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUSHOOT_RSP.ToString(), ResponesDrillShootMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUBOMB_RSP.ToString(), ResponesDrillBombMsg);
        }

        public void RequestDrillAimMsg(int chairId, int drillAngel, int drillUID)
        {
            ZuanTouAimReq mes = new ZuanTouAimReq
            {
                usChairId = chairId,
                usZuanTouId = drillUID,
                usAngle = drillAngel,
            };
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUAIM_REQ, WebSocketTool.Serialize(mes));
        }

        public void ResponesDrillAimMsg(byte[] bytes)
        {
            ZuanTouAimRsp data = WebSocketTool.Deserialize<ZuanTouAimRsp>(bytes);
            var drillSkillItem = CurrentUseSkillInsList[data.usZuanTouId];
            if (drillSkillItem != null)
            {
                drillSkillItem.ChangeDrillSkillAim(data.usAngle);
            }
        }

        public void RequestDrillShootMsg(int chairId, int drillAngel, int drillUID)
        {
            ZuanTouShootReq mes = new ZuanTouShootReq
            {
                usChairId = chairId,
                usZuanTouId = drillUID,
                usAngle = drillAngel,
            };
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUSHOOT_REQ, WebSocketTool.Serialize(mes));
        }

        public void ResponesDrillShootMsg(byte[] bytes)
        {
            var data = WebSocketTool.Deserialize<ZuanTouShootRsp>(bytes);
            FishDrillSkillItem drillSkillItem;
            if (CurrentUseSkillInsList.TryGetValue(data.usZuanTouId, out drillSkillItem))
            {
                FishGameUIManager.Instance.IsGamePress(false);
                drillSkillItem.skillVo.traceId = data.usTraceId;
                drillSkillItem.skillVo.usProcUserChairId = data.usProcUserChairId;
                drillSkillItem.ShootGunDrill(data.usChairId, data.usAngle, 0);
            }
        }

        public void RequestDrillHitFishMsg(int chairId, int UID, List<int> hitFishTable, int robotChairId)
        {
            ZuanTouHitFishReq mes = new ZuanTouHitFishReq
            {
                usChairId = chairId,
                usZuanTouId = UID,
                usRobotChairId = robotChairId,
            };
            mes.SubFishes = new int[hitFishTable.Count];
            for (int i = 0; i < hitFishTable.Count; i++)
            {
                mes.SubFishes[i] = hitFishTable[i];
            }
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_ZUANTOUHITFISH_REQ, WebSocketTool.Serialize(mes));
        }

        public void ResponesDrillBombMsg(byte[] bytes)
        {
            ZuanTouBombRsp data = WebSocketTool.Deserialize<ZuanTouBombRsp>(bytes);
            FishDrillSkillItem drillSkillItem;
            bool isGet = CurrentUseSkillInsList.TryGetValue(data.usZuanTouId, out drillSkillItem);
            if (isGet)
            {
                drillSkillItem.BombDrillSkill(data.usTotalScore, data.usTotalMul);
            }
        }

        public void DrillAimOnClick()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var item in CurrentUseSkillInsList)
                {
                    item.Value.DrillAimOnClick();
                }
            }
        }

        public void EnterDrillSkillMode(CreateZuanTouRsp data)
        {
            var fishIns = FishFishManager.Instance.GetUsingFishByFishUID(data.usKilledFishId);
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usZuanTouId;
            int traceId = data.usTraceId;
            int traceStartPt = data.usTraceStartPt;
            int skillStatus = data.usZuanTouStatus;
            float skillTime = data.usZuanTouStatusTime;
            int bombFishId = data.bombFishId;
            Vector3 beginPos = Vector3.zero;
            if (fishIns != null)
            {
                beginPos = fishIns.transform.position;
            }

            if (!CurrentUseSkillInsList.ContainsKey(data.usZuanTouId) || CurrentUseSkillInsList[data.usZuanTouId] == null)
            {
                CreateDrillSkill(UID, beginPos, playerIns, traceId, traceStartPt, skillStatus, skillTime);
            }
            else
            {
                if (skillStatus == 1)
                {
                    CurrentUseSkillInsList[data.usZuanTouId].isCanDestroy = true;
                    UpdateRemoveDrillSkill();
                    CreateDrillSkill(UID, beginPos, playerIns, traceId, traceStartPt, skillStatus, skillTime);
                }
                else if (skillStatus == 2)
                {
                    Debug.LogError("钻头螃蟹正处在发射状态");
                }
            }

            if (playerIns.GetOnlineState())
            {
                ShowSpecialDeclareEffect(UID, bombFishId, playerIns);
            }
        }

        public void CreateDrillSkill(int UID, Vector3 beginPos, FishPlayerInfo playerIns, int traceID, int traceStartPt, int skillStatus, float skillTime)
        {
            var drillSkillVo = GetDrillSkillVo(UID, traceID, traceStartPt, playerIns);
            var tempdrillSkillIns = GetDrillSkill(drillSkillVo);
            if (tempdrillSkillIns != null)
            {
                tempdrillSkillIns.ResetSkillState(beginPos, skillStatus, skillTime);
            }
            else
            {
                Debug.LogError("获取DrillSkill失败==>");
            }
        }

        public SkillVo GetDrillSkillVo(int UID, int traceID, int traceStartPt, FishPlayerInfo playerIns)
        {
            SkillVo vo = new SkillVo();
            vo.UID = UID;
            vo.traceId = traceID;
            vo.startPoint = traceStartPt;
            vo.PlayerIns = playerIns;
            vo.chairId = playerIns.GetPlayerChairId();
            vo.IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId);
            return vo;
        }

        public FishDrillSkillItem GetDrillSkill(SkillVo drillSkillVo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                FishDrillSkillItem tempDrillSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempDrillSkillIns != null)
                {
                    tempDrillSkillIns.ResetSkillVo(drillSkillVo);
                    CurrentUseSkillInsList[drillSkillVo.UID] = tempDrillSkillIns;
                    return tempDrillSkillIns;
                }
            }
            else
            {
                FishDrillSkillItem tempDrillSkillIns = new FishDrillSkillItem();
                if (tempDrillSkillIns != null)
                {
                    tempDrillSkillIns.ResetSkillVo(drillSkillVo);
                    CurrentUseSkillInsList[drillSkillVo.UID] = tempDrillSkillIns;
                    return tempDrillSkillIns;
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

        public void ShowSpecialDeclareScore(int drillUID, int score, int multiple)
        {
            int specialDeclareUID = SpecialDeclareList[drillUID];
            if (specialDeclareUID != -1)
            {
                SpecialDeclareList.Remove(drillUID);
                FishSpecialDeclareEffectManager.Instance.BeginSpecialDeclareEffectChangeScore(specialDeclareUID, score, multiple);
            }
        }

        public void ClearAllDrillSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    kvp.Value.isCanDestroy = true;
                }
                UpdateRemoveDrillSkill();
            }
            CurrentUseSkillInsList.Clear();
        }

        public void ClearOtherDrillSkill(int charidID)
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    if (kvp.Value.skillVo.chairId == charidID && !kvp.Value.IsDrillShoot())
                    {
                        kvp.Value.isCanDestroy = true;
                    }
                }
                UpdateRemoveDrillSkill();
            }
        }

        public void RecycleDrillSkill(SkillVo drillSkillVo)
        {
            var tempDrillSkillIns = CurrentUseSkillInsList[drillSkillVo.UID];
            if (tempDrillSkillIns != null)
            {
                AllUseSkillInsList.Add(tempDrillSkillIns);
                CurrentUseSkillInsList.Remove(drillSkillVo.UID);
            }
            else
            {
                Debug.LogError("回收DrillSkillItem失败==> " + drillSkillVo.UID);
            }
        }

        public void UpdateRemoveDrillSkill()
        {
            if (CurrentUseSkillInsList != null && CurrentUseSkillInsList.Count > 0)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    var v = kvp.Value;
                    if (v.isCanDestroy)
                    {
                        v.Destroy();
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleDrillSkill(CurrentUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        void OnUpdate()
        {
            if (CurrentUseSkillInsList != null && CurrentUseSkillInsList.Count > 0)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    var v = kvp.Value;
                    if (!v.isCanDestroy)
                    {
                        v.Update();
                    }
                }
            }
        }

        private void Update()
        {
            OnUpdate();
            UpdateRemoveDrillSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
