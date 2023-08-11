using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishBossDeclareEffectItem
    {
        private float animTime = 4f;
        private Transform transform;
        private Text scoreText;
        private Action callBack;
        public EffectVo effectVo;
        private float currentTime;
        private int curScore;
        private bool isChangeScore;
        public bool isCanDestroy;
        public FishBossDeclareEffectItem(GameObject obj)
        {
            transform = obj.transform;
            InitView();
        }

        private void InitView()
        {
            scoreText = transform.Find("Num").GetComponent<Text>();

        }

        private void SetScoreText(string score)
        {
            scoreText.text = score;
        }

        public void ResetEffectVo(EffectVo vo)
        {
            effectVo = vo;
        }

        public void ResetState(Vector3 beginPos, Action callBack = null)
        {
            this.callBack = callBack;
            transform.localScale = Vector3.zero;
            transform.localPosition = beginPos;
            var lightObj = FishGameObjectPoolManager.Instance.GetGameObject("DeclareLight", PoolType.SpecialDeclarePool);
            lightObj.transform.localPosition = beginPos;
            lightObj.GetComponent<Animator>().Play("DeclareLight", 0, 0);
            SetScoreText("0");
            var playerIns = FishPlayerManager.Instance.GetPlayerInsByChairId(effectVo.chairId);
            Vector3 targetPos = playerIns.Panel.GetPlayerCatchFishPos(1);

            AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 0.5f, () =>
            {
                AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, transform, Vector3.zero, Vector3.one, 0.5f, TweenUtils.VectorTweenLinear, 0, () =>
                {
                    AsyncActionUtils.ApplyTextColor(FishSpecialDeclareEffectManager.Instance, scoreText, new Color(1, 1, 1, 0), Color.white, 0.5f, TweenUtils.ColorTweenInSine, 0, () =>
                    {
                        AsyncActionUtils.ApplyLocalMovement(FishSpecialDeclareEffectManager.Instance, transform, transform.localPosition, targetPos, animTime, TweenUtils.VectorTweenLinear);
                        isChangeScore = true;
                    });
                });
            });
            AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 1.27f, () =>
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(lightObj, PoolType.SpecialDeclarePool);
            });
        }

        private void ChangeScore()
        {
            if (currentTime < animTime - 0.2f && curScore < effectVo.score)
            {
                currentTime += Time.deltaTime;
                curScore = Mathf.CeilToInt(effectVo.score * (currentTime / animTime));
                if (curScore < effectVo.score)
                    SetScoreText(curScore.ToString());
                else
                    ChangeScoreEnd();
            }
        }

        private void ChangeScoreEnd()
        {
            curScore = effectVo.score;
            SetScoreText(curScore.ToString());
            Transform trans = scoreText.transform;
            AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, trans, Vector3.one, Vector3.one * 1.5f, 0.3f, TweenUtils.VectorTweenLinear, 0, () =>
            {
                AsyncActionUtils.ApplyScaling(FishSpecialDeclareEffectManager.Instance, trans, Vector3.one * 1.5f, Vector3.one, 0.3f, TweenUtils.VectorTweenLinear, 0, () =>
                {
                    AsyncActionUtils.DelayedAction(FishSpecialDeclareEffectManager.Instance, 1, () =>
                    {
                        isChangeScore = false;
                        callBack?.Invoke();
                        isCanDestroy = true;
                    });
                });
            });
        }

        public void Update()
        {
            if (isChangeScore)
            {
                if (currentTime < animTime - 0.2f)
                    ChangeScore();
                else if (curScore != effectVo.score)
                    ChangeScoreEnd();
            }
        }

        public void Destroy()
        {
            isChangeScore = false;
            isCanDestroy = false;
            callBack = null;
            transform.localPosition = new Vector3(10000, 10000, 0);
        }
    }
}
