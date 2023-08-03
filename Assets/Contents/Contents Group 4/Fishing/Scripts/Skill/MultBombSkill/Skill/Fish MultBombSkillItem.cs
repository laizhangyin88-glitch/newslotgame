using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishMultBombSkillItem
    {
        enum MultBombSkillStats
        {
            Move = 1,
            Idle = 2,
        }
        MultBombSkillStats curSkillStats;

        string Skill_MultBomb_Res = "Skill_MultiBomb";
        string[] AnimParams = { "MultBombDrab_Catch", "MultBombDrab_Catching" };

        public bool isCanDestory = false;
        bool isPlayingAnim = false;

        Vector3 beginPos;
        Vector3 targetPos;

        float currentTime = 0;
        float MoveTotalTime = 1f;
        float IdleTotalTime = 2f;

        int score = 0;
        int multiple = 0;

        Transform SkillGunParent;
        Transform SkillPanelParent;

        GameObject Skill_MultBomb;

        Animator Skill_MultBomb_Anim;

        Text Skill_MultBomb_Text;

        public SkillVo SkillVo;

        Action callBack;

        public void ResetSkillVo(SkillVo vo)
        {
            SkillVo = vo;
            isCanDestory = false;
            isPlayingAnim = false;
        }

        public void ResetSkillState(Vector3 beginPos, int skillStatus, float skillTime, Action callBack = null)
        {
            isCanDestory = false;
            SkillGunParent = SkillVo.PlayerIns.Panel.SkillGun;
            SkillPanelParent = SkillVo.PlayerIns.Panel.SKillPanel;
            this.beginPos = beginPos;
            targetPos = SkillGunParent.position;
            this.callBack = callBack;

            score = 0;
            multiple = 0;

            GetCurrentMultBombStep(skillTime);
            ShowMultBombSkill();
            isPlayingAnim = true;
        }

        public void GetCurrentMultBombStep(float skillTime)
        {
            if (skillTime <= 0.1f)
            {
                curSkillStats = MultBombSkillStats.Move;
            }
            else
            {
                curSkillStats = MultBombSkillStats.Idle;
            }
            currentTime = skillTime * 0.001f;
        }

        public void ShowMultBombSkill()
        {
            if (curSkillStats == MultBombSkillStats.Move)
            {
                GetMultBombResItem(beginPos);
                MultBombMove();
            }
            else if (curSkillStats == MultBombSkillStats.Idle)
            {
                GetMultBombResItem(SkillVo.NextPos);
            }
            SetMultBomNumber(SkillVo.BombCount);
        }

        public void GetMultBombResItem(Vector3 pos)
        {
            Skill_MultBomb = FishGameObjectPoolManager.Instance.GetGameObject(Skill_MultBomb_Res, PoolType.EffectPool);
            Transform trans = Skill_MultBomb.transform;
            trans.position = pos;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;
            Skill_MultBomb_Text = trans.Find("Content/Icon/Text").GetComponent<Text>();
            Skill_MultBomb_Anim = trans.GetComponent<Animator>();
            FishAudioManager.Instance.PlayNormalAudio(47, 1, true);
        }

        public void SetMultBomNumber(int number)
        {
            Skill_MultBomb_Text.text = number.ToString();
        }

        public void MultBombMove()
        {
            Transform trans = Skill_MultBomb.transform;
            AsyncActionUtils.ApplyMovement(FishMultBombSkillManager.Instance, trans, trans.position, SkillVo.NextPos, 1, TweenUtils.VectorTweenLinear, 0, MultBombIdle);
            PlayMultBombAnim(0);
        }

        public void MultBombIdle()
        {
            curSkillStats = MultBombSkillStats.Idle;
            PlayMultBombAnim(1);
            FishAudioManager.Instance.StopNormalAudio(47);
            FishAudioManager.Instance.PlayNormalAudio(47, 1, true);
        }

        public void MultBombExplose()
        {
            SetMultBomNumber(SkillVo.BombCount);
            ShowMultBombExploseEffect();
            FishGameUIManager.Instance.SetShake(false);
            FishAudioManager.Instance.StopNormalAudio(47);
            FishAudioManager.Instance.PlayNormalAudio(48);
            curSkillStats = MultBombSkillStats.Move;
            if (SkillVo.haveNextPos)
            {
                MultBombMove();
            }
            else
            {
                Skill_MultBomb.transform.localPosition = new Vector3(10000, 10000, 0);
            }
        }

        public void DelayMultBombDestroy(int score, int mul)
        {
            this.score = score;
            this.multiple = mul;
            AsyncActionUtils.DelayedAction(FishMultBombSkillManager.Instance, 1.5f, MultBombDestroy);
        }

        public void MultBombDestroy()
        {
            isCanDestory = true;
            FishMultBombSkillManager.Instance.ShowSpecialDeclareScore(SkillVo.UID, score, multiple);
        }

        public void ShowMultBombExploseEffect()
        {
            Vector3 beginPos = Skill_MultBomb.transform.position;
            string name = "Effect_Bomb";
            int type = 1;
            float delayTime = 0;
            float lifeTime = 1.5f;
            string effectAudio = null;
            FishFishEffectManager.Instance.ShowFishEffect(beginPos, type, name, delayTime, lifeTime, effectAudio,() => {
                //Debug.LogError("Dispatch multbomb => " + SkillVo.killFishUID);
                MessageDispatcher.Dispatch("FishBombEnd", new EventData<int>("MainFishUID", SkillVo.killFishUID));
            });
        }

        public void PlayMultBombAnim(int index)
        {
            string animationName = AnimParams[index];
            if (animationName != null)
            {
                Skill_MultBomb_Anim.Play(animationName, 0, 0);
            }
        }

        public void Destroy()
        {
            FishAudioManager.Instance.StopNormalAudio(47);
            if (Skill_MultBomb != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_MultBomb, PoolType.EffectPool);
            }
            Skill_MultBomb = null;

            isCanDestory = false;
            isPlayingAnim = false;
            callBack = null;
        }
    }
}
