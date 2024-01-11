using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace BagelCode
{
    public class FishDragonFish : FishFishBase
    {
        float Score = 0;
        float Multiple = 0;
        float specialDeclareUID = 0;
        private Transform centerTrans;
        private List<Vector3> effectPosList;
        private Animator animator;
        private SkeletonAnimation skeletonAni;
        private MeshRenderer meshRender;

        public new bool CheckBoundValid()
        {
            float ResolutionWidthHalf = FishCsharpManager.curResolutionWidth * 0.5f;
            float ResolutionHeightHalf = FishCsharpManager.curResolutionHeight * 0.5f;
            Vector3 pos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.FishPool).transform.InverseTransformPoint(centerTrans.position);
            if (pos.x < -ResolutionWidthHalf || pos.x > ResolutionWidthHalf) return false;
            if (pos.y < -ResolutionHeightHalf || pos.y > ResolutionHeightHalf) return false;
            return true;
        }

        public override void BuildFish(FishVo vo, GameObject obj)
        {
            base.BuildFish(vo, obj);
            FindView();
            InitViewData();
            PlayBornAnim();
        }

        public override void ResetFishState(FishVo vo)
        {
            base.ResetFishState(vo);
            IsEnableAnimator(true);
            effectPosList = new List<Vector3>();
            PlayBornAnim();
            //PlayMoveAnim();
        }

        public void FindView()
        {
            animator = gameObject.GetComponent<Animator>();
            skeletonAni = transform.Find("Bone/Fish").GetComponent<SkeletonAnimation>();
            centerTrans = transform.Find("Collider/BoxCollider_02");
            meshRender = transform.Find("Bone/Fish").GetComponent<MeshRenderer>();
        }

        public void InitViewData()
        {
            PlayMoveAnim();
        }

        public override List<Vector3> GetEffectPoint(int partId = 0)
        {
            effectPosList = new List<Vector3>();
            for (int i = 0; i < 5; i++)
            {
                Vector3 tempPos = gameObject.transform.Find("Collider/BoxCollider_0" + i).position;
                effectPosList.Add(tempPos);
            }
            return effectPosList;
        }

        public Transform GetLockPartPoint()
        {
            return centerTrans;
        }

        public override void PlayMoveAnim()
        {
            if (skeletonAni == null) return;
            skeletonAni.timeScale = 0.3f;
            string animName = fishVo.fishCfg.fishMoveAnimationName;
            if (!string.IsNullOrEmpty(animName))
            {
                //再次播放会倒序播放, 直接用循环返回第0帧
                skeletonAni.state.SetAnimation(0, animName, true);
                skeletonAni.state.Complete += StateComplete;
            }
        }

        private void StateComplete(Spine.TrackEntry trackEntry)
        {
            skeletonAni.timeScale = 0;
            isCanDestroy = true;
        }

        public void PauseSpineAnimation()
        {
            if (skeletonAni == null) return;
            skeletonAni.timeScale = 0;
        }

        public void IsEnableAnimator(bool isEnabled)
        {
            if (animator != null)
                animator.enabled = isEnabled;
            if (skeletonAni != null)
                skeletonAni.enabled = isEnabled;
        }

        public void PlayBornAnim()
        {
            if (animator != null)
                animator.Play("Fish_Born", 0, 0);
        }

        public override void PlayDieAnim()
        {
            string animName = fishVo.fishCfg.fishDieAnimationName;
            if (!string.IsNullOrEmpty(animName) && animator != null)
                animator.Play(animName, 0, 0);
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            if (meshRender != null)
            {
                meshRender.sortingOrder = orderIndex;
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
            if (skeletonAni == null) return;
            skeletonAni.skeleton.R = color.r;
            skeletonAni.skeleton.G = color.g;
            skeletonAni.skeleton.B = color.b;
            skeletonAni.skeleton.A = color.a;
        }

        public override void Destroy()
        {
            ResetNormalColor();
            IsEnableAnimator(false);
            BaseDestroy();
        }
    }
}
