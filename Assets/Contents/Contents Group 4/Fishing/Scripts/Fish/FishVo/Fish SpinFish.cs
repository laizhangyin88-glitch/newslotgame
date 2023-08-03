using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace BagelCode
{
    public class FishSpinFish : FishFishBase
    {
        SkeletonAnimation SpineAnim;
        MeshRenderer SpineMeshRenderer;

        public FishSpinFish()
        {
            Init();
        }

        private void Init()
        {
            InitData();
        }

        private void InitData()
        {

        }

        public override void BuildFish(FishVo vo, GameObject obj)
        {
            InitBaseFish(vo, obj);
            FindView();
            InitViewData();
        }

        public override void ResetFishState(FishVo vo)
        {
            ResetBaseFishStateData(vo);
            IsEnableAnimator(true);
            PlayMoveAnim(true);
        }

        public void FindView()
        {
            Transform trans = gameObject.transform;
            SpineAnim = trans.Find("Bone/Fish").GetComponent<SkeletonAnimation>();
            SpineMeshRenderer = trans.Find("Bone/Fish").GetComponent<MeshRenderer>();
        }

        public void InitViewData()
        {
            PlayMoveAnim(true);
        }

        public override void PlayMoveAnim(bool isLoop)
        {
            if (SpineAnim == null) return;
            string animName = FishVo.FishConfig.fishMoveAnimationName;
            if (!string.IsNullOrEmpty(animName))
            {
                SpineAnim.loop = isLoop;
                SpineAnim.state.SetAnimation(0, animName, isLoop);
            }
        }

        public void IsEnableAnimator(bool isEnable)
        {
            if (SpineAnim != null)
            {
                SpineAnim.enabled = isEnable;
            }
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            if (SpineMeshRenderer != null)
            {
                SpineMeshRenderer.sortingOrder = orderIndex;
            }
        }

        public override void SetHitFlyDirection(Vector3 direction)
        {
            hitFlyDirection = direction;
        }

        public override void SetBeHitColor()
        {
            if (!isHit)
            {
                SetMainFishColor(beHitColor);
                isHit = true;
            }
        }

        public override void ResetNormalColor()
        {
            SetMainFishColor(NormalColor);
            isHit = false;
            currentHitTime = 0;
        }

        public override void SetMainFishColor(Color color)
        {
            if (SpineAnim == null) return;
            SpineAnim.skeleton.R = color.r;
            SpineAnim.skeleton.G = color.g;
            SpineAnim.skeleton.B = color.b;
            SpineAnim.skeleton.A = color.a;
        }

        public override void PlayMoveAnim()
        {
            
        }

        public override void PlayDieAnim(bool isLoop)
        {
            if (SpineAnim == null)
                return;
            var aniName = FishVo.FishConfig.fishDieAnimationName;
            if (aniName != "nil")
            {
                SpineAnim.loop = isLoop;
                SpineAnim.state.SetAnimation(0, aniName, isLoop);
            }
        }

        public override void Destroy()
        {
            ResetNormalColor();
            IsEnableAnimator(false);
            BaseDestroy();
        }

        public override List<Vector3> GetEffectPoint(int partId = 0)
        {
            return null;
        }

        public override void RemoveFishPart(int id)
        {
        }
    }
}
