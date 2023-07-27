using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace BagelCode.EpicPass
{
    public class EpicPassButtonIconController : MonoBehaviour
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

        private GameObject rewardIconObj;
        private ClientModels.RewardType prevRewardType;

        private bool isInit = false;

        private Blackboard _metaEnterInfoBB;
        private Blackboard MetaEnterInfoBB
        {
            get
            {
                if(_metaEnterInfoBB == null)
                    _metaEnterInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
                return _metaEnterInfoBB;
            }
        }

        private const string EP_COMMON_BUNDLE = "mgepicpasscommon";
        private const string EP_REWARD_COIN = "Epic Pass Reward Coin";
        private const string EP_REWARD_GEM = "Epic Pass Reward Gem";
        private const string EP_REWARD_GIFT = "Epic Pass Reward Gift";
        private const string EP_REWARD_VIP_LOUNGE_TICKET = "Epic Pass Reward VIP Lounge Ticket";

        private Coroutine flipEffectEnumerator = null;
        private Coroutine gettingEffectEnumerator = null;

        private UnityAction ownerRewardCallback;

        private float effectFromDelta = 0f;
        private float effectTargetDelta = 0f;
        private long effectExp = 0L;
        private long effectRequiredExp = 0L;

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
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);

            seasonIconImageElement = ContextUtils.FindElement(iconAreaElement, "Epic Pass Point Web Image/Image", ContextSearchingType.FullNameSearch);

            pointSliderElement = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);getPointTextElement = ContextUtils.FindElement(rootElement, "Get Point Text", ContextSearchingType.ChildrenSearch);

            expAreaElement = ContextUtils.FindElement(rootElement, "Count Base", ContextSearchingType.ChildrenSearch);
            expTextElement = ContextUtils.FindElement(expAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            prevRewardType = ClientModels.RewardType.UNKNOWN;

            isInit = true;
        }

        public void SetRewardCallback(UnityAction callback)
        {
            ownerRewardCallback = callback;
        }

        public void UpdateValues()
        {
            if(MetaEnterInfoBB == null) return;

            UpdateExpGauge();
            UpdateSeasonPassIcon();
            UpdateReward();

            // Make Backup info.
            EpicPassUtils.BackUpInfo();
        }

        public void GettingPoint()
        {
            if (!isInit) return;
            if(EpicPassUtils.PrevLevel >= EpicPassUtils.MaxLevel) return;

            long earnPoint = EpicPassUtils.EarnPoint;

            if(earnPoint > 0)
            {
                PlayGettingPointEffect();

                if (gettingEffectEnumerator != null)
                    StopCoroutine(gettingEffectEnumerator);

                UpdateReward();
                MetaContextElementUtils.SetTextGlobal(getPointTextElement, "EPIC_PASS_GET_POINT_TEXT", earnPoint);

                effectFromDelta = (float)EpicPassUtils.PrevPoint / (float)EpicPassUtils.PrevRequiredPoint;

                int levelGap = EpicPassUtils.Level - EpicPassUtils.PrevLevel;

                if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
                {
                    effectTargetDelta = 1f;

                    if(levelGap > 0)
                        levelGap -= 1;
                }
                else
                {
                    effectTargetDelta = (float)EpicPassUtils.Point / (float)EpicPassUtils.RequiredPoint;
                }

                effectExp = EpicPassUtils.PrevPoint;
                effectRequiredExp = EpicPassUtils.PrevRequiredPoint;

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
            // Bug.. don't update exp gauge.
            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
            {
                UpdateExpGauge();
            }
            else
            {
                long point = EpicPassUtils.Point;
                long required = EpicPassUtils.RequiredPoint;
                UpdateExpTextOnly(point, required);
            }

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_FINISH_GETTING_META_GAME_ITEM);

            if(ownerRewardCallback != null)
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
            if (MetaEnterInfoBB == null) return;
            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
            {
                MetaContextElementUtils.SetFloatProperty(pointSliderElement, 1f);
                MetaContextElementUtils.SetTextGlobal(expTextElement, "EPIC_PASS_BUTTON_EXP_MAX_TEXT");
            }
            else
            {
                long point = EpicPassUtils.Point;
                long required = EpicPassUtils.RequiredPoint;

                float expRatio = (float)point / (float)required;

                MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);
                UpdateExpTextOnly(point, required);
            }
        }

        // Effect use only.
        private void UpdateExpGaugeEffect(long exp, long requiredExp)
        {
            if (MetaEnterInfoBB == null) return;

            float expRatio = (float)exp / (float)requiredExp;

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, expRatio);
            UpdateExpTextOnly(exp, requiredExp);
        }

        private void UpdateExpTextOnly(long exp, long required)
        {
            if(EpicPassUtils.Level >= EpicPassUtils.MaxLevel)
                MetaContextElementUtils.SetTextGlobal(expTextElement, "EPIC_PASS_BUTTON_EXP_MAX_TEXT");
            else
            {
                double expPercent = exp < required ? ((double)exp / (double)required) * 100.0 : 100.0;
                MetaContextElementUtils.SetTextGlobal(expTextElement, "EPIC_PASS_BUTTON_EXP_PERCENT_TEXT", expPercent);
            }
        }

        //

        private void UpdateSeasonPassIcon()
        {
            if(MetaEnterInfoBB == null) return;

            string pointIconImageURL = BlackboardUtils.GetOrCreateVariable<string>(MetaEnterInfoBB, "pointIconImageUrl").value;

            if(!string.IsNullOrEmpty(pointIconImageURL))
            {
                MetaContextElementUtils.SetWebImage(seasonIconImageElement, pointIconImageURL);
            }
        }

        private void UpdateReward()
        {
            if(MetaEnterInfoBB == null) return;
            // iconAreaElement.transform

            var nextRewardBB = MetaEnterInfoBB.GetVariable<Blackboard>("nextReward");

            if(nextRewardBB == null || nextRewardBB.value == null)
            {
                // if(rewardIconObj)
                //     GameObject.Destroy(rewardIconObj);
            }
            else
            {
                var rewardType = nextRewardBB.value.GetValue<ClientModels.RewardType>("type");

                if(rewardType != prevRewardType && rewardIconObj)
                {
                    GameObject.Destroy(rewardIconObj);
                    rewardIconObj = null;
                }

                prevRewardType = rewardType;

                string rewardText = "";

                switch(rewardType)
                {
                    case ClientModels.RewardType.CREDIT:
                    case ClientModels.RewardType.CREDIT_WITH_MULTIPLIER:
                        {
                            if(rewardIconObj == null)
                                rewardIconObj = MetaObjectUtils.MakePrefab(EP_COMMON_BUNDLE, EP_REWARD_COIN, iconAreaElement.transform, null, "Reward Icon");

                            var credit = BlackboardUtils.FindVariable<long>(nextRewardBB.value, "credit");
                            rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_BUTTON_REWARD_COIN_TEXT", credit.value);
                        }
                        break;
                    case ClientModels.RewardType.GEM:
                        {
                            if(rewardIconObj == null)
                                rewardIconObj = MetaObjectUtils.MakePrefab(EP_COMMON_BUNDLE, EP_REWARD_GEM, iconAreaElement.transform, null, "Reward Icon");

                            var gem = BlackboardUtils.FindVariable<long>(nextRewardBB.value, "gem");
                            rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_BUTTON_REWARD_GEM_TEXT", gem.value);
                        }
                        break;
                    case ClientModels.RewardType.RP:
                    case ClientModels.RewardType.DAILY_BONUS_WHEEL_SPIN:
                    case ClientModels.RewardType.GAME_SPIN:
                    case ClientModels.RewardType.GAME_DEAL:
                    case ClientModels.RewardType.GAME_PLAY:
                    case ClientModels.RewardType.RANDOM:
                    case ClientModels.RewardType.SCRATCHER:
                    case ClientModels.RewardType.SCRATCHER_FOR_INBOX:
                    case ClientModels.RewardType.DAILY_DELIVERY:
                        {
                            if(rewardIconObj == null)
                                rewardIconObj = MetaObjectUtils.MakePrefab(EP_COMMON_BUNDLE, EP_REWARD_GIFT, iconAreaElement.transform, null, "Reward Icon");
                        }
                        break;
                    case ClientModels.RewardType.VIP_LOUNGE_OPEN_TICKET:
                        {
                            if (rewardIconObj == null)
                                rewardIconObj = MetaObjectUtils.MakePrefab(EP_COMMON_BUNDLE, EP_REWARD_VIP_LOUNGE_TICKET, iconAreaElement.transform, null, "Reward Icon");

                            var openDays = BlackboardUtils.FindVariable<int>(nextRewardBB.value, "openDays");
                            rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_REWARD_VIP_LOUNGE_TICKET_TEXT", openDays.value);
                        }
                        break;
                }

                if(rewardIconObj != null)
                {
                    var rewardElement = rewardIconObj.GetComponent<ContextElement>();
                    if(rewardElement != null)
                    {
                        rewardElement.UpdateContext();
                        var textElement = ContextUtils.FindElement(rewardElement, "Text", ContextSearchingType.ChildrenSearch);
                        MetaContextElementUtils.SetText(textElement, rewardText);
                    }

                    UpdateRewardFlipEffect();
                }
            }
        }

        private void UpdateRewardFlipEffect()
        {
            if(rewardIconObj == null)
            {
                if(flipEffectEnumerator != null)
                    StopCoroutine(flipEffectEnumerator);
            }
            else
            {
                if(flipEffectEnumerator != null)
                {
                    StopCoroutine(flipEffectEnumerator);
                    flipEffectEnumerator = null;
                }

                if(flipEffectEnumerator == null)
                    flipEffectEnumerator = StartCoroutine(FlipEffect());
            }
        }

        private IEnumerator FlipEffect()
        {
            int viewIndex = 0;
            float[] waitTime = {iconWaitTime, iconRewardWaitTime};

            seasonIconImageElement.gameObject.SetActive(viewIndex == 0);
            SetRewardIconActive(viewIndex == 1);

            while(true)
            {
                if(rewardIconObj == null)
                    break;

                yield return new WaitForSeconds(waitTime[viewIndex]);
                viewIndex++;
                viewIndex%=2;

                rootAnimator.SetTrigger("isFlip");
                yield return new WaitForSeconds(0.2f);

                seasonIconImageElement.gameObject.SetActive(viewIndex == 0);
                SetRewardIconActive(viewIndex == 1);
            }

            seasonIconImageElement.gameObject.SetActive(true);
            SetRewardIconActive(false);
        }

        private void SetRewardIconActive(bool isActive)
        {
            if(rewardIconObj != null)
                rewardIconObj.SetActive(isActive);
        }

        private void PlayGettingPointEffect()
        {
            GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_GET_POINT).Play();

            rootAnimator.SetBool("isGetPoint", true);
        }

        private void PlayLevelUpEffect(int level)
        {
            GSManager.Instance.GetHandler(EpicPassUtils.Sounds.EPIC_PASS_LEVEL_UP).Play();

            rootAnimator.SetBool("isLevelUp", true);

            MetaContextElementUtils.SetFloatProperty(pointSliderElement, 1f);
            UpdateExpGaugeEffect(effectRequiredExp, effectRequiredExp);

            // Update Effect Exp Required
            List<long> requiredPointList = EpicPassUtils.SkippedRequiredPointMaxList;
            if(requiredPointList != null && requiredPointList.Count > level - 1)
            {
                effectRequiredExp = requiredPointList[level-1];
            }
            else
            {
                effectRequiredExp = EpicPassUtils.RequiredPoint;
            }

            effectExp = 0;
        }

        private void ProgressEffectUpdateCallback(float totalProgress)
        {
            float delta = (effectFromDelta + totalProgress) % 1f;
            MetaContextElementUtils.SetFloatProperty(pointSliderElement, delta);

            // Get Effect Exp.
            long exp = 0L;
            if(effectExp > 0)
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
            if(magnitude <= increaseMin)
                return increaseEffectTimeMin;
            if(increaseEffectTimeMax <= magnitude)
                return increaseEffectTimeMax;

            return Mathf.Lerp(increaseEffectTimeMin, increaseEffectTimeMax, (magnitude-increaseMin)/increaseMax);
        }

#if UNITY_EDITOR
        public long targetPoint;
        public long targetRequiredPointMax;
        public int levelGap;

        [Button]
        public void TestIncreaseEffectNew()
        {
            long testTimestamp = 0L;

            var metaGameInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "metaGameEnterInfo");
            var timestamp = metaGameInfoBB.GetVariable<long>("serverTime");
            if(timestamp != null)
                testTimestamp = timestamp.value + 1L;

            List<long> requiredExpList = new List<long>();
            // Dummy Data
            if(levelGap > 1)
            {
                for(int i=0; i < levelGap-1; ++i)
                {
                    requiredExpList.Add(targetRequiredPointMax + i + 1);
                }
            }

            var updateInfo = new ClientModels.SeasonPassPointUpdateInfo();
            updateInfo.earnPoint = 300;
            updateInfo.level = EpicPassUtils.Level + levelGap;
            updateInfo.point = 0;
            updateInfo.requiredPoint = targetPoint;
            updateInfo.requiredPointMax = targetRequiredPointMax;
            updateInfo.nextReward = null;
            updateInfo.unclaimedRewardCount = EpicPassUtils.UnclaimedRewardCount + levelGap;
            updateInfo.skippedRequiredPointMaxList = requiredExpList;

            ClientModels.MetaGameInfoV1 metaGameInfo = new ClientModels.MetaGameInfoV1();
            metaGameInfo.type = ClientModels.EventInfoType.SEASON_PASS;
            metaGameInfo.info = updateInfo;

            BlackboardQueryUtils.UpdateMetaGameInfoEndTurn(metaGameInfo, testTimestamp);
        }

        private void TestEffectEnd()
        {
            Debug.Log("end effect");
        }

        public float nowDelta = 0f;
        [Button]
        public void TestIncreaseEffecTime()
        {
            Debug.LogError(GetIncreaseEffectTime(nowDelta));
        }
#endif
    }
}
