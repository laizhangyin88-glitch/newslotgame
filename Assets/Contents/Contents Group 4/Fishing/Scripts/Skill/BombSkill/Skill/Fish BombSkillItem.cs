using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishBombSkillItem
    {
        enum BombSkillStats
        {
            Born = 1,
            Prepare = 2,
            Bomb = 3,
        }
        BombSkillStats curSkillStats = BombSkillStats.Born;

        FishGameData gameData;
        public SkillVo SkillVo;

        string Skill_Bomb_Res = "Skill_Bomb";
        string[] AnimParams = { "Skill_Bomb_Prepare", "Skill_Bomb_Explosion" };

        public bool isCanDestory = false;
        bool isPlayingAnim = false;
        bool hasShowCoin = false;

        Vector3 beginPos = new Vector3(10000, 10000, 0);
        Vector3 targetPos;

        float currentTime = 0;
        float ExplosionTotalTime = 1.5f;
        float BornTotalTime = 1.5f;
        float PrepareTotalTime = 2f;
        float showCoinTime = 0.75f;

        int score = 0;
        int multiple = 0;
        int skillStatus = 0;

        GameObject Skill_Bomb;

        Animator Skill_Bomb_Anim;

        Transform SkillGunParent;
        Transform SkillPanelParent;

        Action callBack;

        public void ResetSkillVo(SkillVo vo)
        {
            SkillVo = vo;
            isCanDestory = false;
            isPlayingAnim = false;
            score = 0;
            multiple = 0;
        }

        public void ResetSkillState(Vector3 beginPos, int skillStatus, float skillTime, Action callBack = null)
        {
            isCanDestory = false;
            SkillGunParent = SkillVo.PlayerIns.Panel.SkillGun;
            SkillPanelParent = SkillVo.PlayerIns.Panel.SKillPanel;
            this.beginPos = beginPos;
            targetPos = SkillGunParent.position;
            this.callBack = callBack;

            GetCurrentBombStep(skillStatus, skillTime);
            ShowBombSkill(skillTime);
            isPlayingAnim = true;
            hasShowCoin = false;
        }

        public void GetCurrentBombStep(int skillStatus, float skillTime)
        {
            this.skillStatus = skillStatus;
            if (skillTime < BornTotalTime)
            {
                curSkillStats = BombSkillStats.Born;
            }
            else
            {
                curSkillStats = BombSkillStats.Prepare;
            }
            currentTime = skillTime * 0.001f;
        }

        public void ShowBombSkill(float timeElapsed)
        {
            if (curSkillStats == BombSkillStats.Born)
            {
                GetBombResItem();
            }
            else if (curSkillStats == BombSkillStats.Prepare)
            {
                if (Skill_Bomb == null)
                    GetBombResItem();
                BombCrabPrepare(currentTime);
            }
        }

        public void BombCrabPrepare(float timeElapsed)
        {
            Skill_Bomb.transform.position = beginPos;
            curSkillStats = BombSkillStats.Prepare;
            PlayBombSkillAnim(0);
            FishAudioManager.Instance.StopNormalAudio(47);
            FishAudioManager.Instance.PlayNormalAudio(47, 1, true);
        }

        public void GetBombResItem()
        {
            Skill_Bomb = FishGameObjectPoolManager.Instance.GetGameObject(Skill_Bomb_Res, PoolType.EffectPool);
            Transform trans = Skill_Bomb.transform;
            trans.SetParent(SkillPanelParent);
            trans.position = beginPos;
            trans.localScale = Vector3.one;
            Skill_Bomb_Anim = trans.Find("Content").GetComponent<Animator>();
        }

        public void BombCrabExplosion(int score, int mul)
        {
            this.score = score;
            this.multiple = mul;
            this.currentTime = 0;
            this.curSkillStats = BombSkillStats.Bomb;
            PlayBombSkillAnim(1);
            FishGameUIManager.Instance.SetShake(false);
            FishAudioManager.Instance.StopNormalAudio(47);
            FishAudioManager.Instance.PlayNormalAudio(48);
        }

        public void PlayBombSkillAnim(int index)
        {
            string animationName = AnimParams[index];
            if (animationName != null)
            {
                Skill_Bomb_Anim.Play(animationName, 0, 0);
            }
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (curSkillStats == BombSkillStats.Born)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= BornTotalTime)
                    {
                        currentTime = 0;
                        BombCrabPrepare(0);
                    }
                }
                else if (curSkillStats == BombSkillStats.Prepare)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= PrepareTotalTime)
                    {
                        FishAudioManager.Instance.StopNormalAudio(47);
                        currentTime = 0;
                    }
                }
                else if (curSkillStats == BombSkillStats.Bomb)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= showCoinTime && !hasShowCoin)
                    {
                        MessageDispatcher.Dispatch("FishBombEnd", new EventData<int>("MainFishUID", SkillVo.killFishUID));
                        hasShowCoin = true;
                    }
                    if (currentTime >= ExplosionTotalTime)
                    {
                        currentTime = 0;
                        isPlayingAnim = false;
                        isCanDestory = true;
                        FishBombSkillManager.Instance.ShowSpecialDeclareScore(SkillVo.UID, score, multiple);
                        callBack?.Invoke();
                    }
                }
            }
        }

        public void Destroy()
        {
            FishAudioManager.Instance.StopNormalAudio(47);
            FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_Bomb, PoolType.EffectPool);
            isCanDestory = false;
            isPlayingAnim = false;
            callBack = null;
        }
    }
}
