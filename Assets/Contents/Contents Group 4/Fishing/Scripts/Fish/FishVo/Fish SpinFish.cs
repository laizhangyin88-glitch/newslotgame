using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSpinFish : FishFishBase
    {
        SkeletonAnimation spineAnim;
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
            if (vo.fishId == 47)
            {
                BuildFishBase(vo, obj);
                AddBehaviourScript();
                SetFishChildTag();
                IsShowFishLight(false);
            }
            else
                base.BuildFish(vo, obj); 
            FindView();
            InitViewData();
        }

        public override void ResetFishState(FishVo vo)
        {
            IsEnableAnimator(true);
            //龙龟特殊处理
            if (vo.fishId == 47)
            {
                UpdateFishVo(vo);
                IsShowFishLight(false);
                isCanDestroy = false;
                gameObject.SetActive(true);
                PlayBornAnim();
            }
            else
            {
                base.ResetFishState(vo);
                PlayMoveAnim();
            }
        }

        public void FindView()
        {
            Transform trans = gameObject.transform;
            spineAnim = trans.Find("Bone/Fish").GetComponent<SkeletonAnimation>();
            SpineMeshRenderer = trans.Find("Bone/Fish").GetComponent<MeshRenderer>();
        }

        public void InitViewData()
        {
            if (fishVo.fishId == 47)
                PlayBornAnim();
            else
                PlayMoveAnim();
        }

        public override void PlayMoveAnim()
        {
            if (spineAnim == null) return;
            string animName = fishVo.fishCfg.fishMoveAnimationName;
            if (!string.IsNullOrEmpty(animName))
            {
                spineAnim.loop = true;
                spineAnim.state.SetAnimation(0, animName, true);
            }
        }

        public void PlayBornAnim()
        {
            if (spineAnim == null) return;
            spineAnim.state.SetAnimation(0, "1rotate", true);
            fishBehaviour.DragonTurtleBeginMove();
        }

        public void IsEnableAnimator(bool isEnable)
        {
            if (spineAnim != null)
            {
                spineAnim.enabled = isEnable;
            }
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            if (SpineMeshRenderer != null)
            {
                SpineMeshRenderer.sortingOrder = orderIndex;
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
            if (spineAnim == null) return;
            spineAnim.skeleton.R = color.r;
            spineAnim.skeleton.G = color.g;
            spineAnim.skeleton.B = color.b;
            spineAnim.skeleton.A = color.a;
        }

        public override void PlayDieAnim()
        {
            if (spineAnim == null)
                return;
            var aniName = fishVo.fishCfg.fishDieAnimationName;
            if (aniName != "nil")
            {
                spineAnim.loop = false;
                spineAnim.state.SetAnimation(0, aniName, false);
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

        public void Change2Normal()
        {
            spineAnim.state.SetEmptyAnimation(0, 0);
            spineAnim.state.SetAnimation(0, "3come_in", false);
            spineAnim.state.Complete += SpineAnim;
        }

        private void SpineAnim(TrackEntry trackEntry)
        {
            spineAnim.state.SetAnimation(0, "4walk", true);
            IsEnableBoxcollider(true);
            BeginMove();
            spineAnim.state.Complete -= SpineAnim;
        }
    }
}
