using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishThunderHammerSkillItem
    {
        enum ThunderHammerSkillState
        {
            Create = 1,
            Wait = 2,
            Bomb = 3,
        }
        ThunderHammerSkillState curSkillState = ThunderHammerSkillState.Create;

        string resName = "ThunderHammer";
        float createTotalTime = 0.46f;
        float waitTotalTime = 2.52f;
        float showCoinTime = 4.65f;
        float bombTotalTime = 5f;
        float curTime = 0;
        int score = 0;
        int mul = 0;
        string[] animNames = { "Effect_ThunderHammerCreate", "Effect_ThunderHammerWait", "Effect_ThunderHammerBomb"};

        public bool isCanDestroy = false;
        public bool isPlayingAnim = false;
        public bool hasShowCoin = false;

        public SkillVo skillVo;
        GameObject gameObject;
        Animator animator;
        Action callBack;

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            score = 0;
            mul = 0;
            isCanDestroy = false;
        }

        public void ResetSkillState(float skillTime,Action callBack = null)
        {
            isCanDestroy = false;
            this.callBack = callBack;
            GetCurrentSkillStep(skillTime);
            ShowSkill();
            isPlayingAnim = true;
            hasShowCoin = false;
        }

        public void GetCurrentSkillStep(float skillTime)
        {
            curSkillState = skillTime < createTotalTime ? ThunderHammerSkillState.Create : ThunderHammerSkillState.Wait;
            curTime = skillTime * 0.001f;
        }

        public void ShowSkill()
        {
            if (curSkillState == ThunderHammerSkillState.Create)
            {
                GetSkillItem();
                PlaySkillAnim(0);
            }
            else if (curSkillState == ThunderHammerSkillState.Wait)
            {
                if (gameObject == null)
                    GetSkillItem();
                SkillWaiting();
            }
        }

        public void SkillWaiting()
        {
            gameObject.transform.localPosition = Vector3.zero;
            curSkillState = ThunderHammerSkillState.Wait;
            PlaySkillAnim(1);
        }

        public void GetSkillItem()
        {
            gameObject = FishGameObjectPoolManager.Instance.GetGameObject(resName, PoolType.EffectPool);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            animator = gameObject.GetComponent<Animator>();
        }

        public void PlayBombAnim(int score, int mul)
        {
            this.score = score;
            this.mul = mul;
            curSkillState = ThunderHammerSkillState.Bomb;
            PlaySkillAnim(2);
        }

        private void PlaySkillAnim(int index)
        {
            animator.Play(animNames[index], 0, 0);
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (curSkillState == ThunderHammerSkillState.Create)
                {
                    curTime += Time.deltaTime;
                    if (curTime >= createTotalTime)
                    {
                        curTime = 0;
                        SkillWaiting();
                    }
                }
                else if (curSkillState == ThunderHammerSkillState.Wait)
                {
                    curTime += Time.deltaTime;
                    if (curTime >= waitTotalTime)
                    {
                        curTime = 0; 
                    }
                }
                else if (curSkillState == ThunderHammerSkillState.Bomb)
                {
                    curTime += Time.deltaTime;
                    if (curTime >= showCoinTime && !hasShowCoin)
                    {
                        MessageDispatcher.Dispatch("FishBombEnd", new EventData<int>("MainFishUID", skillVo.killFishUID));
                        hasShowCoin = true;
                    }
                    if (curTime >= bombTotalTime)
                    {
                        curTime = 0;
                        isPlayingAnim = false;
                        isCanDestroy = true;
                        FishThunderHammerSkillManager.Instance.ShowSpecialDeclareScore(skillVo.UID, score, mul);
                        callBack?.Invoke();
                    }
                }
            }
        }

        public void Destroy()
        {
            FishGameObjectPoolManager.Instance.ReCycleToGameObject(gameObject, PoolType.EffectPool);
            isCanDestroy = false;
            isPlayingAnim = false;
            callBack = null;
        }
    }
}
