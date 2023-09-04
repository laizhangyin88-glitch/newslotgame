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
        private SkeletonAnimation SpineAnim;
        private MeshRenderer SpineMeshRenderer;

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
            PlayMoveAnim();
        }

        public void FindView()
        {
            Transform mTransform = gameObject.transform;
            animator = gameObject.GetComponent<Animator>();
            SpineAnim = mTransform.Find("Bone/Fish").GetComponent<SkeletonAnimation>();
            centerTrans = mTransform.Find("Collider/BoxCollider_02");
            SpineMeshRenderer = mTransform.Find("Bone/Fish").GetComponent<MeshRenderer>();
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
            if (SpineAnim == null) return;
            SpineAnim.timeScale = 1.0f;
            string animName = fishVo.FishConfig.fishMoveAnimationName;
            if (!string.IsNullOrEmpty(animName))
            {
                SpineAnim.loop = true;
                SpineAnim.state.SetAnimation(0, animName, true);
            }
        }

        public void PauseSpineAnimation()
        {
            if (SpineAnim == null) return;
            SpineAnim.timeScale = 0;
        }

        public void IsEnableAnimator(bool isEnabled)
        {
            if (animator != null)
                animator.enabled = isEnabled;
            if (SpineAnim != null)
                SpineAnim.enabled = isEnabled;
        }

        public void PlayBornAnim()
        {
            if (animator != null)
                animator.Play("Fish_Born", 0, 0);
        }

        public override void PlayDieAnim()
        {
            string animName = fishVo.FishConfig.fishDieAnimationName;
            if (!string.IsNullOrEmpty(animName) && animator != null)
                animator.Play(animName, 0, 0);
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
            if (SpineAnim == null) return;
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
