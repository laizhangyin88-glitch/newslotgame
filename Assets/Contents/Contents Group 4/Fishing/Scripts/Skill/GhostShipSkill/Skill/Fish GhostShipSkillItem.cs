using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishGhostShipSkillItem
    {
        enum GhostShipSkillState
        {
            Born = 1,
            Idle = 2,
            Play = 3,
            End = 4,
            Destroy = 5,
        }
        public SkillVo skillVo;
        public bool isCanDestroy;
        private float curTime;
        private float changeScoreCurTime;
        private float changeHitCurTime;
        private float changeHitTotalTime = 0.5f;
        private bool isPlaying;
        private bool isChangeScore;
        private bool isChangeMul;
        private Action callBack;
        private int skillScore;
        private int skillHit;
        private int hitTemp;
        private List<int> ghostShipHitList;
        private Transform ghostShipTrans;
        private Transform scoreAnimTrans;
        private Animator anchorAnim;
        private Animator enviorAnim;
        private Animator shipsAnim;
        private Animator bombAnim;
        private Animator hungeAnimator;
        private GhostShipSkillState curSkillState;
        private Text hitText;
        private int hitCount;

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            isCanDestroy = false;
            isPlaying = false;
        }

        public void ResetSkillState(int skillStatus, float skillTime, int skillScore, int skillMul, Action callBack = null)
        {
            isCanDestroy = false;
            hitCount = 0;
            changeScoreCurTime = 0;
            changeHitCurTime = 0;
            this.skillScore = skillScore;
            isChangeScore = false;
            isChangeMul = false;
            this.skillHit = skillMul;
            this.callBack = callBack;
            ghostShipHitList = new List<int>();
            SetCurGhostShipStep(skillStatus, skillTime);
            ShowGhostShipSkill();
            isPlaying = true;
        }

        private void SetCurGhostShipStep(int skillStatus, float skillTime)
        {
            if (skillStatus == 1)
                if (skillTime <= 0.1f)
                    curSkillState = GhostShipSkillState.Born;
                else
                    curSkillState = GhostShipSkillState.Idle;
            else if (skillStatus == 2)
                curSkillState = GhostShipSkillState.Play;
            curTime = skillTime * 0.001f;
        }

        public void ShowGhostShipSkill()
        {
            GetGhostShipSkill();
            if (curSkillState == GhostShipSkillState.Born)
                ShowGhostShipAppear();
            else if (curSkillState == GhostShipSkillState.Idle)
                ShowGhostShipIdel();
            else if (curSkillState == GhostShipSkillState.Play)
                GhostShipPlay();
        }

        private void GetGhostShipSkill()
        {
            GameObject ghostShipObj = FishGameObjectPoolManager.Instance.GetGameObject("Skill_GhostShip", PoolType.EffectPool);
            ghostShipTrans = ghostShipObj.transform;
            ghostShipTrans.localPosition = Vector3.zero;
            scoreAnimTrans = ghostShipTrans.Find("scoreAnimator");
            hitText = ghostShipTrans.Find("scoreAnimator/hitText").GetComponent<Text>();
        }

        public void ShowGhostShipAppear()
        {
            Animator apearAnim = ghostShipTrans.Find("apearAnimator").GetComponent<Animator>();
            apearAnim.gameObject.SetActive(true);
            apearAnim.Play("GhostShipAppear", 0, 0);   //during 0.29f
            AsyncActionUtils.DelayedAction(FishGhostShipSkillManager.Instance, 0.29f, () =>
            {
                apearAnim.gameObject.SetActive(false);
            });
            ShowGhostShipIdel();
        }

        public void ShowGhostShipIdel()
        {
            Debug.LogError("ShowGhostShipIdel");
            curSkillState = GhostShipSkillState.Idle;
            anchorAnim = ghostShipTrans.Find("anchorAnimator").GetComponent<Animator>();
            anchorAnim.gameObject.SetActive(true);
            anchorAnim.Play("GhostAnchor", 0, 0);
            AsyncActionUtils.ApplyScaling(FishGhostShipSkillManager.Instance, anchorAnim.transform, Vector3.zero, Vector3.one, 0.19f, TweenUtils.VectorTweenLinear, 0.1f);
        }

        public void GhostShipPlay()
        {
            Debug.LogError("GhostShipPlay");
            if (anchorAnim != null)
                anchorAnim.gameObject.SetActive(false);
            bombAnim = ghostShipTrans.Find("bombAnimator").GetComponent<Animator>();
            bombAnim.gameObject.SetActive(true);
            bombAnim.Play("GhostAnchorBomb", 0, 0);
            enviorAnim = ghostShipTrans.Find("enviorAnimator").GetComponent<Animator>();
            AsyncActionUtils.DelayedAction(FishGhostShipSkillManager.Instance, 1.1f, () =>
            {
                enviorAnim.gameObject.SetActive(true);
                enviorAnim.Play("GhostShipEnvior", 0, 0);
                shipsAnim = ghostShipTrans.Find("shipsAnimator").GetComponent<Animator>();
                shipsAnim.gameObject.SetActive(true);
                shipsAnim.Play("GhostShipSkill", 0, 0);
                curSkillState = GhostShipSkillState.Play;
            });
            AsyncActionUtils.DelayedAction(FishGhostShipSkillManager.Instance, 2.02f, () =>
            {
                bombAnim.gameObject.SetActive(false);
            });
        }

        public void RefreshGhostShip(int score, int hit)
        {
            hitCount++;
            isChangeMul = true;
            hitTemp = hit;
            if (ghostShipHitList == null)
                ghostShipHitList = new List<int>();
            ghostShipHitList.Add(hit);
            FishGameUIManager.Instance.SetShake(false);
            if (hitCount == 7)
                ShowHungeShipAmimate();
        }

        private void ChangeGhostShipHit()
        {
            if (!isChangeMul) return;
            scoreAnimTrans.gameObject.SetActive(true);
            if (ghostShipHitList != null && ghostShipHitList.Count > 0)
            {
                changeHitCurTime += Time.deltaTime;

                if (changeHitCurTime <= changeHitTotalTime)
                {
                    int hitResult = skillHit + (int)Math.Ceiling(hitTemp * (changeHitCurTime / changeHitTotalTime));
                    SetGhostShipHit(hitResult);
                }
                else
                {
                    ghostShipHitList.RemoveAt(0);
                    changeHitCurTime = 0;
                    skillHit += hitTemp;
                    SetGhostShipHit(skillHit);

                    if (ghostShipHitList.Count > 0)
                        hitTemp = ghostShipHitList[0];
                }
            }
        }

        public void SetGhostShipHit(int hit)
        {
            hitText.text = hit.ToString();
            AsyncActionUtils.ApplyScaling(FishGhostShipSkillManager.Instance, scoreAnimTrans, Vector3.one, Vector3.one * 1.5f, 0.01f, TweenUtils.VectorTweenLinear, 0, () =>
            {
                AsyncActionUtils.ApplyScaling(FishGhostShipSkillManager.Instance, scoreAnimTrans, Vector3.one * 1.5f, Vector3.one, 0.01f, TweenUtils.VectorTweenLinear);
            });
        }

        private void ShowHungeShipAmimate()
        {
            hungeAnimator = ghostShipTrans.Find("hungeAnimator").GetComponent<Animator>();
            hungeAnimator.gameObject.SetActive(true);
            hungeAnimator.Play("HungeMove", 0, 0);
            AsyncActionUtils.DelayedAction(FishGhostShipSkillManager.Instance, 2.05f, () =>
            {
                hungeAnimator.gameObject.SetActive(false);
            });
            AsyncActionUtils.DelayedAction(FishGhostShipSkillManager.Instance, 1.05f, () =>
            {
                bombAnim.gameObject.SetActive(true);
                bombAnim.Play("GhostAnchorBomb", 0, 0);
                AsyncActionUtils.DelayedAction(FishGhostShipSkillManager.Instance, 2.02f, () =>
                {
                    bombAnim.gameObject.SetActive(false);
                });
            });
        }

        public void Destroy()
        {
            isCanDestroy = false;
            isPlaying = false;
            callBack = null;
            if (ghostShipTrans != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(ghostShipTrans.gameObject, PoolType.EffectPool);
                ghostShipTrans = null;
            }
        }

        public void EndGhostShipSkill(int score, int mul)
        {
            curSkillState = GhostShipSkillState.End;
            curTime = 0;
        }

        public void Update()
        {
            if (isPlaying)
            {
                if (curSkillState == GhostShipSkillState.Play)
                {
                    curTime += Time.deltaTime;
                    ChangeGhostShipHit();
                    if (curTime >= 7.15f)
                        curTime = 0;
                }
                else if (curSkillState == GhostShipSkillState.End)
                {
                    curTime += Time.deltaTime;
                    if (curTime >= 3.07f)
                    {
                        curSkillState = GhostShipSkillState.Destroy;
                        curTime = 0;
                    }
                }
                else if (curSkillState == GhostShipSkillState.Destroy)
                {
                    Debug.LogError("Set can Destroy");
                    isPlaying = false;
                    isCanDestroy = true;
                    callBack?.Invoke();
                }
            }
        }
    }
}
