using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishSpiderCrabBoardScore
    {
        public enum ScoreStep
        {
            OneStep = 1,
            TwoStep = 2,
            End = 4,
        }
        GameObject gameObject;
        string[] animParms1;
        string[] animParms2;
        ScoreStep targetChangeScoreStep = ScoreStep.OneStep;
        ScoreStep curScoreStep = ScoreStep.TwoStep;
        float changeScoreCurrentTime = 0f;
        float changeScoreTotalTime = 1.5f;
        int changeScoreInitScore = 0;
        int changeScoreTempScore = 0;
        float delayTime = 0f;
        float currentTime = 0f;
        public bool isCanDestroy;
        bool isPlayingAnim;
        bool isDelayPlay;
        bool isChangeScore;
        Vector3 targetPos;
        Vector3 beginPos;
        Animator animator;
        Text scoreText;
        GameObject twoMultiple_Sprite_gameObject;
        GameObject threeMultiple_Sprite_gameObject;
        public EffectVo effectVo;
        Action callBack;

        public FishSpiderCrabBoardScore(GameObject obj)
        {
            gameObject = obj;
            InitData();
            FindView();
        }

        private void InitData()
        {
            animParms1 = new string[] { "bosskilled_start_chs", "bosskilled_NOlove_chs", "bosskilled_NOlove_off_chs" };
            animParms1 = new string[] { "bosskilled_start_chs", "bosskilled_love_chs", "bosskilled_love_off_chs" };
            targetPos = Vector3.zero;
            beginPos = Vector3.zero;
        }

        private void FindView()
        {
            animator = gameObject.GetComponent<Animator>();
            scoreText = gameObject.transform.Find("mainwidow/number").GetComponent<Text>();
            twoMultiple_Sprite_gameObject = gameObject.transform.Find("mainwidow/X2X3/ImageX2").gameObject;
            threeMultiple_Sprite_gameObject = gameObject.transform.Find("mainwidow/X2X3/ImageX3").gameObject;
        }

        public void SetShowScoreText(string score)
        {
            scoreText.text = score;
        }

        public void PlayAnim(int index)
        {
            if (effectVo.partMul == 1)
                animator.Play(animParms1[index], 0, 0);
            else
                animator.Play(animParms2[index], 0, 0);
        }

        public void ResetEffectVo(EffectVo vo)
        {
            effectVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayPlay = false;
            isChangeScore = false;
        }

        public void ResetState(Vector3 targetPos, Action callBack = null)
        {
            isCanDestroy = false;
            this.targetPos = targetPos;
            if (effectVo.isMe)
            {
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localScale = Vector3.one;
            }
            else
            {
                gameObject.transform.position = targetPos;
                gameObject.transform.localScale = Vector3.one * 0.75f;
            }

            currentTime = 0;
            delayTime = 0;
            this.callBack = callBack;
            isDelayPlay = true;
            isChangeScore = false;
            changeScoreCurrentTime = 0;
            if (effectVo.partMul == 2)
            {
                twoMultiple_Sprite_gameObject.SetActive(true);
                threeMultiple_Sprite_gameObject.SetActive(false);
            }
            else
            {
                twoMultiple_Sprite_gameObject.SetActive(false);
                threeMultiple_Sprite_gameObject.SetActive(true);
            }

            FishAudioManager.Instance.PlayNormalAudio(64);
            SetShowScoreText("0");
        }

        public void BeginSpiderCrabBossScoreStep()
        {
            OneStepCrabBossScore();
        }

        public void OneStepCrabBossScore()
        {
            if (effectVo.partMul == 1)
                AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 2, () => { SetChangeScore(); });
            else
            {
                AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 2, () => { PlayAnim(1); });
                AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 4.5f, () => { SetChangeScore(); });
            }
            PlayAnim(0);
        }

        public void SetChangeScore()
        {
            changeScoreCurrentTime = 0;
            isChangeScore = true;
            if (effectVo.partMul == 1)
            {
                changeScoreInitScore = 0;
                changeScoreTempScore = effectVo.totalScore;
            }
            else
            {
                changeScoreInitScore = effectVo.selfScore;
                changeScoreTempScore = effectVo.totalScore - effectVo.selfScore;
            }
            SetShowScoreText(changeScoreInitScore.ToString());
            if (effectVo.isMe)
            {
                FishAudioManager.Instance.StopNormalAudio(63);
                FishAudioManager.Instance.PlayNormalAudio(63, 0.4f, true);
            }
        }

        public void ChangeScore()
        {
            if (isChangeScore)
            {
                changeScoreCurrentTime += Time.deltaTime;
                if (changeScoreCurrentTime <= changeScoreTotalTime)
                {
                    int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (changeScoreCurrentTime / changeScoreTotalTime));
                    SetShowScoreText(result.ToString());
                }
                else
                {
                    changeScoreCurrentTime = 0;
                    changeScoreInitScore = effectVo.totalScore;
                    SetShowScoreText(effectVo.totalScore.ToString());
                    ChangeScoreEnd();
                }
            }
        }

        public void ChangeScoreEnd()
        {
            isChangeScore = false;
            isPlayingAnim = false;
            if (effectVo.isMe)
                FishAudioManager.Instance.StopNormalAudio(63);

            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 0.5f, () =>
            {
                PlayAnim(2);
            });
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1.5f, () =>
            {
                EndCallBack();
                isCanDestroy = true;
            });
        }

        public void EndCallBack()
        {
            callBack?.Invoke();
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
                    BeginSpiderCrabBossScoreStep();
                    isPlayingAnim = true;
                }
            }
            if (isPlayingAnim)
                ChangeScore();
        }

        public void Destroy()
        {
            if(effectVo.isMe)
                FishAudioManager.Instance.StopNormalAudio(63);
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }

    }
}
