using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{

    public class BulletVo
    {
        public int BulletUID;
        public int chairId;
        public int BulletSpeedIndex;
        public int BulletIntervalTimeIndex;
        public int BulletLevel;
        public int BulletAngle;
        public int usProcUserChairId;
        public int errorCode;
    }
    public class FishBulletManager : MonoSingleton<FishBulletManager> 
    {
        FishGameUIManager gameUIManager;
        public FishGameData gameData;
        string BulletPrefabName;
        List<FishBullet> AllBulletInsList;
        Dictionary<int, Dictionary<int, FishBullet>> CurrentUseBulletInsList;
        List<int> RemoveBulletKeyCatch;
        private void Awake()
        {
            InitData();
            AddEventListener();
        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            BulletPrefabName = "bullet_1";
            AllBulletInsList = new List<FishBullet>();
            CurrentUseBulletInsList = new Dictionary<int, Dictionary<int, FishBullet>>();
        }

        public void LocalCreatBullet(int myChairId, int bulletUID, int bulletSpeedIndex, int bulletIntervalTimeIndex, int bulletLevel)
        {
            BulletVo vo = new BulletVo
            {
                BulletUID = bulletUID,
                chairId = myChairId,
                BulletSpeedIndex = bulletSpeedIndex,
                BulletIntervalTimeIndex = bulletIntervalTimeIndex,
                BulletLevel = bulletLevel
            };
            CreateBullet(vo, false);
        }

        public void CreateBullet(BulletVo bulletVo, bool isServerCallBack)
        {
            FishBullet bulletIns = GetBullet(bulletVo);
            int playerIndex = FishGameManager.Instance.GetPlayerSeatByServerChairId(bulletVo.chairId);
            FishPlayerInfo playerIns = FishPlayerManager.Instance.GetPlayerInsBySeatId(playerIndex);
            if (playerIns != null)
            {
                bulletIns.SetBulletTargetPlayer(playerIns);
                if (isServerCallBack)
                    playerIns.ServerToShootBullet(bulletIns);
                else
                    playerIns.LocalToShootBullet(bulletIns);
            }
            else
                Debug.LogError("当前位置玩家不存在==> " + bulletVo.chairId);
        }

        public BulletVo ParseBulletMsg(ShootBulletRsp msg)
        {
            BulletVo vo = new BulletVo();
            vo.BulletUID = msg.usBulletId;
            vo.BulletAngle = msg.sAngle;
            vo.BulletSpeedIndex = msg.usSpeedIndex;
            vo.BulletIntervalTimeIndex = msg.usIntervalIndex;
            vo.BulletLevel = msg.usLevelIndex;
            vo.chairId = msg.usChairId;
            vo.errorCode = msg.usErrorCode;
            vo.usProcUserChairId = msg.usProcUserChairId;    //机器人指定真实玩家座位号上传子弹击中消息
            return vo;
        }

        public FishBullet GetBullet(BulletVo vo)
        {
            if (AllBulletInsList != null && AllBulletInsList.Count > 0)
            {
                FishBullet bulletIns = AllBulletInsList[0];
                AllBulletInsList.RemoveAt(0);
                if (bulletIns != null)
                {
                    bulletIns.UpdateBulletVo(vo);
                    bulletIns.BaseInitViewData();
                    if (!CurrentUseBulletInsList.ContainsKey(vo.chairId) || CurrentUseBulletInsList[vo.chairId] == null)
                    {
                        CurrentUseBulletInsList[vo.chairId] = new Dictionary<int, FishBullet> ();
                    }
                    CurrentUseBulletInsList[vo.chairId][vo.BulletUID] = bulletIns;
                    return bulletIns;
                }
                else
                {
                    Debug.LogError("创建Bullet失败" + vo.BulletUID);
                }
            }
            else
            {
                //var parentPrefab = FishResourcesManager.Instance.GetFishPackResList()["Fish_Pack_01"];
                //var prefab = parentPrefab.transform.Find("bullet_1").gameObject;
                //var tempBT = Instantiate<GameObject>(prefab, FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.BulletPool).transform);
                //tempBT.transform.position = new Vector3(10000, 1000, 0);

                GameObject tempBT = FishGameObjectPoolManager.Instance.GetGameObject(BulletPrefabName, PoolType.BulletPool);
                FishBullet bulletIns = new FishBullet(tempBT);
                if (bulletIns != null)
                {
                    bulletIns.UpdateBulletVo(vo);
                    bulletIns.BaseInitViewData();
                    if (!CurrentUseBulletInsList.ContainsKey(vo.chairId) || CurrentUseBulletInsList[vo.chairId] == null)
                    {
                        CurrentUseBulletInsList[vo.chairId] = new Dictionary<int, FishBullet>();
                    }
                    CurrentUseBulletInsList[vo.chairId][vo.BulletUID] = bulletIns;
                    return bulletIns;
                }
                else
                {
                    FishGameObjectPoolManager.Instance.ReCycleToGameObject(tempBT, PoolType.BulletPool);
                    Debug.LogError("创建Bullet失败" + vo.BulletUID);
                }
            }
            return null;
        }
        public int GetPlayerBulletCount(int chairID)
        {
            if (CurrentUseBulletInsList != null && CurrentUseBulletInsList.ContainsKey(chairID) && CurrentUseBulletInsList[chairID] != null)
            {
                return CurrentUseBulletInsList[chairID].Count;
            }
            return 0;
        }

        public void RecycleBullet(FishBullet bulletIns)
        {
            //Destroy(bulletIns.gameObject);
            //return;
            FishBullet tempBullet = CurrentUseBulletInsList[bulletIns.BulletVo.chairId][bulletIns.BulletVo.BulletUID];
            if (tempBullet != null)
            {
                if (AllBulletInsList == null)
                {
                    AllBulletInsList = new List<FishBullet>();
                }
                AllBulletInsList.Add(tempBullet);
                CurrentUseBulletInsList[bulletIns.BulletVo.chairId].Remove(bulletIns.BulletVo.BulletUID);
            }
            else
                Debug.LogError("移除的Bullet为nil==>UID " + bulletIns.BulletVo.BulletUID);
        }

        public void ClearAllBullet()
        {
            if (CurrentUseBulletInsList != null)
            {
                foreach (var item in CurrentUseBulletInsList.Values)
                {
                    foreach (var item1 in item.Values)
                    {
                        item1.Destroy();
                        RecycleBullet(item1);
                    }
                }
            }

            if (AllBulletInsList != null)
            {
                foreach (var item in AllBulletInsList)
                {
                    item.Destroy();
                }
            }
        }

        public void UpdateRemoveBullet()
        {
            if (CurrentUseBulletInsList != null)
            {
                foreach (var item in CurrentUseBulletInsList.Values)
                {
                    if (item.Count > 0)
                    {
                        foreach (var item1 in item)
                        {
                            item1.Value.Update();
                            if (item1.Value.isCanDestroy)
                            {
                                if (RemoveBulletKeyCatch == null)
                                {
                                    RemoveBulletKeyCatch = new List<int>();
                                }
                                item1.Value.Destroy();
                                RemoveBulletKeyCatch.Add(item1.Key);
                            }
                        }
                        if (RemoveBulletKeyCatch != null)
                        {
                            for (int i = 0; i < RemoveBulletKeyCatch.Count; i++)
                                RecycleBullet(item[RemoveBulletKeyCatch[i]]);
                            RemoveBulletKeyCatch.Clear();
                        }
                    }
                }
            }
        }

        public void Update()
        {
            UpdateRemoveBullet();
        }

        public void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_ShootBulletRsp.ToString(), ResponesBulletNetMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_AutoShootRsp.ToString(), ResponesAutoShootBulletSwitchNetMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_BulletSpeedRsp.ToString(), ResponesBulletSpeedNetMsg);
        }

        public void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_ShootBulletRsp.ToString(), ResponesBulletNetMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_AutoShootRsp.ToString(), ResponesAutoShootBulletSwitchNetMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_BulletSpeedRsp.ToString(), ResponesBulletSpeedNetMsg);
        }

        public void RequestSendBulletMsg(int bulletAngle, int bulletUID)
        {
            ShootBulletReq sendBullet = new ShootBulletReq();
            sendBullet.sAngle = bulletAngle;
            sendBullet.usBulletId = (uint)bulletUID;
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_ShootBulletReq, WebSocketTool.Serialize(sendBullet));
        }

        public void ResponesBulletNetMsg(byte[] bytes)
        {
            if (!FishGameManager.Instance.isFocus)
                return;
            ResponesBulletMsg(WebSocketTool.Deserialize<ShootBulletRsp>(bytes));
        }

        public void ResponesBulletMsg(ShootBulletRsp msg)
        {
            BulletVo bulletVo = ParseBulletMsg(msg);
            if (bulletVo.errorCode == 0)
            {
                if (bulletVo.chairId != gameData.playerChairId)
                {
                    CreateBullet(bulletVo, true);
                }
            }
        }

        public void RequestAutoShootBulletSwitchMsg(AutoShootReq mes)
        {
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_AutoShootReq, WebSocketTool.Serialize(mes));
        }

        public void ResponesAutoShootBulletSwitchNetMsg(byte[] bytes)
        {
            AutoShootReq autoShootReq = WebSocketTool.Deserialize<AutoShootReq>(bytes);
            ResponesAutoShootBulletMsgProcess(autoShootReq);
        }

        public void ResponesAutoShootBulletMsgProcess(AutoShootReq autoShootReq)
        {

        }

        public void RequestBulletSpeedNetMsg(BulletSpeedReq mes)
        {
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_BulletSpeedReq, WebSocketTool.Serialize(mes));
        }

        public void ResponesBulletSpeedNetMsg(byte[] bytes)
        {
            BulletSpeedRsp msg = WebSocketTool.Deserialize<BulletSpeedRsp>(bytes);
            if (msg.usErrorCode != 0 || msg.usChairId == gameData.playerChairId)
                return;
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(msg.usChairId);
            playerIns.SetShootBulletRateLevel(msg.usIntervalIndex);
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
