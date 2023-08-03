using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishFishOutTipsEffecItem
    {
        public GameObject gameObject;
        public FishGameUIManager gameUIManager;
        public FishGameData gameData;
        public bool isCanDestroy;
        public bool isPlayingAnim;
        public bool isDelayTime;
        public float delayTime;
        public float currentTime;
        public float totalTime;
        public Animator animator;
        public FishOutTipsEffectVo EffectVo;
        public bool isCanMove;
        public Action callBack;
        public FishFishOutTipsEffecItem(GameObject obj)
        {
            gameObject = obj;
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayTime = false;
            delayTime = 0;
            currentTime = 0;
            totalTime = 0;
            FindView();
        }

        public void FindView()
        {
            Transform tf = gameObject.transform;
            animator = tf.Find("Fish_Tips").GetComponent<Animator>();
        }

        public void PlayAnim()
        {
            animator.Play(EffectVo.animationName, 0, 0);
        }

        public void ResetEffectVo(FishOutTipsEffectVo vo)
        {
            EffectVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayTime = false;
        }

        public void ResetState(Action callBack)
        {
            isCanMove = false;
            isCanDestroy = false;
            delayTime = 0;
            this.callBack = callBack;
            totalTime = EffectVo.LifeTime;
            currentTime = 0;
            isDelayTime = true;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.SetActive(true);
        }

        public void Update()
        {
            if (isDelayTime)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= delayTime)
                {
                    isDelayTime = false;
                    currentTime = 0f;
                    PlayAnim();
                    isPlayingAnim = true;
                }
            }

            if (isPlayingAnim)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= totalTime)
                {
                    currentTime = 0f;
                    isPlayingAnim = false;
                    EndCallBack();
                    isCanDestroy = true;
                }
            }
        }

        public void EndCallBack()
        {
            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
