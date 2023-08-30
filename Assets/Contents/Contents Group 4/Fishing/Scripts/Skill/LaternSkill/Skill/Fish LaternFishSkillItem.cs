using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishLaternFishSkillItem
    {
        enum LaternFishSkillState
        {
            Born = 1,
            Idel = 2,
            Play = 3,
        }
        LaternFishSkillState curSkillState;

        Vector3 beginPos;
        Action callBack;
        int score;
        int multiple;
        GameObject laternSkillObj;
        Transform laternSkillTrans;
        Text playTimes;
        Animator bgAnimator;
        Animator starAnimator;
        Animator bombAnimator;

        public bool isCanDestroy = false;
        public SkillVo skillVo;

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            isCanDestroy = false;
        }

        public void ResetSkillState(Vector3 beginPos, int skillStatus, float skillTime, Action callBack = null)
        {
            isCanDestroy = false;
            this.callBack = callBack;
            this.beginPos = beginPos;
            score = 0;
            multiple = 0;

            GetCurrentLaternSkillStep(skillTime);
            ShowLaternSkill();
        }

        private void GetCurrentLaternSkillStep(float skillTime)
        {
            if (skillTime <= 0.1f)
                curSkillState = LaternFishSkillState.Born;
            else
                curSkillState = LaternFishSkillState.Play;
        }

        public void ShowLaternSkill()
        {
            if (curSkillState == LaternFishSkillState.Born)
            {
                GetLaternSkillResouceItem();
                ShowBornAnimate();
            }
            else
                ShowBombAnimate();
            SetPlayTimesNumber(skillVo.BombCount);
        }

        private void GetLaternSkillResouceItem()
        {
            laternSkillObj = FishGameObjectPoolManager.Instance.GetGameObject("Skill_LaternFish", PoolType.EffectPool);
            laternSkillTrans = laternSkillObj.transform;
            laternSkillTrans.position = beginPos;
            laternSkillTrans.localRotation = Quaternion.identity;
            laternSkillTrans.localScale = Vector3.zero;
            playTimes = laternSkillTrans.Find("num").GetComponent<Text>();
            bgAnimator = laternSkillTrans.Find("BgAnimator").GetComponent<Animator>();
            starAnimator = laternSkillTrans.Find("StarAnimator").GetComponent<Animator>();
            bombAnimator = laternSkillTrans.Find("BombAnimator").GetComponent<Animator>();
        }

        private void ShowBornAnimate()
        {
            float tweenDuring = 1f;
            float delayTime = 0.8f;
            starAnimator.gameObject.SetActive(true);
            starAnimator.Play("LaternStart", 0, 0);
            playTimes.gameObject.SetActive(true);
            var lightObj = FishGameObjectPoolManager.Instance.GetGameObject("DeclareLight", PoolType.SpecialDeclarePool);
            lightObj.transform.position = beginPos;
            lightObj.GetComponent<Animator>().Play("DeclareLight", 0, 0);
            AsyncActionUtils.DelayedAction(FishLaternSkillManager.Instance, delayTime, () =>
            {
                AsyncActionUtils.ApplyScaling(FishLaternSkillManager.Instance, laternSkillTrans, Vector3.zero, Vector3.one, tweenDuring, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.ApplyLocalMovement(FishLaternSkillManager.Instance, laternSkillTrans, laternSkillTrans.localPosition, Vector3.zero, tweenDuring, TweenUtils.VectorTweenLinear, tweenDuring);
                AsyncActionUtils.ApplyScaling(FishLaternSkillManager.Instance, playTimes.transform, Vector3.zero, Vector3.one, tweenDuring, TweenUtils.VectorTweenLinear, tweenDuring + 1, () =>
                {
                    bgAnimator.gameObject.SetActive(true);
                    bgAnimator.Play("LaternBg", 0, 0);
                    curSkillState = LaternFishSkillState.Idel;
                });
            });
            AsyncActionUtils.DelayedAction(FishLaternSkillManager.Instance, 1.27f, () =>
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(lightObj, PoolType.SpecialDeclarePool);
            });
        }

        public void OnBombMsg()
        {
            SetPlayTimesNumber(skillVo.BombCount);
            curSkillState = LaternFishSkillState.Play;
            ShowBombAnimate();
        }

        private void ShowBombAnimate()
        {
            bombAnimator.gameObject.SetActive(true);
            bombAnimator.Play("LaternBomb", 0, 0);
            MessageDispatcher.Dispatch("FishBombEnd", new EventData<int>("MainFishUID", skillVo.killFishUID));
        }

        private void SetPlayTimesNumber(int num)
        {
            playTimes.text = num.ToString();
            AsyncActionUtils.ApplyScaling(FishLaternSkillManager.Instance, playTimes.transform, Vector3.one, Vector3.one * 1.5f, 0.75f, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.ApplyScaling(FishLaternSkillManager.Instance, playTimes.transform, Vector3.one * 1.5f, Vector3.one, 0.75f, TweenUtils.VectorTweenLinear, 0.75f);
        }

        public void DelayLaternFishSkillDestroy()
        {
            isCanDestroy = true;
        }

        public void Destroy()
        {
            if (laternSkillObj != null)
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(laternSkillObj, PoolType.EffectPool);
            laternSkillObj = null;

            isCanDestroy = false;
            callBack = null;
        }
    }
}
