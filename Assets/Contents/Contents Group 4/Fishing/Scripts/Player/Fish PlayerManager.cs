using BagelCode;
using fishMsg;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishLockOnOffRsp
    {
        public int chairId;
        public bool onOff;
        public int usErrorCode;
    }

    public class FishUserState
    {
        public int chair_id;
        public bool lockfish_onoff;
        public int lockfish_uid;
        public bool autoshoot_onoff;
        public int bullet_speedlev;
    }
    public class FishPlayerManager : MonoSingleton<FishPlayerManager>
    {
        private FishGameUIManager gameUIManager;
        private FishGameData gameData;
        private long ShowFishUIDMark;
        private FishPlayerInfoPanel playerInfoPanel;
        private List<GameObject> PlayerSeatList;
        private Dictionary<int, FishPlayerInfo> PlayerInsList;
        private List<int> PlayerBulletUIDList;
        private GameObject playerSeatObj;

        private int playerCount;
        private int PlayerBulletUIDBaseMultiple;

        private void Awake()
        {
            InitData();
            InitInstance();
            InitViewData();
            InitBulletUID();
            AddEventListener();
        }

        private void Update()
        {
            foreach (var item in PlayerInsList.Values)
            {
                item.Update();
            }
        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            ShowFishUIDMark = 10000000000;
            playerCount = 6;
            PlayerBulletUIDBaseMultiple = 1000;
            PlayerBulletUIDList = new List<int>();
            PlayerInsList = new Dictionary<int, FishPlayerInfo> ();
        }


        private void InitInstance()
        {
            playerSeatObj = FishGameManager.Instance.contentGameObject.transform.Find("GamePanel/GamePanel/PlayerSeat").gameObject;
            playerInfoPanel = new FishPlayerInfoPanel(playerSeatObj);
        }

        private void InitViewData()
        {
            PlayerSeatList = playerInfoPanel.PlayerSeatList;
        }

        public void InitPlayerInfo(int index, GameObject playerPrefab)
        {
            Transform trans = playerPrefab.transform;
            trans.SetParent(PlayerSeatList[index].transform);
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            FishPlayerInfo playerInfo = new FishPlayerInfo(playerPrefab);
            playerInfo.InitPlayerData(index);
            PlayerInsList[index] = playerInfo;
        }

        public void InitPlayerStateData(List<UserInfo> playerInfoList)
        {
            int count = playerInfoList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    PlayerEnter(playerInfoList[i]);
                }
            }
        }

        public void ClearAllPlayerState()
        {
            for (int i = 0; i < PlayerInsList.Count; i++)
            {
                if (PlayerInsList[i].GetPlayerChairId() != gameData.playerChairId)
                {
                    PlayerInsList[i].PlayerLeaveState();
                }
                else
                {
                    PlayerInsList[i].ClearBulletCache();
                    PlayerInsList[i].SetAutoSendShootBullet(false);
                    PlayerInsList[i].SetOnlineState(false);
                }

            }
        }

        public void SyncPlayerState(UserStatus syncData)
        {
            FishPlayerInfo playerIns = GetPlayerInsByChairId((int)syncData.chair_id);
            if (playerIns != null)
            {
                playerIns.SetAutoLockFish(syncData.lockfish_onoff);
                if (syncData.lockfish_onoff && syncData.lockfish_uid >= 0)
                {
                    FishFishBase tempFish = FishFishManager.Instance.GetUsingFishByFishUID((int)syncData.lockfish_uid);
                    if (tempFish != null)
                    {
                        playerIns.SetLockFish(tempFish);
                    }
                }
            }
        }

        public void InitBulletUID()
        {
            for (int i = 1; i <= playerCount; i++)
            {
                int tempMultiple = PlayerBulletUIDBaseMultiple * i;
                PlayerBulletUIDList.Add(tempMultiple);
            }
        }

        public void InitGunLevelConfig(List<CannonInfo> gunInfo)
        {
            gameData.GunLevelConfig = gunInfo;
        }

        public int GetPlayerBulletUID(int serverChairId)
        {
            if (serverChairId >= 0 && serverChairId <= PlayerBulletUIDList.Count)
            {
                return PlayerBulletUIDList[serverChairId];
            }
            return 0;
        }

        public FishPlayerInfo GetPlayerInsBySeatId(int index)
        {
            return PlayerInsList[index];
        }

        public FishPlayerInfo GetPlayerInsByChairId(int chairId)
        {
            int playerIndex = FishGameManager.Instance.GetPlayerSeatByServerChairId(chairId);
            FishPlayerInfo playerIns = GetPlayerInsBySeatId(playerIndex);
            return playerIns;
        }

        public FishPlayerInfo SetPlayerChairID(int chairId)
        {
            int playerIndex = FishGameManager.Instance.GetPlayerSeatByServerChairId(chairId);
            FishPlayerInfo playerIns = GetPlayerInsBySeatId(playerIndex);
            if (playerIns != null)
            {
                return playerIns;
            }else
            {
                return null;
            }
        }

        public void SetPlayerChairID(int index, int chairId)
        {
            GetPlayerInsBySeatId(index).SetPlayerChairId(chairId);
        }

        public void PlayerEnter(UserInfo playerInfo)
        {
            FishPlayerInfo playerIns = GetPlayerInsByChairId((int)playerInfo.chair_id);
            if (playerIns != null)
            {
                IsShowWaitPlayerBG((int)playerInfo.chair_id, false);
                playerIns.PlayerEnterState(playerInfo);
            }
            else
                Debug.LogError("当前位置玩家进入异常==> " + playerInfo.chair_id);
        }

        public void OtherPlayerLeave(int playerChairID)
        {
            FishPlayerInfo playerIns = GetPlayerInsByChairId(playerChairID);
            if (playerIns != null)
            {
                IsShowWaitPlayerBG(playerChairID, true);
                playerIns.PlayerLeaveState();
            }
            else
                Debug.LogError("当前位置玩家离开异常==> " + playerChairID);
        }

        public void ServerToClientLockTragetFishProcess(LockFishRsp msg)
        {
            FishPlayerInfo playerIns = GetPlayerInsByChairId(msg.chairId);
            FishFishBase fishIns = FishFishManager.Instance.GetUsingFishByFishUID(msg.fishId);
            if (playerIns != null && fishIns != null)
            {
                playerIns.SetLockFish(fishIns);
            }
        }

        public void ServerToClientLockFishSwitchProcess(LockOnOffRsp msg)
        {
            FishPlayerInfo playerIns = GetPlayerInsByChairId(msg.chairId);
            if (playerIns != null)
            {
                playerIns.SetAutoLockFish(msg.onOff);
                playerIns.ResetLockTargetFishState(msg.onOff);
                if (msg.chairId == gameData.playerChairId)
                {
                    FishGameManager.Instance.lockBtnEffect.gameObject.SetActive(msg.onOff);
                    FishGameManager.Instance.isLock = msg.onOff;
                }
            }
        }

        public void IsShowWaitPlayerBG(int chairId, bool isDisplay)
        {
            int playerIndex = FishGameManager.Instance.GetPlayerSeatByServerChairId(chairId);
            playerInfoPanel.IsShowWaitPlayerBG(playerIndex, isDisplay);
        }

        public void ResetAllSeatWaitPlayerBG()
        {
            playerInfoPanel.ResetAllSeatWaitPlayerBG();
        }

        public void CancelLockFishState(int chairId)
        {
            GetPlayerInsByChairId(chairId).UploadAutoLockFish(false);
        }

        public void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_ChangeCannonRsp.ToString(), ResponesChangeGunMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_UserMoneyRsp.ToString(), ResponesChangePlayerMoneyMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_UserEnterDeskRsp.ToString(), ResponesPlayerEnterGameMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_UserLeaveDeskRsp.ToString(), ResponesPlayerLeaveGameMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_DOUBLEGUNONOFF_RSP.ToString(), ResponesPlayerDoubleGunMsg);
        }

        public void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_ChangeCannonRsp.ToString(), ResponesChangeGunMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_UserMoneyRsp.ToString(), ResponesChangePlayerMoneyMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_UserEnterDeskRsp.ToString(), ResponesPlayerEnterGameMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_UserLeaveDeskRsp.ToString(), ResponesPlayerLeaveGameMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_DOUBLEGUNONOFF_RSP.ToString(), ResponesPlayerDoubleGunMsg);
        }

        public void SendChangeGunMsg(int gunIndex)
        {
            ChangeCannonReq mes = new ChangeCannonReq();
            mes.cannon_id = (uint)gunIndex;
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_ChangeCannonReq, WebSocketTool.Serialize(mes));
        }

        public void ResponesChangeGunMsg(byte[] bytes)
        {
            SetChangeGun(WebSocketTool.Deserialize<ChangeCannonRsp>(bytes));
        }

        public void SetChangeGun(ChangeCannonRsp data)
        {
            FishPlayerInfo playerIns = GetPlayerInsByChairId((int)data.chair_id);
            if (playerIns != null)
            {
                playerIns.SetGunLevelValue((int)data.cannon_id);
                playerIns.SetGunParticleEffect();
            }
        }

        public void ResponesChangePlayerMoneyMsg(byte[] bytes)
        {
            UserMoneyRsp data = WebSocketTool.Deserialize<UserMoneyRsp>(bytes);
            FishPlayerInfo playerIns = GetPlayerInsByChairId((int)data.chair_id);
            if (playerIns != null)
                playerIns.SetPlayerMoneyScore(data.user_money);
        }

        public void ResponesPlayerEnterGameMsg(byte[] bytes)
        {
            PlayerEnter(WebSocketTool.Deserialize<UserEnterDeskRsp>(bytes).userInfo);
        }

        public void ResponesPlayerLeaveGameMsg(byte[] bytes)
        {
            OtherPlayerLeave((int)WebSocketTool.Deserialize<UserLeaveDeskRsp>(bytes).chair_id);
        }

        public void ResponesPlayerDoubleGunMsg(byte[] bytes)
        {
            DoubleGunOnOffRsp data = WebSocketTool.Deserialize<DoubleGunOnOffRsp>(bytes);
            FishPlayerInfo playerIns = GetPlayerInsByChairId((int)data.chairId);
            playerIns?.DoubleGunOnOffState((int)data.onOff);
        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }
}
