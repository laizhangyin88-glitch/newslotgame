using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSingleGoldEffect
    {
        GameObject gameObject;
        Transform transform;
        FishGoldEffectManager goldEffectManager;
        Animator Anim;
        GameObject GoldImageObj;
        public EffectVo EffectVo;
        Action callBack;
        string MyAnimName;
        string OtherAnimName;
        public bool isCanDestroy;
        Vector3 endPos;
        float jumpHeight;
        float yValue;

        public FishSingleGoldEffect(GameObject gameObj)
        {
            gameObject = gameObj;
            transform = gameObj.transform;
            InitData();
            InitView();
        }

        private void InitData()
        {
            goldEffectManager = FishGoldEffectManager.Instance;
            MyAnimName = "Gold01";
            OtherAnimName = "Gold02";

            isCanDestroy = false;
            endPos = Vector3.zero;
            jumpHeight = 100;
            yValue = 0;
        }

        private void InitView()
        {
            FindView();
        }

        private void FindView()
        {
            Anim = gameObject.GetComponent<Animator>();
            GoldImageObj = transform.Find("Gold_Image").gameObject;
        }

        public void PlayAnim()
        {
            if (EffectVo.isMe)
                Anim.Play(MyAnimName, 0, 0);
            else
                Anim.Play(OtherAnimName, 0, 0);
        }

        public void ResetEffectVo(EffectVo vo)
        {
            EffectVo = vo;
            isCanDestroy = false;
        }

        public void ResetState(Vector3 beginPos, Vector3 endPos, float delayTime, Action callBack)
        {
            this.endPos = endPos;
            this.callBack = callBack;
            Anim.enabled = true;
            transform.position = beginPos;
            transform.localScale = Vector3.one;
            GoldImageObj.SetActive(true);
            this.yValue = transform.localPosition.y;

            Action showGoldCallBack = () => {
                GoldImageObj.SetActive(true);
                PlayAnim();
            };

            float duration1 = 0.2f;
            float y1 = jumpHeight + yValue;

            Vector3 targetPos = new Vector3(transform.localPosition.x, y1, transform.localPosition.z);
            AsyncActionUtils.DelayedAction(FishGoldEffectManager.Instance, delayTime, () =>
            {
                showGoldCallBack();
                AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, duration1, TweenUtils.VectorTweenOutCubic, 0, () =>
                {
                    targetPos = new Vector3(transform.localPosition.x, yValue, transform.localPosition.z);
                    AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, duration1, TweenUtils.VectorTweenInCubic, 0, () =>
                    {
                        TwoStepJump();
                    });
                });
            });
        }

        public void TwoStepJump()
        {
            //float y2 = jumpHeight * 0.4f + yValue;
            //Vector3 targetPos = new Vector3(transform.localPosition.x, y2, transform.localPosition.z);
            //AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, 0.1f, TweenUtils.VectorTweenOutCubic, 0, () => {
            //    targetPos = new Vector3(transform.localPosition.x, yValue, transform.localPosition.z);
            //    AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, 0.1f, TweenUtils.VectorTweenInCubic, 0, () =>
            //    {
            //        targetPos = new Vector3(transform.localPosition.x, y2, transform.localPosition.z);
            //        AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, 0.07f, TweenUtils.VectorTweenOutCubic, 0, () =>
            //        {
            //            targetPos = new Vector3(transform.localPosition.x, yValue, transform.localPosition.z);
            //            AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, 0.07f, TweenUtils.VectorTweenOutCubic, 0, CenterToJumpOver);
            //        });
            //    });
            //});
            float y2 = jumpHeight * 0.4f + yValue;
            Vector3 targetPos = new Vector3(transform.localPosition.x, y2, transform.localPosition.z);
            AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, 0.1f, TweenUtils.VectorTweenOutCubic, 0, () => {
                targetPos = new Vector3(transform.localPosition.x, yValue, transform.localPosition.z);
                AsyncActionUtils.ApplyLocalMovement(FishGoldEffectManager.Instance, transform, transform.localPosition, targetPos, 0.1f, TweenUtils.VectorTweenInCubic, 0, CenterToJumpOver);
            });
        }

        public void CenterToJumpOver()
        {
            goldEffectManager.GoldCenterToJumpOver(this);
        }

        public void MoveToEndPoint(float delay)
        {
            float duration = EffectVo.DurationTime;
            duration = duration < 0.8f ? 0.8f : duration;
            duration = duration > 1.5f ? 1.5f : duration;
            AsyncActionUtils.DelayedAction(FishGoldEffectManager.Instance, delay, () =>
            {
                Vector3 beginPos = transform.TransformPoint(transform.position);
                Vector2 temp = transform.GetComponent<RectTransform>().anchoredPosition;
                AsyncActionUtils.ApplyAnchoredMovement(FishGoldEffectManager.Instance, transform, temp, new Vector2(endPos.x, endPos.y), duration, TweenUtils.VectorTweenInSine);
                AsyncActionUtils.ApplyScaling(FishGoldEffectManager.Instance, transform, transform.localScale, new Vector3(0.6f, 0.6f, 1), duration, TweenUtils.VectorTweenInSine, 0, GoldMoveEnd);
            });
        }

        public void GoldMoveEnd()
        {
            isCanDestroy = true;
            EndBackCall();
        }

        public void EndBackCall()
        {
            if (callBack != null)
                callBack();
        }

        public void Destroy()
        {
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }
    }
}
