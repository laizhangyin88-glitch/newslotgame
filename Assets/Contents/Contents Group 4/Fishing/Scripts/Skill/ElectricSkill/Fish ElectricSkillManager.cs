using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishElectricSkillManager : MonoSingleton<FishElectricSkillManager>
    {
        FishGameData gameData;
        Dictionary<int, FishElectricSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishElectricSkillItem> ();
        List<FishElectricSkillItem> AllUseSkillInsList = new List<FishElectricSkillItem>();
        Dictionary<int, int> SpecialDeclareList = new Dictionary<int, int> ();
        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListner();
        }

        private void AddEventListner()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONAIM_RSP.ToString(), ResponesDianCiCannonAimMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONSHOOT_RSP.ToString(), ResponesDianCiCannonShootMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONDESTORY_RSP.ToString(), ResponesDianCiCannonDestroyMsg);
        }

        private void RemoveEventListner()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONAIM_RSP.ToString(), ResponesDianCiCannonAimMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONSHOOT_RSP.ToString(), ResponesDianCiCannonShootMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONDESTORY_RSP.ToString(), ResponesDianCiCannonDestroyMsg);
        }

        public void RequestDianCiCannonAimMsg(int chairId, int electricAngle, int electricUID)
        {
            DianCiCannonAimReq mes = new DianCiCannonAimReq();
            mes.usChairId = chairId;
            mes.usDianCiCannonId = electricUID;
            mes.usAngle = electricAngle;
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONAIM_REQ, WebSocketTool.Serialize(mes));
        }

        public void ResponesDianCiCannonAimMsg(byte[] bytes)
        {
            DianCiCannonAimRsp data = WebSocketTool.Deserialize<DianCiCannonAimRsp>(bytes);
            var electricSkillItem = CurrentUseSkillInsList[data.usDianCiCannonId];
            if (electricSkillItem != null)
            {
                electricSkillItem.ChangeElectricSkillAim(data.usAngle);
            }
        }

        public void RequestDianCiCannonShootMsg(int chairId, int electricAngle, int electricUID)
        {
            DianCiCannonShootReq mes = new DianCiCannonShootReq
            {
                usChairId = chairId,
                usDianCiCannonId = electricUID,
                usAngle = electricAngle
            };
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONSHOOT_REQ, WebSocketTool.Serialize(mes));
        }

        public void ResponesDianCiCannonShootMsg(byte[] bytes)
        {
            DianCiCannonShootRsp data = WebSocketTool.Deserialize<DianCiCannonShootRsp>(bytes);
            var electricSkillItem = CurrentUseSkillInsList[data.usDianCiCannonId];
            if (electricSkillItem != null)
            {
                FishGameUIManager.Instance.IsGamePress(false);
                electricSkillItem.skillVo.usProcUserChairId = data.usProcUserChairId;
                electricSkillItem.ShootGunElectric(data.usAngle, 0);
            }
        }

        public void RequestDianCiCannonHitFishMsg(int chairId, int UID, int fishUID, int robotChairID)
        {
            DianCiCannonHitFishReq mes = new DianCiCannonHitFishReq
            {
                usChairId = chairId,
                usDianCiCannonId = UID,
                usRobotChairId = robotChairID,
                SubFishes = new int[] { fishUID }
            };
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_DIANCICANNONHITFISH_REQ, WebSocketTool.Serialize(mes));
        }

        public void ResponesDianCiCannonDestroyMsg(byte[] bytes)
        {
            DianCiCannonDestroyRsp data = WebSocketTool.Deserialize<DianCiCannonDestroyRsp>(bytes);
            int specialDeclareUID = SpecialDeclareList[data.usDianCiCannonId];
            if (specialDeclareUID > 0)
            {
                ShowSpecialDeclareScore(specialDeclareUID, data.usTotalScore, data.usTotalMul);
                SpecialDeclareList[data.usDianCiCannonId] = -1;
            }
        }

        public void ElectricAimOnClick()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var item in CurrentUseSkillInsList.Values)
                {
                    item.ElectricAimOnClick();
                }
            }
        }

        public void EnterEletricSkillMode(CreateDianCiCannonRsp data)
        {
            FishFishBase fishIns = FishFishManager.Instance.GetUsingFishByFishUID(data.usKilledFishId);
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usDianCiCannonId;
            int electricSkillStatus = data.usDianCiCannonStatus;
            float electricSkillTime = data.usDianCiCannonStatusTime;
            int fishId = data.usKilledFishKind;
            Vector3 beginPos = Vector3.zero;
            if (fishIns != null)
                beginPos = fishIns.gameObject.transform.position;
            if (!CurrentUseSkillInsList.ContainsKey(UID) || CurrentUseSkillInsList[UID] == null)
                CreateEletricSkill(UID, beginPos, playerIns, electricSkillStatus, electricSkillTime);
            else
            {
                CurrentUseSkillInsList[UID].isCanDestroy = true;
                UpdateRemoveElectricSkill();
                CreateEletricSkill(UID, beginPos, playerIns, electricSkillStatus, electricSkillTime);
            }

            if (playerIns.GetOnlineState())
            {
                ShowSpecialDeclareEffect(UID, fishId, playerIns);
            }
        }

        public void CreateEletricSkill(int UID, Vector3 beginPos, FishPlayerInfo playerIns, int electricSkillStatus, float electricSkillTime)
        {
            var vo = GetEletricSkillVo(UID, playerIns);
            var skillIns = GetElectricSkill(vo);
            if (skillIns != null)
            {
                skillIns.ResetSkillState(beginPos, electricSkillStatus, electricSkillTime);
            }
            else
                Debug.LogError("获取EletricSkill失败==>" + UID);
        }

        public SkillVo GetEletricSkillVo(int UID, FishPlayerInfo playerIns)
        {
            SkillVo skillVo = new SkillVo
            {
                UID = UID,
                PlayerIns = playerIns,
                chairId = playerIns.GetPlayerChairId(),
                IsMe = playerIns.GetPlayerChairId() == gameData.playerChairId
            };
            return skillVo;
        }

        public FishElectricSkillItem GetElectricSkill(SkillVo vo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                var tempSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempSkillIns != null)
                {
                    tempSkillIns.ResetSkillVo(vo);
                    CurrentUseSkillInsList[vo.UID] = tempSkillIns;
                    return tempSkillIns;
                }
            }
            else
            {
                var tempSkillIns = new FishElectricSkillItem();
                if (tempSkillIns != null)
                {
                    tempSkillIns.ResetSkillVo(vo);
                    CurrentUseSkillInsList[vo.UID] = tempSkillIns;
                    return tempSkillIns;
                }
            }
            return null;
        }

        public void ShowSpecialDeclareEffect(int UID, int fishId, FishPlayerInfo playerIns)
        {
            var fishCfg = gameData.FishConfigList[fishId];
            var dieEffectCfg = gameData.DieEffectConfigList[fishCfg.dieEffectId];
            var specialDeclareCfg = FishSpecialDeclareEffectManager.Instance.GetSpecialDeclareEffectConfig(dieEffectCfg.specialDeclareID);
            var specialDeclareUID = FishSpecialDeclareEffectManager.Instance.SetSpecialDeclareEffectShowMode(playerIns.GetPlayerChairId(), playerIns.specialDeclarePanel.position, specialDeclareCfg);
            SpecialDeclareList[UID] = specialDeclareUID;
        }

        public void ShowSpecialDeclareScore(int specialDeclareUID, int score, int multiple)
        {
            FishSpecialDeclareEffectManager.Instance.BeginSpecialDeclareEffectChangeScore(specialDeclareUID, score, multiple);
        }

        public void ClearAllElectricSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var item in CurrentUseSkillInsList.Values)
                {
                    item.isCanDestroy = true;
                }
                UpdateRemoveElectricSkill();
                CurrentUseSkillInsList.Clear();
            }
        }

        public void ClearOtherPlayerElectricSkill(int chairId)
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var item in CurrentUseSkillInsList.Values)
                {
                    if (item.skillVo.chairId == chairId)
                        item.isCanDestroy = true;
                }
                UpdateRemoveElectricSkill();
                CurrentUseSkillInsList.Clear();
            }
        }

        public void RecycleElectricSkill(SkillVo vo)
        {
            var tempSkillIns = CurrentUseSkillInsList[vo.UID];
            if (tempSkillIns != null)
            {
                AllUseSkillInsList.Add(tempSkillIns);
                CurrentUseSkillInsList.Remove(vo.UID);
            }
            else
                Debug.LogError("回收ElectricSkillItem失败==>" + vo.UID);
        }

        public void UpdateRemoveElectricSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var item in CurrentUseSkillInsList)
                {
                    if (item.Value.isCanDestroy)
                    {
                        item.Value.Destroy();
                        removeKeyCatch.Add(item.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleElectricSkill(CurrentUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        private void Update()
        {
            foreach (var item in CurrentUseSkillInsList.Values)
            {
                item.Update();
            }
            UpdateRemoveElectricSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListner();
        }
    }
}

