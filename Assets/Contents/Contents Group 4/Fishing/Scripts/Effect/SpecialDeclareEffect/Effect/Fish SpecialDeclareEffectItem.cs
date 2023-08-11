using System;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public enum ScoreStep
    {
        OneStep = 1,
        TwoStep = 2,
        ThreeStep = 3,
        End = 4,
    }

    public class FishSpecialDeclareEffectItem
    {
        private GameObject gameObject;
        private string[][] myAnimParms;
        private string[][] otherAnimParms;
        private ScoreStep targetChangeScoreStep;
        private ScoreStep curScoreStep;
        private float changeScoreCurrentTime;
        private float changeScoreTotalTime;
        private int changeScoreInitScore;
        private int changeScoreTempScore;
        public bool isCanDestroy;
        private bool isPlayingAnim;
        private bool isDelayPlay;
        private float delayTime;
        private float currentTime;
        private bool isChangeScore;
        private Vector3 targetPos;
        private Animator animator;
        private Text scoreText;
        public EffectVo effectVo;
        private Action callBack;
        public bool isCanMove;

        public FishSpecialDeclareEffectItem(GameObject obj)
        {
            gameObject = obj;
            InitData();
            InitView();
        }

        private void InitData()
        {
            myAnimParms = new string[][]
            {
                new string[]{"My_OneStep_01","My_ToStep_01","My_ThreeStep_01","My_FourStep_01","My_FiveStep_01" },
                new string[]{"My_OneStep_02","My_TwoStep_02","My_ThreeStep_02","My_FourStep_02","My_FiveStep_02" },
            };
            otherAnimParms = new string[][]
            {
                new string[]{ "Other_OneStep_01","My_ThreeStep_01","My_ThreeStep_01","My_FourStep_01","My_FiveStep_01" },
                new string[]{"Other_OneStep_02","My_ThreeStep_02","My_ThreeStep_02","My_FourStep_02","My_FiveStep_02" },
            };
            targetChangeScoreStep = ScoreStep.OneStep;
            curScoreStep = ScoreStep.TwoStep;

            changeScoreTotalTime = 1;
            targetPos = Vector3.zero;
        }

        private void InitView()
        {
            FindView();
        }

        private void FindView()
        {
            animator = gameObject.transform.Find("Object").GetComponent<Animator>();
            scoreText = gameObject.transform.Find("Object/Foreground/num").GetComponent<Text>();
        }

        public void SetShowScoreText(string score)
        {
            scoreText.text = score;
        }

        public void PlayAnim(int type, int index)
        {
            string animName;
            type = type - 1;
            if (effectVo.isMe)
                animName = myAnimParms[type][index];
            else
                animName = otherAnimParms[type][index];
            animator.Play(animName, 0, 0);
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
            isCanMove = false;
            isCanDestroy = false;
            this.targetPos = targetPos;
            if (effectVo.isMe)
                gameObject.transform.localPosition = new Vector3(1200, 0, 0);
            else
                gameObject.transform.position = targetPos;
            gameObject.transform.localScale = Vector3.one;
            currentTime = 0;
            delayTime = 0;
            this.callBack = callBack;
            isDelayPlay = true;
            isChangeScore = false;
            changeScoreCurrentTime = 0;
            targetChangeScoreStep = ScoreStep.OneStep;
            curScoreStep = ScoreStep.TwoStep;
        }

        public void BeginSpecialDeclareStep()
        {
            if (effectVo.isMe)
                MySelfOneStepSpecialDeclare();
            else
                OtherOneStepSpecialDeclare();
        }

        public void MySelfOneStepSpecialDeclare()
        {
            float moveDuration = 0.4f;
            float oneStepDuration = 1.45f;
            AsyncActionUtils.ApplyMovement(FishSpecialDeclareEffectManager.Instance, gameObject.transform,
                gameObject.transform.position, Vector3.zero, moveDuration, TweenUtils.VectorTweenLinear, 0, () =>
                {
                    AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, oneStepDuration, MySelfTwoStepSpecialDeclare);
                });
            PlayAnim(effectVo.AnimType, 0);
        }

        public void MySelfTwoStepSpecialDeclare()
        {
            float moveDuration = 0.25f;
            float twoStepDuration = 0.32f;
            AsyncActionUtils.ApplyMovement(FishSpecialDeclareEffectManager.Instance, gameObject.transform,
                gameObject.transform.position, targetPos, moveDuration, TweenUtils.VectorTweenLinear, 0, () =>
                {
                    AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, twoStepDuration, () =>
                    {
                        PlayAnim(effectVo.AnimType, 2);
                    });
                });
            PlayAnim(effectVo.AnimType, 1);
        }

        public void OtherOneStepSpecialDeclare()
        {
            float moveDuration = 0.4f;
            float oneStepDuration = 1.45f;
            AsyncActionUtils.ApplyMovement(FishSpecialDeclareEffectManager.Instance, gameObject.transform,
                gameObject.transform.position, targetPos, moveDuration, TweenUtils.VectorTweenLinear, 0, () =>
                {
                    AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, oneStepDuration, () =>
                    {
                        PlayAnim(effectVo.AnimType, 2);
                    });
                });
            PlayAnim(effectVo.AnimType, 0);
        }

        public void ComFourStepSpecialDeclare()
        {
            float fourStepDuration = 0.35f;
            AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, fourStepDuration, () =>
            {
                if (effectVo.isMe)
                    FishAudioManager.Instance.PlayNormalAudio(28);
                ComFiveStepSpecialDeclare();
            });

            PlayAnim(effectVo.AnimType, 3);
        }

        public void ComFiveStepSpecialDeclare()
        {
            isChangeScore = true;
            changeScoreCurrentTime = 0;
            PlayAnim(effectVo.AnimType, 4);
        }

        public void BeginShowScore()
        {
            SetChangeScore();
            ComFourStepSpecialDeclare();
        }

        public void SetChangeScore()
        {
            changeScoreCurrentTime = 0;
            changeScoreInitScore = 0;
            isChangeScore = false;
            curScoreStep = ScoreStep.OneStep;
            changeScoreTempScore = 0;
            if (effectVo.multiple >= 100)
            {
                targetChangeScoreStep = ScoreStep.ThreeStep;
                changeScoreTempScore = Mathf.CeilToInt(effectVo.score * UnityEngine.Random.Range(0.3f, 0.5f));
            }
            else
            {
                targetChangeScoreStep = ScoreStep.TwoStep;
                changeScoreTempScore = Mathf.CeilToInt(effectVo.score * UnityEngine.Random.Range(0.4f, 0.6f));
            }
            SetShowScoreText("0");
        }

        public void ChangeScore()
        {
            if (isChangeScore)
            {
                if (curScoreStep == ScoreStep.OneStep)
                {
                    changeScoreCurrentTime += Time.deltaTime;
                    if (changeScoreCurrentTime <= changeScoreTotalTime - 0.2f)
                    {
                        int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (changeScoreCurrentTime / changeScoreTotalTime));
                        SetShowScoreText(result.ToString());
                    }
                    else
                    {
                        isChangeScore = false;
                        changeScoreCurrentTime = 0;
                        curScoreStep = ScoreStep.TwoStep;
                        changeScoreInitScore = changeScoreTempScore;
                        SetShowScoreText(changeScoreInitScore.ToString());
                        if (effectVo.isMe)
                        {
                            FishAudioManager.Instance.StopNormalAudio(28);
                            FishAudioManager.Instance.PlayNormalAudio(31);
                        }
                        ChangeScoreEnd();
                    }
                }
                else if (curScoreStep == ScoreStep.TwoStep)
                {
                    changeScoreCurrentTime += Time.deltaTime;
                    if (changeScoreCurrentTime <= changeScoreTotalTime - 0.4f)
                    {
                        int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (changeScoreCurrentTime / changeScoreTotalTime));
                        SetShowScoreText(result.ToString());
                    }
                    else
                    {
                        isChangeScore = false;
                        changeScoreCurrentTime = 0;
                        if (curScoreStep >= targetChangeScoreStep)
                        {
                            curScoreStep = ScoreStep.End;
                            changeScoreInitScore = effectVo.score;
                        }
                        else
                        {
                            curScoreStep = ScoreStep.ThreeStep;
                            changeScoreInitScore += changeScoreTempScore;
                        }
                        SetShowScoreText(changeScoreInitScore.ToString());
                        if (effectVo.isMe)
                        {
                            FishAudioManager.Instance.StopNormalAudio(29);
                            FishAudioManager.Instance.PlayNormalAudio(32);
                        }
                        ChangeScoreEnd();
                    }
                }
                else if (curScoreStep == ScoreStep.ThreeStep)
                {
                    changeScoreCurrentTime += Time.deltaTime;
                    if (changeScoreCurrentTime <= changeScoreTotalTime - 0.6f)
                    {
                        int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (changeScoreCurrentTime / changeScoreTotalTime));
                        SetShowScoreText(result.ToString());
                    }
                    else
                    {
                        isChangeScore = false;
                        changeScoreCurrentTime = 0;
                        curScoreStep = ScoreStep.End;
                        changeScoreInitScore = effectVo.score;
                        SetShowScoreText(effectVo.score.ToString());
                        if (effectVo.isMe)
                        {
                            FishAudioManager.Instance.StopNormalAudio(30);
                            FishAudioManager.Instance.PlayNormalAudio(33);
                        }
                        ChangeScoreEnd();
                    }
                }
            }
        }

        public void ChangeScoreEnd()
        {
            scoreText.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            Action changeScoreWaitCallBack = () => {
                if (curScoreStep == ScoreStep.TwoStep)
                {
                    if (effectVo.isMe)
                        FishAudioManager.Instance.PlayNormalAudio(29);
                }
                else if (curScoreStep == ScoreStep.ThreeStep)
                {
                    if (effectVo.isMe)
                        FishAudioManager.Instance.PlayNormalAudio(30);
                }
                isChangeScore = true;
            };
            if (curScoreStep == ScoreStep.TwoStep)
            {
                if (targetChangeScoreStep == ScoreStep.TwoStep)
                    changeScoreTempScore = effectVo.score - changeScoreInitScore;
                else if (targetChangeScoreStep == ScoreStep.ThreeStep)
                    changeScoreTempScore = Mathf.CeilToInt((effectVo.score - changeScoreInitScore) * UnityEngine.Random.Range(0.4f, 0.6f));
                AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, scoreText.transform,
                    scoreText.transform.localScale, new Vector3(0.8f, 0.8f, 1), 0.3f, TweenUtils.VectorTweenLinear, 0, () =>
                    {
                        AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 0.7f, changeScoreWaitCallBack);
                    });
            }
            else if (curScoreStep == ScoreStep.ThreeStep)
            {
                changeScoreTempScore = effectVo.score - changeScoreInitScore;
                AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, scoreText.transform,
                    scoreText.transform.localScale, new Vector3(0.8f, 0.8f, 1), 0.3f, TweenUtils.VectorTweenLinear, 0, () =>
                    {
                        AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 0.7f, changeScoreWaitCallBack);
                    });
            }
            else if (curScoreStep == ScoreStep.End)
            {
                AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, scoreText.transform,
                    scoreText.transform.localScale, new Vector3(0.8f, 0.8f, 1), 0.3f, TweenUtils.VectorTweenLinear, 0, () =>
                    {
                        AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 0.7f, changeScoreWaitCallBack);
                    });
                AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, gameObject.transform,
                    gameObject.transform.localScale, Vector3.zero, 0.3f, TweenUtils.VectorTweenLinear, 1.7f, () =>
                    {
                        AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 2f, () =>
                        {
                            isPlayingAnim = false;
                            EndCallBack();
                            isCanDestroy = true;
                        });
                    });
            }
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
                    BeginSpecialDeclareStep();
                    isPlayingAnim = true;
                }
            }

            if (isPlayingAnim)
            {
                ChangeScore();
            }
        }

        public void EndCallBack()
        {
            callBack?.Invoke();
        }

        public void Destroy()
        {
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.position = new Vector3(10000, 10000, 0);
        }
    }
}
