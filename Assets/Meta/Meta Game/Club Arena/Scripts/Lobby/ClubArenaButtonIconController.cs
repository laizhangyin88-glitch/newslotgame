using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace BagelCode.ClubArena
{
    public class ClubArenaButtonIconController : MonoBehaviour
    {
        public float increaseEffectTimeMin = 0.5f;
        public float increaseEffectTimeMax = 4f;
        public float increaseMin = 0.5f;
        public float increaseMax = 2f;

        public float iconWaitTime = 3f;
        public float iconRewardWaitTime = 7f;

        public float levelUpEffectTime = 0.0f;

        public bool isClubber
        {
            get { return ClubArenaUtils.IsClubber(); }
        }

        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement pointSliderElement;

        private ContextElement expAreaElement;
        private ContextElement expTextElement;

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

            //isClubber = ClubArenaUtils.IsClubber();

            UpdateExpGauge();

            // Make Backup info. 
            ClubArenaUtils.BackUpInfo();
        }

        public void GettingPoint(long backupEarnEnergy)
        {
            if (!isInit) return;

            if (backupEarnEnergy > 0)
            {
                PlayGettingPointEffect();

                if (gettingEffectEnumerator != null)
                    StopCoroutine(gettingEffectEnumerator);

                effectFromDelta = (float)ClubArenaUtils.PrevCurrentEnergy / (float)ClubArenaUtils.CurrentEnergy;

                // max check?
                effectTargetDelta = 1.0f;

                int overflow = 0;

                effectExp = ClubArenaUtils.PrevCurrentEnergy;
                effectRequiredExp = ClubArenaUtils.CurrentEnergy;

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
            UpdateExpTextOnly(ClubArenaUtils.CurrentEnergy);

            ClubArenaUtils.BackUpInfo();

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

            long energy = ClubArenaUtils.Energy;
            long required = ClubArenaUtils.RequiredEnergy;

            float expRatio = (float)energy / (float)required;

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);
            UpdateExpTextOnly(ClubArenaUtils.CurrentEnergy);
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
            MetaContextElementUtils.SetTextGlobal(expTextElement, "CLUB_ARENA_BUTTON_EXP_TEXT", exp);
        }

        private void PlayGettingPointEffect()
        {
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_ENERGY_GET).Play();

            rootAnimator.SetTrigger("isGetPoint");
        }

        private void PlayLevelUpEffect(int level)
        {
            //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_GAUGE).Play();

            //rootAnimator.SetBool("isLevelUp", true);

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, 1f);
            UpdateExpGaugeEffect(effectRequiredExp, effectRequiredExp);

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