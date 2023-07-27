using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode.EpicPass
{
    public class EpicPassV2ButtonIconController : MonoBehaviour
    {
        public float increaseEffectTimeMin = 0.5f;
        public float increaseEffectTimeMax = 4f;
        public float increaseMin = 0.5f;
        public float increaseMax = 2f;

        public float iconWaitTime = 3f;
        public float iconRewardWaitTime = 7f;

        public float levelUpEffectTime = 1.8f;

        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement iconAreaElement;

        private ContextElement seasonIconImageElement;

        private ContextElement pointSliderElement;

        private ContextElement expAreaElement;
        private ContextElement expTextElement;
        private ContextElement getPointTextElement;

        private List<GameObject> rewardIconObjList;

        private bool isInit = false;

        private Blackboard EpicPassInfoBB
        {
            get { return EpicPassUtilsV2.EpicPassInfo; }
        }

        private Coroutine flipEffectEnumerator = null;
        private Coroutine gettingEffectEnumerator = null;

        private UnityAction ownerRewardCallback;

        private float effectFromDelta = 0f;
        private float effectTargetDelta = 0f;
        private long effectExp = 0L;
        private long effectRequiredExp = 0L;
        private int iconRewardCount = 2;

        private void OnEnable()
        {
            UpdateRewardFlipEffect();
        }

        private void OnDisable()
        {
            if (flipEffectEnumerator != null)
                StopCoroutine(flipEffectEnumerator);
            if (gettingEffectEnumerator != null)
                StopCoroutine(gettingEffectEnumerator);
        }

        //

        public void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);

            seasonIconImageElement = ContextUtils.FindElement(iconAreaElement, "Epic Pass Always Point Web Image/Image", ContextSearchingType.FullNameSearch);

            pointSliderElement = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);

            expAreaElement = ContextUtils.FindElement(rootElement, "Count Base", ContextSearchingType.ChildrenSearch);
            expTextElement = ContextUtils.FindElement(expAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            getPointTextElement = ContextUtils.FindElement(rootElement, "Get Point Text", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void SetRewardCallback(UnityAction callback)
        {
            ownerRewardCallback = callback;
        }

        public void UpdateValues()
        {
            if (EpicPassInfoBB == null) return;

            UpdateExpGauge();
            UpdateSeasonPassIcon();
            UpdateReward();

            // Make Backup info.
            EpicPassUtilsV2.BackUpInfo();
        }

        public void GettingPoint()
        {
            if (!isInit) return;
            if (EpicPassUtilsV2.PrevLevel >= EpicPassUtilsV2.MaxLevel) return;

            long earnPoint = EpicPassUtilsV2.EarnPoint;

            if (earnPoint > 0)
            {
                PlayGettingPointEffect();

                if (gettingEffectEnumerator != null)
                    StopCoroutine(gettingEffectEnumerator);

                UpdateReward();
                MetaContextElementUtils.SetTextGlobal(getPointTextElement, "EPIC_PASS_GET_POINT_TEXT", earnPoint);

                effectFromDelta = (float)EpicPassUtilsV2.PrevPoint / (float)EpicPassUtilsV2.PrevRequiredPoint;

                int levelGap = EpicPassUtilsV2.Level - EpicPassUtilsV2.PrevLevel;

                if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
                {
                    effectTargetDelta = 1f;

                    if (levelGap > 0)
                        levelGap -= 1;
                }
                else
                {
                    effectTargetDelta = (float)EpicPassUtilsV2.Point / (float)EpicPassUtilsV2.RequiredPoint;
                }

                effectExp = EpicPassUtilsV2.PrevPoint;
                effectRequiredExp = EpicPassUtilsV2.PrevRequiredPoint;

                // effect increase time
                float totalProgressDelta = effectTargetDelta - effectFromDelta + (float)levelGap;

                gettingEffectEnumerator = StartCoroutine(MetaContextElementUtils.IncreaseProgressEffect(
                                    effectFromDelta,
                                    effectTargetDelta,
                                    levelGap,
                                    GetIncreaseEffectTime(totalProgressDelta),
                                    levelUpEffectTime,
                                    ProgressEffectUpdateCallback,
                                    PlayLevelUpEffect,
                                    GettingPointEnd
                ));
            }
        }

        public void GettingPointEnd()
        {
            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
            {
                UpdateExpGauge();
            }
            else
            {
                long point = EpicPassUtilsV2.Point;
                long required = EpicPassUtilsV2.RequiredPoint;
                UpdateExpTextOnly(point, required);
            }

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_FINISH_GETTING_META_GAME_ITEM);

            if (ownerRewardCallback != null)
                ownerRewardCallback.Invoke();
        }

        public void SetLocked(bool isLocked)
        {
            if (!isInit) return;

            rootAnimator.SetBool("isLocked", isLocked);
        }

        // from owner button controller
        public void UpdateExpGauge()
        {
            if (EpicPassInfoBB == null) return;
            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
            {
                MetaContextElementUtils.SetFloatProperty(pointSliderElement, 1f);
                MetaContextElementUtils.SetTextGlobal(expTextElement, "EPIC_PASS_BUTTON_EXP_MAX_TEXT");
            }
            else
            {
                long point = EpicPassUtilsV2.Point;
                long required = EpicPassUtilsV2.RequiredPoint;

                float expRatio = (float)point / (float)required;

                MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);
                UpdateExpTextOnly(point, required);
            }
        }

        // Effect use only.
        private void UpdateExpGaugeEffect(long exp, long requiredExp)
        {
            if (EpicPassInfoBB == null) return;

            float expRatio = (float)exp / (float)requiredExp;

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);
            UpdateExpTextOnly(exp, requiredExp);
        }

        private void UpdateExpTextOnly(long exp, long required)
        {
            if (EpicPassUtilsV2.Level >= EpicPassUtilsV2.MaxLevel)
                MetaContextElementUtils.SetTextGlobal(expTextElement, "EPIC_PASS_BUTTON_EXP_MAX_TEXT");
            else
            {
                double expPercent = exp < required ? ((double)exp / (double)required) * 100.0 : 100.0;
                MetaContextElementUtils.SetTextGlobal(expTextElement, "EPIC_PASS_BUTTON_EXP_PERCENT_TEXT", expPercent);
            }
        }

        private void UpdateSeasonPassIcon()
        {
            if (EpicPassInfoBB == null) return;

            string iconImageURL = EpicPassUtilsV2.TabIconImageUrl;

            if (!string.IsNullOrEmpty(iconImageURL))
            {
                MetaContextElementUtils.SetWebImage(seasonIconImageElement, iconImageURL);
            }
        }

        private void UpdateReward()
        {
            if (EpicPassInfoBB == null) return;

            iconRewardCount = 2;

            var nextRewardBB = EpicPassInfoBB.GetVariable<List<Blackboard>>("nextRewardList");

            if (nextRewardBB == null || nextRewardBB.value == null || nextRewardBB.value.Count == 0)
            {
                if (rewardIconObjList != null)
                {
                    for (int i = 0; i < rewardIconObjList.Count; ++i)
                        Destroy(rewardIconObjList[i]);
                    rewardIconObjList.Clear();
                }
                iconRewardCount = 1;
            }
            else
            {
                iconRewardCount = nextRewardBB.value.Count + 1;
                if (rewardIconObjList == null)
                    rewardIconObjList = new List<GameObject>();

                for (int i = 0; i < nextRewardBB.value.Count; ++i)
                {
                    var rewardType = nextRewardBB.value[i].GetValue<RewardType>("type");

                    GameObject rewardIconObj = rewardIconObjList.Count > i ? rewardIconObjList[i] :
                        MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Button Icon Reward Epic Pass Always", iconAreaElement.transform, null, string.Format("Reward Icon {0}", i));

                    EpicPassV2ButtonIconRewardController controller = rewardIconObj?.GetComponent<EpicPassV2ButtonIconRewardController>() ?? null;

                    if (rewardIconObj != null && controller != null)
                    {
                        if (rewardIconObjList.Count > i)
                            rewardIconObjList[i] = rewardIconObj;
                        else
                            rewardIconObjList.Add(rewardIconObj);
                        controller.OnInit(nextRewardBB.value[i]);
                    }
                    else
                    {
                        if (rewardIconObjList.Contains(rewardIconObj))
                            rewardIconObjList.Remove(rewardIconObj);
                        Destroy(rewardIconObj);
                        continue;
                    }
                }
                UpdateRewardFlipEffect();
            }
        }

        private void UpdateRewardFlipEffect()
        {
            if (rewardIconObjList == null)
            {
                if (flipEffectEnumerator != null)
                    StopCoroutine(flipEffectEnumerator);
            }
            else
            {
                if (flipEffectEnumerator != null)
                {
                    StopCoroutine(flipEffectEnumerator);
                    flipEffectEnumerator = null;
                }

                if (flipEffectEnumerator == null)
                    flipEffectEnumerator = StartCoroutine(FlipEffect());
            }
        }

        private IEnumerator FlipEffect()
        {
            int viewIndex = 0;
            float[] waitTime = { iconWaitTime, iconRewardWaitTime, iconRewardWaitTime };

            seasonIconImageElement.gameObject.SetActive(viewIndex == 0);
            SetRewardIconActive(viewIndex);

            while (true)
            {
                if (rewardIconObjList == null)
                    break;

                yield return new WaitForSeconds(waitTime[viewIndex]);
                viewIndex++;
                viewIndex %= iconRewardCount;

                rootAnimator.SetTrigger("isFlip");
                yield return new WaitForSeconds(0.2f);

                seasonIconImageElement.gameObject.SetActive(viewIndex == 0);
                SetRewardIconActive(viewIndex);
            }

            seasonIconImageElement.gameObject.SetActive(true);
            SetRewardIconActive(0);
        }

        private void SetRewardIconActive(int viewIndex)
        {
            if (rewardIconObjList != null)
            {
                for (int i = 0; i < rewardIconObjList.Count; ++i)
                    rewardIconObjList[i].SetActive(viewIndex == i + 1);
            }
        }

        private void PlayGettingPointEffect()
        {
            GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_GET_POINT).Play();

            rootAnimator.SetBool("isGetPoint", true);
        }

        private void PlayLevelUpEffect(int level)
        {
            GSManager.Instance.GetHandler(EpicPassUtilsV2.Sounds.EPIC_PASS_LEVEL_UP).Play();

            rootAnimator.SetBool("isLevelUp", true);

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, 1f);
            UpdateExpGaugeEffect(effectRequiredExp, effectRequiredExp);

            // Update Effect Exp Required
            List<long> requiredPointList = EpicPassUtilsV2.SkippedRequiredPointMaxList;
            if (requiredPointList != null && requiredPointList.Count > level - 1)
            {
                effectRequiredExp = requiredPointList[level - 1];
            }
            else
            {
                effectRequiredExp = EpicPassUtilsV2.RequiredPoint;
            }

            effectExp = 0;
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