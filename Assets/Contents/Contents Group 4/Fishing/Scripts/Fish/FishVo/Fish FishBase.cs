using fishMsg;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static BagelCode.FishBehaviour;

namespace BagelCode
{
    public abstract class FishFishBase
    {
        FishGameUIManager gameUIManager;
        FishGameData gameData;
        string _tag;
        public GameObject gameObject;
        public Transform transform;
        Animator animator;
        public bool isCanSeen;
        FishStatus fishMoveStatus;
        public FishBehaviour fishBehaviour;
        public bool isCanDestroy;
        bool isDie;
        public FishPlayerInfo playerIns;
        public int chairId;
        Vector3 dieMoveToTargetPos;
        KillFishRsp hitFishMsg;
        public Vector3 hitFlyDirection;
        public Color beHitColor;
        public Color NormalColor;
        public bool isHit;
        public float currentHitTime;
        public float hitTotalTime;
        float moveTime;
        bool isShowingTipsContent;
        public FishVo fishVo;
        Collider collider;
        public Collider[] colliders;

        public FishFishBase()
        {
            BaseInit();
        }

        private void BaseInit()
        {
            BaseInitData();
        }

        private void BaseInitData()
        {
            InitBaseData();
            InitFishVo();
        }

        private void InitBaseData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            _tag = "Fish";
            fishBehaviour = null;
            transform = null;
            gameObject = null;
            isCanSeen = false;
            fishMoveStatus = FishStatus.Born;
            isCanDestroy = false;
            isDie = false;
            playerIns = null;
            chairId = 0;
            dieMoveToTargetPos = Vector3.zero;
            hitFishMsg = null;
            hitFlyDirection = Vector3.zero;
            beHitColor = new Color(0.76f, 0.28f, 0.28f, 1f);
            NormalColor = Color.white;
            isHit = false;
            currentHitTime = 0f;
            hitTotalTime = 0.15f;
            isShowingTipsContent = false;
        }

        public abstract void BuildFish(FishVo vo, GameObject obj);

        public abstract void SetMainFishOrder(int orderIndex);

        public abstract void PlayMoveAnim();

        public abstract void PlayMoveAnim(bool isLoop);

        public abstract void PlayDieAnim(bool isLoop);

        public abstract void SetMainFishColor(Color color);

        public void InitFishVo()
        {
            fishVo = new FishVo();
        }

        public void UpdateFishVo(FishVo vo)
        {
            fishVo = vo;
        }

        public void UpdateDieEffectConfig(FishDieEffectConfig dieEffectConfig)
        {
            fishVo.DieEffectConfig = dieEffectConfig;
        }

        public void BuildFishBase(FishVo vo, GameObject obj)
        {
            gameObject = obj;
            transform = obj.transform;
            fishVo = vo;
            gameObject.tag = _tag;
            gameObject.SetActive(true);
            animator = gameObject.GetComponent<Animator>();
        }

        public void IsShowFishLight(bool show)
        {
            Transform fishTrans = gameObject.transform.Find("Bone/Fish");
            if (fishTrans != null)
                fishTrans.gameObject.SetActive(!show);
            Transform lightTrans = gameObject.transform.Find("Bone/Light");
            if (lightTrans != null)
                lightTrans.gameObject.SetActive(show);
        }

        public void AddBehaviourScript()
        {
            fishBehaviour = gameObject.GetComponent<FishBehaviour>();
            if (fishBehaviour == null)
            {
                fishBehaviour = gameObject.AddComponent<FishBehaviour>();
                fishBehaviour.fishIns = this;
                fishBehaviour.onPressCallBack = OnPressCallBack;
            }
            colliders = gameObject.GetComponentsInChildren<Collider>();
        }

        public void OnPressCallBack(bool press)
        {
            FishPlayerInfo temPlayerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(gameData.playerChairId);
            if (temPlayerIns != null)
            {
                temPlayerIns.OnClickSetLockFish(press, this);
                temPlayerIns.OnClickSendShootBullet(press);
            }
        }

        public void InitBaseFish(FishVo vo, GameObject obj)
        {
            BuildFishBase(vo, obj);
            AddBehaviourScript();
            SetFishChildTag();
            IsEnableBoxcollider(true);
            IsShowFishLight(false);
        }

        public abstract void ResetFishState(FishVo vo);

        public void ResetBaseFishStateData(FishVo vo)
        {
            UpdateFishVo(vo);
            IsEnableBoxcollider(true);
            IsShowFishLight(false);
            isCanDestroy = false;
            gameObject.SetActive(true);
        }

        public void BeginMove()
        {
            isCanSeen = false;
            float fishWidth = 0;
            float fishHeight = 0;
            if (collider != null)
            {
                Vector3 size = collider.bounds.size;
                fishWidth = size.x * 0.5f + 8;
                fishHeight = size.y * 0.5f + 8;
            }
            int curPointIndex = fishVo.StartPointIndex + fishVo.OffsetIndex;
            fishBehaviour.FishBeginMove(fishVo.TraceId, fishVo.OffsetPosX, fishVo.OffsetPosY, fishWidth, fishHeight, curPointIndex, fishVo.DelayBornTime);
            fishMoveStatus = fishBehaviour.curFishStatus;
        }

        public void SetFishParent(Transform parent)
        {
            gameObject.transform.parent = parent;
            FishCsharpManager.SetLocalEulerAngles(gameObject.transform, 0, 0, 0);
            FishCsharpManager.SetLocalPosition(gameObject.transform, 0, 0, 0);
            FishCsharpManager.SetLocalScale(gameObject.transform, 1, 1, 1);
        }

        public void IsEnableBoxcollider(bool isEnable)
        {
            if (colliders != null && colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = isEnable;
                }
            }
        }

        public void SetFishChildTag()
        {
            if (colliders != null && colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].gameObject.tag = _tag;
                }
            }
        }

        public Vector3 GetPosition()
        {
            return gameObject.transform.position;
        }

        public Vector3 GetLocalPosition()
        {
            return gameObject.transform.localPosition;
        }

        public bool GetIsDie()
        {
            return isDie;
        }

        public void SetIsDie(bool isDie)
        {
            this.isDie = isDie;
        }

        public bool CheckBoundValid()
        {
            return isCanSeen;
        }

        public FishStatus GetFishMoveStatus()
        {
            return fishMoveStatus;
        }

        public void SetFishMoveStatus(FishStatus status)
        {
            fishMoveStatus = status;
            fishBehaviour.curFishStatus = status;
        }

        public bool GetIsDestroy()
        {
            return isCanDestroy;
        }

        public void SetDestroy(bool isDestroy)
        {
            isCanDestroy = isDestroy;
        }

        private void FishBaseDie()
        {
            //SetFishMoveStatus(FishStatus.Stop);
            SetIsDie(true);
        }

        public void ShowCoinEffect()
        {
            int coinEffectId = fishVo.DieEffectConfig.coinEffectId;
            FishFishConfig fishConfigData = fishVo.FishConfig;
            if (coinEffectId != 0 && fishConfigData.coinEffectCount > 0)
            {
                Vector3 endPos = playerIns.GetFlyCoinPos();
                endPos = gameObject.transform.parent.InverseTransformPoint(endPos);
                FishGoldEffectManager.Instance.SetCoinEffectShowMode(transform, playerIns.GetPlayerChairId(), fishVo.UID, coinEffectId,
                    fishConfigData.coinEffectCount, endPos, fishVo.DieEffectConfig.fishDieBehavior);
                PlayCoinAudio();
            }
        }

        public void PlayCoinAudio()
        {
            if (fishVo.DieEffectConfig.fishDieBehavior == 1 || playerIns.GetPlayerChairId() == gameData.playerChairId) 
            {
                if (fishVo.fishId <= 14)
                    FishAudioManager.Instance.PlayNormalAudio(35);
                else if(fishVo.fishId > 14 && fishVo.fishId <= 25)
                    FishAudioManager.Instance.PlayNormalAudio(36);
                else
                    FishAudioManager.Instance.PlayNormalAudio(37);
            }
        }

        //文字分数
        public void ShowWinScoreEffect(Vector3 beginPos)
        {
            if (fishVo.DieEffectConfig.winScoreID > 0)
            {
                FishScoreEffectConfig tempConfig = FishScoreEffectManager.Instance.GetScoreEffectConfig(fishVo.DieEffectConfig.winScoreID);
                if (tempConfig != null)
                {
                    int parentFishID = hitFishMsg.bombUID;
                    int scoreType = tempConfig.scoreType;
                    if (parentFishID == 0)
                    {
                        if (scoreType == (int)FishScoreEffectManager.ScoreType.AllShow)
                            FishScoreEffectManager.Instance.SetScoreEffectShowMode(beginPos, chairId, hitFishMsg.mainScore, fishVo.DieEffectConfig.winScoreID, fishVo.DieEffectConfig.fishDieBehavior);
                        else if (scoreType == (int)FishScoreEffectManager.ScoreType.OnlyShow)
                            FishScoreEffectManager.Instance.SetScoreEffectShowMode(beginPos, chairId, hitFishMsg.totalScore, fishVo.DieEffectConfig.winScoreID, fishVo.DieEffectConfig.fishDieBehavior);
                    }
                    else if (scoreType == (int)FishScoreEffectManager.ScoreType.AllShow)
                        FishScoreEffectManager.Instance.SetScoreEffectShowMode(beginPos, chairId, hitFishMsg.mainScore, fishVo.DieEffectConfig.winScoreID, fishVo.DieEffectConfig.fishDieBehavior);
                }
            }
        }

        public void SetTipsContentState(bool isShow)
        {
            isShowingTipsContent = isShow;
        }

        public bool GetTipsContentState()
        {
            return isShowingTipsContent; 
        }

        public void SetTipsContentInfo()
        {
            int isTC = fishVo.FishConfig.isTipsContent;
            if (isTC == 1)
            {
                if (!GetTipsContentState())
                {

                    float probability = UnityEngine.Random.Range(1, 100) / 100;
                    if (probability <= fishVo.FishConfig.TCProbability)
                    {
                        FishTipsContentManager.Instance.SetShowTipsContent(this);
                    }
                }
            }
        }

        public void FishNormalDie(FishPlayerInfo playerIns, KillFishRsp hitFishMsg)
        {
            this.playerIns = playerIns;
            this.hitFishMsg = hitFishMsg;
            chairId = this.playerIns.GetPlayerChairId();
            FishBaseDie();
            ResetNormalColor();


            bool isLoop = false;
            if (FishFishManager.Instance.GetFishClientBuildFishType(fishVo.fishId) == (int)FishGameConfig.FishType.Spine)
            {
                isLoop = true;
            }
            PlayDieAnim(isLoop);

            SetMainFishOrder(fishVo.FishConfig.fishDieLayer + UnityEngine.Random.Range(0, 10));
            if (fishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Dragon)
                FishAudioManager.Instance.PlayNormalAudio(64);
            AsyncActionUtils.DelayedAction(fishBehaviour, fishVo.FishConfig.fishDieTime, () => SetDestroy(true));
            float delayTime = 0;
            if (fishVo.DieEffectConfig.fishDieOneShowTime != 0)
                delayTime = fishVo.DieEffectConfig.fishDieOneShowTime;

            switch (fishVo.DieEffectConfig.fishDieBehavior)
            {
                case 1:
                    SetFishMoveStatus(FishStatus.Stop);
                    ShowWinScoreEffect(gameObject.transform.localPosition);
                    AsyncActionUtils.DelayedAction(fishBehaviour, delayTime, FishDieRotation);
                    break;
                case 2:
                    FishHitFly();
                    ShowWinScoreEffect(gameObject.transform.localPosition);
                    break;
                case 3:
                    SetFishMoveStatus(FishStatus.Stop);
                    dieMoveToTargetPos = playerIns.Panel.GetPlayerCatchFishPos();
                    AsyncActionUtils.DelayedAction(fishBehaviour, delayTime, FishCatchToGun);
                    break;
                case 4:
                    SetFishMoveStatus(FishStatus.Stop);
                    Vector3 localPos = gameObject.transform.parent.InverseTransformPoint(playerIns.SwrilPanel.position);
                    dieMoveToTargetPos = localPos - new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), 0);
                    ShowWinScoreEffect(gameObject.transform.localPosition);
                    AsyncActionUtils.DelayedAction(fishBehaviour, delayTime, FishAdsorptionToSwirl);
                    break;
                case 5:
                    SetFishMoveStatus(FishStatus.Stop);
                    dieMoveToTargetPos = playerIns.Panel.GetPlayerCatchFishPos();
                    AsyncActionUtils.DelayedAction(fishBehaviour, delayTime, FishShake);
                    break;
                default:
                    break;
            }
        }

        public bool IsGetHitFishMsg()
        {
            return hitFishMsg != null;
        }

        public void ShowBossDeclareEffect()
        {
            int parentFishId = hitFishMsg.bombUID;
            int score = hitFishMsg.totalScore;
            int multiple = hitFishMsg.totalRatio;

            Vector3 beginPos = gameObject.transform.localPosition;
            FishSpecialDeclareConfig specialDeclareConfig = FishSpecialDeclareEffectManager.Instance.GetSpecialDeclareEffectConfig(fishVo.DieEffectConfig.specialDeclareID);
            int specialDeclareUID = FishSpecialDeclareEffectManager.Instance.SetSpecialDeclareEffectShowMode(playerIns.GetPlayerChairId(), beginPos, specialDeclareConfig, true);
            AsyncActionUtils.DelayedAction(FishBombManager.Instance, specialDeclareConfig.delayTime, () =>
            {
                FishSpecialDeclareEffectManager.Instance.BeginBossDeclare(specialDeclareUID, score, multiple);
            });
        }

        public void FishHitFly()
        {
            AsyncActionUtils.DelayedAction(FishBombManager.Instance, 0.8f, () =>
            {
                SetFishMoveStatus(FishStatus.Stop);
                ShowCoinEffect();
                gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
            });
        }

        public void FishDieRotation()
        {
            float time = fishVo.DieEffectConfig.fishDieThreeShowTime;
            playerIns.Panel.RemovePlayerCatchFishPos();
            if (time == 0)
            {
                gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
                return;
            }
            Vector3 angle = new Vector3(0, 0, fishVo.FishConfig.dieRotationAngle);

            AsyncActionUtils.ApplyRotation(fishBehaviour, gameObject.transform, gameObject.transform.rotation.eulerAngles, angle, time, TweenUtils.VectorTweenLinear, 0f);
            AsyncActionUtils.ApplyScaling(fishBehaviour, gameObject.transform, gameObject.transform.localScale, new Vector3(0, 0, 0), time, TweenUtils.VectorTweenInCubic, 0f, () => {
                ShowCoinEffect();
                gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
            });
        }

        public void FishCatchToGun()
        {
            Vector2 targetPos = dieMoveToTargetPos;
            if (targetPos == null) return;
            float duration = 0.6f;
            AsyncActionUtils.ApplyAnchoredMovement(fishBehaviour, transform, transform.GetComponent<RectTransform>().anchoredPosition, targetPos, duration, TweenUtils.VectorTweenOutCubic, 0, () =>
            {
                ShowWinScoreEffect(gameObject.GetComponent<RectTransform>().anchoredPosition);
                float temp = fishVo.DieEffectConfig.fishDieTwoShowTime == 2.5f ? temp = 540 : 900;
                temp = fishVo.fishId == 26 || fishVo.fishId == 28 ? 45 : temp;
                Vector3 angle = new Vector3(0, 0, temp);
                AsyncActionUtils.ApplyRotation(fishBehaviour, gameObject.transform, gameObject.transform.rotation.eulerAngles, angle, fishVo.DieEffectConfig.fishDieTwoShowTime, TweenUtils.VectorTweenLinear, 0f);
                AsyncActionUtils.DelayedAction(fishBehaviour, fishVo.DieEffectConfig.fishDieTwoShowTime, () =>
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                    FishDieRotation();
                });
            });
        }

        public void FishShake()
        {
            float originalY = transform.GetComponent<RectTransform>().anchoredPosition.y;
            float ranY = UnityEngine.Random.Range(10, 21);
            Vector2 targetPos = new Vector2(transform.GetComponent<RectTransform>().anchoredPosition.x, originalY + ranY);
            AsyncActionUtils.ApplyAnchoredMovement(fishBehaviour, transform, transform.GetComponent<RectTransform>().anchoredPosition, targetPos, 0.05f, TweenUtils.VectorTweenLinear, 0, () =>
            {
                int tweenCount = 0;
                for (int i = 1; i < 10; i++)
                {
                    ranY = UnityEngine.Random.Range(5, 11) * Mathf.Pow(-1, i);
                    targetPos = new Vector2(transform.GetComponent<RectTransform>().anchoredPosition.x, originalY + ranY);
                    AsyncActionUtils.ApplyAnchoredMovement(fishBehaviour, transform, transform.GetComponent<RectTransform>().anchoredPosition, targetPos, 0.05f, TweenUtils.VectorTweenLinear, (i - 1) * 0.05f, () =>
                    {
                        tweenCount++;
                        if (tweenCount == 9)
                        {
                            ShowCoinEffect();
                            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
                        }
                    });
                }
            });
        }

        public void FishAdsorptionToSwirl()
        {
            Vector2 targetPos = dieMoveToTargetPos;
            AsyncActionUtils.ApplyAnchoredMovement(fishBehaviour, transform, transform.GetComponent<RectTransform>().anchoredPosition, targetPos, 0.5f, TweenUtils.VectorTweenOutCubic, 0f, FishDieRotation);
        }

        public abstract void SetHitFlyDirection(Vector3 direction);

        public abstract void SetBeHitColor();

        public abstract void ResetNormalColor();

        public abstract void RemoveFishPart(int id);

        public abstract List<Vector3> GetEffectPoint(int partId = 0);

        public void Update()
        {
            UpdateHit();
            moveTime += Time.deltaTime;
            if (moveTime > 5)
            {
                moveTime = 0;
                int temp = UnityEngine.Random.Range(0, 11);
                int fishRuleType = FishFishManager.Instance.GetFishRuleType(fishVo.fishId);
                if (temp > 5 && (fishVo.fishId == 20 || fishVo.fishId == 4) && fishRuleType != 2)
                {
                    animator.SetTrigger("doAction");
                }
            }
        }

        public void UpdateHit()
        {
            if (isHit)
            {
                currentHitTime += Time.deltaTime;
                if (currentHitTime >= hitTotalTime)
                    ResetNormalColor();
            }
        }

        public void BaseDestroy()
        {
            IsEnableBoxcollider(false);
            isDie = false;
            fishMoveStatus = FishStatus.Stop;
            isCanDestroy = false;
            isCanSeen = false;
            fishBehaviour.curFishStatus = FishStatus.Stop;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
            gameObject.SetActive(false);
        }

        public abstract void Destroy();
    }

    public class FishVo
    {
        public int fishId = 0;
        public int UID = 0;
        public List<uint> FishKindGroup = new List<uint>();
        public int TraceId = 0;
        public int StartPointIndex = 0;
        public int OffsetIndex = 0;
        public int byChairId = 0;
        public FishFishConfig FishConfig;
        public float DelayBornTime;
        public FishDieEffectConfig DieEffectConfig;
        public float OffsetPosX;
        public float OffsetPosY;
        public int IsRedFish;
        public int usGroupId;
    }
}
