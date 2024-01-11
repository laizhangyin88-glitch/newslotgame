using fishMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace BagelCode
{
    public class FishBullet : FishBulletBase
    {
        string FishTag;
        string Left_Leg_Tag;
        string Right_Leg_Tag;

        public FishBullet(GameObject gameObject) : base(gameObject)
        {
            Init();
        }

        private void Init()
        {
            InitData();
            AddEvenListener();
        }

        private void InitData()
        {
            FishTag = "Fish";
            Left_Leg_Tag = "Left_Leg";
            Right_Leg_Tag = "Right_Leg";
        }

        private void AddEvenListener()
        {
            behaviour.onTriggerCallBack = OnTriggerEnter;
        }

        public void DestoryBullet()
        {
            IsEnabledCollider(false);
            isCanDestroy = true;
            behaviour.curFishStatus = FishBehaviour.FishStatus.Stop;
        }

        public void SendPlayerHitFishMsg(int FishUID, bool isSendRobotChairId)
        {
            HitfishReq mes = new HitfishReq
            {
                fishId = FishUID,
                bulletid = BulletVo.BulletUID
            };
            if (isSendRobotChairId)
            {
                mes.usRobotChairId = BulletVo.chairId;
            }
            else
                mes.usRobotChairId = -1;
            FishFishManager.Instance.RequestPlayerHitFishMsg(mes);
        }

        public void SendPlayerHitFishProcess(int FishUID)
        {
            int tempPlayerChairId = GetBulletBelongPlayerID();
            SendPlayerHitFishMsg(FishUID, false);



            //if (tempPlayerChairId != -1)
            //{
            //    if (BulletVo.usProcUserChairId != -1 && BulletVo.usProcUserChairId == FishBulletManager.Instance.gameData.playerChairId)
            //        SendPlayerHitFishMsg(FishUID, true);
            //    else if (tempPlayerChairId != FishBulletManager.Instance.gameData.playerChairId)
            //        return;
            //    SendPlayerHitFishMsg(FishUID, false);
            //}
        }

        public void CreateNet(Vector3 targetNetPos)
        {
            GetBulletTargerPlayer().CreateNet(targetNetPos);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(FishTag))
            {
                FishFishBase hitFishBase = other.transform.parent.parent.GetComponent<FishBehaviour>().fishIns;
                if (hitFishBase != null)
                {
                    //if (hitFishBase.fishVo.fishId == 48)
                    //    Debug.LogError("命中龙");
                    if (hitFishBase.GetIsDie())
                        return;
                    if (targetFish != null && hitFishBase != targetFish)
                        return;
                    if (targetFish != null && hitFishBase.fishVo.fishCfg.clientBuildFishType == (int)FishGameConfig.FishType.Part)
                    {
                        FishPartFish HitPartFish = hitFishBase as FishPartFish;
                        if (!other.gameObject.CompareTag(HitPartFish.GetColliderObj().gameObject.tag))return;
                    }
                    hitFishBase.SetHitFlyDirection(behaviour.m_direction);
                    hitFishBase.SetBeHitColor();
                    DestoryBullet();
                    SendPlayerHitFishProcess(hitFishBase.fishVo.UID);
                    CreateNet(gameObject.transform.localPosition);
                    if (gameData.playerChairId == GetBulletTargerPlayer().GetPlayerChairId())
                    {
                        //HitFishBase.SetTipsContentInfo();
                    }
                }
            }
            else if (other.gameObject.CompareTag(Left_Leg_Tag) || other.gameObject.CompareTag(Right_Leg_Tag))
            {
                FishFishBase HitFishBase = other.transform.parent.parent.GetComponent<FishBehaviour>().fishIns;
                if (HitFishBase != null)
                {
                    if (HitFishBase.GetIsDie())
                        return;
                    if (targetFish != null && HitFishBase != targetFish)
                        return;
                    if (targetFish != null && HitFishBase.fishVo.fishCfg.clientBuildFishType == (int)FishGameConfig.FishType.Part)
                    {
                        FishPartFish HitPartFish = HitFishBase as FishPartFish;
                        if (!other.gameObject.CompareTag(HitPartFish.GetColliderObj().gameObject.tag))
                        {
                            return;
                        }
                    }
                    HitFishBase.SetHitFlyDirection(behaviour.m_direction);
                    HitFishBase.SetBeHitColor();
                    DestoryBullet();

                    int partID = 1;
                    if (other.gameObject.CompareTag(Left_Leg_Tag))
                        partID = 1;
                    else if (other.gameObject.CompareTag(Right_Leg_Tag))
                        partID = 2;
                    SendPlayerHitFishPart(HitFishBase.fishVo.UID, partID);
                    CreateNet(gameObject.transform.localPosition);
                }
            }
        }
        public void SendPlayerHitFishPart(int FishUID, int partID)
        {
            int tempPlayerID = GetBulletBelongPlayerID();
            if (tempPlayerID != 0 && tempPlayerID != FishBulletManager.Instance.gameData.playerChairId)
                return;
            //todo
        }

        public void Update()
        {
            if (targetFish != null)
            {
                if (targetFish.fishVo.fishCfg.clientBuildFishType == (int)FishType.Part)
                {
                    if (targetFish.GetIsDie() || targetFish.GetIsDestroy() || !targetFish.CheckBoundValid())
                    {
                        SetBulletLockTargetFish(null);
                    }
                    else if(behaviour.m_targetTrans != null)
                    {
                        var fish = targetFish as FishPartFish;
                        if (fish.RemoveLockPart(behaviour.m_targetTrans))
                        {
                            SetTargetFishTransform(null);
                            SetTargetFish(null);
                        }
                    }
                }
                else
                {
                    if (targetFish.GetIsDie() || targetFish.GetIsDestroy() || !targetFish.CheckBoundValid())
                    {
                        SetBulletLockTargetFish(null);
                    }
                }
            }
        }

        public void Destroy()
        {
            behaviour.curFishStatus = FishBehaviour.FishStatus.Stop;
            isCanDestroy = false;
            gameObject.SetActive(false);
            IsEnabledFishBehaviour(false);
            IsEnabledAnimator(false);
            SetBulletLockTargetFish(null);
            gameObject.transform.localPosition = new Vector3(Random.Range(2000, 5000), 5000, 0);
        }
    }
}
