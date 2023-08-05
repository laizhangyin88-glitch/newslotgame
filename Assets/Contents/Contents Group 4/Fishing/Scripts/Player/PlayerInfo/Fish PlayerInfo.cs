using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishPlayerInfo
    {
        GameObject gameObject;
        FishGameUIManager gameUIManager;
        FishGameData gameData;
        private FishFishBase LockTargetFish;
        public bool IsLockFish;
        private bool IsAutoShootFish;
        private bool IsContinuePress;
        private float PressTimes = 0f;
        private float[] ShootBulletRate = new float[3];
        private int CurrentShootBulletRateIndex = 0;
        private float CurrentShootBulletTime = 0f;
        private List<FishBullet> CacheBulletList = new List<FishBullet>();
        private float CurrentShootBulletIntervalTime = 0f;
        private int ChairID = 0;
        private int UserId = 0;
        public int PrecisionValue = 0;
        private int BulletUID = 0;
        private int BulletUIDMax = 1;
        private int BulletIntervalDistance = 0;
        private List<FishFishBase> ShowLockFishList = new List<FishFishBase>();
        public int CurrentGunLevel = 0;
        private string currentNetAnimName = "";
        private string currentBulletAnimName = "";
        public int currentUseRealGunLevel = 0;
        private bool IsDoubleScoreStatus = false;
        private bool IsFreeStatus = false;
        private bool isOnLine = false;
        private int LimitBulletCount = 0;
        private bool IsCanShootBullet = false;
        private ulong currentPlayMoney = 0;
        public FishPlayerPanel Panel;
        public GameObject gunTeamObj;
        private Transform bulletPosTrans;
        private Vector3 bulletCreatePos;
        private Transform gunLockPointTrans;
        private List<GameObject> lockPointList;
        private GameObject lockTips;
        private Button addBetBtn;
        private Button ReduceBetBtn;
        private Text BetScoreLabel;
        private Text playerMoneyLabel;
        private Text PlayerNameLabel;
        private Transform FlyCoinPos;
        public Transform flyScorePos;
        public Transform specialDeclarePanel;
        public Transform SwrilPanel;
        public FishPlayerInfo(GameObject gameObj)
        {
            InitData();
            InitView(gameObj);
            InitViewData();
            AddBtnEventListener();
        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            LockTargetFish = null;      // the locked fish
            IsLockFish = false;         // whether the fish is locked
            IsAutoShootFish = false;    // whether it is automatic
            IsContinuePress = false;    // whether it is continuously clicked
            PressTimes = 0.5f;          // time to hold down without releasing
            ShootBulletRate = new float[] { 0.3f, 0.2f, 0.13f };    // shooting interval
            CurrentShootBulletRateIndex = 0;
            CurrentShootBulletTime = 0f;
            CacheBulletList = new List<FishBullet>();    // collection of bullets issued by the server
            CurrentShootBulletIntervalTime = 0f;
            ChairID = 0;               // seat id
            UserId = 154183;            // player ID
            PrecisionValue = 10000;     // precision value
            BulletUID = 1;
            BulletUIDMax = 1;
            BulletIntervalDistance = 50;    // bullet interval distance
            ShowLockFishList = new List<FishFishBase>();    // list of fish for display
            CurrentGunLevel = 0;        // current cannon level
            currentNetAnimName = "Net_01_net01";     // current Net level animation
            currentBulletAnimName = "Bullet_01";     // current bullet level animation
            currentUseRealGunLevel = 0;               // current client real use of the turret resource level
            IsDoubleScoreStatus = false;    // whether it is in double score status
            IsFreeStatus = false;           // whether it is in free status
            isOnLine = false;
            LimitBulletCount = 999999;
            IsCanShootBullet = true;        // whether bullets can be fired
            currentPlayMoney = 0;
        }

        public void InitPlayerData(int tempId)
        {
            InitBulletUID(tempId);
        }

        public void InitBulletUID(int ChairID)
        {
            BulletUID = FishPlayerManager.Instance.GetPlayerBulletUID(ChairID);
            BulletUIDMax = 2 * BulletUID;
        }

        private void InitView(GameObject gameObj)
        {
            gameObject = gameObj;
            Panel = new FishPlayerPanel(gameObj);
        }

        private void InitViewData()
        {
            IsShowPlayerPanel(false);

            gunTeamObj = Panel.GunTeamObj;
            bulletPosTrans = Panel.BulletPos;
            bulletCreatePos = new Vector3(Panel.BulletPos.position.x, Panel.BulletPos.position.y, Panel.BulletPos.position.z);
            gunLockPointTrans = Panel.GunLockPoint.transform;
            lockPointList = Panel.LockPointList;
            lockTips = Panel.LockTips;
            addBetBtn = Panel.AddBetBtn;
            ReduceBetBtn = Panel.ReduceBetBtn;
            BetScoreLabel = Panel.BetScoreLabel;
            playerMoneyLabel = Panel.PlayerMoneyLabel;
            PlayerNameLabel = Panel.PlayerNameLabel;
            FlyCoinPos = Panel.FlyCoinPos;
            flyScorePos = Panel.FlyScorePos;
            specialDeclarePanel = Panel.SpecialDeclarePanel;
            SwrilPanel = Panel.SwrilPanel;
        }

        public void SetPlayerChairId(int chairId)
        {
            ChairID = chairId;
        }

        public int GetPlayerChairId()
        {
            return ChairID;
        }

        public void SetUserID(int userId)
        {
            UserId = userId;
        }

        public int GetUserID()
        {
            return UserId;
        }

        public void SetOnlineState(bool onLine)
        {
            isOnLine = onLine;
        }

        public bool GetOnlineState()
        {
            return isOnLine;
        }

        public void SetPlayerMoney(ulong money)
        {
            currentPlayMoney = money;
        }

        public float GetPlayerMoney()
        {
            return currentPlayMoney;
        }

        public int GetCurrentBetValue()
        {
            return (int)gameData.GunLevelConfig[CurrentGunLevel].cannon_value;
        }

        public void ClearBulletCache()
        {
            CacheBulletList.Clear();
        }

        public void ResetBulletRate()
        {
            CurrentShootBulletRateIndex = 0;
            FishGameManager.Instance.speedBtnEffect.gameObject.SetActive(false);
        }

        public void IsShowPlayerPanel(bool isDisplay)
        {
            gameObject.SetActive(isDisplay);
        }

        public void RotationPaoTao()
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
        }

        public void AddBtnEventListener()
        {
            addBetBtn.onClick.AddListener(OnclickAddBet);
            ReduceBetBtn.onClick.AddListener(OnclickReduceBet);
        }

        public void PlayerEnterState(UserInfo userInfo)
        {
            SetPlayerChairId((int)userInfo.chair_id);
            SetPlayerName(userInfo.user_name);
            SetPlayerMoneyScore(userInfo.user_money);
            Panel.isSpeed = false;
            SetGunLevelValue((int)userInfo.cannon_id);
            IsShowBetPanel();
            IsShowPlayerPanel(true);
            IsShowImHere();
            ClearBulletCache();
            SetOnlineState(true);
            SetCanShootBullet(true);
            Panel.IsShowLockFishPanel(true);
            IsDoubleScoreStatus = false;
            IsFreeStatus = false;
        }

        public void PlayerLeaveState()
        {
            SetPlayerChairId(0);
            ResetBulletRate();
            SetAutoSendShootBullet(false);
            SetAutoLockFish(false);
            LockTargetFish = null;
            IsShowPlayerPanel(false);
            Panel.SetShowPanel(false);
            ClearBulletCache();
            SetOnlineState(false);
        }

        public void EnterFreeScoreState()
        {
            Panel.IsShowBetPanel(false);
            IsFreeStatus = true;
            SetGunParticleEffect();
            SetFreeStatsGunLevelValue(CurrentGunLevel);
        }

        public void LeaveFreeScoreState()
        {
            Panel.IsShowBetPanel(true);
            IsFreeStatus = false;
            SetGunParticleEffect();
            SetGunLevelValue(CurrentGunLevel);
        }

        public void EnterDoubleScoreState()
        {
            Panel.IsShowBetPanel(false);
            SetDoubleStatsGunLevelValue(CurrentGunLevel);
        }

        public void LeaveDoubleScoreState()
        {
            Panel.IsShowBetPanel(true);
            SetGunLevelValue(CurrentGunLevel);
        }

        public void DoubleGunOnOffState(int state)
        {
            if (state == 1)
            {
                EnterDoubleScoreState();
            }
            else
                LeaveDoubleScoreState();
        }

        public void OnclickAddBet()
        {
            FishAudioManager.Instance.PlayNormalAudio(25);
            FishAudioManager.Instance.PlayNormalAudio(gameData.GunConfigList[(int)gameData.GunLevelConfig[CurrentGunLevel].cannon_value_gun_id - 1].makeUpAudio);
            int tempGunLevel = CurrentGunLevel + 1;
            if (tempGunLevel >= gameData.GunLevelConfig.Count)
                tempGunLevel = 0;
            FishPlayerManager.Instance.SendChangeGunMsg(tempGunLevel);
        }

        public void OnclickReduceBet()
        {
            FishAudioManager.Instance.PlayNormalAudio(25);
            FishAudioManager.Instance.PlayNormalAudio(gameData.GunConfigList[(int)gameData.GunLevelConfig[CurrentGunLevel].cannon_value_gun_id - 1].makeUpAudio);
            int tempGunLevel = CurrentGunLevel - 1;
            if (tempGunLevel < 0)
                tempGunLevel = gameData.GunLevelConfig.Count - 1;
            FishPlayerManager.Instance.SendChangeGunMsg(tempGunLevel);
        }

        public void SetFreeStatsGunLevelValue(int level)
        {
            CurrentGunLevel = level;
            SetBetScore(gameData.GunLevelConfig[level].cannon_value);
            SetShowGun(gameData.GunConfigList[(int)gameData.GunLevelConfig[level].cannon_value_gun_id - 1].freeGunRes);
            SetNetAnimName(gameData.GunConfigList[level].freeNetRes);
            SetBulletAnimName(gameData.GunConfigList[level].freeBulletRes);
        }

        public void SetDoubleStatsGunLevelValue(int level)
        {
            CurrentGunLevel = level;
            SetBetScore(gameData.GunLevelConfig[level].cannon_value);
            SetShowGun(gameData.GunConfigList[(int)gameData.GunLevelConfig[level].cannon_value_gun_id - 1].doubleGunRes);
            SetNetAnimName(gameData.GunConfigList[level].doubleNetRes);
            SetBulletAnimName(gameData.GunConfigList[level].doubleBulletRes);
        }

        public void SetGunLevelValue(int level)
        {
            CurrentGunLevel = level;
            SetBetScore(gameData.GunLevelConfig[level].cannon_value);
            SetShowGun(gameData.GunConfigList[(int)gameData.GunLevelConfig[level].cannon_value_gun_id - 1].normalGunRes);
            SetNetAnimName(gameData.GunConfigList[level].normalNetRes);
            SetBulletAnimName(gameData.GunConfigList[level].normalBulletRes);
        }

        public void SetGunParticleEffect()
        {
            Panel.IsShowGunParticleEffect();
        }

        public void SetShowGun(string index)
        {
            currentUseRealGunLevel = int.Parse(index) - 1;
            Panel.SetShowGunPanel();
        }

        public int GetGunLevel()
        {
            return CurrentGunLevel;
        }

        public void SetCanShootBullet(bool isShootBullet)
        {
            IsCanShootBullet = isShootBullet;
        }

        public void SetNetAnimName(string netName)
        {
            currentNetAnimName = netName;
        }

        public void SetBulletAnimName(string bulletName)
        {
            currentBulletAnimName = bulletName;
        }

        public void SetBetScore(float score)
        {
            BetScoreLabel.text = FormatBaseProportionalScore(score).ToString();
            float offset = 0.7f - BetScoreLabel.text.Length * 0.1f;
            //1 0.6 10 0.5 100 0.4, 1000 0.3
            Vector3 targetScale = Vector3.one * offset;
            AsyncActionUtils.ApplyScaling(FishPlayerManager.Instance, BetScoreLabel.transform, Vector3.one, targetScale, 0.5f, TweenUtils.VectorTweenInSine);
            AsyncActionUtils.ApplyTextColor(FishPlayerManager.Instance, BetScoreLabel, Color.white, new Color(1,1,1,0), 0.5f, TweenUtils.ColorTweenInSine, 0, () =>
            {
                BetScoreLabel.color = Color.white;
            });
        }

        public void SetPlayerMoneyScore(ulong score)
        {
            playerMoneyLabel.text = FormatBaseProportionalScore(score).ToString();
            SetPlayerMoney(score);
        }

        public void SetPlayerName(string name)
        {
            PlayerNameLabel.text = name;
        }

        public Vector3 GetFlyCoinPos()
        {
            return Panel.FlyCoinPos.TransformPoint(FlyCoinPos.position);
        }

        public void IsShowBetPanel()
        {
            bool isDisplay = false;
            if (ChairID == gameData.playerChairId)
                isDisplay = true;
            Panel.IsShowBetPanel(isDisplay);
        }

        public void IsShowImHere()
        {
            bool isDisplay = false;
            if (ChairID == gameData.playerChairId)
                isDisplay = true;
            Panel.IsShowImHere(isDisplay);
        }

        public void CreateNet(Vector3 targetNetPos)
        {
            FishNet netIns = FishNetManager.Instance.GetNet(targetNetPos);
            netIns.SetPlayAnimState(true);
            netIns.PlayNetAnimator(currentNetAnimName);
        }

        public void SetLockFish(FishFishBase fish)
        {
            LockFishProcess(fish);
            LockTargetFish = fish;
        }

        public void LockFishProcess(FishFishBase fish)
        {
            if (fish != null && IsLockFish)
            {
                int tempUID = 0;
                if (LockTargetFish != null)
                    tempUID = LockTargetFish.FishVo.UID;
                if (tempUID != fish.FishVo.UID)
                {
                    UpLoadLockFish(fish);
                    Panel.PlayLockTipsAnim();
                }
            }
        }

        public FishFishBase GetLockFish()
        {
            return LockTargetFish;
        }

        public FishFishBase GetAutoLockFish()
        {
            int fishCount = gameData.GameConfig.LockFishList.Length;
            for (int i = 0; i < fishCount; i++)
            {
                FishFishBase targetFish = FishFishManager.Instance.GetUsingFishByID(Random.Range(1, fishCount));
                if (targetFish != null)
                    return targetFish;
            }
            return null;
        }

        public void UpdateAutoLockFish()
        {
            if (IsLockFish && LockTargetFish == null && ChairID == gameData.playerChairId)
            {
                FishFishBase LockTragetFish = GetAutoLockFish();
                if (LockTragetFish != null)
                {
                    SetLockFish(LockTragetFish);
                }
            }
        }

        public void UpLoadLockFish(FishFishBase lockFish)
        {
            int lockFishUID = lockFish.FishVo.UID;
            if (lockFishUID != 0)
            {
                IsShowLockFishStatePanel(true);
                if (ChairID == gameData.playerChairId)
                {
                    LockFishReq mes = new LockFishReq();
                    mes.fishId = lockFishUID;
                    FishFishManager.Instance.RequestLockTargetFishMsg(mes);
                }
            }
        }

        public void UpdateCheckLockFishState()
        {
            if (IsLockFish && LockTargetFish != null)
            {
                if (LockTargetFish.GetIsDie() || LockTargetFish.GetIsDestroy() || !LockTargetFish.CheckBoundValid())
                {
                    LockTargetFish = null;
                    IsShowLockFishStatePanel(false);
                }
            }
        }

        public void UpdateLockFish()
        {
            UpdateAutoLockFish();
            UpdateCheckLockFishState();
            UpdateCaculateLockPoint();
        }

        public void SetAutoLockFish(bool isAuto)
        {
            IsLockFish = isAuto;
        }

        public void UploadAutoLockFish(bool isAuto)
        {
            LockOnOffReq lockOnOffReq = new LockOnOffReq();
            lockOnOffReq.onOff = isAuto;
            FishFishManager.Instance.RequestLockFishSwitchNetMsg(lockOnOffReq);
        }

        public void ResetLockTargetFishState(bool isAuto)
        {
            if (!isAuto)
            {
                SetLockFish(null);
                IsShowLockFishStatePanel(false);
            }
        }

        public void UpdateCaculateLockPoint()
        {
            if (IsLockFish && LockTargetFish != null)
            {
                if (!LockTargetFish.GetIsDie() && !LockTargetFish.GetIsDestroy() && LockTargetFish.CheckBoundValid())
                {
                    CaculateLockFishDistance(LockTargetFish, gunLockPointTrans);
                    SetTargetLockFishTips(LockTargetFish);
                }
            }
        }

        public void CaculateLockFishDistance(FishFishBase targetFish, Transform currentTrans)
        {
            Vector3 lockFishPos;
            if (targetFish.FishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Part)
            {
                FishPartFish partFish = targetFish as FishPartFish;
                lockFishPos = currentTrans.InverseTransformPoint(partFish.GetLockPartPoint().position);
            }
            else if (targetFish.FishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Dragon)
            {
                FishDragonFish dragonFish = targetFish as FishDragonFish;
                lockFishPos = currentTrans.InverseTransformPoint(dragonFish.GetLockPartPoint().position);
            }
            else
                lockFishPos = currentTrans.InverseTransformPoint(targetFish.gameObject.transform.position);

            Vector3 bulletPos = currentTrans.localPosition;
            float distance = Vector3.Distance(lockFishPos, bulletPos);
            Vector3 dir = (lockFishPos - bulletPos).normalized;
            int pointCount = Mathf.FloorToInt(distance / BulletIntervalDistance) + 1;
            GameObject lockPoint;
            Vector3 tempPos;
            for (int i = 0; i < lockPointList.Count; i++)
            {
                lockPoint = lockPointList[i];
                if (i <= pointCount)
                {
                    tempPos = bulletPos + dir * BulletIntervalDistance * (i - 1);
                    FishCsharpManager.SetLocalPosition(lockPoint, tempPos.x, tempPos.y, tempPos.z);
                    lockPoint.SetActive(true);
                }
                else
                    lockPoint.SetActive(false);
            }
        }

        public void IsShowLockFishStatePanel(bool isDisplay)
        {
            Panel.IsShowLockTips(isDisplay);
            Panel.IsShowGunLockPoint(isDisplay);
            Panel.IsShowlockRotation(isDisplay);
        }

        public void SetTargetLockFishTips(FishFishBase targetFish)
        {
            if (targetFish.FishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Part)
            {
                FishPartFish partFish = targetFish as FishPartFish;
                Panel.SetLockTipsPos(partFish.GetLockPartPoint().position);
            }
            else if (targetFish.FishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Dragon)
            {
                FishDragonFish dragonFish = targetFish as FishDragonFish;
                Panel.SetLockTipsPos(dragonFish.GetLockPartPoint().position);
            }
            else
                Panel.SetLockTipsPos(targetFish.gameObject.transform.position);
        }

        public void DeleteCopyFish()
        {
            if (ShowLockFishList != null && ShowLockFishList.Count > 0)
            {
                for (int i = 0; i < ShowLockFishList.Count; i++)
                {
                    FishGameObjectPoolManager.Instance.SetPoolParent(ShowLockFishList[i].gameObject, PoolType.FishPool);
                    ShowLockFishList[i].SetDestroy(true);
                }
            }
        }

        public void SetAutoSendShootBullet(bool isAuto)
        {
            IsAutoShootFish = isAuto;
        }

        public void UploadAutoShootBullet(bool isAuto)
        {
            AutoShootReq mes = new AutoShootReq();
            mes.onOff = isAuto;
            FishBulletManager.Instance.RequestAutoShootBulletSwitchMsg(mes);
        }

        public int SetShootBulletRateLevel()
        {
            if (CurrentShootBulletRateIndex == 0)
                Panel.ChangeGunModeAnim(true);
            CurrentShootBulletRateIndex++;
            if (CurrentShootBulletRateIndex >= ShootBulletRate.Length)
            {
                CurrentShootBulletRateIndex = 0;
                Panel.ChangeGunModeAnim(false);
            }
            FishGameManager.Instance.speedBtnEffect.gameObject.SetActive(CurrentShootBulletRateIndex > 0);
            for (int i = 1; i < 3; i++)
                FishGameManager.Instance.speedBtnEffect.Find("speedText" + i).gameObject.SetActive(i == CurrentShootBulletRateIndex);
            return CurrentShootBulletRateIndex;
        }

        public void UploadShootBulletRateLevel(int index)
        {
            BulletSpeedReq mes = new BulletSpeedReq();
            mes.usSpeedIndex = 100;
            mes.usIntervalIndex = index;
            FishBulletManager.Instance.RequestBulletSpeedNetMsg(mes);
        }

        public void OnClickSetLockFish(bool isPress, FishFishBase fish)
        {
            if (IsLockFish && isPress)
            {
                SetLockFish(fish);
            }
        }

        public void OnClickSendShootBullet(bool isPress)
        {
            IsContinuePress = isPress;
            if (isPress && GetOnlineState())
            {
                if (IsAutoShootFish)
                    return;
                if (IsCanShootBullet)
                {
                    ResetShootBulletTime();
                    SendShootBullet();
                }
            }
        }

        public void SendShootBullet()
        {
            int sendBulletCount = FishBulletManager.Instance.GetPlayerBulletCount(ChairID);

            if (sendBulletCount > LimitBulletCount)
            {
                FishGameManager.Instance.ShowUITips("发射的子弹太多了!!!", 1);
                return;
            }

            if (GetPlayerMoney() < GetCurrentBetValue())
            {
                FishGameManager.Instance.ShowUITips("玩家下注金额不足!!!", 3);
                return;
            }

            UpdateMuzzleRotation();
            int shootAngle = Mathf.FloorToInt(gunTeamObj.transform.eulerAngles.z * PrecisionValue);
            CaculateBulletUID();
            FishBulletManager.Instance.RequestSendBulletMsg(shootAngle, BulletUID);
            LocalCreateBullet();
        }

        public void CaculateBulletUID()
        {
            BulletUID++;
            if (BulletUID >= BulletUIDMax)
            {
                BulletUID = Mathf.CeilToInt(BulletUIDMax / 2);
            }
        }

        public void LocalCreateBullet()
        {
            if (ChairID == gameData.playerChairId)
            {
                FishBulletManager.Instance.LocalCreatBullet(ChairID, BulletUID, 1, 1, 1);
                if (!IsFreeStatus)
                {
                    FishAudioManager.Instance.PlayNormalAudio(gameData.GunConfigList[(int)gameData.GunLevelConfig[CurrentGunLevel].cannon_value_gun_id - 1].shootAudio);
                }
                else
                {
                    FishAudioManager.Instance.PlayNormalAudio(55);
                }
            }
        }

        public void UpdateSendShootBullet()
        {
            if (IsCanShootBullet)
            {
                if (IsContinuePress || (IsAutoShootFish && ChairID == gameData.playerChairId))
                {
                    CurrentShootBulletTime += Time.deltaTime;
                    if (CurrentShootBulletTime >= ShootBulletRate[CurrentShootBulletRateIndex])
                    {
                        ResetShootBulletTime();
                        SendShootBullet();
                    }
                }
            }
        }

        public void ResetShootBulletTime()
        {
            CurrentShootBulletTime = 0;
        }

        public void SetMuzzleEulerAngles(float angle)
        {
            FishCsharpManager.SetLocalEulerAngles(gunTeamObj, 0, 0, angle);
        }

        public void ServerToShootBullet(FishBullet bulletIns)
        {
            if (bulletIns.BulletVo.chairId != gameData.playerChairId)
            {
                AddShootBulletCache(bulletIns);
            }
        }

        public void LocalToShootBullet(FishBullet bulletIns)
        {
            if (bulletIns.BulletVo.chairId == gameData.playerChairId)
            {
                AddShootBulletCache(bulletIns);
            }
        }

        public void AddShootBulletCache(FishBullet bulletIns)
        {
            CacheBulletList.Add(bulletIns);
        }

        public void ShootBullet(FishBullet bulletIns)
        {
            if ((bulletIns.BulletVo.chairId != gameData.playerChairId && !IsLockFish) || (IsLockFish && LockTargetFish == null && bulletIns.BulletVo.chairId != gameData.playerChairId))
            {
                SetMuzzleEulerAngles(bulletIns.BulletVo.BulletAngle / PrecisionValue);
            }
            if (IsLockFish && LockTargetFish != null)
                bulletIns.SetBulletLockTargetFish(LockTargetFish);
            Panel.PlayGunShotAnim();
            bulletIns.SetBulletKindAnim(currentBulletAnimName);
            bulletIns.gameObject.transform.position = bulletPosTrans.position;
            if (bulletIns.gameObject.transform.localPosition.z < 0)
                bulletIns.gameObject.transform.localPosition = new Vector3(bulletIns.gameObject.transform.localPosition.x, bulletIns.gameObject.transform.localPosition.y, 0);
            bulletIns.gameObject.transform.localScale = Vector3.one;
            bulletIns.gameObject.transform.eulerAngles = bulletPosTrans.eulerAngles;
            bulletIns.BulletBeginMove(bulletIns.bulletSpeed);
        }

        public void NormalRotationGun()
        {
            Vector3 upVetor = Vector3.up;
            Camera UICamera = gameUIManager.UICamera;
            Vector3 vecMouse = Input.mousePosition;
            Vector3 vecMouseWorld = UICamera.ScreenToWorldPoint(vecMouse);
            if (vecMouse.x > 0 && vecMouse.x <= Screen.width && vecMouse.y >= 0 && vecMouse.y <= Screen.height)
            {
                Vector3 gunTeamVerctor3Pos = gunTeamObj.transform.position;
                Vector3 v1 = Vector3.Normalize(vecMouseWorld - gunTeamVerctor3Pos);
                gunTeamObj.transform.eulerAngles = new Vector3(0, 0, Quaternion.FromToRotation(Vector3.up, v1).eulerAngles.z);
            }
        }

        public void  LockingRotationGun()
        {
            Vector3 upVector = Vector3.up;
            Vector3 gunTeamVector3Pos = gunTeamObj.transform.position;
            Vector3 v1 = Vector3.Normalize(LockTargetFish.GetPosition() - gunTeamVector3Pos);
            gunTeamObj.transform.eulerAngles = Quaternion.FromToRotation(upVector, v1).eulerAngles;
        }

        public void UpdateMuzzleRotation()
        {
            if (IsLockFish && LockTargetFish != null)
            {
                LockingRotationGun();
            }
            else if (IsContinuePress)
            {
                NormalRotationGun();
            }
        }

        public void UpdateShootBullet()
        {
            if (CacheBulletList != null && CacheBulletList.Count > 0)
            {
                CurrentShootBulletIntervalTime += Time.deltaTime;
                if (CurrentShootBulletIntervalTime >= ShootBulletRate[2])
                {
                    CurrentShootBulletIntervalTime = 0;
                    FishBullet bulletIns = CacheBulletList[0];
                    CacheBulletList.RemoveAt(0);
                    ShootBullet(bulletIns);
                }
            }
        }

        public void Update()
        {
            UpdateMuzzleRotation();
            UpdateSendShootBullet();
            UpdateShootBullet();
            UpdateLockFish();
            if (Panel.ImHere != null && Panel.ImHere.activeSelf && !Panel.showImHere)
                Panel.ImHere.SetActive(false);
        }

        public float FormatBaseProportionalScore(float score)
        {
            float baseRatio = 1;
            float currentIntPart, currentDecimalPart;
            currentIntPart = (int)score;
            currentDecimalPart = score - currentIntPart;

            if (currentDecimalPart > 0)
            {
                int count = Mathf.FloorToInt(Mathf.Log10(currentDecimalPart));
                if (baseRatio < 100 && baseRatio > 0)
                {
                    if (count > 7)
                    {
                        currentDecimalPart = Mathf.Round(currentDecimalPart * Mathf.Pow(10, 7)) / Mathf.Pow(10, 7);
                    }
                }
                else
                {
                    if (count > 2)
                    {
                        currentDecimalPart = Mathf.Round(currentDecimalPart * Mathf.Pow(10, 2)) / Mathf.Pow(10, 2);
                    }
                }
                score = currentIntPart + currentDecimalPart;
            }
            else
            {
                score = currentIntPart;
            }
            return score;
        }

        
    }
}
