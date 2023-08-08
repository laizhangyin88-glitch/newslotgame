using fishMsg;
using hall;
using SlotMaker;
using System;
using UnityEngine;

namespace BagelCode
{
    public class FishGameManager : MonoSingleton<FishGameManager>
    {
        public GameObject contentGameObject;
        public FishGameData gameData;
        public bool isFocus;
        private bool isResourceLoadComplete;

        PIDButton returnBtn;

        PIDButton autoBtn;

        PIDButton lockBtn;
        public Transform lockBtnEffect;

        PIDButton speedBtn;
        public Transform speedBtnEffect;

        public Camera UICamera;
        public Canvas fishCanvas;
        private int gameId;
        private bool isAutoShoot;
        private bool isLock;
        private bool isSpeed;
        private bool isInGame;


        public void Init(GameObject gameObject)
        {

            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            UnityEngine.Random.InitState(Convert.ToInt32(ts.TotalSeconds));
            contentGameObject = gameObject;
            InitData();
            InitInstance();

            //临时
            ResetAllManager();
        }

        private void InitData()
        {
            isFocus = true;
        }

        public void InitInstance()
        {
            gameData = new FishGameData();
            gameData.GameConfig = new FishGameConfig();
            FishResourcesManager.Instance.CorotineLoadResources();
            InitView();
        }

        public bool IsGameContentLoadComplete()
        {
            if (contentGameObject != null)
                return contentGameObject.activeSelf;
            return false;
        }

        public bool IsResourceLoadComplete()
        {
            return  isResourceLoadComplete;
        }

        public bool IsInGame()
        {
            return isInGame;
        }

        public void LoadResourcesCompleteCallBack()
        {
            isResourceLoadComplete = true;

            FishFishManager.Instance.UpdateFish();
            FishAudioManager.Instance.PlayBGAudio(22, 0.4f);
        }



        public void ShowBubble()
        {
            FishGameUIManager.Instance.ShowBubble();
        }

        public void InitView()
        {
            returnBtn = contentGameObject.transform.Find("GamePanel/UIPanel/Button Return").GetComponent<PIDButton>();
            returnBtn.onClick.AddListener(LeaveGame);

            autoBtn = contentGameObject.transform.Find("GamePanel/UIPanel/Button Auto").GetComponent<PIDButton>();
            autoBtn.onClick.AddListener(OnClickAutoBtn);

            lockBtn = contentGameObject.transform.Find("GamePanel/UIPanel/Button Lock").GetComponent<PIDButton>();
            lockBtn.onClick.AddListener(OnClickClockBtn);
            lockBtnEffect = lockBtn.transform.Find("Effect");

            speedBtn = contentGameObject.transform.Find("GamePanel/UIPanel/Button Speed").GetComponent<PIDButton>();
            speedBtn.onClick.AddListener(OnClickSpeedBtn);
            speedBtnEffect = speedBtn.transform.Find("Effect");

            UICamera = contentGameObject.transform.Find("Cameras/Camera_UI").GetComponent<Camera>();
            fishCanvas = contentGameObject.transform.Find("FishPanel").GetComponent<Canvas>();
            AddEventListener();
        }

        public void AddEventListener()
        {
            WebSocketTool.RegisterReceiveHandler(HALL_CMD.HALL_CMD_EnterGame_Rsp.ToString(), EnterGame);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_GameStatusRsp.ToString(), ResponesStateSyncMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_UserStatusRsp.ToString(), ResponesUserStateMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_SC_MSG_ChangeSceneRsp.ToString(), ResponesChangeSceneMsg);
            WebSocketTool.RegisterReceiveHandler(Proto_Fish_CMD.NF_FISH_CMD_PROMPTINFO_RSP.ToString(), ResponesSceneFishOutTipsMsg);
            WebSocketTool.RegisterReceiveHandler(HALL_CMD.HALL_CMD_ExitGame_Rsp.ToString(), ResponesExitGame);
        }


        public void RemoveEventListener()
        {
            WebSocketTool.UnRegisterHandler(HALL_CMD.HALL_CMD_EnterGame_Rsp.ToString(), EnterGame);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_GameStatusRsp.ToString(), ResponesStateSyncMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_SC_MSG_ChangeSceneRsp.ToString(), ResponesChangeSceneMsg);
            WebSocketTool.UnRegisterHandler(Proto_Fish_CMD.NF_FISH_CMD_PROMPTINFO_RSP.ToString(), ResponesSceneFishOutTipsMsg);
            WebSocketTool.UnRegisterHandler(HALL_CMD.HALL_CMD_ExitGame_Rsp.ToString(), ResponesExitGame);
        }

        public void InitGameGroup()
        {
            //原本作为初始化game Content
            //转移后Game Content已生成
        }

        public void IsShowGamePanel(bool isDisplay)
        {
            gameObject.SetActive(isDisplay);
        }

        public void LoadBinaryTraceFile(Action trackeCallBack)
        {
            bool isRotation = false;
            if (gameData.playerChairId >= 3)
            {
                isRotation = true;
            }
            string gameName = "Fishing";
            FishCsharpManager.AccordingDeskLoadBinaryTraceFile(gameName, isRotation, trackeCallBack);
        }

        public int GetPlayerSeatByServerChairId(int index)
        {
            if (gameData.playerChairId >= (gameData.playerTotalCount / 2 + 1))
            {
                if (index >= (gameData.playerTotalCount / 2 + 1))
                    return index - (gameData.playerTotalCount / 2) - 1;
                else
                    return index + (gameData.playerTotalCount / 2) - 1;
            }
            return index;
        }

        public void ShowUITips(string tipsContext, float showTime)
        {

        }

        //public void OnApplicationFocus(bool isFocus)
        //{
        //    if (!isLoadComplete)
        //        return;
        //    if (isFocus)
        //    {
        //        //todo
        //        //ClearNetMsgQueue()
        //        ResetGameState();

        //    }
        //    else
        //    {
        //        FishPlayerManager.Instance.CancelLockFishState(gameData.playerChairId);
        //        this.isFocus = isFocus;
        //        ResetAllManager();
        //    }
        //}

        public void ResetGameState()
        {
            ResetAllManager();
            SendStateSyncMsg();
        }

        public void ResetAllManager()
        {
            FishPlayerManager.Instance.ClearAllPlayerState();
            FishFishManager.Instance.ClearAllUsingFish();
            FishScoreEffectManager.Instance.ClearAllScoreEffect();
            FishBulletManager.Instance.ClearAllBullet();
            FishPlayerManager.Instance.ResetAllSeatWaitPlayerBG();
            FishGoldEffectManager.Instance.ClearAllGoldEffect();
            FishPlusTipsEffectManager.Instance.ClearAllPlusTipsEffect();
            FishSpecialDeclareEffectManager.Instance.ClearAllSpecialDeclareEffect();
            FishSpiderCrabEffectManager.Instance.ClearSpiderCrabBossHurtEffect();
            FishSpiderCrabEffectManager.Instance.ClearSpiderCrabBossScoreEffect();
            FishFishEffectManager.Instance.ClearAllFishEffect();
            FishFishOutTipsEffectManager.Instance.ClearAllFishOutTipsEffect();
            FishLightningEffectManager.Instance.ClearLightningEffect();
            FishElectricSkillManager.Instance.ClearAllElectricSkill();
            FishFireStormSkillManager.Instance.ClearAllFireStormSkill();
            FishDrillSkillManager.Instance.ClearAllDrillSkill();
            FishBisonSkillManager.Instance.ClearAllBisonSkill();
            FishBombSkillManager.Instance.ClearAllBombSkill();
            FishMultBombSkillManager.Instance.ClearAllMultBombSkill();
            FishThunderHammerSkillManager.Instance.ClearAllSkill();
            //isFocus = false;
        }

        public void EnterGame(byte[] bytes)
        {
            //todo
            hall.EnterGameRsp data = WebSocketTool.Deserialize<hall.EnterGameRsp>(bytes);
            SetBaseDeskInfoState(data);
        }

        public void SyncGameState()
        {
            LoadBinaryTraceFile(SendStateSyncMsg);
        }

        public void SendStateSyncMsg()
        {
            GameStatusReq gameStatusReq = new GameStatusReq();
            gameStatusReq.gameId = gameId;
            WebSocketManager.Instance.SendGameMessage(Proto_Fish_CMD.NF_CS_MSG_GameStatusReq, WebSocketTool.Serialize(gameStatusReq));
        }

        public void ResponesStateSyncMsg(byte[] bytes)
        {
            //todo
            GameStatusRsp gameStatusRsp = WebSocketTool.Deserialize<GameStatusRsp>(bytes);
            SyncDeskInfoState(gameStatusRsp);
        }

        public void SetBaseDeskInfoState(hall.EnterGameRsp enterGameRsp)
        {
            if (enterGameRsp.result == 0)
            {
                FishGameUIManager.Instance.gameData.playerTotalCount = (int)enterGameRsp.chair_count;
                FishGameUIManager.Instance.gameData.playerChairId = (int)enterGameRsp.my_chair_id;
                gameId = (int)enterGameRsp.game_id;
                SyncGameState();
                isInGame = true;
            }
            else
            {
                Debug.LogError("进入游戏错误ErrorID==> " + enterGameRsp.result);
                //todo
            }
        }

        public void EnterGameErrorTips(int errorId)
        {
            //todo
        }

        public void SyncDeskInfoState(GameStatusRsp gameStatusRsp)
        {
            if (gameStatusRsp.cannonlist != null)
                FishPlayerManager.Instance.InitGunLevelConfig(gameStatusRsp.cannonlist);
            else
            {
                Debug.LogError("初始化玩家炮值异常");
                ShowUITips("初始化玩家炮值异常", 3);
                return;
            }

            if (gameStatusRsp.userlist != null)
                FishPlayerManager.Instance.InitPlayerStateData(gameStatusRsp.userlist);
            else
            {
                Debug.LogError("初始化玩家信息失败");
                ShowUITips("初始化玩家信息失败", 3);
            }

            if (gameStatusRsp.background_index > 0)
                SyncGameScene((int)gameStatusRsp.background_index);
        }

        public void SyncGameScene(int sceneId)
        {
            FishGameUIManager.Instance.SysncGameSceneBG(sceneId);
            if (sceneId == 5)
                FishAudioManager.Instance.PlayBGAudio(UnityEngine.Random.Range(66, 68), 0.4f);
            else
                FishAudioManager.Instance.PlayBGAudio(22 + UnityEngine.Random.Range(0, 3), 0.4f);
        }

        public void ResponesUserStateMsg(byte[] bytes)
        {
            UserStatusRsp userStatusRsp = WebSocketTool.Deserialize<UserStatusRsp>(bytes);
            SyncUserState(userStatusRsp);
        }

        public void SyncUserState(UserStatusRsp userStatusRsp)
        {
            if (userStatusRsp != null)
            {
                for (int i = 0; i < userStatusRsp.userstatuslist.Count; i++)
                    FishPlayerManager.Instance.SyncPlayerState(userStatusRsp.userstatuslist[i]);
            }
        }

        public void ResponesChangeSceneMsg(byte[] bytes)
        {
            FishGameUIManager.Instance.ChangeGameScene(WebSocketTool.Deserialize<ChangeSceneRsp>(bytes));
        }

        public void ResponesSceneFishOutTipsMsg(byte[] bytes)
        {
            //PromptInfoRsp data = WebSocketTool.Deserialize<PromptInfoRsp>(bytes);
            //FishGameUIManager.Instance.SceneFishOutTips((int)data.infoType, (int)data.fishKindId);
        }

        public void ClearGameResources()
        {
            //todo
        }

        public void LoadGameProtoBufferFile(string filePath)
        {
            //todo
        }

        public UnityEngine.Object LoadGameResuorceText(string textPath)
        {
            UnityEngine.Object prefabBase = LoadResource(textPath, typeof(TextAsset));
            return prefabBase;
        }

        private UnityEngine.Object LoadResource(string assetPath, Type assetType)
        {
            string basePath = "Assets/Contents/Contents Group 4/Fishing/";
            string fullPath = basePath + assetPath;
            FishResourceBase prefabBase = FishCsharpResourceManager.Instance.GetResource(fullPath, assetType);
            if (prefabBase != null && prefabBase.content != null)
            {
                return prefabBase.content;
            }
            else
                Debug.LogError("资源加载失败==>" + fullPath);
            return null;
        }

        public void AsyncLoadResource(string assetPath, Type assetType, Action<FishResourceBase, float> completeCallBack)
        {
            string basePath = "Assets/Contents/Contents Group 4/Fishing/";
            string fullPath = basePath + assetPath;
            FishCsharpResourceManager.Instance.AsyncGetResource(fullPath, assetType, completeCallBack);
        }

        public void ResponesExitGame(byte[] bytes)
        {
            hall.ExitGameRsp exitGameRsp = WebSocketTool.Deserialize<hall.ExitGameRsp>(bytes);
            if (exitGameRsp.exit_type == 1)
            {
                isInGame = false;
                returnBtn.GetComponent<SendEvent>().DispatchContentEvent("LeaveGame");
                WebSocketManager.Instance.CloseConnect();
                ClearFishManager();
                GSManager.Instance.MusicVolume = 1.0f;
            }
        }

        public void ClearFishManager()
        {
            Destroy(FishResourcesManager.Instance.gameObject);
            Destroy(FishCsharpResourceManager.Instance.gameObject);
            Destroy(FishGameObjectPoolManager.Instance.gameObject);
            Destroy(FishGameUIManager.Instance.gameObject);
            Destroy(FishPlayerManager.Instance.gameObject);
            Destroy(FishFishManager.Instance.gameObject);
            Destroy(FishScoreEffectManager.Instance.gameObject);
            Destroy(FishBulletManager.Instance.gameObject);
            Destroy(FishGoldEffectManager.Instance.gameObject);
            Destroy(FishPlusTipsEffectManager.Instance.gameObject);
            Destroy(FishSpecialDeclareEffectManager.Instance.gameObject);
            Destroy(FishSpiderCrabEffectManager.Instance.gameObject);
            Destroy(FishFishEffectManager.Instance.gameObject);
            Destroy(FishFishOutTipsEffectManager.Instance.gameObject);
            Destroy(FishLightningEffectManager.Instance.gameObject);
            Destroy(FishElectricSkillManager.Instance.gameObject);
            Destroy(FishFireStormSkillManager.Instance.gameObject);
            Destroy(FishDrillSkillManager.Instance.gameObject);
            Destroy(FishBisonSkillManager.Instance.gameObject);
            Destroy(FishBombManager.Instance.gameObject);
            Destroy(FishBombSkillManager.Instance.gameObject);
            Destroy(FishAudioManager.Instance.gameObject);
            Destroy(FishMultBombSkillManager.Instance.gameObject);
            Destroy(FishThunderHammerSkillManager.Instance.gameObject);
            Destroy(WebSocketManager.Instance.gameObject);
            Destroy(gameObject);
        }

        public void LeaveGame()
        {
            fishMsg.ExitGameReq mes = new fishMsg.ExitGameReq();
            mes.reserved = 0;
            WebSocketManager.Instance.SendHallMessage(HALL_CMD.HALL_CMD_ExitGame_Req, WebSocketTool.Serialize(mes));
        }

        public void OnClickAutoBtn()
        {
            //todo Audio
            isAutoShoot = !isAutoShoot;
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(gameData.playerChairId);
            playerIns.SetAutoSendShootBullet(isAutoShoot);
            playerIns.UploadAutoShootBullet(isAutoShoot);
            autoBtn.transform.Find("Effect").gameObject.SetActive(isAutoShoot);
        }

        public void OnClickClockBtn()
        {
            //todo audio
            isLock = !isLock;
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(gameData.playerChairId);
            playerIns.UploadAutoLockFish(isLock);
        }

        public void OnClickSpeedBtn()
        {
            //todo audio
            if (!isSpeed)
                isSpeed = true;
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(gameData.playerChairId);
            int level = playerIns.SetShootBulletRateLevel();
            playerIns.UploadShootBulletRateLevel(level);
            
            //todo UI显示
        }

        public void DisConnectFishServer()
        {

        }

        protected override void OnDestroy()
        {
            RemoveEventListener();
        }
    }


}
