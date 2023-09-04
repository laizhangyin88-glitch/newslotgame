using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BagelCode
{
    public class FishBulletBase
    {
        public GameObject gameObject;
        public bool isCanDestroy;
        public FishGameUIManager gameUIManager;
        public FishGameData gameData;
        public float bulletSpeed = 1000;
        public FishFishBase targetFish;
        public FishBehaviour behaviour;
        public BulletVo BulletVo;
        Animator animator;
        Collider colliders;
        public FishPlayerInfo player;
        public FishBulletBase(GameObject gameObject)
        {
            this.gameObject = gameObject;
            BaseInit();
        }

        public void BaseInit()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            bulletSpeed = 1000;
            isCanDestroy = false;
            BaseInitBulletVo();
            BaseInitView();
        }

        public void BaseInitBulletVo()
        {
            BulletVo = new BulletVo();
            BulletVo.chairId = 0;
            BulletVo.BulletLevel = 1;
            BulletVo.BulletAngle = 1;
            BulletVo.BulletUID = 0;
        }

        public void BaseInitView()
        {
            BaseFindView();
            BaseInitViewData();
        }

        public void BaseFindView()
        {
            Transform tf = gameObject.transform;
            animator = gameObject.GetComponent<Animator>();
            behaviour = gameObject.GetComponent<FishBehaviour>();
            if (behaviour == null)
            {
                behaviour = gameObject.AddComponent<FishBehaviour>();
            }
            colliders = gameObject.GetComponent<Collider>();
        }

        public void BaseInitViewData()
        {
            gameObject.SetActive(true);
            IsEnabledCollider(true);
            IsEnabledAnimator(true);
            IsEnabledFishBehaviour(true);
        }

        public void UpdateBulletVo(BulletVo bulletVo)
        {
            BulletVo = bulletVo;
        }

        public void SetBulletTargetPlayer(FishPlayerInfo playerIns)
        {
            player = playerIns;
        }

        public FishPlayerInfo GetBulletTargerPlayer()
        {
            return player;
        }

        public int GetBulletBelongPlayerUserID()
        {
            FishPlayerInfo targetPlayer = GetBulletTargerPlayer();
            if (targetPlayer != null)
                return targetPlayer.GetUserID();
            return 0;
        }

        public int GetBulletBelongPlayerID()
        {
            FishPlayerInfo targetPlayer = GetBulletTargerPlayer();
            if (targetPlayer != null)
                return targetPlayer.GetPlayerChairId();
            return -1;
        }

        public void IsEnabledFishBehaviour(bool isEnabled)
        {
            if (behaviour != null)
                behaviour.enabled = isEnabled;
        }

        public void IsEnabledAnimator(bool isEnabled)
        {
            if (animator != null)
                animator.enabled = isEnabled;
        }

        public void IsEnabledCollider(bool isEnabled)
        {
            if (colliders != null)
                colliders.enabled = isEnabled;
        }

        public void SetBulletKindAnim(string bulletName)
        {
            PlayBulletAnim(bulletName);
        }

        public void PlayBulletAnim(string bulletName)
        {
            if (animator == null)
            {
                Debug.LogError("bulletName = > " + bulletName);
            }
            
            animator.Play(bulletName);
        }

        public void SetLocalPosition(float x, float y, float z)
        {
            gameObject.transform.localPosition = new UnityEngine.Vector3(x, y, z);
        }

        public void BulletBeginMove(float speed)
        {
            behaviour.BulletBeginMove(speed);
        }

        public void IsShowBullet(bool isDisplay)
        {
            gameObject.SetActive(isDisplay);
        }

        public void SetTargetFish(FishFishBase targetFish)
        {
            this.targetFish = targetFish;
        }

        public void SetTargetFishTransform(Transform targetFishTrans)
        {
            behaviour.m_targetTrans = targetFishTrans;
        }

        public void SetBulletLockTargetFish(FishFishBase lockFish)
        {
            SetTargetFish(lockFish);
            if (lockFish != null)
            {
                if (lockFish.fishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Part)
                {
                    FishPartFish partFish = lockFish as FishPartFish;
                    Transform lockFishTrans = partFish.GetLockPartPoint();
                    if (lockFishTrans != null)
                        SetTargetFishTransform(lockFishTrans);
                }
                else if (lockFish.fishVo.FishConfig.clientBuildFishType == (int)FishGameConfig.FishType.Dragon)
                {
                    FishDragonFish dargonFish = lockFish as FishDragonFish;
                    Transform lockFishTrans = dargonFish.GetLockPartPoint();
                    if (lockFishTrans != null)
                        SetTargetFishTransform(lockFishTrans);
                }
                else
                {
                    Transform lockFishTrans = lockFish.gameObject.transform;
                    if (lockFishTrans != null)
                        SetTargetFishTransform(lockFishTrans);
                }
            }
            else
                SetTargetFishTransform(null);
        }
    }
}
