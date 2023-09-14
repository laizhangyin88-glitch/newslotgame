using fishMsg;
using ParadoxNotion.Serialization.FullSerializer;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace BagelCode
{
    public class FishFishManager : MonoSingleton<FishFishManager>
    {
        private FishGameData gameData;
        private FishGameUIManager gameUIManager;
        private string FishPrefixName;
        private Dictionary<int, List<FishFishBase>> AllUsedFishInsList;
        private Dictionary<int, FishFishBase> CurrentUseFishList;
        private Dictionary<int, int> FishSortingOrderList = new Dictionary<int, int>();
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            InitData();
            InitViewData();
            AddEventListener();
        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            FishPrefixName = "Fish_";
        }

        private void InitViewData()
        {
            AllUsedFishInsList = new Dictionary<int, List<FishFishBase>>();
            CurrentUseFishList = new Dictionary<int, FishFishBase>();
            FishSortingOrderList = new Dictionary<int, int>();
        }

        public FishVo CreateChildFishVo(int fishID)
        {
            FishFishConfig fishConfig = gameData.FishConfigList[fishID];
            FishVo vo = new FishVo
            {
                fishId = fishID,
                FishConfig = fishConfig
            };
            return vo;
        }

        public FishFishBase GetChildFish(int fishId)
        {
            FishVo vo = CreateChildFishVo(fishId);
            return GetFish(vo, false);
        }

        public void AddChildFishToAllUsedFishList(FishFishBase fish)
        {
            if (!AllUsedFishInsList.ContainsKey(fish.fishVo.fishId) || AllUsedFishInsList[fish.fishVo.fishId] == null)
                AllUsedFishInsList[fish.fishVo.fishId] = new List<FishFishBase>();
            AllUsedFishInsList[fish.fishVo.fishId].Add(fish);
        }

        public FishFishBase GetFish(FishVo vo, bool isAddCurrentUseFishList)
        {
            if (AllUsedFishInsList != null && AllUsedFishInsList.Count > 0
                && AllUsedFishInsList.ContainsKey(vo.fishId) && AllUsedFishInsList[vo.fishId] != null && AllUsedFishInsList[vo.fishId].Count > 0)
            {

                FishFishBase fish = AllUsedFishInsList[vo.fishId][0];
                AllUsedFishInsList[vo.fishId].RemoveAt(0);
                if (fish != null)
                {
                    if (isAddCurrentUseFishList)
                    {
                        if (CurrentUseFishList.ContainsKey(vo.UID) && CurrentUseFishList[vo.UID] != null)
                        {
                            Debug.LogError("FishUID存在相同==>> " + vo.UID);
                            AllUsedFishInsList[vo.fishId].Add(fish);
                            return null;
                        }
                    }
                    CurrentUseFishList[vo.UID] = fish;
                }
                fish.ResetFishState(vo);
                return fish;
            }
            else
            {
                string fishName = string.Format("{0}{1:D2}", FishPrefixName, vo.fishId);
                GameObject fishObj = FishGameObjectPoolManager.Instance.GetGameObject(fishName, PoolType.FishPool);
                FishFishBase fish = null;
                if (fishObj != null)
                {
                    int rawFishType = vo.FishConfig.clientBuildFishType;
                    switch (rawFishType)
                    {
                        case (int)FishGameConfig.FishType.Normal:
                            fish = new FishFish();
                            break;
                        case (int)FishGameConfig.FishType.Combo:
                            fish = new FishComboFish();
                            break;
                        case (int)FishGameConfig.FishType.Spine:
                            fish = new FishSpinFish();
                            break;
                        case (int)FishGameConfig.FishType.Tips:
                            fish = new FishTipsFish();
                            break;
                        case (int)FishGameConfig.FishType.Part:
                            fish = new FishPartFish();
                            break;
                        case (int)FishGameConfig.FishType.Dragon:
                            fish = new FishDragonFish();
                            break;
                        default:
                            Debug.LogError("rawFishType类型不存在==>" + vo.fishId);
                            break;
                    }
                    if (fish != null)
                    {
                        if (isAddCurrentUseFishList)
                        {
                            if (CurrentUseFishList.ContainsKey(vo.UID) && CurrentUseFishList[vo.UID] != null)
                            {
                                FishGameObjectPoolManager.Instance.ReCycleToGameObject(fishObj, PoolType.FishPool);
                                return null;
                            }
                            CurrentUseFishList[vo.UID] = fish;
                        }
                        fish.BuildFish(vo, fishObj);
                    }
                    else
                    {
                        FishGameObjectPoolManager.Instance.ReCycleToGameObject(fishObj, PoolType.FishPool);
                        Debug.LogError("Fish生成失败==> " + vo.fishId);
                        return null;
                    }
                }
                else
                {
                    Debug.LogError("当前FishPool中不存在==> " + fishName);
                    return null;
                }
                return fish;
            }
        }

        public void RemoveFish(FishFishBase fish)
        {
            FishFishBase tempFish = CurrentUseFishList[fish.fishVo.UID];
            if (tempFish != null)
            {
                if (!AllUsedFishInsList.ContainsKey(fish.fishVo.fishId) || AllUsedFishInsList[fish.fishVo.fishId] == null)
                {
                    AllUsedFishInsList[fish.fishVo.fishId] = new List<FishFishBase>();
                }
                AllUsedFishInsList[fish.fishVo.fishId].Add(tempFish);
                CurrentUseFishList.Remove(fish.fishVo.UID);
            }
            else
                Debug.LogError("移除的FishUID为null==> " + fish.fishVo.UID);
        }

        public void ClearAllUsingFish()
        {
            if (CurrentUseFishList != null)
            {
                List<int> removeCatch = new List<int>();
                foreach (var item in CurrentUseFishList)
                {
                    // Destroy(item.Value.gameObject);
                    item.Value.Destroy();
                    removeCatch.Add(item.Key);
                }
                for (int i = 0; i < removeCatch.Count; i++)
                {
                    RemoveFish(CurrentUseFishList[removeCatch[i]]);
                }
            }
            gameData.fishRawDataList?.Clear();
        }

        public void ClearOtherPlayerKillFish(int chairID)
        {
            if (CurrentUseFishList != null)
            {
                for (int i = 0; i < CurrentUseFishList.Count; i++)
                {
                    if (CurrentUseFishList[i].GetIsDie() && CurrentUseFishList[i].chairId == chairID && CurrentUseFishList[i].fishVo.DieEffectConfig.fishDieBehavior == 3)
                    {
                        CurrentUseFishList[i].Destroy();
                        RemoveFish(CurrentUseFishList[i]);
                    }
                }
            }
        }

        public void FishQuickOutScene(int speed)
        {
            speed = speed >= 0 ? speed : 10;
            if (CurrentUseFishList != null)
            {
                foreach (var item in CurrentUseFishList.Values)
                {
                    item.fishBehaviour.FishQuickOutScene(speed);
                }
            }
        }

        public FishFishBase GetCacheFishById(int FishUID)
        {
            FishFishBase tempFish = CurrentUseFishList[FishUID];
            if (tempFish != null)
                return tempFish;
            else
                Debug.LogError("获取的本地鱼为null==> " + FishUID);
            return null;
        }

        public FishFishBase GetUsingFishByID(int fishId)
        {
            foreach (var item in CurrentUseFishList.Values)
            {
                if (item.fishVo.fishId == fishId)
                {
                    if (!item.GetIsDie() && !item.GetIsDestroy() && item.CheckBoundValid())
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public FishFishBase GetUsingFishByFishUID(int FishUID)
        {
            FishFishBase tempFish;
            bool isGet = CurrentUseFishList.TryGetValue(FishUID, out tempFish);
            if (isGet && !tempFish.GetIsDie())
                return tempFish;
            //else
            //    Debug.LogError("获取的本地鱼为null==> " + FishUID);
            return null;
        }

        public FishVo ParseFishConfig(FishInfo msg)
        {
            FishFishConfig localConfig = gameData.FishConfigList[(int)msg.usFishKind];
            FishDieEffectConfig localDieEffectConfig = gameData.DieEffectConfigList[localConfig.dieEffectId];
            List<uint> fishKindGroup = null;
            if (msg.subFishKinds != null)
                fishKindGroup = msg.subFishKinds.ToList();
            FishVo vo = new FishVo
            {
                fishId = (int)msg.usFishKind,
                UID = (int)msg.usFishID,
                FishConfig = localConfig,
                DieEffectConfig = localDieEffectConfig,
                FishKindGroup = fishKindGroup,
                TraceId = (int)msg.usTraceId,
                StartPointIndex = (int)msg.usStartIndex,
                OffsetIndex = (int)msg.usOffsetIndex,
                OffsetPosX = msg.usOffsetPosX,
                OffsetPosY = msg.usOffsetPoxY,
                DelayBornTime = msg.usBirthDelay,
                IsRedFish = (int)msg.usIsRedFish,
                usGroupId = (int)msg.usGroupId
            };
            if (vo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Combo && vo.FishKindGroup == null)
            {
                Debug.LogError("组合鱼异常==> " + vo.fishId);
                return null;
            }
            return vo;
        }

        public void UpdateFish()
        {
            if (gameData.fishRawDataList != null && gameData.fishRawDataList.Count > 0 && FishGameManager.Instance.isFocus)
            {
                foreach (var item in gameData.fishRawDataList)
                {
                    FishVo vo = ParseFishConfig(item);
                    if (vo != null)
                    {
                        FishFishBase fish = GetFish(vo, true);
                        if (fish != null)
                        {
                            SetFishSortingOrder(fish);
                            if (vo.fishId < 47)
                                fish.BeginMove();
                            else if (vo.fishId == 48)
                                fish.transform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                gameData.fishRawDataList.Clear();
            }
        }

        public void UpdateFishAutoDestory()
        {
            if (CurrentUseFishList != null)
            {
                List<int> removeKeysCatch = new List<int>();
                foreach (KeyValuePair<int, FishFishBase> kvp in CurrentUseFishList)
                {
                    kvp.Value.Update();
                    if (kvp.Value.isCanDestroy)
                    {
                        removeKeysCatch.Add(kvp.Key);
                        kvp.Value.Destroy();
                    }
                }

                for (int i = 0; i < removeKeysCatch.Count; i++)
                {
                    RemoveFish(CurrentUseFishList[removeKeysCatch[i]]);
                }

            }
        }

        private void Update()
        {
            UpdateFishAutoDestory();
        }

        public void SetFishSortingOrder(FishFishBase fish)
        {
            int orderIndex = GetFishSortingOrderIndex(fish);
            fish.SetMainFishOrder(orderIndex);
        }

        public int GetFishSortingOrderIndex(FishFishBase fish)
        {
            int fishId = fish.fishVo.fishId;
            int layerMin = fish.fishVo.FishConfig.layerMin;
            int layerMax = fish.fishVo.FishConfig.layerMax;

            if (!FishSortingOrderList.ContainsKey(fishId))
            {
                FishSortingOrderList[fishId] = layerMin;
                return layerMin;
            }

            int currentOrder = FishSortingOrderList[fishId];
            if (currentOrder < layerMax)
            {
                currentOrder += 2;
                FishSortingOrderList[fishId] = currentOrder;
                return currentOrder;
            }
            else
            {
                FishSortingOrderList[fishId] = layerMin;
                return layerMin;
            }
        }

        public void PauseAssignationFish(List<FishFishBase> fishList)
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                if (!fishList[i].GetIsDestroy())
                {
                    fishList[i].SetFishMoveStatus(FishBehaviour.FishStatus.Pause);
                }
            }
        }

        public void ResumeAssignationFish(List<FishFishBase> fishList)
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                if (!fishList[i].GetIsDestroy() && !fishList[i].GetIsDie())
                {
                    fishList[i].SetFishMoveStatus(FishBehaviour.FishStatus.Move);
                }
            }
        }

        public FishFishBase GetCheckLockSaveFish(KillFishRsp hitFishMsg, out FishPlayerInfo playerIns)
        {
            if (hitFishMsg.usErrorCode == 0)
            {
                playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(hitFishMsg.chairId);
                if (playerIns != null)
                {
                    FishFishBase hitFish = GetUsingFishByFishUID(hitFishMsg.mainFishUID);
                    if (hitFish != null)
                        return hitFish;
                    //else
                    //    Debug.LogError("获取本地KillFish为null-KillFish的UID==> " + hitFishMsg.mainFishUID);
                }
                else
                    Debug.LogError("ResponesPlayerHitFishMsg获取玩家为null");
            }
            else
                Debug.LogError("Server返回的HitFishCodeErrer==>" + 0);
            playerIns = null;
            return null;
        }

        public void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_FishListRsp.ToString(), ReceiveFishTraceInfo);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_KillFishRsp.ToString(), ResponesKillFishNetMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_FreezeFishesRsp.ToString(), ResponesFixedScreenBombNetMsg);

            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_LockOnOffRsp.ToString(), ResponesLockFishSwitchNetMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_LockFishRsp.ToString(), ResponesLockTargetFishNetMsg);

            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEDIANCICANNON_RSP.ToString(), ResponesCreateDianCiCannonMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEZUANTOU_RSP.ToString(), ResponesCreateZuanTouMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATESOMEZUANTOU_RSP.ToString(), ResponesCreateSerialZuanTouMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEFIRESTORM_RSP.ToString(), ResponesCreateFireStormMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEMADCOW_RSP.ToString(), ResponesCreateBisonMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEGHOSTSHIP_RSP.ToString(), ResponesCreateGhostShipMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEDELAYBOMB_RSP.ToString(), ResponesCreateBombMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATESERIALBOMBCRAB_RSP.ToString(), ResponesCreateMultBombMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATETHUNDERHAMMER_RSP.ToString(), ResponesCreateThunderHammerMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEANGLERFISH_RSP.ToString(), ResponesCreateAnglerFishMsg);

            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_HAIWANGCRAB_KILLEDPART_RSP.ToString(), ResponesKillFishPartMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_HAIWANGCRABKILLEDDEAD_RSP.ToString(), ResponesKillFishDeadMsg);
        }

        public void RemoveEventListenner()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_FishListRsp.ToString(), ReceiveFishTraceInfo);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_KillFishRsp.ToString(), ResponesKillFishNetMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_FreezeFishesRsp.ToString(), ResponesFixedScreenBombNetMsg);

            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_LockOnOffRsp.ToString(), ResponesLockFishSwitchNetMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_LockFishRsp.ToString(), ResponesLockTargetFishNetMsg);

            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEDIANCICANNON_RSP.ToString(), ResponesCreateDianCiCannonMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEZUANTOU_RSP.ToString(), ResponesCreateZuanTouMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATESOMEZUANTOU_RSP.ToString(), ResponesCreateSerialZuanTouMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEFIRESTORM_RSP.ToString(), ResponesCreateFireStormMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEMADCOW_RSP.ToString(), ResponesCreateBisonMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEDELAYBOMB_RSP.ToString(), ResponesCreateBombMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATESERIALBOMBCRAB_RSP.ToString(), ResponesCreateMultBombMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATETHUNDERHAMMER_RSP.ToString(), ResponesCreateThunderHammerMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_CREATEANGLERFISH_RSP.ToString(), ResponesCreateAnglerFishMsg);

            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_HAIWANGCRAB_KILLEDPART_RSP.ToString(), ResponesKillFishPartMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_HAIWANGCRABKILLEDDEAD_RSP.ToString(), ResponesKillFishDeadMsg);
        }

        public void ReceiveFishTraceInfo(byte[] bytes)
        {
            if (!FishGameManager.Instance.isFocus)
            {
                gameData.fishRawDataList?.Clear();
                return;
            }
            FishListRsp data = WebSocketTool.Deserialize<FishListRsp>(bytes);
            if (data.Fishes != null)
            {
                SetFishTraceCondition(data.Fishes);
            }
        }

        public void SetFishTraceCondition(List<FishInfo> fishInfoList)
        {
            foreach (var item in fishInfoList)
            {
                int curIndex = (int)(item.usStartIndex + item.usOffsetIndex);
                bool isLimit = FishCsharpManager.LimitTracePoint((int)item.usTraceId, curIndex);
                if (isLimit)
                {
                    if (gameData.fishRawDataList == null)
                    {
                        gameData.fishRawDataList = new List<FishInfo>();
                    }
                    gameData.fishRawDataList.Add(item);
                }
            }
            UpdateFish();
        }

        public void RequestPlayerHitFishMsg(HitfishReq mes)
        {
            byte[] bytes = WebSocketTool.Serialize(mes);
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_HitfishReq, bytes);
        }

        public void RequestPlayerHitFishPartMsg(HaiWangCrabHitPartReq mes)
        {
            byte[] bytes = WebSocketTool.Serialize(mes);
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_FISH_CMD_HAIWANGCRABHITPART_REQ, bytes);
        }

        public void ResponesKillFishNetMsg(byte[] bytes)
        {
            if (!FishGameManager.Instance.isFocus)
                return;
            KillFishRsp killFishRsp = WebSocketTool.Deserialize<KillFishRsp>(bytes);
            ResponesPlayerKillFishProcess(killFishRsp);
        }

        public void ResponesPlayerKillFishProcess(KillFishRsp killFishRsp)
        {
            FishBombManager.Instance.CheckFishState(killFishRsp);
        }

        public void ResponesFixedScreenBombNetMsg(byte[] bytes)
        {
            FishBombManager.Instance.SetFixedScreenBombProcess(WebSocketTool.Deserialize<FreezeFishesRsp>(bytes));   
        }

        public void RequestLockFishSwitchNetMsg(LockOnOffReq mes)
        {
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_LockOnOffReq, WebSocketTool.Serialize(mes));
        }

        public void ResponesLockFishSwitchNetMsg(byte[] bytes)
        {
            LockFishSwitchProcess(WebSocketTool.Deserialize<LockOnOffRsp>(bytes));
        }

        public void LockFishSwitchProcess(LockOnOffRsp data)
        {
            if (data.usErrorCode == 0)
                FishPlayerManager.Instance.ServerToClientLockFishSwitchProcess(data);
        }

        public void RequestLockTargetFishMsg(LockFishReq mes)
        {
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_LockFishReq, WebSocketTool.Serialize(mes));
        }

        public void ResponesLockTargetFishNetMsg(byte[] bytes)
        {
            LockFishRsp data = WebSocketTool.Deserialize<LockFishRsp>(bytes);
            LockTargetFishProcess(data);
        }

        public void LockTargetFishProcess(LockFishRsp data)
        {
            if (data.usErrorCode == 0)
            {
                if (data.chairId != gameData.playerChairId)
                {
                    FishPlayerManager.Instance.ServerToClientLockTragetFishProcess(data);
                }
            }
        }

        public void ResponesCreateDianCiCannonMsg(byte[] bytes)
        {
            CreateDianCiCannonRsp data = WebSocketTool.Deserialize<CreateDianCiCannonRsp>(bytes);
            FishElectricSkillManager.Instance.EnterEletricSkillMode(data);
        }

        public void ResponesCreateZuanTouMsg(byte[] bytes)
        {
            CreateZuanTouRsp data = WebSocketTool.Deserialize<CreateZuanTouRsp>(bytes);
            FishDrillSkillManager.Instance.EnterDrillSkillMode(data);
        }

        public void ResponesCreateSerialZuanTouMsg(byte[] bytes)
        {
            CreateSomeZuanTouRsp data = WebSocketTool.Deserialize<CreateSomeZuanTouRsp>(bytes);
            FishSerialDrillSkillManager.Instance.EnterSerialDrillSkillMode(data);
        }

        public void ResponesCreateFireStormMsg(byte[] bytes)
        {
            CreateFireStormRsp data = WebSocketTool.Deserialize<CreateFireStormRsp>(bytes);
            FishFireStormSkillManager.Instance.EnterFireStormSkillMode(data);
        }

        public void ResponesCreateBisonMsg(byte[] bytes)
        {
            CreateMadCowRsp data = WebSocketTool.Deserialize<CreateMadCowRsp>(bytes);
            FishBisonSkillManager.Instance.EnterBisonSkillMode(data);
        }

        public void ResponesCreateGhostShipMsg(byte[] bytes)
        {
            CreateGhostShipRsp data = WebSocketTool.Deserialize<CreateGhostShipRsp>(bytes);
            if (data.usDieType == 1)
                FishGhostShipSkillManager.Instance.EnterGhostShipSkillMode(data);
            else
            {
                var fishIns = GetUsingFishByFishUID(data.usKilledFishId);
                StartCoroutine(ShowBossDeclareEffect(fishIns));
            }
        }

        IEnumerator ShowBossDeclareEffect(FishFishBase fishIns)
        {
            yield return new WaitUntil(fishIns.IsGetHitFishMsg);
            fishIns.ShowBossDeclareEffect();
        }

        public void ResponesCreateBombMsg(byte[] bytes)
        {
            CreateDelayBombRsp data = WebSocketTool.Deserialize<CreateDelayBombRsp>(bytes);
            FishBombSkillManager.Instance.EnterBombSkillMode(data);
        }

        public void ResponesCreateMultBombMsg(byte[] bytes)
        {
            CreateSerialBombCrabRsp data = WebSocketTool.Deserialize<CreateSerialBombCrabRsp>(bytes);
            FishMultBombSkillManager.Instance.EnterMultBombSkillMode(data);
        }

        public void ResponesCreateThunderHammerMsg(byte[] bytes)
        {
            CreateThunderHammerRsp data = WebSocketTool.Deserialize<CreateThunderHammerRsp>(bytes);
            FishThunderHammerSkillManager.Instance.EnterSkillMode(data);
        }

        public void ResponesCreateAnglerFishMsg(byte[] bytes)
        {
            CreateAnglerFishRsp data = WebSocketTool.Deserialize<CreateAnglerFishRsp>(bytes);
            if (data.usDieType == 1)
                FishLaternSkillManager.Instance.EnterLaternFishSkillMode(data);
            else
            {
                var fishIns = GetUsingFishByFishUID(data.usKilledFishId);
                if (fishIns != null)
                    StartCoroutine(ShowBossDeclareEffect(fishIns));
            }
        }

        public void ResponesKillFishPartMsg(byte[] bytes)
        {
            HaiWangCrabKilledPartRsp data = WebSocketTool.Deserialize<HaiWangCrabKilledPartRsp>(bytes);
            FishBombManager.Instance.KillPartFishSection(data);
        }

        public void ResponesKillFishDeadMsg(byte[] bytes)
        {
            HaiWangCrabKilledDeadRsp data = WebSocketTool.Deserialize<HaiWangCrabKilledDeadRsp>(bytes);
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(data.usChairId);
            FishSpiderCrabEffectManager.Instance.CreateSpiderCrabBoardScore(data.usChairId, data.usPartMul, data.usSelfScore, data.usTotalScore, data.usTotalMul, playerIns.specialDeclarePanel.position);
        }

        public void ResponesKillGhostShipMsg(byte[] bytes)
        {

        }

        public int GetFishRuleType(int fishId)
        {
            return gameData.FishConfigList[fishId].fishRuleType;
        }

        public int GetFishClientBuildFishType(int fishId)
        {
            return gameData.FishConfigList[fishId].clientBuildFishType;
        }

        protected override void OnDestroy()
        {
            RemoveEventListenner();
        }
    }

    public class SkillVo
    {
        public int UID;
        public bool IsMe;
        public bool IsLockFish;
        public int traceId;
        public int startPoint;
        public int chairId;
        public int Direction;
        public int BombCount;
        public int usProcUserChairId;
        public int killFishUID;
        public Vector3 NextPos;
        public FishPlayerInfo PlayerIns;
        public bool haveNextPos;
        public Dictionary<int, SerialDrillBulletVo> serialDrillBulletList;
    }

}
