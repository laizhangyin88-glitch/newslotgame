using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishLightningEffectItem
    {
        GameObject gameObject;
        public bool isCanDestroy;
        bool isPlayingAnim;
        bool isDelayPlay;
        bool isCanMove;
        float delayTime;
        float currentTime;
        float totalTime;
        Vector3 beginPos;
        Vector3 targetPos;
        float distance;
        Animator animator;
        RectTransform lightningRect;
        float width;
        public EffectVo lightningVo;
        Action callBack;
        public FishLightningEffectItem(GameObject obj)
        {
            gameObject = obj;
            InitData();
            FindView();
        }

        private void InitData()
        {
            totalTime = 1;
            beginPos = Vector3.zero;
            targetPos = Vector3.zero;
        }

        private void FindView()
        {
            animator = gameObject.GetComponent<Animator>();
            lightningRect = gameObject.transform.Find("lightningImage").GetComponent<RectTransform>();
            width = lightningRect.rect.width;
        }

        private void PlayAnim()
        {
            gameObject.transform.position = beginPos;
            gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, (targetPos - beginPos));
            lightningRect.sizeDelta = new Vector2(width, distance);
            animator.Play(lightningVo.effectName, 0, 0);
            FishAudioManager.Instance.PlayNormalAudio(40);
        }

        public void ResetEffectVo(EffectVo vo)
        {
            lightningVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayPlay = false;
        }

        public void ResetState(Vector3 beginPos, Vector3 targetPos, float distance, Action callBack = null)
        {
            isCanMove = false;
            isCanDestroy = false;
            this.beginPos = beginPos;
            this.targetPos = targetPos;
            this.distance = distance;
            gameObject.transform.position = new Vector3(10000, 10000, 0);
            delayTime = lightningVo.delayTime;
            totalTime = lightningVo.lifeTime;
            this.callBack = callBack;
            currentTime = 0;
            isDelayPlay = true;
        }

        public void Update()
        {
            if (isDelayPlay)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= delayTime)
                {
                    isDelayPlay = false;
                    currentTime = 0;
                    PlayAnim();
                    isPlayingAnim = true;
                }
            }

            if (isPlayingAnim)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= totalTime)
                {
                    currentTime = 0;
                    isPlayingAnim = false;
                    EndCallBack();
                    isCanDestroy = true;
                }
            }
        }

        private void EndCallBack()
        {
            callBack?.Invoke();
        }

        public void Destroy()
        {
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.position = new Vector3(10000, 10000, 0);
        }
    }
}
