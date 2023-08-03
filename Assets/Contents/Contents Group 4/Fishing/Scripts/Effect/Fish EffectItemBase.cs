using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagelCode
{
    public abstract class FishEffectItemBase
    {
        public bool isCanDestroy;
        public bool isPlayingAnim;
        public bool isDelayPlay;
        public bool isCanMove;
        public float delayTime;
        public float currentTime;
        public float totalTime;
        public Vector3 beginPos;
        public GameObject gameObject;
        public FishGameUIManager gameUIManager;
        public FishGameData gameData;
        public FishEffectVo EffectVo;
        public Action callBack;

        public FishEffectItemBase(GameObject obj)
        {
            gameObject = obj;
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayPlay = false;
            delayTime = 0;
            currentTime = 0;
            totalTime = 1;
            beginPos = Vector3.zero;
            FindView();
        }

        public abstract void FindView();

        public void ResetEffectVo(FishEffectVo vo)
        {
            EffectVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayPlay = false;
        }

        public void ResetState(Vector3 beginPos, Action callBack)
        {
            isCanMove = false;
            isCanDestroy = false;
            this.beginPos = beginPos;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
            delayTime = EffectVo.EffectDelayTime;
            totalTime = EffectVo.EffectLifeTime;
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

        public abstract void PlayAnim();


        public void EndCallBack()
        {
            callBack?.Invoke();
        }

        public void Destroy()
        {
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.localPosition = new Vector3 (10000, 10000, 0);
        }
    }
}

