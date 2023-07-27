using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersButtonIconController : MonoBehaviour
    {
        public float increaseEffectTimeMin = 0.5f;
        public float increaseEffectTimeMax = 4f;
        public float increaseMin = 0.5f;
        public float increaseMax = 2f;

        public float iconWaitTime = 3f;
        public float iconRewardWaitTime = 7f;

        public float levelUpEffectTime = 0.0f;

        public bool isClubber
        { get { return BossRaidersUtils.IsClubber(); } }

        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement pointSliderElement;

        private ContextElement expAreaElement;
        private ContextElement expTextElement;
        //private ContextElement getPointTextElement;

        private bool isInit = false;

        private Blackboard _metaEnterInfoBB;
        private Blackboard MetaEnterInfoBB
        {
            get
            {
                if (_metaEnterInfoBB == null)
                    _metaEnterInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
                return _metaEnterInfoBB;
            }
        }

        private Coroutine gettingEffectEnumerator = null;

        private UnityAction ownerRewardCallback;

        private float effectFromDelta = 0f;
        private float effectTargetDelta = 0f;
        private long effectExp = 0L;
        private long effectRequiredExp = 0L;

        private void OnDisable()
        {
            if (gettingEffectEnumerator != null)
                StopCoroutine(gettingEffectEnumerator);
        }

        public void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            pointSliderElement = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            //getPointTextElement = ContextUtils.FindElement(rootElement, "Get Point Text", ContextSearchingType.ChildrenSearch);

            expAreaElement = ContextUtils.FindElement(rootElement, "Count Base", ContextSearchingType.ChildrenSearch);
            expTextElement = ContextUtils.FindElement(expAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void SetRewardCallback(UnityAction callback)
        {
            ownerRewardCallback = callback;
        }

        public void UpdateValues()
        {
            if (MetaEnterInfoBB == null) return;

            //UpdateClubber();

            UpdateExpGauge();

            // Make Backup info.
            BossRaidersUtils.BackUpInfo();
        }

        //public void UpdateClubber()
        //{
        //    isClubber = ClubUtils.IsClubber();
        //}

        public void GettingPoint(long backupEarnEnergy)
        {
            if (!isInit || !isActiveAndEnabled) return;

            if (backupEarnEnergy > 0L)
            {
                PlayGettingPointEffect();

                if (gettingEffectEnumerator != null)
                    StopCoroutine(gettingEffectEnumerator);

                //MetaContextElementUtils.SetTextGlobal(getPointTextElement, "BOSS_RAIDERS_GET_ENERGY_TEXT", earnEnergy);

                effectFromDelta = (float)BossRaidersUtils.PrevCurrentEnergy / (float)BossRaidersUtils.CurrentEnergy;

                // max check?
                effectTargetDelta = 1.0f;   // (float)BossRaidersUtils.CurrentEnergy;

                int overflow = 0; // Mathf.Clamp(BossRaidersUtils.SpinPossibleCount - BossRaidersUtils.PrevSpinPossibleCount, 0, 1);

                effectExp = BossRaidersUtils.PrevCurrentEnergy;
                effectRequiredExp = BossRaidersUtils.CurrentEnergy;

                float totalProgressDelta = effectTargetDelta - effectFromDelta + (float)overflow;

                gettingEffectEnumerator = StartCoroutine(MetaContextElementUtils.IncreaseProgressEffect(
                    effectFromDelta,
                    effectTargetDelta,
                    overflow,
                    GetIncreaseEffectTime(totalProgressDelta),
                    levelUpEffectTime,
                    ProgressEffectUpdateCallback,
                    PlayLevelUpEffect,
                    GettingPointEnd));
            }
        }

        public void GettingPointEnd()
        {
            UpdateExpTextOnly(BossRaidersUtils.CurrentEnergy);

            BossRaidersUtils.BackUpInfo();

            if (ownerRewardCallback != null)
                ownerRewardCallback.Invoke();
        }

        public void SetLocked(bool isLocked)
        {
            if (!isInit) return;
            if (!isClubber) isLocked = true;
            rootAnimator.SetBool("isLocked", isLocked);
        }

        public void UpdateExpGauge()
        {
            if (MetaEnterInfoBB == null) return;

            long energy = BossRaidersUtils.Energy;
            long required = BossRaidersUtils.RequiredEnergy;

            float expRatio = (float)energy / (float)required;

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);
            UpdateExpTextOnly(BossRaidersUtils.CurrentEnergy);
        }

        private void UpdateExpGaugeEffect(long exp, long requiredExp)
        {
            if (MetaEnterInfoBB == null) return;

            float expRatio = (float)exp / (float)requiredExp;

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);

            UpdateExpTextOnly(exp);
        }

        private void UpdateExpTextOnly(long exp)
        {
            MetaContextElementUtils.SetTextGlobal(expTextElement, "BOSS_RAIDERS_BUTTON_EXP_TEXT", exp);
        }

        private void PlayGettingPointEffect()
        {
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_POINTS_GET).Play();

            rootAnimator.SetTrigger("isGetPoint");
        }

        private void PlayLevelUpEffect(int level)
        {
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_GAUGE).Play();

            //rootAnimator.SetBool("isLevelUp", true);

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, 1f);
            UpdateExpGaugeEffect(effectRequiredExp, effectRequiredExp);

            // Update Effect Exp Required
            //effectRequiredExp = BossRaidersUtils.RequiredEnergy;
            //effectExp = 0;

            if (ownerRewardCallback != null)
                ownerRewardCallback.Invoke();
        }

        private void ProgressEffectUpdateCallback(float totalProgress)
        {
            float delta = (effectFromDelta + totalProgress) % 1f;
            MetaContextElementUtils.SetFloatProperty(pointSliderElement, delta);

            // Get Effect Exp.
            long exp = 0L;
            if (effectExp > 0)
            {
                float totalProgressDelta = totalProgress % 1f;
                exp = (effectRequiredExp * (long)(totalProgressDelta * 10000f)) / 10000L;
                exp += effectExp;
            }
            else
            {
                exp = (effectRequiredExp * (long)(delta * 10000f)) / 10000L;
            }

            UpdateExpGaugeEffect(exp, effectRequiredExp);
        }

        private float GetIncreaseEffectTime(float magnitude)
        {
            if (magnitude <= increaseMin)
                return increaseEffectTimeMin;
            if (increaseEffectTimeMax <= magnitude)
                return increaseEffectTimeMax;

            return Mathf.Lerp(increaseEffectTimeMin, increaseEffectTimeMax, (magnitude - increaseMin) / increaseMax);
        }
    }
}
