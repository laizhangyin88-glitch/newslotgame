using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace BagelCode
{
    public enum FishPartType
    {
        NormalLeg = 0,
        LeftLeg = 1,
        RightLeg = 2,
        NoneLeg = 3,
    }
    public class FishPartFish : FishFishBase
    {
        private FishPartType curPartType;
        private Collider[] partCollders;
        private Animator animator;
        private SkeletonAnimation SpineAnim;
        private MeshRenderer SpineMeshRenderer;
        private List<Transform> lockPointList;
        private List<Transform> effectPointList;
        private int PartCount;

        public FishPartFish()
        {
            curPartType = FishPartType.NormalLeg;
            partCollders = new Collider[4];
        }

        public override void BuildFish(FishVo vo, GameObject obj)
        {
            base.BuildFish(vo, obj);
            FindView();
            BuildPartFish();
            InitViewData();
        }

        public override void ResetFishState(FishVo vo)
        {
            base.ResetFishState(vo);
            BuildPartFish();
            InitViewData();
        }

        public void FindView()
        {
            Transform mTransform = gameObject.transform;
            animator = gameObject.GetComponent<Animator> ();
            SpineAnim = mTransform.Find("Bone/Fish").GetComponent<SkeletonAnimation>();
            SpineMeshRenderer = mTransform.Find("Bone/Fish").GetComponent<MeshRenderer>();
            lockPointList = new List<Transform>();
            effectPointList = new List<Transform>();
            for (int i = 0; i < 3; i++)
            {
                Transform lockPoint = mTransform.Find("LockPoint/LockPoint" + i);
                Transform effectPoint = mTransform.Find("EffectPoint/EffectPoint" + i);
                lockPointList.Add(lockPoint);
                effectPointList.Add(effectPoint);
            }
        }

        public bool RemoveLockPart(Transform lockTrans)
        {
            for (int i = 0; i < 3; i++)
            {
                if (lockPointList[i] == lockTrans && !partCollders[i].enabled)
                {
                    return true;
                }
            }
            return false;
        }

        public void BuildPartFish()
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.name == "BoxCollider_01")
                    partCollders[1] = colliders[i];
                else if (colliders[i].gameObject.name == "BoxCollider_02")
                    partCollders[1] = colliders[i];
                else if (colliders[i].gameObject.name == "BoxCollider_03")
                    partCollders[1] = colliders[i];
            }

            if (fishVo.FishKindGroup != null)
            {
                if (fishVo.FishKindGroup.Count > 0)
                {
                    if (fishVo.FishKindGroup[(int)FishPartType.LeftLeg - 1] == 1)
                    {
                        curPartType = FishPartType.LeftLeg;
                        //partCollders[(int)FishPartType.LeftLeg - 1].enabled = true;
                        PartCount = 1;
                    }
                    else
                    {
                        //partCollders[(int)FishPartType.LeftLeg - 1].enabled = false;
                    }

                    if (fishVo.FishKindGroup[(int)FishPartType.RightLeg - 1] == 1)
                    {
                        curPartType = FishPartType.RightLeg;
                        //partCollders[(int)FishPartType.RightLeg - 1].enabled = true;
                        PartCount = 1;
                    }
                    else
                    {
                        partCollders[(int)FishPartType.RightLeg - 1].enabled = false;
                    }

                    if (fishVo.FishKindGroup[(int)FishPartType.LeftLeg - 1] == 1 && fishVo.FishKindGroup[(int)FishPartType.RightLeg] == 1)
                    {
                        curPartType = FishPartType.NormalLeg;
                        PartCount = 2;
                    }

                    if (fishVo.FishKindGroup[(int)FishPartType.LeftLeg - 1] == 0 && fishVo.FishKindGroup[(int)FishPartType.RightLeg] == 0)
                    {
                        curPartType = FishPartType.NoneLeg;
                        //partCollders[(int)FishPartType.LeftLeg - 1].enabled = false;
                        //partCollders[(int)FishPartType.RightLeg - 1].enabled = false;
                        PartCount = 0;
                    }
                }
            }
            partCollders[1].gameObject.tag = "Left_Leg";
            partCollders[2].gameObject.tag = "Right_Leg";
        }

        private void InitViewData()
        {
            IsEnableAnimator(true);
            PlayBornAnim();
            PlayMovePartAnim();
        }

        public override List<Vector3> GetEffectPoint(int partID)
        {
            return new List<Vector3> {lockPointList[partID].position };
        }

        public Transform GetLockPartPoint()
        {
            switch (curPartType)
            {
                case FishPartType.NormalLeg:
                    return lockPointList[2];
                case FishPartType.LeftLeg:
                    return lockPointList[1];
                case FishPartType.RightLeg:
                    return lockPointList[2];
                case FishPartType.NoneLeg:
                    return lockPointList[3];
                default:
                    return null;
            }
        }

        public Collider GetColliderObj()
        {
            switch (curPartType)
            {
                case FishPartType.NormalLeg:
                    return partCollders[2];
                case FishPartType.LeftLeg:
                    return partCollders[1];
                case FishPartType.RightLeg:
                    return partCollders[2];
                case FishPartType.NoneLeg:
                    return partCollders[3];
                default:
                    return null;
            }
        }

        public override void RemoveFishPart(int partID)
        {
            if (PartCount == 2)
            {
                if (partID == 1)
                {
                    SpineAnim.skeleton.ScaleX = -1;
                    PlayHitPartAnim("HURT_L");
                    partCollders[(int)FishPartType.LeftLeg].enabled = false;
                    curPartType = FishPartType.RightLeg;
                }
                else if (partID == 2)
                {
                    SpineAnim.skeleton.ScaleX = 1;
                    PlayHitPartAnim("HURT_L");
                    partCollders[(int)FishPartType.RightLeg].enabled = false;
                    curPartType = FishPartType.LeftLeg;
                }
                PartCount -= 1;
                AsyncActionUtils.DelayedAction(fishBehaviour, 0.6f, PlayMovePartAnim);

            }
            else if (PartCount == 1)
            {
                if (partID == 1)
                {
                    SpineAnim.skeleton.ScaleX = 1;
                    partCollders[(int)FishPartType.LeftLeg].enabled = false;
                    PlayHitPartAnim("HURT_R");
                }
                else if(partID == 2)
                {
                    SpineAnim.skeleton.ScaleX = -1;
                    partCollders[(int)FishPartType.RightLeg].enabled = false;
                    PlayHitPartAnim("HURT_R");
                }
                curPartType = FishPartType.NoneLeg;
                PartCount -= 1;
                AsyncActionUtils.DelayedAction(fishBehaviour, 0.6f, PlayMovePartAnim);
            }
        }

        private void PlayHitPartAnim(string animationName)
        {
            SpineAnim.timeScale = 1;
            SpineAnim.state.SetAnimation(0, animationName, false);
        }

        private void PlayMovePartAnim()
        {
            SpineAnim.timeScale = 1;
            if (curPartType == FishPartType.NormalLeg)
                SpineAnim.state.SetAnimation(0, "SWIM", true);
            else if (curPartType == FishPartType.LeftLeg)
            {
                SpineAnim.skeleton.ScaleX = 1;
                SpineAnim.state.SetAnimation(0, "CRAZY", true);
            }
            else if (curPartType == FishPartType.RightLeg)
            {
                SpineAnim.skeleton.ScaleX = -1;
                SpineAnim.state.SetAnimation(0, "CRAZY", true);
            }
            else if (curPartType == FishPartType.NoneLeg)
            {
                SpineAnim.state.SetAnimation(0, "WEAK", true);
            }
        }

        public void PauseSpineAnimation()
        {
            SpineAnim.timeScale = 0;
        }

        public void PlayBornAnim()
        {
            string animName = fishVo.FishConfig.fishMoveAnimationName;
            if (!string.IsNullOrEmpty(animName))
                animator.Play(animName, 0, 0);
        }

        public override void PlayDieAnim()
        {
            string animName = fishVo.FishConfig.fishDieAnimationName;
            if (!string.IsNullOrEmpty(animName))
                animator.Play(animName, 0, 0);
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            if (SpineMeshRenderer != null)
            {
                SpineMeshRenderer.sortingOrder = orderIndex;
            }
        }

        public void IsEnableAnimator(bool isEnabled)
        {
            if (animator != null)
                animator.enabled = isEnabled;
            if (SpineAnim != null)
                SpineAnim.enabled = isEnabled;
        }

        public override void ResetNormalColor()
        {
            SetMainFishColor(NormalColor);
            isHit = false;
            currentHitTime = 0;
        }

        public override void SetMainFishColor(Color color)
        {
            SpineAnim.skeleton.R = color.r;
            SpineAnim.skeleton.G = color.g;
            SpineAnim.skeleton.B = color.b;
            SpineAnim.skeleton.A = color.a;
        }

        public override void Destroy()
        {
            ResetNormalColor();
            IsEnableAnimator(false);
            BaseDestroy();
        }
    }
}
