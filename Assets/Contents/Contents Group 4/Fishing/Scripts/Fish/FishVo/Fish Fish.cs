using System;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFish : FishFishBase
    {
        private Animator animator;
        private SpriteRenderer mainFishSpriteRender;
        private SpriteRenderer shadowFishSpriteRender;
        private SpriteRenderer lightSpriteRender;
        public override void BuildFish(FishVo vo, GameObject obj)
        {
            InitBaseFish(vo, obj);
            FindView();
            InitViewData();
        }

        public override void ResetFishState(FishVo vo)
        {
            ResetBaseFishStateData(vo);
            IsEnableRenderer(true);
            IsEnableAnimator(true);
            PlayMoveAnim();
        }

        public void FindView()
        {
            Transform mTransform = gameObject.transform;
            FindMainFishRenderView(mTransform);
            animator = gameObject.GetComponent<Animator>();
        }

        public void FindMainFishRenderView(Transform tf)
        {
            Transform tempObj = tf.Find("Bone/Fish");
            if (tempObj != null)
                mainFishSpriteRender = tempObj.GetComponent<SpriteRenderer>();
            tempObj = tf.Find("Bone/Shadow");
            if (tempObj != null)
                shadowFishSpriteRender = tempObj.GetComponent<SpriteRenderer>();
            tempObj = tf.Find("Bone/Light");
            if (tempObj != null)
                lightSpriteRender = tempObj.GetComponent <SpriteRenderer>();
        }

        public void InitViewData()
        {
            PlayMoveAnim();
        }

        public override void PlayMoveAnim()
        {
            string animName = FishVo.FishConfig.fishMoveAnimationName;
            if ( animator != null && !string.IsNullOrEmpty(animName))
                animator.Play(animName);
        }

        public override void PlayDieAnim(bool isLoop)
        {
            string animName = FishVo.FishConfig.fishDieAnimationName;
            if (animator != null && !string.IsNullOrEmpty(animName))
                animator.Play(animName);
        }

        public override void SetMainFishOrder(int orderIndex)
        {
            if (mainFishSpriteRender != null)
                mainFishSpriteRender.sortingOrder = orderIndex;
            if (shadowFishSpriteRender != null)
                shadowFishSpriteRender.sortingOrder = orderIndex - 1;
            if (lightSpriteRender != null)
                lightSpriteRender.sortingOrder = orderIndex + 1;
        }

        public override void SetHitFlyDirection(Vector3 direction)
        {
            hitFlyDirection = direction;
        }

        public void IsEnableAnimator(bool isEnable)
        {
            if (animator != null)
                animator.enabled = isEnable;
        }

        public override void SetBeHitColor()
        {
            if (!isHit)
            {
                SetMainFishColor(beHitColor);
                isHit = true;
            }
        }

        public void IsEnableRenderer(bool isEnable)
        {
            if (mainFishSpriteRender != null)
                mainFishSpriteRender.enabled = isEnable;
        }

        public override void ResetNormalColor()
        {
            SetMainFishColor(NormalColor);
            isHit = false;
            currentHitTime = 0;
        }

        public override void SetMainFishColor(Color color)
        {
            mainFishSpriteRender.color = color;
        }

        public override void PlayMoveAnim(bool isLoop)
        {
        }

        public override void Destroy()
        {
            ResetNormalColor();
            IsEnableRenderer(false);
            IsEnableAnimator(false);
            BaseDestroy();
        }

        public override List<Vector3> GetEffectPoint(int partId = 0)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 1; i < 4; i++)
            {
                var temp = transform.Find("Collider/BoxCollider_0" + i);
                if (temp != null)
                    points.Add(temp.position);
            }
            if (points.Count > 0)
                return points;
            else
                return null;
        }

        public override void RemoveFishPart(int id)
        {
        }
    }

}
