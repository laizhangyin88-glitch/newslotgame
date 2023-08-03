using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishBisonSkillItem
    {
        enum BisonSkillStats
        {
            Born = 1,
            Idle = 2,
            Shoot = 3,
            End = 4,
            Destroy = 5,
        }

        string Skill_Bison_Res = "Skill_Bison";
        string BisonCutIn_Res = "Bison_CutIn";
        string Bison_SpecialDeclare_Res = "SpecialDeclare_Bison";
        string OtherBison_SpecialDeclare_Res = "SpecialDeclare_BisonsOther";
        string[] BisonLogo_AnimParams = { "Bison_Start", "Bison_Showing", "Bison_into", "Bison_loop", "Bison_end", "Bison_OtherEnding" };

        bool IsChangeScore = false;
        bool IsChangeMul = false;
        public bool isCanDestory = false;
        bool isPlayingAnim = false;

        Vector3 beginPos;
        Vector3 targetPos;

        BisonSkillStats curSkillStats;

        float currentTime = 0;
        float ShootTotalTime = 8.5f;
        float IdleTotalTime = 3f;
        float EndTotalTime = 4f;
        float DestroyTotalTime = 0.8f;
        float changeScoreCurrentTime = 0;
        float changeScoreTotalTime = 0;
        float BisonsCutInTime = 2.2f;
        float changeMulCurrentTime = 0;
        float changeMulTotalTime = 0.5f;

        int changeScoreTempScore = 0;
        int changScoreInitScore = 0;
        int changeMultipleTempMul = 0;
        int changMultipleInitMul = 0;

        Transform SkillGunParent;
        Transform SkillPanelParent;

        Text Bison_MulText;
        Text Bison_ScoreText;
        Text OtherBison_MulText;
        Text OtherBison_ScoreText;

        GameObject Skill_Bison;
        GameObject SpecialDeclare_Bison;
        GameObject BisonCutIn;
        GameObject OtherSpecialDeclare_Bison;

        Animator MyBison_SpecialDeclare_Animator;
        Animator Bison_Board_Mul_Animator;
        Animator OtherBison_SpecialDeclare_Animator;

        public SkillVo SkillVo;
        List<int> bisonMultipleTable;
        Action callBack;

        public void ResetSkillVo(SkillVo vo)
        {
            SkillVo = vo;
            isCanDestory = false;
            isPlayingAnim = false;
        }

        public void ResetSkillState(int skillStatus, float skillTime, int skillScore, int skillMul, Action callBack = null)
        {
            isCanDestory = false;
            SkillGunParent = SkillVo.PlayerIns.Panel.SkillGun;
            SkillPanelParent = SkillVo.PlayerIns.Panel.SKillPanel;

            beginPos = Vector3.zero;

            targetPos = SkillGunParent.position;

            changeScoreCurrentTime = 0;
            changeScoreTempScore = 0;
            changScoreInitScore = skillScore;
            IsChangeScore = false;

            changeMulCurrentTime = 0;
            changeMultipleTempMul = 0;
            changMultipleInitMul = skillMul;
            IsChangeMul = false;

            bisonMultipleTable = new List<int>();

            this.callBack = callBack;
            GetCurrentBisonStep(skillStatus, skillTime);
            ShowBisonSkill();
            isPlayingAnim = true;
        }

        public void GetCurrentBisonStep(int skillStatus, float skillTime)
        {
            if (skillStatus == 1)
            {
                if (skillTime <= 0.1f)
                {
                    curSkillStats = BisonSkillStats.Born;
                }
                else
                {
                    curSkillStats = BisonSkillStats.Idle;
                }
            }
            else if (skillStatus == 2)
            {
                curSkillStats = BisonSkillStats.Shoot;
            }
            currentTime = skillTime * 0.001f;
        }

        public void ShowBisonSkill()
        {
            if (curSkillStats == BisonSkillStats.Born)
            {
                GetBisonCutInRes();
            }
            else if (curSkillStats == BisonSkillStats.Idle)
            {
                ShowBisonsSpecialDeclare();
                PlayBisonSpecialDeclareAnimation(1);
                SetBisonMul(changMultipleInitMul);
            }
            else if (curSkillStats == BisonSkillStats.Shoot)
            {
                ShowBisonsSpecialDeclare();
                PlayBisonSpecialDeclareAnimation(1);
                SetBisonMul(changMultipleInitMul);
                BeginBisonShoot(currentTime);
            }
        }

        public void GetBisonCutInRes()
        {
            BisonCutIn = FishGameObjectPoolManager.Instance.GetGameObject(BisonCutIn_Res, PoolType.EffectPool);
            var trans = BisonCutIn.transform;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;

            AsyncActionUtils.DelayedAction(FishBisonSkillManager.Instance, BisonsCutInTime - 0.3f, () =>
            {
                ShowBisonsSpecialDeclare();
                PlayBisonSpecialDeclareAnimation(0);
            });
            AsyncActionUtils.DelayedAction(FishBisonSkillManager.Instance, BisonsCutInTime, () =>
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(BisonCutIn, PoolType.EffectPool);
                BisonCutIn = null;
            });
        }

        public void ShowBisonsSpecialDeclare()
        {
            SpecialDeclare_Bison = FishGameObjectPoolManager.Instance.GetGameObject(Bison_SpecialDeclare_Res, PoolType.SpecialDeclarePool);
            var trans = SpecialDeclare_Bison.transform;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;

            Bison_MulText = trans.Find("Root/Logo/TextRoot/Text").GetComponent<Text>();
            Bison_ScoreText = trans.Find("Root/RewardROOT/Reward").GetComponent<Text>();
            MyBison_SpecialDeclare_Animator = trans.Find("Root").GetComponent<Animator>();
            Bison_Board_Mul_Animator = trans.Find("Root/Logo/TextRoot").GetComponent<Animator>();
            SetBisonMul(changMultipleInitMul);
        }

        public void PlayBisonSpecialDeclareAnimation(int index)
        {
            string animationName = BisonLogo_AnimParams[index];
            if (!string.IsNullOrEmpty(animationName))
            {
                MyBison_SpecialDeclare_Animator.Play(animationName, 0, 0);
            }
        }

        public void PlayOtherBisonSpecialDeclareAnimation(string animationName)
        {
            OtherBison_SpecialDeclare_Animator.Play(animationName, 0, 0);
        }

        public void GetBisonSkillRes()
        {
            Skill_Bison = FishGameObjectPoolManager.Instance.GetGameObject(Skill_Bison_Res, PoolType.EffectPool);

            Transform trans = Skill_Bison.transform;
            Animator animator = trans.Find("Animator").GetComponent<Animator>();
            animator.Play("BisonSkill01_Move", 0, (currentTime / ShootTotalTime));
            trans.localPosition = Vector3.zero;

            if (SkillVo.Direction == 1)
            {
                trans.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (SkillVo.Direction == 2)
            {
                trans.localRotation = Quaternion.Euler(0, 180, 0);
            }
            trans.localScale = Vector3.one;
        }

        public void BeginBisonShoot(float timeElapse)
        {
            currentTime = timeElapse;
            GetBisonSkillRes();
            curSkillStats = BisonSkillStats.Shoot;
            FishAudioManager.Instance.PlayNormalAudio(60, 1, true);
        }

        public void SetBisonMul(int multiple)
        {
            Bison_MulText.text = "x" + multiple.ToString();
        }

        public void SetBisonScore(int score)
        {
            Bison_ScoreText.text = score.ToString();
        }

        public void RefreshBisonInfo(int score, int mul)
        {
            IsChangeMul = true;
            changeMultipleTempMul = mul;

            if (bisonMultipleTable == null)
            {
                bisonMultipleTable = new List<int>();
            }
            bisonMultipleTable.Add(mul);
            Bison_Board_Mul_Animator.Play("Bison_Board_Mul", 0, 0);
            FishGameUIManager.Instance.SetShake(false);
        }

        public void ChangeBisonMul()
        {
            if (!IsChangeMul)
                return;

            if (bisonMultipleTable != null && bisonMultipleTable.Count > 0)
            {
                changeMulCurrentTime += Time.deltaTime;

                if (changeMulCurrentTime <= changeMulTotalTime)
                {
                    int multipleResult = changMultipleInitMul + (int)Math.Ceiling(changeMultipleTempMul * (changeMulCurrentTime / changeMulTotalTime));
                    SetBisonMul(multipleResult);
                }
                else
                {
                    bisonMultipleTable.RemoveAt(0);
                    changeMulCurrentTime = 0;
                    changMultipleInitMul += changeMultipleTempMul;
                    SetBisonMul(changMultipleInitMul);
                    Bison_Board_Mul_Animator.Play("Bison_Board_Mul", 0, 0);

                    if (bisonMultipleTable.Count > 0)
                        changeMultipleTempMul = bisonMultipleTable[0];
                }
            }
            else
            {
                IsChangeMul = false;
            }
        }

        public void ChangeBisonScore()
        {
            if (SkillVo.IsMe)
                return;

            if (!IsChangeScore)
                return;

            changeScoreCurrentTime += Time.deltaTime;
            if (changeScoreCurrentTime <= changeScoreTotalTime)
            {
                int scoreResult = changScoreInitScore + (int)Math.Ceiling(changeScoreTempScore * (changeScoreCurrentTime / changeScoreTotalTime));
                SetBisonScore(scoreResult);
            }
            else
            {
                changeScoreCurrentTime = 0;
                changScoreInitScore = changeScoreTempScore;
                SetBisonScore(changScoreInitScore);
                IsChangeScore = false;
            }
        }

        public void EndBisonSkill(int score, int mul)
        {
            curSkillStats = BisonSkillStats.End;
            currentTime = 0;

            if (Skill_Bison != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_Bison, PoolType.EffectPool);
                Skill_Bison = null;
            }
            FishAudioManager.Instance.StopNormalAudio(60);
            if (SkillVo.IsMe)
            {
                IsChangeScore = true;
                changeScoreTempScore = score;
                changScoreInitScore = 0;
                changeScoreCurrentTime = 0;
                SetBisonMul(mul);
                SetBisonScore(score);
                PlayBisonSpecialDeclareAnimation(2);
                FishAudioManager.Instance.PlayNormalAudio(59);
            }
            else
            {
                PlayBisonSpecialDeclareAnimation(5);
                OtherSpecialDeclare_Bison = FishGameObjectPoolManager.Instance.GetGameObject(OtherBison_SpecialDeclare_Res, PoolType.SpecialDeclarePool);
                Transform trans = OtherSpecialDeclare_Bison.transform;
                trans.position = SkillVo.PlayerIns.SwrilPanel.position;
                trans.localRotation = Quaternion.Euler(0, 0, 0);
                trans.localScale = Vector3.one;

                OtherBison_MulText = trans.Find("Root/Logo/TextRoot/Text").GetComponent<Text>();
                OtherBison_ScoreText = trans.Find("Root/RewardROOT/Reward").GetComponent<Text>();
                OtherBison_SpecialDeclare_Animator = trans.Find("Root").GetComponent<Animator>();
                OtherBison_MulText.text = mul.ToString();
                OtherBison_ScoreText.text = score.ToString();
                PlayOtherBisonSpecialDeclareAnimation("Bison_OtherInto");
            }
        }

        public void DestroyBison()
        {
            isPlayingAnim = false;
            isCanDestory = true;
            if (callBack != null)
            {
                callBack();
            }
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (curSkillStats == BisonSkillStats.Born || curSkillStats == BisonSkillStats.Idle)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= IdleTotalTime)
                    {
                        currentTime = 0;
                    }
                }
                else if (curSkillStats == BisonSkillStats.Shoot)
                {
                    currentTime += Time.deltaTime;
                    ChangeBisonMul();
                    if (currentTime >= ShootTotalTime)
                    {
                        currentTime = 0;
                        FishAudioManager.Instance.StopNormalAudio(60);
                    }
                }
                else if (curSkillStats == BisonSkillStats.End)
                {
                    currentTime += Time.deltaTime;
                    ChangeBisonScore();
                    if (currentTime >= EndTotalTime)
                    {
                        curSkillStats = BisonSkillStats.Destroy;
                        currentTime = 0;
                        if (SkillVo.IsMe)
                        {
                            PlayBisonSpecialDeclareAnimation(4);
                        }
                        else
                        {
                            PlayOtherBisonSpecialDeclareAnimation("Bison_end");
                        }
                    }
                }
                else if (curSkillStats == BisonSkillStats.Destroy)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= DestroyTotalTime)
                    {
                        currentTime = 0;
                        DestroyBison();
                    }
                }
            }
        }

        public void Destroy()
        {
            FishAudioManager.Instance.StopNormalAudio(60);

            if (SpecialDeclare_Bison != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(SpecialDeclare_Bison, PoolType.EffectPool);
                SpecialDeclare_Bison = null;
            }

            if (Skill_Bison != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_Bison, PoolType.EffectPool);
                Skill_Bison = null;
            }

            if (BisonCutIn != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(BisonCutIn, PoolType.EffectPool);
                BisonCutIn = null;
            }

            if (OtherSpecialDeclare_Bison != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(OtherSpecialDeclare_Bison, PoolType.EffectPool);
                OtherSpecialDeclare_Bison = null;
            }

            isCanDestory = false;
            isPlayingAnim = false;
            callBack = null;
        }
    }
}

