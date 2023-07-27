using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.GemJackpot
{
    public class GemJackpotLobbyButtonController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;

        private ContextElement iconAreaElement;
        private ContextElement buttonAreaElement;
        private ContextElement badgeAreaElement;

        private ContextElement flippingElement;
        private ContextElement badgeTextElement;
        private ContextElement jackpotTextElement;

        private ContextElement timerAreaElement;
        private ContextElement remainingTimerElement;

        private ContextElement lockedIconElement;
        private ContextElement lockedSpeechBalloonElement;
        private Animator lockedSpeechAnimator;

        private Variable<bool> inhouseAdsEnabled;
        private Variable<bool> videoAdsEnabled;

        private bool isInit = false;
        private bool ignoreSpeechBalloon = false;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private List<string> flippingTextList;

        private const string GEM_JACKPOT_CLICK = "OnEnterGemJackpot";
        private readonly string[] gemjackpotSpinElementNames = { "Free Spin Timmer", "Free Spin", "Free Spin Ad" };
        //0 : Free Spin Timmer, 1 : Free Spin, 2 : Free Spin Ad
        private List<ContextElement> buttonContextElementList;

        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        public Variable<int> selectIndex;
        public string testClimeTimestamp;
        public string testCurrentTimestamp;

        private void InitProperty()
        {
            if (isInit) return;
            // Context Element & Blackboard Set
            rootBB = gameObject.GetComponent<Blackboard>();
            
            ignoreSpeechBalloon = rootBB.GetVariable<bool>("ignoreSpeechBalloon")?.value ?? false;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            buttonAreaElement = ContextUtils.FindElement(rootElement, "Button Area", ContextSearchingType.ChildrenSearch);
            badgeAreaElement = ContextUtils.FindElement(rootElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            jackpotTextElement = ContextUtils.FindElement(iconAreaElement, "Text", ContextSearchingType.ChildrenSearch);
            flippingElement = ContextUtils.FindElement(badgeAreaElement, "Flipping Text", ContextSearchingType.ChildrenSearch);
            badgeTextElement = ContextUtils.FindElement(flippingElement, "Text", ContextSearchingType.ChildrenSearch);

            timerAreaElement = ContextUtils.FindElement(buttonAreaElement, "Free Spin Timmer", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(timerAreaElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);

            ContextElement lockedAreaElement = ContextUtils.FindElement(rootElement, "Locked Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Gem Jackpot Icon Locked", ContextSearchingType.ChildrenSearch);

            if (!ignoreSpeechBalloon)
            {
                lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Gem Jackpot Locked Info Speech Balloon", ContextSearchingType.ChildrenSearch);
                lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();
            }

            selectIndex = BlackboardUtils.FindVariable<int>(rootBB, "selectIndex");

            if (buttonContextElementList == null)
                buttonContextElementList = new List<ContextElement>();
            else
                buttonContextElementList.Clear();

            for (int i = 0; i < gemjackpotSpinElementNames.Length; ++i)
            {
                buttonContextElementList.Add(ContextUtils.FindElement(buttonAreaElement, gemjackpotSpinElementNames[i], ContextSearchingType.ChildrenSearch));
                buttonContextElementList[i].gameObject.SetActive(false);
            }

            // Jackpot Info Get
            GemJackpotUtils.UpdateJackpotInfo(GemJackpotUtils.GrandJackpotMultiplyNumerator);
            Blackboard jackpotInfo = GemJackpotUtils.JackpotInfo;

            // flippingTextList Set
            if (flippingTextList == null)
                flippingTextList = new List<string>();
            else
                flippingTextList.Clear();

            // inhouseAdsTriggerType Set
            InAppMessageTriggerType inAppMessageTriggerType = InAppMessageTriggerType.INHOUSE_ADS_FOR_GEM_JACKPOT;

            SetAdsValues(); // INHOUSE_ADS_ENABLED, VIDEO_ADS_ENABLED

            // Set blackboard
            BlackboardUtils.SetOrCreateValue(rootBB, "jackpotInfo", jackpotInfo);
            BlackboardUtils.SetOrCreateValue(rootBB, "freeSpinIcons", buttonContextElementList);
            BlackboardUtils.SetOrCreateValue(rootBB, "jackpotText", jackpotTextElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "badgeText", badgeTextElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "flippingElement", flippingElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "flippingTextList", flippingTextList);

            BlackboardUtils.SetOrCreateValue(rootBB, "metaGameEnterInfo", GemJackpotUtils.GemJackpotInfo);
            BlackboardUtils.SetOrCreateValue(rootBB, "_badgeArea", badgeAreaElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "_timer", remainingTimerElement);
            BlackboardUtils.SetOrCreateValue(rootBB, "_inhouseAdsTriggerType", inAppMessageTriggerType);

            MetaContextElementUtils.SetClickable(
                rootElement,
                GEM_JACKPOT_CLICK,
                rootElement,
                null
            );

            isInit = true;
        }

        public void OnInit()
        {
            InitProperty();
            InitText();
            UpdateTimer();

            SetActiveFreeSpinIcon(selectIndex.value);
            UpdateUnlockedLevel();
        }

        private void InitText()
        {
            int level = MetaGameUtils.GetMetaUnlockedLevel();
            MetaContextElementUtils.SimpleSetText(lockedIconElement, "Text", level.ToString());

            if (!ignoreSpeechBalloon)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", ContextSearchingType.ChildrenSearch, level);
            }
        }

        private void UpdateTimer()
        {
            if (rootBB == null) return;

            Blackboard gemJackpotInfo = GemJackpotUtils.GemJackpotInfo;
            if (gemJackpotInfo == null)
            {
                return;
            }

            long lastVideoTimestamp = GemJackpotUtils.LastVideoAdsClaimTimestamp;
            long coolTimestamp = GemJackpotUtils.CoolTime;

            long endVideoTimestamp = GetCheckTimeAddCoolTime(lastVideoTimestamp, coolTimestamp);

            SetAdsValues();

            if (endVideoTimestamp < 0)
            {
                // 1 : FreeSpin / 2 : Ads FreeSpin
                selectIndex.value = inhouseAdsEnabled.value ? 1 : 2;
            }
            else
            {
                selectIndex.value = 0;
                MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement, remainingTimerElement, lastVideoTimestamp + coolTimestamp);
            }
        }

        public void UpdatePassiveEvent()
        {
            List<EventInfo> listEventInfo = BlackboardQueryUtils.GetActiveEventInfoList(new List<EventInfoType>(
                new EventInfoType[] { EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY, EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE }));

            if (badgeAreaElement == null)
                badgeAreaElement = BlackboardUtils.FindValue<ContextElement>(rootBB, "_badgeArea");

            if (rootBB != null && listEventInfo != null && listEventInfo.Count > 0)
            {
                flippingTextList = BlackboardUtils.FindValue<List<string>>(rootBB, "flippingTextList");
                if (flippingTextList == null)
                    flippingTextList = new List<string>();
                flippingTextList.Clear();

                foreach(EventInfo eventInfo in listEventInfo)
                {
                    switch (eventInfo.type)
                    {
                        case EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY:
                            if (BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM_JACKPOT))
                            {
                                // n % MORE
                                flippingTextList.Add(StringTableUtils.GetString(tableType, "GEM_JACKPOT_REWARD_MULTIPLY_TEXT_1", PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo)));
                            }
                            else
                            {
                                // n x WIN
                                flippingTextList.Add(StringTableUtils.GetString(tableType, "GEM_JACKPOT_REWARD_MULTIPLY_TEXT_2", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo)));
                            }
                            break;
                        case EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE:
                            flippingTextList.Add(StringTableUtils.GetString(tableType, "GEM_JACKPOT_SPIN_GEM_SALE_TEXT", PassiveEventManager.Instance.GetEventInfoViewPercent(eventInfo)));
                            break;
                        default:
                            break;
                    }
                }
                BlackboardUtils.SetOrCreateValue(rootBB, "_defaultText", flippingTextList[0]);
                MetaContextElementUtils.SetFlippingText(flippingElement, flippingTextList[0], flippingTextList, true, 3.0f);
                badgeAreaElement.gameObject.SetActive(true);
            }
            else
            {
                badgeAreaElement.gameObject.SetActive(false);
            }
        }

        public void UpdateUnlockedLevel()
        {
            bool isLockedLevel = CheckUnlockedLevel();

            iconAreaElement.gameObject.SetActive(!isLockedLevel);
            buttonAreaElement.gameObject.SetActive(!isLockedLevel);
            badgeAreaElement.gameObject.SetActive(!isLockedLevel);

            lockedIconElement.gameObject.SetActive(isLockedLevel);
            lockedSpeechBalloonElement?.gameObject.SetActive(isLockedLevel);
        }

        public void CheckUpdateTimer()
        {
            UpdateTimer();
        }

        public void CheckIndexStateToEvent()
        {
            SetAdsValues();
            CheckVideoAdsTime();
            if (selectIndex.value == 0)
            {
                MetaContextElementUtils.SendEvent(rootElement, "OnGemJackpotTimerEvent", null, null);
            }
            else
            {
                MetaContextElementUtils.SendEvent(rootElement, "OnGemJackpotEndCooltimeEvent", null, null);
            }
        }

        public bool CheckVideoAdsTime()
        {
            bool isShow = false;

            Blackboard gemJackpotInfo = GemJackpotUtils.GemJackpotInfo;

            long endTimestamp = GetCheckTimeAddCoolTime(GemJackpotUtils.LastVideoAdsClaimTimestamp, GemJackpotUtils.CoolTime);

            if (endTimestamp < 0)
                isShow = true;

            BlackboardUtils.SetOrCreateValue(rootBB, "_isShowAds", isShow);

            return isShow;
        }

        public void SetSlotEnterContextID(string BISlotEnterContextID)
        {
            // Lobby last init -> GemJackpotUtils.BISlotEnterContextID set
            GemJackpotUtils.BISlotEnterContextID = BISlotEnterContextID;
        }

        private void SetAdsValues()
        {
            inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");
            videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

            if (inhouseAdsEnabled.value)
            {
                if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_GEM_JACKPOT))
                    rootBB.SetValue("placementKey", "");
            }
            else if (videoAdsEnabled.value)
            {
                string placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/gemJackpot").value;
                if (!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                    rootBB.SetValue("placementKey", placement);
            }
            else
                rootBB.SetValue("placementKey", "");

            BlackboardUtils.SetOrCreateValue(rootBB, "_inhouseAds", inhouseAdsEnabled.value);
            BlackboardUtils.SetOrCreateValue(rootBB, "_videoAds", videoAdsEnabled.value);
        }

        public void SetActiveFreeSpinIcon(int index)
        {
            if (index < buttonContextElementList.Count && index >= 0)
            {
                for (int i = 0; i < buttonContextElementList.Count; ++i)
                    buttonContextElementList[i].gameObject.SetActive(i == index);
            }
        }

        private long GetCheckTimeAddCoolTime(long checkTimeStamp, long coolTimeStamp)
        {
            long returnValue = (checkTimeStamp + coolTimeStamp) - TimeUtils.GetTimeStamp();
            DateTime t = TimeUtils.ParseTimestampToDateTime(checkTimeStamp + coolTimeStamp);
            testClimeTimestamp = t.ToString();
            testCurrentTimestamp = TimeUtils.ParseTimestampToDateTime(TimeUtils.GetTimeStamp()).ToString();
            return returnValue;
            //return (checkTimeStamp + coolTimeStamp) - TimeUtils.GetTimeStamp();
        }

        public bool CheckMetaGameData()
        {
            return GemJackpotUtils.GemJackpotInfo != null;
        }

        public bool CheckUnlockedLevel()
        {
            return MetaGameUtils.IsMetaGameLevelLocked();
        }

        public void ActiveLevelLockedSpeechBalloon()
        {
            if (ignoreSpeechBalloon) return;

            if (levelLockedSpeechBalloonEnumerator != null)
                StopCoroutine(levelLockedSpeechBalloonEnumerator);
            levelLockedSpeechBalloonEnumerator = StartCoroutine(LevelLockedSpeechBalloonEnumerator());
        }

        private IEnumerator LevelLockedSpeechBalloonEnumerator()
        {
            lockedSpeechAnimator?.SetBool("IsActive", true);
            yield return new WaitForSeconds(1.0f);
            lockedSpeechAnimator?.SetBool("IsActive", false);
        }

        public void SetMetaGameCrashReport()
        {
            BlackboardQueryUtils.MetaGameCrashReport(StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_NAME"));
        }
    }
}
