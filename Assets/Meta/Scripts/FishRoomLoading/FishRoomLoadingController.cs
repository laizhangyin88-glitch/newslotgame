using BagelCode.ClientModels;
using hall;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWebSocket;

namespace BagelCode
{
    public class FishRoomLoadingController : MonoBehaviour
    {
        private Blackboard mBlackboard => GetComponent<Blackboard>();
        private bool loginSucceed;

        private void Awake()
        {
            AddEventListenner();
            GSManager.Instance.MusicVolume = 0.0f;
        }

        public IEnumerator ConnectFishServer()
        {
            //test
            //yield return true;

            WebSocketManager.Instance.ConnetServer();
            yield return new WaitUntil(() => loginSucceed);
        }

        private void AddEventListenner()
        {
            WebSocketTool.RegisterReceiveHandler(HALL_CMD.HALL_CMD_LOGIN_Rsp.ToString(), OnLoginRespone);
            MessageDispatcher.Register("ConnectedFishServer", LoginFishServer);
        }

        private void OnLoginRespone(byte[] bytes)
        {

            LoginHallRsp loginHallRsp = WebSocketTool.Deserialize<LoginHallRsp>(bytes);
            if (loginHallRsp.result == 1)
            {
                loginSucceed = true;
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "fishModels");
                BlackboardUtils.SetOrCreateValue(bb, "loginHallRsp", loginHallRsp);
            }
            else
            {
                Debug.LogError("登录失败!");
            }
        }

        private void LoginFishServer(EventData eventData)
        {
            if (eventData.name != "ConnectedFishServer")
                return;
            LoginHallReq mes = new LoginHallReq();
            mes.playerId = BlackboardQueryUtils.GetMyUserId();
            WebSocketManager.Instance.SendHallMessage(HALL_CMD.HALL_CMD_LOGIN_Req, WebSocketTool.Serialize(mes));
        }

        private void RemoveEventListenner()
        {
            WebSocketTool.UnRegisterHandler(HALL_CMD.HALL_CMD_LOGIN_Rsp.ToString(), OnLoginRespone);
            MessageDispatcher.UnRegister("ConnectedFishServer", LoginFishServer);
        }

        public void AddMetaGameDownloadList()
        {
            var downloadList = BlackboardUtils.GetOrCreateVariable<List<string>>(mBlackboard, "downloadList")?.value;

            // Hog
            if (BlackboardQueryUtils.IsHiddenObjectsActive())
                downloadList.Add("mghiddenobjectscommon");

            // Vip
            if (BlackboardQueryUtils.IsVipLoungeActive())
                downloadList.Add("mgviploungecommon");
        }

        public Orientation GetOrientation()
        {
            return BlackboardUtils.FindValue<Orientation>("/enterGameInfo/orientation");
        }

        public IEnumerator LoadTutorialSceneAsync(Orientation orientation)
        {
            bool isMobile = Common.CheckPlatform(TargetPlatform.Android | TargetPlatform.IOS);
            bool isLandscape = orientation == Orientation.LANDSCAPE || !isMobile;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isLandscape ? "In Game Tutorial Scene" : "In Game Tutorial Portrait Scene";
            Transform parent = GameObject.Find("Anchor/Right Event").transform;

            GameObject tutorialObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject obj) => tutorialObj = obj));

            var bb = GetComponent<Blackboard>();
            var gameSceneObj = bb.GetVariable<GameObject>("_gameScene")?.value;
            MetaObjectUtils.SetCalleeCaller(tutorialObj, gameSceneObj);

            tutorialObj.SetActive(true);
        }

        public IEnumerator LoadInGameSceneAsync(GameType gameType, Orientation orientation, string saveAs)
        {
            // Game Type
            switch (gameType)
            {
                case GameType.SLOT_MACHINE:
                    {
                        if (orientation == Orientation.LANDSCAPE)
                        {
                            yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                        else if (orientation == Orientation.PORTRAIT)
                        {
                            if (OrientationUtils.Instance.PossibleChangeOrientation())
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Vertical Mode Scene Slot", "Main Canvas/Area", saveAs);
                            else
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                    }
                    break;
                case GameType.VIDEO_POKER:
                    {
                        yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Video Poker", "Main Canvas/Area", saveAs);
                    }
                    break;
                case GameType.KENO:
                    {
                        yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                    }
                    break;
                case GameType.FISH:
                    {

                    }
                    break;
                default:
                    {
                        // throw exception.

                        if (orientation == Orientation.LANDSCAPE)
                        {
                            yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                        else if (orientation == Orientation.PORTRAIT)
                        {
                            if (OrientationUtils.Instance.PossibleChangeOrientation())
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Vertical Mode Scene Slot", "Main Canvas/Area", saveAs);
                            else
                                yield return LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "In Game Scene Slot", "Main Canvas/Area", saveAs);
                        }
                    }
                    break;
            }
        }

        public IEnumerator LoadContentSceneAsync(GameType gameType, Orientation orientation, string saveAs)
        {
            string gameBundleName = mBlackboard.GetValue<string>("_gameTitle");
            gameBundleName = "fishing";
            if (orientation == Orientation.LANDSCAPE)
            {
                yield return LoadSceneAsync(gameBundleName, "Game Contents Scene", "Fish Canvas", saveAs);
            }
            else if (orientation == Orientation.PORTRAIT)
            {
                yield return LoadSceneAsync(gameBundleName, "Game Contents Scene", "Fish Canvas", saveAs);
            }
            else
            {
            }
        }

        public void LoadFishGameManager(GameObject gameContent)
        {
            FishGameManager.Instance.Init(gameContent);
        }

        public IEnumerator LoadFishConfigAsync()
        {
            yield return new WaitUntil(() => FishGameManager.Instance.IsResourceLoadComplete());
        }

        private IEnumerator LoadSceneAsync(string bundleName, string assetName, string root, string saveAs)
        {
            var loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(bundleName, assetName);
            yield return new WaitUntil(() => loadSceneInfoOperation.IsDone());
            var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();
            var sceneLoadOperation = SceneManager.LoadSceneAsync(GameObject.Find(root).transform, sceneInfo, true);
            yield return new WaitUntil(() => sceneLoadOperation.IsDone());
            if (!string.IsNullOrEmpty(saveAs))
            {
                mBlackboard.AddVariable(saveAs, sceneLoadOperation.GetScene());


            }
        }

        private void OnDestroy()
        {
            RemoveEventListenner();
        }

    }
}
