using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishFireStormSkillItem
    {
        enum FireStormSkillStats
        {
            Born = 1,
            Idle = 2,
            Shoot = 3,
            End = 4,
            Destroy = 5,
        }

        string Skill_FireStorm_Res = "Skill_FireStorm";
        string FireStorm_SpecialDeclare_Res = "SpecialDeclare_FireStorm";
        string FireStormYouWin_Res = "FireStormYouWin";

        bool isPlayingAnim = false;
        public bool isCanDestory = false;
        FireStormSkillStats curSkillStats = FireStormSkillStats.Born;
        Vector3 beginPos;
        Vector3 targetPos;
        float currentTime = 0;
        float shootTotalTime = 30;
        float idleTotalTime = 7.4f;
        float endTotalTime = 2.5f;
        float destroyTotalTime = 0.5f;

        float changeScoreCurrentTime = 0;
        int changeScoreTempScore = 0;
        float changeScoreTotalTime = 0;
        int changScoreInitScore = 0;
        bool IsChangeScore = false;

        int changeMultipleTempMul;
        int changMultipleInitMul;

        bool isEndWarning;

        List<int> fireStormScoreTable = new List<int>();
        List<int> fireStormMultipleTable = new List<int>();

        Transform SkillGunParent;
        Transform SkillPanelParent;

        GameObject FireStormInfo;
        GameObject FireStormObject;
        GameObject Skill_FireStorm;
        GameObject FireStorm_SpecialDeclare;
        GameObject FireStormYouWin;

        Text Mul_F;
        Text Sec_F;
        Text Sec_B;
        Text Score_T;

        Animator FS_Board_Multiplier_Animator;
        Animator FS_Board_Score_Animator;
        Animator FireStormYouWinAnimator;

        Action callBack;

        public SkillVo SkillVo;

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

            if (SkillVo.IsMe)
            {
                SkillVo.PlayerIns.ResetBulletRate();
                SkillVo.PlayerIns.UploadShootBulletRateLevel(1);
            }

            SkillVo.PlayerIns.SetCanShootBullet(false);
            beginPos = FishGameObjectPoolManager.Instance.GetPoolParent(PoolType.EffectPool).transform.position;

            targetPos = SkillPanelParent.position;
            this.callBack = callBack;

            changeScoreCurrentTime = 0;
            changeScoreTempScore = 0;
            changScoreInitScore = skillScore;

            changeMultipleTempMul = 0;
            changMultipleInitMul = skillMul;

            fireStormScoreTable = new List<int>();
            fireStormMultipleTable = new List<int>();

            IsChangeScore = false;

            isEndWarning = true;

            ChangePlayerSpeedBtn(false);

            GetCurrentFireStormStep(skillStatus, skillTime);

            ShowFireStormSkill();

            isPlayingAnim = true;
        }

        public void ChangePlayerSpeedBtn(bool isDisPlay)
        {
            if (SkillVo.IsMe)
            {
                //todo
                //GameSetManager.GetInstance().PlayerSetPanel.SetSpeedBtnDisable(isDisPlay);
                //GameSetManager.GetInstance().PlayerSetPanel.ResetSpeedState(false);
            }
        }

        public void GetCurrentFireStormStep(int skillStatus, float skillTime)
        {
            if (skillStatus == 1)
            {
                if (skillTime <= 0.05f)
                {
                    curSkillStats = FireStormSkillStats.Born;
                }
                else
                {
                    curSkillStats = FireStormSkillStats.Idle;
                }
            }
            else if (skillStatus == 2)
            {
                curSkillStats = FireStormSkillStats.Shoot;
            }
            currentTime = skillTime * 0.001f;
        }

        public void ShowFireStormSkill()
        {
            if (curSkillStats == FireStormSkillStats.Born)
            {
                GetFireStormResItem(beginPos);
                PlayFirestormAudioBorn();
                MoveFireStormItemToGun();
            }
            else if (curSkillStats == FireStormSkillStats.Idle)
            {
                GetFireStormResItem(targetPos);
                ShowGunFireStorm();
                FireStormInfo.SetActive(true);
                FireStormObject.SetActive(false);
                SetFireStormInfo(changScoreInitScore, changMultipleInitMul);
                BeginShootCountdown(0);
            }
            else if (curSkillStats == FireStormSkillStats.Shoot)
            {
                GetFireStormResItem(targetPos);
                ShowGunFireStorm();
                FireStormInfo.SetActive(true);
                FireStormObject.SetActive(false);
                SetFireStormInfo(changScoreInitScore, changMultipleInitMul);
                BeginShootCountdown(currentTime);
                BeginFireStormShoot(currentTime);
            }
        }

        public void PlayFirestormAudioBorn()
        {
            if (SkillVo.IsMe)
            {
                AsyncActionUtils.DelayedAction(FishFireStormSkillManager.Instance, 0.6f, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(51);
                });
                AsyncActionUtils.DelayedAction(FishFireStormSkillManager.Instance, 1.2f, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(51);
                });
                AsyncActionUtils.DelayedAction(FishFireStormSkillManager.Instance, 1.8f, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(51);
                });
                AsyncActionUtils.DelayedAction(FishFireStormSkillManager.Instance, 2.4f, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(51);
                });
                AsyncActionUtils.DelayedAction(FishFireStormSkillManager.Instance, 2.8f, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(52);
                });
            }
        }

        public void GetFireStormResItem(Vector3 pos)
        {
            Skill_FireStorm = FishGameObjectPoolManager.Instance.GetGameObject(Skill_FireStorm_Res, PoolType.EffectPool);
            Transform trans = Skill_FireStorm.transform;
            trans.SetParent(SkillPanelParent);
            trans.position = pos;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;
            FireStormObject = trans.Find("FireStormObject").gameObject;
            FireStormInfo = trans.Find("FireStormInfo").gameObject;
            Mul_F = trans.Find("FireStormInfo/Multiple/Text2/Text_F").GetComponent<Text>();
            FS_Board_Multiplier_Animator = trans.Find("FireStormInfo/Multiple/Text2").GetComponent<Animator>();
            Sec_F = trans.Find("FireStormInfo/Sec/Text2/Text_F").GetComponent<Text>();
            Sec_B = trans.Find("FireStormInfo/Sec/Text2/Text _B").GetComponent<Text>();
            Score_T = trans.Find("FireStormInfo/Score/Text").GetComponent<Text>();
            FS_Board_Score_Animator = trans.Find("FireStormInfo/Score").GetComponent<Animator>();
            FireStormInfo.SetActive(false);
            FireStormObject.SetActive(true);
        }

        public void MoveFireStormItemToGun()
        {
            Transform trans = Skill_FireStorm.transform;
            AsyncActionUtils.ApplyMovement(FishFireStormSkillManager.Instance, trans, trans.position, targetPos, 0.5f, TweenUtils.VectorTweenLinear, 4, () => {
                AsyncActionUtils.DelayedAction(FishFireStormSkillManager.Instance, 0f, () =>
                {
                    FireStormObject.SetActive(false);
                    ShowFireStormSpecialDeclare();
                    ShowGunFireStorm();
                });
            });
        }

        public void ShowGunFireStorm()
        {
            SkillVo.PlayerIns.SetMuzzleEulerAngles(0 / SkillVo.PlayerIns.PrecisionValue);
            SkillVo.PlayerIns.EnterFreeScoreState();
            PlayGunFireStormAnim("FireStormGun_Idle");
        }

        public void BeginFireStormShoot(float timeElapse)
        {
            currentTime = timeElapse;
            curSkillStats = FireStormSkillStats.Shoot;
            SkillVo.PlayerIns.SetCanShootBullet(true);
        }

        public void PlayGunFireStormAnim(string animationName)
        {
            int curUseRealGunLevel = SkillVo.PlayerIns.currentUseRealGunLevel;
            SkillVo.PlayerIns.Panel.PlayFireStormShootAnim(curUseRealGunLevel, animationName);
        }

        public void ShowFireStormSpecialDeclare()
        {
            if (SkillVo.IsMe)
            {
                FireStorm_SpecialDeclare = FishGameObjectPoolManager.Instance.GetGameObject(FireStorm_SpecialDeclare_Res, PoolType.SpecialDeclarePool);
                Transform trans = FireStorm_SpecialDeclare.transform;
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.Euler(0, 0, 0);
                trans.localScale = Vector3.one;
                PlayReadGoAudio();
            }
        }

        public void PlayReadGoAudio()
        {
            if (SkillVo.IsMe)
            {
                AsyncActionUtils.DelayedAction(FishGameObjectPoolManager.Instance, 1, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(54);
                });
                AsyncActionUtils.DelayedAction(FishGameObjectPoolManager.Instance, 1.8f, () =>
                {
                    FishAudioManager.Instance.PlayNormalAudio(53);
                });
            }
        }

        public void HideFireStormSpecialDeclare()
        {
            if (SkillVo.IsMe)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(FireStorm_SpecialDeclare, PoolType.SpecialDeclarePool);
            }
            FireStormInfo.SetActive(true);
            FireStormObject.SetActive(false);
            SetFireStormInfo(changScoreInitScore, changMultipleInitMul);
            BeginShootCountdown(0);
            FireStorm_SpecialDeclare = null;
        }

        public void EndFireStorm(int score)
        {
            Skill_FireStorm.SetActive(false);
            FireStormYouWin = FishGameObjectPoolManager.Instance.GetGameObject(FireStormYouWin_Res, PoolType.SpecialDeclarePool);
            Transform trans = FireStormYouWin.transform;
            trans.position = SkillVo.PlayerIns.specialDeclarePanel.position;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;
            SkillVo.PlayerIns.LeaveFreeScoreState();
            SkillVo.PlayerIns.IsShowBetPanel();
            ChangePlayerSpeedBtn(true);
            FireStormYouWinAnimator = FireStormYouWin.GetComponent<Animator>();
            FireStormYouWinAnimator.Play("FireStormYouWin_start", 0, 0);
            Text scoreText = trans.Find("ScoreTextROOT/ScoreText").GetComponent<Text>();
            scoreText.text = score.ToString();
            curSkillStats = FireStormSkillStats.End;
            currentTime = 0;
            if (SkillVo.IsMe)
            {
                FishAudioManager.Instance.StopNormalAudio(56);
                FishAudioManager.Instance.PlayNormalAudio(37);
                FishAudioManager.Instance.PlayNormalAudio(57);
            }
        }

        public void DestroyFireStorm()
        {
            isCanDestory = true;
            if (callBack != null)
            {
                callBack();
            }
        }

        public void BeginShootCountdown(float timeElapse)
        {
            float time = shootTotalTime - timeElapse;
            if (time <= 0)
            {
                time = 0;
            }
            int t1 = (int)time;
            float t2 = time - t1;
            if (t1 < 10)
            {
                if (isEndWarning == true)
                {
                    isEndWarning = false;
                    if (SkillVo.IsMe)
                        FishAudioManager.Instance.PlayNormalAudio(56, 1, true);
                }
                Sec_F.text = "0" + t1.ToString();
            }
            else
            {
                Sec_F.text = t1.ToString();
            }

            t2 = Mathf.Floor(t2 * 100);
            if (t2 < 10)
            {
                Sec_B.text = ".0" + t2.ToString();
            }
            else
            {
                Sec_B.text = "." + t2.ToString();
            }
        }

        public void SetFireStormInfo(int score, int multiple)
        {
            Score_T.text = score.ToString();
            Mul_F.text = "x" + multiple.ToString();
        }

        public void RefreshFireStormInfo(int score, int mul)
        {
            IsChangeScore = true;
            changeScoreTempScore = score;
            changeMultipleTempMul = mul;

            if (fireStormScoreTable == null)
            {
                fireStormScoreTable = new List<int>();
            }
            if (fireStormMultipleTable == null)
            {
                fireStormMultipleTable = new List<int>();
            }
            fireStormScoreTable.Add(score);
            fireStormMultipleTable.Add(mul);
            FS_Board_Score_Animator.Play("FireStorm_Board_Score", 0, 0);
            FS_Board_Multiplier_Animator.Play("FireStorm_Board_Score", 0, 0);
        }

        public void UpdateFireStormInfo()
        {
            if (!IsChangeScore)
            {
                return;
            }

            if (fireStormScoreTable != null && fireStormScoreTable.Count > 0)
            {
                changeScoreCurrentTime += Time.deltaTime;
                if (changeScoreCurrentTime <= changeScoreTotalTime)
                {
                    int scoreResult = changScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (changeScoreCurrentTime / changeScoreTotalTime));
                    int multipleResult = changMultipleInitMul + Mathf.CeilToInt(changeMultipleTempMul * (changeScoreCurrentTime / changeScoreTotalTime));
                    SetFireStormInfo(scoreResult, multipleResult);
                }
                else
                {
                    fireStormScoreTable.RemoveAt(0);
                    fireStormMultipleTable.RemoveAt(0);
                    changeScoreCurrentTime = 0;
                    changScoreInitScore += changeScoreTempScore;
                    changMultipleInitMul += changeMultipleTempMul;
                    SetFireStormInfo(changScoreInitScore, changMultipleInitMul);
                    FS_Board_Score_Animator.Play("FireStorm_Board_Score", 0, 0);
                    FS_Board_Multiplier_Animator.Play("FireStorm_Board_Score", 0, 0);
                    if (fireStormScoreTable != null && fireStormScoreTable.Count > 0)
                    {
                        changeScoreTempScore = fireStormScoreTable[0];
                        changeMultipleTempMul = fireStormMultipleTable[0];
                    }
                }
            }
            else
            {
                IsChangeScore = false;
            }
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (curSkillStats == FireStormSkillStats.Born || curSkillStats == FireStormSkillStats.Idle)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= idleTotalTime)
                    {
                        currentTime = 0;
                    }
                }
                else if (curSkillStats == FireStormSkillStats.Shoot)
                {
                    currentTime += Time.deltaTime;
                    BeginShootCountdown(currentTime);
                    UpdateFireStormInfo();
                    if (currentTime >= shootTotalTime)
                    {
                        currentTime = 0;
                    }
                }
                else if (curSkillStats == FireStormSkillStats.End)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= endTotalTime)
                    {
                        currentTime = 0;
                        curSkillStats = FireStormSkillStats.Destroy;
                        FireStormYouWinAnimator.Play("FireStormYouWin_end", 0, 0);
                    }
                }
                else if (curSkillStats == FireStormSkillStats.Destroy)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= destroyTotalTime)
                    {
                        currentTime = 0;
                        isPlayingAnim = false;
                        DestroyFireStorm();
                    }
                }
            }
        }

        public void Destroy()
        {
            if (FireStormYouWin != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(FireStormYouWin, PoolType.SpecialDeclarePool);
            }
            FireStormYouWin = null;

            if (Skill_FireStorm != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_FireStorm, PoolType.EffectPool);
            }
            Skill_FireStorm = null;

            if (FireStorm_SpecialDeclare != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(FireStorm_SpecialDeclare, PoolType.SpecialDeclarePool);
            }
            FireStorm_SpecialDeclare = null;

            isCanDestory = false;
            isPlayingAnim = false;
            callBack = null;
            FireStormYouWin = null;
        }
    }
}
