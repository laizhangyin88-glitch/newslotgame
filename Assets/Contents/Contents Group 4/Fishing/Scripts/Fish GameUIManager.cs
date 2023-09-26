using fishMsg;
using SlotMaker;
using SlotMaker.IoC.Tween;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishGameUIManager : MonoSingleton<FishGameUIManager>
    {
        public FishGameData gameData;
        private Dictionary<int, Texture> AllGameBG;
        private bool GameIsPress;
        private CanvasScaler canvasScaler;
        public GameObject GamePanelRoot;
        private GameObject Bg;
        private RawImage HaiWang_Bg;
        private RawImage GameBG;
        private RawImage BG2;
        private RectTransform tideGroupRectTrans;
        public Camera UICamera;
        private FishBehaviour BGFishLuaBehaviour;
        private Timer changeSceneClearTimer;
        public GameObject conetnGameObject;

        private void Awake()
        {
            conetnGameObject = FishGameManager.Instance.contentGameObject;
            InitData();
            InitView();
            FindView();
            AddEventListener();
        }

        private void InitData()
        {
            gameData = FishGameManager.Instance.gameData;
            AllGameBG = new Dictionary<int, Texture>();
            GameIsPress = true;
        }

        private void InitView()
        {
            InitInstance();
            InitUIViewData();
        }

        private void InitInstance()
        {
            //Todo
        }

        private void InitUIViewData()
        {
            FishCsharpManager.Init(gameData.ResolutionWidth, gameData.ResolutionHeight);
            SetScreenResoulution();
        }

        private void SetScreenResoulution()
        {
            canvasScaler = conetnGameObject.GetComponentInChildren<CanvasScaler>();
            float ratio = 1f;
            float tmpRatio = ((float)Screen.width / (float)Screen.height);
            if (ratio != tmpRatio)
            {
                ratio = tmpRatio;
                gameData.ResolutionHeight = canvasScaler.referenceResolution.y;
                gameData.ResolutionHeightHalf = Mathf.FloorToInt(gameData.ResolutionHeight / 2f);
                gameData.ResolutionWidth = Mathf.FloorToInt(gameData.ResolutionHeight * ratio);
                gameData.ResolutionWidthHalf = Mathf.FloorToInt(gameData.ResolutionWidth / 2f);
                FishCsharpManager.curResolutionWidth = gameData.ResolutionWidth;
                FishCsharpManager.curResolutionHeight = gameData.ResolutionHeight;
            }
        }

        public void FindView()
        {
            GamePanelRoot = conetnGameObject.transform.Find("GamePanel").gameObject;
            Bg = conetnGameObject.transform.Find("GamePanel/GameBGPanel/BGPanel/RawImage").gameObject;
            HaiWang_Bg = conetnGameObject.transform.Find("GamePanel/GameBGPanel/BGPanel/HaiWang_RawImage").GetComponent<RawImage>();
            HaiWang_Bg.gameObject.SetActive(false);
            GameBG = Bg.GetComponent<RawImage>();
            BG2 = conetnGameObject.transform.Find("GamePanel/GameBGPanel/BGPanel/BG2").GetComponent<RawImage>();
            BG2.gameObject.SetActive(false);
            tideGroupRectTrans = conetnGameObject.transform.Find("GamePanel/TidePanel/TideGroup").GetComponent<RectTransform>();
            tideGroupRectTrans.gameObject.SetActive(false);
            UICamera = conetnGameObject.transform.Find("Cameras/Camera_UI").GetComponent<Camera>();
            BGFishLuaBehaviour = Bg.AddComponent<FishBehaviour>();
            //TODO
            //TideSequence = null;
            //ChangeBGSequence = null;
        }

        public void ShowBubble()
        {
            tideGroupRectTrans.gameObject.SetActive(true);
        }

        public void AddEventListener()
        {
            BGFishLuaBehaviour.onPressCallBack = (isPress) => {
                BGOnPress(isPress);
            };
        }

        public void IsGamePress(bool press)
        {
            GameIsPress = press;
        }

        public void BGOnPress(bool isPress)
        {
            FishElectricSkillManager.Instance.ElectricAimOnClick();
            FishDrillSkillManager.Instance.DrillAimOnClick();


            if (!GameIsPress) return;
            FishPlayerManager.Instance.GetPlayerInsByChairId(gameData.playerChairId).OnClickSendShootBullet(isPress);
        }

        public void SysncGameSceneBG(int sceneId)
        {
            SetGameBG(sceneId, false);
            FishTideOver(sceneId);
        }

        public void SetGameBG(int index, bool isFadeAnimation)
        {
            isFadeAnimation = true;
            if (!AllGameBG.ContainsKey(index) || AllGameBG[index] == null)
            {
                Texture texture = AssetBundleManager.LoadAsset<Texture>("fishingbg", gameData.GameConfig.BGRes[index].name);
                if (texture != null)
                {
                    AllGameBG[index] = texture;
                    GameBG.texture = AllGameBG[index];
                    ChangeSceneBGImage(index, isFadeAnimation);
                }
                else
                    Debug.LogError("资源加载失败==> " + gameData.GameConfig.BGRes[index].name);
            }
            else
            {
                GameBG.texture = AllGameBG[index];
                ChangeSceneBGImage(index, isFadeAnimation);
            }
        }

        public void SceneFishOutTips(int tipsType, int fishId)
        {

            if (tipsType == 0)
            {
                string tipsResource = "YuChao_Coming";
                float lifeTime = 5f;
                FishFishOutTipsEffectManager.Instance.SetFishOutTipsEffectShowMode(tipsResource, tipsResource, lifeTime);
            }
            else
            {
                FishFishConfig fishConfig = gameData.FishConfigList[fishId];
                if (fishConfig != null)
                {
                    FishFishOutTipsEffectManager.Instance.SetFishOutTipsEffectShowMode(fishConfig.fishOutTipsResource, fishConfig.fishOutTipsAnimation, fishConfig.fishOutTipsTime);
                    if (fishConfig.BornEffectName != "0")
                    {
                        string name = fishConfig.BornEffectName;
                        int type = fishConfig.BornEffectType;
                        float delayTime = 0f;
                        float lifeTime = fishConfig.BornEffectLifeTime;
                        string effectAudio = null;
                        Vector3 beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.position;
                        FishFishEffectManager.Instance.ShowFishEffect(beginPos, type, name, delayTime, lifeTime, effectAudio);
                    }
                    if (fishConfig.fishBornAudio != "0")
                        FishAudioManager.Instance.PlayNormalAudio(int.Parse(fishConfig.fishBornAudio));
                }
            }
        }

        public void ChangeGameScene(ChangeSceneRsp data)
        {
            int sceneId = (int)data.scene_id;
            int changeType = (int)data.scene_change_type;
            float time = data.time_seconds;
            switch (changeType)
            {
                case (int)eChangeSceneType.eType_WaveTide:
                    PlayFishTideAnimation(sceneId);
                    ChangeSceneClearFish(time);
                    break;
                case (int)eChangeSceneType.eInfoType_FishOutScne:
                    ChangeSceneClearFish(time);
                    SetBG2Image();
                    SetGameBG(sceneId, true);
                    break;
                case (int)eChangeSceneType.eInfoType_ChangeSceneBG:
                    SetBG2Image();
                    SetGameBG(sceneId, true);
                    break;
                default:
                    break;
            }
        }

        public void ChangeSceneClearFish(float time)
        {
            FishFishManager.Instance.FishQuickOutScene(15);
            changeSceneClearTimer = new Timer((time - 0.3) * 1000);
            changeSceneClearTimer.Elapsed += ClearSceneFishCallBack;
            changeSceneClearTimer.Start();
        }

        private void ClearSceneFishCallBack(object sender, ElapsedEventArgs e)
        {
            changeSceneClearTimer.Dispose();
            changeSceneClearTimer = null;
            FishFishManager.Instance.ClearAllUsingFish();
        }

        public void SetBG2Image()
        {
            BG2.texture = GameBG.texture;
            BG2.color = Color.white;
            BG2.gameObject.SetActive(true);
        }

        public void ChangeSceneBGImage(int sceneId, bool isFadeAnimation)
        {
            if (isFadeAnimation)
            {
                AsyncActionUtils.ApplyImageColor(this, BG2, BG2.color, new Color(1, 1, 1, 0), 0.5f, TweenUtils.ColorTweenInQuad);
                AsyncActionUtils.ApplyImageColor(this, GameBG, new Color(1, 1, 1, 0), Color.white, 0.5f, TweenUtils.ColorTweenInQuad, 0, () =>
                {
                    BG2.gameObject.SetActive(false);
                    HaiWang_Bg.gameObject.SetActive(sceneId == 2);
                });
            }
        }

        public void PlayFishTideAnimation(int sceneId)
        {
            FishAudioManager.Instance.StopMusic();
            FishAudioManager.Instance.PlayNormalAudio(68);
            tideGroupRectTrans.gameObject.SetActive(true);
            AsyncActionUtils.DelayedAction(this, 1, () => { tideGroupRectTrans.gameObject.SetActive(true); });
            AsyncActionUtils.DelayedAction(this, 2, () => { FishTideOver(sceneId); });

        }

        public void FishTideOver(int sceneId)
        {
            FishAudioManager.Instance.StopNormalAudio(68);

            if (sceneId == 5)
                FishAudioManager.Instance.PlayBGAudio(UnityEngine.Random.Range(66, 68), 0.4f);
            else
                FishAudioManager.Instance.PlayBGAudio(22 + UnityEngine.Random.Range(0, 3), 0.4f);

            tideGroupRectTrans.gameObject.SetActive(false);
        }

        public void SetShake(bool isVibrate)
        {
            FishShakeCamera shakeCamera = FishGameObjectPoolManager.Instance.GetPoolFishRootObj().GetComponent<FishShakeCamera>();
            if (shakeCamera != null)
            {
                Vector3 positionShake = new Vector3(UnityEngine.Random.Range(7, 9), UnityEngine.Random.Range(1, 9), 0);
                Vector3 angleShake = Vector3.zero;
                float cycleTime = 0.15f;
                int cycleCount = 2;
                shakeCamera.Restart(positionShake, angleShake, cycleTime, cycleCount, isVibrate);
            }
        }

        protected override void OnDestroy()
        {

        }
    }

}
