using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class SerialDrillBulletVo
    {
        public int serialDrillBulletID;
        public int startPoint;
        public int traceId;
        public int angle;
    }

    public class FishSerialDrillSkillManager : MonoSingleton<FishSerialDrillSkillManager>
    {
        public FishGameData gameData;
        public List<FishSerialDrillSkillItem> AllUseSkillInsList = new List<FishSerialDrillSkillItem>();
        public Dictionary<int, FishSerialDrillSkillItem> CurrentUseSkillInsList = new Dictionary<int, FishSerialDrillSkillItem>();

        private void Awake()
        {
            gameData = FishGameUIManager.Instance.gameData;
            AddEventListener();
        }

        private void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_SOMEZUANTOUSHOOT_RSP.ToString(), ResponesSerialDrillShootMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_SOMEZUANTOBOMB_RSP.ToString(), ResponesSerialDrillBombMsg);
        }

        private void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_SOMEZUANTOUSHOOT_RSP.ToString(), ResponesSerialDrillShootMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_SOMEZUANTOBOMB_RSP.ToString(), ResponesSerialDrillBombMsg);
        }

        private void ResponesSerialDrillShootMsg(byte[] bytes)
        {
            SomeZuanTouShootRsp data = WebSocketTool.Deserialize<SomeZuanTouShootRsp>(bytes);
            var serialDrillSkillItem = CurrentUseSkillInsList[data.usSomeZuanTouId];
            if (serialDrillSkillItem != null)
            {
                SerialDrillBulletVo vo =new SerialDrillBulletVo();
                vo.serialDrillBulletID = data.someZuanTou.usZuanTouId;
                vo.traceId = data.someZuanTou.usAngle;
                vo.startPoint = data.someZuanTou.usTraceStartPt;
                serialDrillSkillItem.UpdateSkillVo(vo.serialDrillBulletID, vo);
                serialDrillSkillItem.ShootSerialGunDrill(vo.serialDrillBulletID, vo.angle);
            }
        }

        public void RequestSerialDrillHitFishMsg(int chairID, int serialDrillUID, int bulletUID, List<int> hitFishTable)
        {
            SomeZuanTouHitFishReq drillHitFishs = new SomeZuanTouHitFishReq();
            drillHitFishs.usChairId = chairID;
            drillHitFishs.usSomeZuanTouId = serialDrillUID;
            drillHitFishs.usZuanTouId = bulletUID;
            drillHitFishs.SubFishes.AddRange(hitFishTable);
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_SOMEZUANTOUHITFISH_REQ, WebSocketTool.Serialize(drillHitFishs));
        }

        public void ResponesSerialDrillBombMsg(byte[] msg)
        {
            SomeZuanTouBombRsp data = WebSocketTool.Deserialize<SomeZuanTouBombRsp>(msg);
            var serialDrillSkillItem = CurrentUseSkillInsList[data.usSomeZuanTouId];
            if (serialDrillSkillItem != null)
            {
                serialDrillSkillItem.BombSerialDrillSkill();
            }
        }

        public void EnterSerialDrillSkillMode(CreateSomeZuanTouRsp data)
        {
            FishFishBase fishIns = FishFishManager.Instance.GetUsingFishByFishUID(data.usKilledFishId);
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            int UID = data.usSomeZuanTouId;
            Dictionary<int, SerialDrillBulletVo> serialDrillBulletList = new Dictionary<int, SerialDrillBulletVo>();

            if (data.zuanTous != null && data.zuanTous.Count > 0)
            {
                foreach (var zuanTou in data.zuanTous)
                {
                    SerialDrillBulletVo bulletInfo = new SerialDrillBulletVo();
                    bulletInfo.serialDrillBulletID = zuanTou.usZuanTouId;
                    bulletInfo.angle = zuanTou.usAngle;
                    bulletInfo.traceId = zuanTou.usTraceId;
                    bulletInfo.startPoint = zuanTou.usTraceStartPt;
                    serialDrillBulletList[bulletInfo.serialDrillBulletID] = bulletInfo;
                }
            }
            int skillStatus = data.usZuanTouStatus;
            float skillTime = data.usZuanTouStatusTime;
            Vector3 beginPos = fishIns.gameObject.transform.parent.InverseTransformPoint(Vector3.zero);
            CreateSerialDrillSkill(UID, beginPos, playerIns, serialDrillBulletList, skillStatus, skillTime);
        }

        public void CreateSerialDrillSkill(int UID, Vector3 beginPos, FishPlayerInfo playerIns, Dictionary<int, SerialDrillBulletVo> serialDrillBulletList, int skillStatus, float skillTime)
        {
            SkillVo serialDrillSkillVo = GetSerialDrillSkillVo(UID, serialDrillBulletList, playerIns);
            FishSerialDrillSkillItem tempSerialDrillSkillIns = GetSerialDrillSkill(serialDrillSkillVo);
            if (tempSerialDrillSkillIns != null)
            {
                tempSerialDrillSkillIns.ResetSkillState(beginPos, skillStatus, skillTime);
            }
            else
            {
                Debug.LogError("获取SerialDrillSkill失败==>");
            }
        }

        public SkillVo GetSerialDrillSkillVo(int UID, Dictionary<int, SerialDrillBulletVo> serialDrillBulletList, FishPlayerInfo playerIns)
        {
            SkillVo vo = new SkillVo();
            vo.UID = UID;
            vo.serialDrillBulletList = serialDrillBulletList;
            vo.PlayerIns = playerIns;
            vo.IsMe = (playerIns.GetPlayerChairId() == gameData.playerChairId);
            return vo;
        }

        public FishSerialDrillSkillItem GetSerialDrillSkill(SkillVo serialDrillSkillVo)
        {
            if (AllUseSkillInsList != null && AllUseSkillInsList.Count > 0)
            {
                FishSerialDrillSkillItem tempSerialDrillSkillIns = AllUseSkillInsList[0];
                AllUseSkillInsList.RemoveAt(0);
                if (tempSerialDrillSkillIns != null)
                {
                    tempSerialDrillSkillIns.ResetSkillVo(serialDrillSkillVo);
                    CurrentUseSkillInsList[serialDrillSkillVo.UID] = tempSerialDrillSkillIns;
                    return tempSerialDrillSkillIns;
                }
            }
            else
            {
                FishSerialDrillSkillItem tempSerialDrillSkillIns = new FishSerialDrillSkillItem();
                if (tempSerialDrillSkillIns != null)
                {
                    tempSerialDrillSkillIns.ResetSkillVo(serialDrillSkillVo);
                    CurrentUseSkillInsList[serialDrillSkillVo.UID] = tempSerialDrillSkillIns;
                    return tempSerialDrillSkillIns;
                }
            }
            return null;
        }

        public void ClearAllSerialDrillSkill()
        {
            if (CurrentUseSkillInsList != null)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    kvp.Value.isCanDestroy = true;
                }
                UpdateRemoveSerialDrillSkill();
            }
        }

        public void RecycleSerialDrillSkill(SkillVo serialDrillSkillVo)
        {
            if (CurrentUseSkillInsList.ContainsKey(serialDrillSkillVo.UID))
            {
                var tempSerialDrillSkillIns = CurrentUseSkillInsList[serialDrillSkillVo.UID];
                if (tempSerialDrillSkillIns != null)
                {
                    AllUseSkillInsList.Add(tempSerialDrillSkillIns);
                    CurrentUseSkillInsList.Remove(serialDrillSkillVo.UID);
                }
            }
            else
            {
                Debug.LogError("回收SerialDrillSkillItem失败 ==> " + serialDrillSkillVo.UID);
            }
        }

        public void UpdateRemoveSerialDrillSkill()
        {
            if (CurrentUseSkillInsList != null && CurrentUseSkillInsList.Count > 0)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    if (kvp.Value.isCanDestroy)
                    {
                        kvp.Value.Destroy();
                        removeKeyCatch.Add(kvp.Key);
                        //RecycleSerialDrillSkill(kvp.Value);
                    }
                }

                for (int i = 0; i < removeKeyCatch[i]; i++)
                {
                    RecycleSerialDrillSkill(CurrentUseSkillInsList[removeKeyCatch[i]].skillVo);
                }
            }
        }

        public void OnUpdate()
        {
            if (CurrentUseSkillInsList != null && CurrentUseSkillInsList.Count > 0)
            {
                foreach (var kvp in CurrentUseSkillInsList)
                {
                    if (!kvp.Value.isCanDestroy)
                    {
                        kvp.Value.Update();
                    }
                }
            }
        }

        public void Update()
        {
            OnUpdate();
            UpdateRemoveSerialDrillSkill();
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
