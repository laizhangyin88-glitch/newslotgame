using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishPlusTipsEffectItem
    {
        public GameObject gameObject;
        public string animName;
        public bool isCanDestroy;
        public bool isPlayingAnim;
        public bool isDelayPlay;
        public bool isCanMove;
        public float delayTime;
        public float currentTime;
        public float totalTime;
        public Vector3 beginPos;
        public Animator anim;
        public Text showScoreText;
        public EffectVo effectVo;
        public Action callBack;

        public FishPlusTipsEffectItem(GameObject obj)
        {
            gameObject = obj;
            Init();
        }

        private void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
            animName = "PlusTips";
            totalTime = 1;
            beginPos = Vector3.zero;
        }

        private void InitView()
        {
            FindView();
        }

        private void FindView()
        {
            anim = gameObject.GetComponent<Animator>();
            showScoreText = gameObject.transform.Find("Content/Text").GetComponent<Text>();
        }

        public void SetShowScoreText(string score)
        {
            showScoreText.text = "+" + score;
        }

        public void PlayAnim()
        {
            gameObject.transform.position = beginPos;
            anim.Play(animName, 0, 0);
        }

        public void ResetEffectVo(EffectVo vo)
        {
            effectVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayPlay = false;
        }

        public void ResetState(Vector3 beginPos, float delayTime, float showTime, Action callBack = null)
        {
            isCanMove = false;
            isCanDestroy = false;
            gameObject.SetActive(true);
            this.beginPos = beginPos;
            gameObject.transform.position = new Vector3(10000, 10000, 0);
            this.delayTime = delayTime;
            this.callBack = callBack;
            totalTime = showTime;
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

        public void EndCallBack()
        {
            callBack?.Invoke();
        }

        public void Destroy()
        {
            gameObject.SetActive(false);
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.position = new Vector3(10000, 10000, 0);
        }
    }
}

