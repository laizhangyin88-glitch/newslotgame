using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishSpiderCrabKillInfo
    {
        enum ScoreStep
        {
            OneStep = 1,
            TwoStep = 2,
            ThreeStep = 3,
            End = 4,
        }

        ScoreStep targetChangeScoreStep = ScoreStep.OneStep;
        ScoreStep curScoreStep = ScoreStep.TwoStep;
        string[][] myAnimParms;
        string[][] otherAnimParms;
        int changeScoreInitScore = 0;
        int changeScoreTempScore = 0;
        float changeScoreCurrentTime = 0;
        float changeScoreTotalTime = 1;
        public bool isCanDestroy;
        bool isPlayingAnim;
        bool isDelayPlay;
        bool isChangeScore;
        float delayTime = 0f;
        float currentTime = 0f;
        Vector3 targetPos;
        Vector3 beginPos;
        GameObject gameObject;
        Transform transform;
        Animator animator;
        Text scoreText;
        GameObject twoMultiple_Sprite_gameObject;
        GameObject threeMultiple_Sprite_gameObject;
        EffectVo effectVo;
        Action callBack;

        public FishSpiderCrabKillInfo(GameObject obj)
        {
            gameObject = obj;
            transform = gameObject.transform;
            myAnimParms = new string[][] {
                new string[]{ "My_OneStep_01", "My_TwoStep_01", "My_ThreeStep_01", "My_FourStep_01", "My_FiveStep_01" },
                new string[]{ "My_OneStep_02", "My_TwoStep_02", "My_ThreeStep_02", "My_FourStep_02", "My_FiveStep_02" }
            };
            otherAnimParms = new string[][]
            {
                new string[]{ "Other_OneStep_01","My_ThreeStep_01","My_ThreeStep_01","My_FourStep_01","My_FiveStep_01" },
                new string[]{ "Other_OneStep_02", "My_ThreeStep_02", "My_ThreeStep_02", "My_FourStep_02", "My_FiveStep_02" }
            };
            FindView();
        }

        private void FindView()
        {
            animator = transform.Find("Object").GetComponent<Animator>();
            scoreText = transform.Find("Object/Foreground/num").GetComponent<Text>();
        }

        public void SetShowScoreText(string score)
        {
            scoreText.text = score;
        }

        public void PlayAnim(int type, int index)
        {
            if (effectVo.isMe)
                animator.Play(myAnimParms[type][index], 0, 0);
            else
                animator.Play(otherAnimParms[type][index], 0, 0);
        }

        public void ResetEffectVo(EffectVo vo)
        {
            effectVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isDelayPlay = false;
            isChangeScore = false;
        }

        public void ResetState(Vector3 targetPos, Action callBack)
        {
            isCanDestroy = false;
            this.targetPos = targetPos;
            if (effectVo.isMe)
            {
                gameObject.transform.localPosition = new Vector3(1200, 0, 0);
            }
            else
            {
                gameObject.transform.position = targetPos;
                
            }
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
            AsyncActionUtils.ApplyLocalMovement(FishSpiderCrabEffectManager.Instance, transform,
                transform.localPosition, Vector3.zero, 0.4f, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1.45f, MySelfTwoStepSpecialDeclare);
            PlayAnim(effectVo.AnimType, 0);
        }

        public void MySelfTwoStepSpecialDeclare()
        {
            AsyncActionUtils.ApplyMovement(FishSpiderCrabEffectManager.Instance, transform,
               transform.position, targetPos, 0.25f, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1.45f, () =>
            {
                PlayAnim(effectVo.AnimType, 2);
            });
            PlayAnim(effectVo.AnimType, 1);
        }

        public void OtherOneStepSpecialDeclare()
        {
            AsyncActionUtils.ApplyMovement(FishSpiderCrabEffectManager.Instance, transform,
               transform.position, targetPos, 0.4f, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1.45f, () =>
            {
                PlayAnim(effectVo.AnimType, 2);
            });
            PlayAnim(effectVo.AnimType, 0);
        }

        public void ComFourStepSpecialDeclare()
        {
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 0.35f, ComFiveStepSpecialDeclare);
            PlayAnim(effectVo.AnimType, 3);
        }

        public void ComFiveStepSpecialDeclare()
        {
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
            changeScoreInitScore = 0;
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
                        int result = changeScoreInitScore + Mathf.CeilToInt(changeScoreTempScore * (changeScoreCurrentTime / changeScoreCurrentTime / changeScoreTotalTime));
                        SetShowScoreText(result.ToString());
                    }
                    else
                    {
                        isChangeScore = false;
                        changeScoreCurrentTime = 0;
                        curScoreStep = ScoreStep.TwoStep;
                        changeScoreInitScore = changeScoreTempScore;
                        SetShowScoreText(changeScoreTempScore.ToString());
                        ChangeScoreEnd();
                    }
                }
                else if (curScoreStep == ScoreStep.TwoStep)
                {
                    changeScoreCurrentTime += Time.deltaTime;
                    if (changeScoreCurrentTime <= (changeScoreTotalTime - 0.4f))
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
                            changeScoreInitScore = changeScoreTempScore + changeScoreTempScore;
                        }
                        SetShowScoreText(changeScoreInitScore.ToString());
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
                        ChangeScoreEnd();
                    }
                }
            }
        }

        public void ChangeScoreEnd()
        {
            scoreText.transform.localScale = Vector3.one * 1.5f;
            if (curScoreStep == ScoreStep.TwoStep)
            {
                if (targetChangeScoreStep == ScoreStep.TwoStep)
                {
                    changeScoreTempScore = effectVo.score - changeScoreInitScore;
                }
                else if (targetChangeScoreStep == ScoreStep.ThreeStep)
                {
                    changeScoreTempScore = Mathf.CeilToInt((effectVo.score - changeScoreInitScore) * UnityEngine.Random.Range(0.4f, 0.6f));
                }
                AsyncActionUtils.ApplyScaling(FishSpiderCrabEffectManager.Instance, scoreText.transform,
                    scoreText.transform.localScale, Vector3.one * 0.8f, 0.3f, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 0.7f, () => { isChangeScore = true; });
            }
            else if (curScoreStep == ScoreStep.ThreeStep)
            {
                changeScoreTempScore = effectVo.score - changeScoreInitScore;
                AsyncActionUtils.ApplyScaling(FishSpiderCrabEffectManager.Instance, scoreText.transform,
                    scoreText.transform.localScale, Vector3.one * 0.8f, 0.3f, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 0.7f, () => { isChangeScore = true; });
            }
            else if (curScoreStep == ScoreStep.End)
            {
                AsyncActionUtils.ApplyScaling(FishSpiderCrabEffectManager.Instance, scoreText.transform,
                    scoreText.transform.localScale, Vector3.one * 0.8f, 0.3f, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.ApplyScaling(FishSpiderCrabEffectManager.Instance, transform,
                    transform.localScale, Vector3.zero, 1.7f, TweenUtils.VectorTweenLinear);
                AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 2f, () => {
                    isPlayingAnim = false;
                    EndCallBack();
                    isCanDestroy = true;
                });
            }
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
                    BeginSpecialDeclareStep();
                    isPlayingAnim = true;
                }
            }
            if (isPlayingAnim)
            {
                ChangeScore();
            }
        }

        public void Destroy()
        {
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }
    }
}
