using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishScoreItem
    {
        public GameObject gameObject;
        string[] MyAnimParams;
        string[] OtherAnimParams;
        Vector3 beginPos;
        public bool isCanDestory;
        public bool isPlayingAnim;
        public bool IsDelayPlay;
        public float delayTime;
        public float currentTime;
        public float totalTime;
        public Text currentScoreLabel;
        public bool IsChangeScore;
        public int targetChangeScoreStep;
        public ScoreStep curScoreStep;
        public float changeScoreCurrentTime;
        public float changeScoreTotalTime;
        public int changeScoreInitScore;
        public int changeScoreTempScore;
        public Timer changeScoreDelayTimer;
        Animator Anim;
        Text text1;
        Text text2;
        public EffectVo EffectVo;
        bool isCanMove;
        Action callBack;
        public enum ScoreStep
        {
            Normal = 1,
            OneStep = 2,
            TwoStep = 3,
            End = 4,
        }
        public FishScoreItem(GameObject obj)
        {
            gameObject = obj;
            InitData();
            InitView();
        }

        private void InitData()
        {
            MyAnimParams = new string[] { "MyScore01", "MyScore02", "MyScore03" };
            OtherAnimParams = new string[] { "OtherScore02", "OtherScore02", "OtherScore03" };
            beginPos = new Vector3(0, 0, 0);
            isCanDestory = false;
            isPlayingAnim = false;
            IsDelayPlay = false;
            delayTime = 0;
            currentTime = 0;
            totalTime = 1;
            IsChangeScore = true;
            targetChangeScoreStep = (int)ScoreStep.Normal;
            curScoreStep = ScoreStep.Normal;
            changeScoreCurrentTime = 0;
            changeScoreTotalTime = 1;
            changeScoreInitScore = 0;
            changeScoreTempScore = 0;
        }

        public void InitView()
        {
            FindView();
        }

        public void FindView()
        {
            Transform tf = gameObject.transform;
            Anim = gameObject.GetComponent<Animator>();
            text1 = tf.Find("Content/Text1").GetComponent<Text>();
            text2 = tf.Find("Content/Text2").GetComponent<Text>();
        }

        public void SetShowScoreText(string score)
        {
            currentScoreLabel.text = score;
        }

        public void PlayAnim(int index)
        {
            if (EffectVo.isMe)
            {
                string animationName = MyAnimParams[index];
                Anim.Play(animationName, 0, 0);
            }
            else
            {
                string animationName = OtherAnimParams[index];
                Anim.Play(animationName, 0, 0);
            }
        }

        public void ResetEffectVo(EffectVo vo)
        {
            EffectVo = vo;
            isCanDestory = false;
            isPlayingAnim = false;
            IsDelayPlay = false;
        }

        public void ResetState(Vector3 beginPos, float delayTime, float showTime, Action callBack)
        {
            isCanMove = false;
            isCanDestory = false;
            gameObject.SetActive(true);
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
            this.beginPos = beginPos;
            this.delayTime = delayTime;
            this.callBack = callBack;
            totalTime = showTime;
            currentTime= 0;
            IsDelayPlay= true;
            SetScoreLabelText();
            SetChangeScore();
        }

        public void SetChangeScore()
        {
            changeScoreCurrentTime = 0;
            changeScoreInitScore = 0;
            targetChangeScoreStep = EffectVo.ScoreEffectConfig.scoreEffect;
            switch (EffectVo.ScoreEffectConfig.scoreEffect)
            {
                case 1:
                    IsChangeScore = false;
                    changeScoreTempScore = 0;
                    curScoreStep = ScoreStep.Normal;
                    SetShowScoreText(EffectVo.score.ToString());
                    break;
                case 2:
                    IsChangeScore = true;
                    changeScoreTempScore = EffectVo.score;
                    curScoreStep = ScoreStep.OneStep;
                    if (EffectVo.isMe)
                        FishAudioManager.Instance.PlayNormalAudio(28);
                    break;
                case 3:
                    IsChangeScore = true;
                    changeScoreTempScore = Mathf.CeilToInt(EffectVo.score * UnityEngine.Random.Range(0.4f, 0.75f));
                    curScoreStep = ScoreStep.OneStep;
                    if (EffectVo.isMe)
                        FishAudioManager.Instance.PlayNormalAudio(28);
                    break;
                default:
                    break;
            }
        }

        public void SetScoreLabelText()
        {
            if (EffectVo.isMe)
            {
                currentScoreLabel = text1;
            }
            else
            {
                currentScoreLabel = text2;
            }
        }

        public void ChangeScore()
        {
            if (IsChangeScore)
            {
                if (curScoreStep == ScoreStep.OneStep)
                {
                    changeScoreCurrentTime += Time.deltaTime;
                    if (changeScoreCurrentTime <= changeScoreTotalTime)
                    {
                        int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (currentTime / totalTime));
                        SetShowScoreText(result.ToString());
                    }
                    else
                    {
                        IsChangeScore = false;
                        changeScoreCurrentTime = 0;
                        curScoreStep = ScoreStep.TwoStep;
                        changeScoreInitScore = changeScoreTempScore;
                        SetShowScoreText(changeScoreTempScore.ToString());
                        if (EffectVo.isMe)
                            FishAudioManager.Instance.PlayNormalAudio(31);
                        ChangeScoreEnd();
                    }
                }
                else if (curScoreStep == ScoreStep.TwoStep)
                {
                    changeScoreCurrentTime += Time.deltaTime;
                    if (changeScoreCurrentTime <= changeScoreTotalTime)
                    {
                        int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (currentTime / totalTime));
                        SetShowScoreText(result.ToString());
                    }
                    else
                    {
                        IsChangeScore = false;
                        changeScoreCurrentTime = 0;
                        curScoreStep = ScoreStep.End;
                        changeScoreInitScore = EffectVo.score;
                        SetShowScoreText(EffectVo.score.ToString());
                        if (EffectVo.isMe)
                        {
                            FishAudioManager.Instance.StopNormalAudio(29);
                            FishAudioManager.Instance.PlayNormalAudio(32);
                        }
                        ChangeScoreEnd();
                    }
                }
            }
        }
        public void ChangeScoreEnd()
        {
            PlayAnim(2);
            if ((int)curScoreStep > targetChangeScoreStep || curScoreStep == ScoreStep.End)
            {
                return;
            }
            if (changeScoreDelayTimer == null)
            {
                changeScoreDelayTimer = new Timer(500);
                changeScoreDelayTimer.Elapsed += DelayChangeScore;
                changeScoreDelayTimer.Start();
            }
        }

        private void DelayChangeScore(object sender, ElapsedEventArgs e)
        {
            IsChangeScore = true;
            if (EffectVo.isMe)
                FishAudioManager.Instance.PlayNormalAudio(29);
            changeScoreTempScore = EffectVo.score - changeScoreTempScore;
            changeScoreDelayTimer.Dispose();
            changeScoreDelayTimer = null;
        }

        public void Update()
        {
            if (IsDelayPlay)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= delayTime)
                {
                    IsDelayPlay = false;
                    currentTime = 0;
                    gameObject.GetComponent<RectTransform>().anchoredPosition = beginPos;
                    if (targetChangeScoreStep == (int)ScoreStep.Normal)
                    {
                        PlayAnim(0);
                    }
                    else if (targetChangeScoreStep == (int)ScoreStep.OneStep || targetChangeScoreStep == (int)ScoreStep.TwoStep)
                    {
                        PlayAnim(1);
                    }
                    isPlayingAnim = true;
                }
            }

            if (isPlayingAnim)
            {
                currentTime += Time.deltaTime;
                ChangeScore();
                if (currentTime >= totalTime)
                {
                    currentTime = 0;
                    isPlayingAnim = false;
                    EndCallBack();
                    isCanDestory = true;
                }
            }
        }

        public void EndCallBack()
        {
            if (callBack != null) { callBack(); }
        }

        public void Destroy()
        {
            gameObject.SetActive(false);
            isPlayingAnim = false;
            IsDelayPlay = false;
            isCanDestory = false;
            callBack = null;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }
    }
}
