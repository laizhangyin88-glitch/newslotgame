using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishSpiderCrabBossHurt
    {
        GameObject gameObject;
        string[] animParms;
        float changeScoreCurrentTime = 0f;
        float changeScoreTotalTime = 1.5f;
        float currentTime = 0;
        float delayTime = 0;
        int changeScoreInitScore = 0;
        int changeScoreTempScore = 0;
        public bool isCanDestroy;
        bool isPlayingAnim;
        bool isDelayPlay;
        bool isChangeScore;
        Vector3 targetPos;
        Vector3 beginPos;
        Animator animator;
        Text scoreText;
        GameObject firstKill_Image_gameObject;
        GameObject sencondKill_Image_gameObject;
        public EffectVo effectVo;
        Action callBack;
        public FishSpiderCrabBossHurt(GameObject obj)
        {
            gameObject = obj;
            FindView();
            animParms = new string[] { "firstbosshurt_start_chs", "firstbosshurt_count_chs", "firstbosshurt_off_chs" };
        }

        private void FindView()
        {
            animator = gameObject.transform.Find("Content").GetComponent<Animator>();
            scoreText = gameObject.transform.Find("Content/number").GetComponent<Text>();
            firstKill_Image_gameObject = gameObject.transform.Find("Content/firstkill").gameObject;
            sencondKill_Image_gameObject = gameObject.transform.Find("Content/secondkill").gameObject;
        }

        public void SetShowScoreText(string score)
        {
            scoreText.text = score;
        }

        public void PlayAnim(int index)
        {
            animator.Play(animParms[index], 0, 0);
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
            }
            else
                gameObject.transform.position = targetPos;

            gameObject.transform.localScale = Vector3.one;
            currentTime = 0;
            delayTime = 0;
            this.callBack = callBack;
            isDelayPlay = true;
            isChangeScore = false;
            changeScoreCurrentTime = 0;

            if (effectVo.hurtNum == 1)
            {
                firstKill_Image_gameObject.SetActive(true);
                sencondKill_Image_gameObject.SetActive(false);
            }
            else
            {
                firstKill_Image_gameObject.SetActive(false);
                sencondKill_Image_gameObject.SetActive(true);
            }
            SetShowScoreText("0");
        }

        public void BeginSpiderCrabBosshurtStep()
        {
            if (effectVo.isMe)
            {
                MySelfOneStepCrabBossHurt();
            }
            else
                OtherOneStepCrabBossHurt();
        }

        public void MySelfOneStepCrabBossHurt()
        {
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1, () => { MySelfTwoStepCrabBossHurt(); });
            PlayAnim(0);
        }

        public void MySelfTwoStepCrabBossHurt()
        {
            float moveDuration = 0.25f;
            float twoStepDuration = 0.75f;
            AsyncActionUtils.ApplyMovement(FishSpiderCrabEffectManager.Instance, gameObject.transform, gameObject.transform.position, targetPos, moveDuration, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.ApplyScaling(FishSpiderCrabEffectManager.Instance, gameObject.transform, gameObject.transform.localScale, Vector3.one * 0.75f, moveDuration, TweenUtils.VectorTweenLinear);
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, twoStepDuration, () => {
                if (effectVo.isMe)
                {
                    FishAudioManager.Instance.StopNormalAudio(63);
                    FishAudioManager.Instance.PlayNormalAudio(63, 0.4f, true);
                }
                SetChangeScore();
            });
            PlayAnim(1);
        }

        public void OtherOneStepCrabBossHurt()
        {
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1, () =>
            {
                PlayAnim(1);
            });
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1.5f, () =>
            {
                SetChangeScore();
            });
            PlayAnim(0);
        }

        public void SetChangeScore()
        {
            changeScoreCurrentTime = 0;
            changeScoreInitScore = 0;
            isChangeScore = true;
            changeScoreInitScore = 0;
            changeScoreTempScore = effectVo.hurtScore;
            SetShowScoreText("0");
        }

        public void ChangeScore()
        {
            if (isChangeScore)
            {
                changeScoreCurrentTime += Time.deltaTime;
                if (changeScoreCurrentTime <= changeScoreTotalTime - 0.2f)
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

            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 0.8f, () =>
            {
                PlayAnim(2);
            });
            AsyncActionUtils.DelayedAction(FishSpiderCrabEffectManager.Instance, 1.3f, () =>
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
                    BeginSpiderCrabBosshurtStep();
                    isPlayingAnim = true;
                }
            }
            if (isPlayingAnim)
                ChangeScore();
        }

        public void Destroy()
        {
            if (effectVo.isMe)
                FishAudioManager.Instance.StopNormalAudio(63);
            isPlayingAnim = false;
            isDelayPlay = false;
            isCanDestroy = false;
            callBack = null;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }
    }

}
