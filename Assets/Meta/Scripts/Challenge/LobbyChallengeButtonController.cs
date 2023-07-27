using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class LobbyChallengeButtonController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard rootBB;

        private ContextElement defaultButtonElement;

        private ContextElement rewardButtonElement;
        private ContextElement rewardTextElement;

        private ContextElement badgeElement;
        private ContextElement badgeTextElement;
        private Animator badgeAnim;

        private ContextElement lockElement;
        private ContextElement lockLevelTextElement;

        private ContextElement buttonSpriteElement;
        private ContextElement buttonCoverElement;

        private ContextElement eventTagAreaElement;
        private ContextElement eventTagElement;
        private ContextElement eventTagParticleElement;
        private EventTagController eventTagController;

        private EventInfoType eventType = EventInfoType.UNKNOWN;

        private bool ignoreEventChallenge;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        //

        public void OnStartEventChallenge()
        {
            UpdateEventType();
            StartCoroutine(ChallengeUtils.RequestChallengeInfo(rootBB));
        }

        public void OnEndEventChallenge()
        {
            var prevEventType = eventType;
            UpdateEventType();

            // Chaneged
            if(prevEventType != eventType)
                UpdateChallengeButton();
        }

        public void OnRefreshClaimMultiplierPassiveEvent()
        {
            var prevEventType = eventType;
            UpdateEventType();

            // Chaneged
            if (prevEventType != eventType)
                UpdateChallengeButton();
        }

        private void UpdateEventType()
        {
            var eventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            eventType = eventInfo?.type ?? EventInfoType.UNKNOWN;
        }

        public void UpdateChallengeButton()
        {
            ignoreEventChallenge = rootBB.GetVariable<bool>("ignoreEventChallenge")?.value ?? false;

            bool isLocked = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.CHALLENGE);
            if (isLocked)
            {
                defaultButtonElement.gameObject.SetActive(false);
                rewardButtonElement.gameObject.SetActive(false);

                lockElement.gameObject.SetActive(true);
                int unlockLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.CHALLENGE);
                MetaContextElementUtils.SetTextGlobal(lockLevelTextElement, "TEXT_FEATURE_LOCK_LEVEL", unlockLevel);
            }
            else
            {
                var challengeSimpleInfo = BlackboardQueryUtils.GetFocusedChallengeSimpleInfo(out MetaChallengeType challengeType, ignoreEventChallenge);
                if (challengeSimpleInfo == null)
                {
                    if (ApplicationSettings.LogTest())
                        Debug.LogWarning("LobbyChallengeButtonController.UpdateChallengeButton failure. The challengeSimpleInfo is null");
                    return;
                }

                // Event Tag
                SetEventTag();

                // Sprite
                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "In Game Challenge Gauge Daily";
                if (challengeType == MetaChallengeType.EXPERT)
                    asset = "In Game Challenge Gauge Expert";
                else if (challengeType == MetaChallengeType.MASTER)
                    asset = "In Game Challenge Gauge Master";
                else if (challengeType == MetaChallengeType.CLUB)
                    asset = "In Game Challenge Gauge Club";
                else if (challengeType == MetaChallengeType.EVENT_PERSONAL)
                    asset = "In Game Challenge Gauge Event";
                else if (challengeType == MetaChallengeType.EVENT_CLUB)
                    asset = "In Game Challenge Base";

                MetaContextElementUtils.SetSprite(buttonSpriteElement, bundle, asset);
                MetaContextElementUtils.SetSprite(buttonCoverElement, bundle, asset);

                // Button State
                int unclaimedCompleteChallengeCount = BlackboardQueryUtils.GetUnclaimedCompleteChallengeCount();
                bool isButtonClaim = unclaimedCompleteChallengeCount > 0;

                defaultButtonElement.gameObject.SetActive(!isButtonClaim);
                rewardButtonElement.gameObject.SetActive(isButtonClaim);
                lockElement.gameObject.SetActive(false);

                // Default State
                if (!isButtonClaim)
                {
                    // Badge
                    bool hasNewChallenge = BlackboardQueryUtils.HasNewChallenge();
                    if (hasNewChallenge) // New Badge
                    {
                        badgeAnim.SetInteger("value", 1);
                        MetaContextElementUtils.SetTextGlobal(badgeTextElement, "CHALLENGE_NEW_BADGE_TEXT");
                    }
                    else // Mission Count Badge
                    {
                        // Badge Count = Non Completed Mission Count + Unclaimed Complete Challenge Count
                        int badgeCount = BlackboardQueryUtils.GetIncompleteMissionCountToComplete(challengeSimpleInfo, challengeType);
                        badgeCount += unclaimedCompleteChallengeCount;

                        badgeAnim.SetInteger("value", badgeCount);
                        MetaContextElementUtils.SetText(badgeTextElement, badgeCount.ToString());
                    }
                }
                // Claim State
                else
                {
                    // Reward Coin
                    long rewardCoin = BlackboardQueryUtils.GetUnclaimedCompleteChallengesTotalCredit();
                    if (rewardCoin > 0L)
                    {
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "LOBBY_CHALLENGE_BUTTON_GET", rewardCoin);
                    }
                }
            }
        }

        public IEnumerator OpenChallengePopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Challenge Scene";
            Transform root = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject challengePopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, root,
                (GameObject sceneObj) => challengePopupObj = sceneObj));

            challengePopupObj.name = "Popup Challenge";

            var popupBB = challengePopupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "ignoreEventChallenge", ignoreEventChallenge);

            MetaObjectUtils.SetCalleeCaller(challengePopupObj, gameObject);

            MetaPopupUtils.OpenPopup(challengePopupObj);

            var calleeCallbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(calleeCallbackTrigger);
        }

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();

            root.UpdateContext(false);

            defaultButtonElement = ContextUtils.FindElement(root, "Button Lobby Challenge", CHILDREN);
            MetaContextElementUtils.SetClickable(defaultButtonElement,
                () => EventSender.SendEvent(gameObject, ChallengeEventManager.OPEN_CHALLENGE_POPUP));

            rewardButtonElement = ContextUtils.FindElement(root, "Button Lobby Challenge Reward", CHILDREN);
            rewardTextElement = ContextUtils.FindElement(root, "Button Lobby Challenge Reward/Text Coins", FULL);
            MetaContextElementUtils.SetClickable(rewardButtonElement,
                () => EventSender.SendEvent(gameObject, ChallengeEventManager.OPEN_CHALLENGE_POPUP));

            var badgeAreaElement = ContextUtils.FindElement(root, "Button Lobby Challenge/Badge Area", FULL);
            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeElement = badgeObj.GetComponent<ContextElement>();
            badgeElement.UpdateContext();
            badgeAnim = badgeElement.GetComponent<Animator>();
            badgeTextElement = ContextUtils.FindElement(badgeElement, "Text", CHILDREN);

            lockElement = ContextUtils.FindElement(root, "Button Lobby Challenge Lock", CHILDREN);
            lockLevelTextElement = ContextUtils.FindElement(root, "Button Lobby Challenge Lock/Text Level", FULL);

            buttonSpriteElement = ContextUtils.FindElement(root, "Button Lobby Challenge/Icon", FULL);
            buttonCoverElement = ContextUtils.FindElement(root, "Button Lobby Challenge/Cover", FULL);

            eventTagAreaElement = ContextUtils.FindElement(root, "Event Tag Area", CHILDREN);
            var eventTagObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag", eventTagAreaElement.transform);
            eventTagAreaElement.UpdateContext(false);
            eventTagElement = ContextUtils.FindElement(eventTagAreaElement, "Event Tag", CHILDREN);
            eventTagParticleElement = ContextUtils.FindElement(eventTagElement, "Particle Event", FULL);

            eventTagController = eventTagElement.GetComponent<EventTagController>();

            defaultButtonElement.gameObject.SetActive(false);
            rewardButtonElement.gameObject.SetActive(false);
            eventTagAreaElement.gameObject.SetActive(false);
        }

        //

        private void SetEventTag()
        {
            var eventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            if (eventInfo == null || 
                (eventInfo.type != EventInfoType.CHALLENGE_CLAIM_MULTIPLY && ignoreEventChallenge))
            {
                eventTagAreaElement.gameObject.SetActive(false);
                eventTagParticleElement.gameObject.SetActive(false);
                return;
            }

            eventTagAreaElement.gameObject.SetActive(true);

            string eventTagText;
            if (eventInfo.type == EventInfoType.CHALLENGE_CLAIM_MULTIPLY)
            {
                double multiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", multiplier);

                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "challengeClaimEventID", eventInfo.id);
                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "challengeClaimEventMultiplier", multiplier);

                UpdateClaimMultiplierState();
            }
            else
            {
                eventTagParticleElement.gameObject.SetActive(true);
                eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_CHALLENGE_BUTTON_EVENT_TAG_TEXT");
            }

            long endTimestamp = eventInfo.endTimestamp;
            eventTagController.Initialize(
                    targetTime: endTimestamp,
                    warningTime: 0L,
                    timeFormatKey: "TIME_FORMAT_HHMMSS_TOTALHOUR",
                    outputFormatKey: null,
                    warningFormatKey: null,
                    expireText: null,
                    useCommonTimer: true,
                    caller: null,
                    eventTextList: null,
                    textChangeDelay: 0f,
                    textChangeSpd: 0f,
                    defaultEventText: eventTagText
                    );
        }

        private void UpdateClaimMultiplierState()
        {
            long newCoin = 0L;

            long eventMultiplierNumerator = NumberUtils.GetGlobalDenominator();
            EventInfo claimMultiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);

            if (claimMultiplierEventInfo != null)
                eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(claimMultiplierEventInfo);

            var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");
            long currentTimestamp = TimeUtils.GetTimeStamp();
            for (int i = 0; i < challengeInfoList.value.Count; ++i)
            {
                if (challengeInfoList.value[i] == null)
                    throw new Exception(string.Format("NullReferenceException in LobbyChallengeController: OnRefreshChallengeClaimPassiveEvent index {0} is null", i));

                var cType = BlackboardUtils.FindVariable<ChallengeType>(challengeInfoList.value[i], "challengeType");
                bool isDone = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done").value;
                bool isClaimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed").value;

                int completeCount = BlackboardUtils.FindVariable<int>(challengeInfoList.value[i], "challengeProgress").value;

                long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
                if (currentTimestamp >= startTimestamp)
                {
                    var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(challengeInfoList.value[i], "rewardList");
                    Variable<long> currentRewardCoins = null;

                    for (int j = 0; j < rewardList.value.Count; j++)
                        if (currentRewardCoins == null)
                            currentRewardCoins = BlackboardUtils.FindVariable<long>(rewardList.value[j], "credit");

                    if (cType.value == ChallengeType.DAILY && !isDone && !isClaimed && completeCount == 0)
                        newCoin = NumberUtils.GetMultiplierNumeratorValue(currentRewardCoins.value, eventMultiplierNumerator);
                }
            }

            long rewardCoin = BlackboardQueryUtils.GetUnclaimedCompleteChallengesTotalCredit();
            if (rewardTextElement != null && rewardCoin > 0L)
                MetaContextElementUtils.SetTextGlobal(rewardTextElement, "LOBBY_CHALLENGE_BUTTON_GET", rewardCoin);

            var newTextElement = ContextUtils.FindElement(root, "Button Lobby Challenge New/Text Coins", FULL);
            if (newTextElement != null && newCoin > 0L)
                MetaContextElementUtils.SetTextGlobal(newTextElement, "LOBBY_CHALLENGE_BUTTON_WIN", newCoin);
        }

        #if UNITY_EDITOR
        [Button]
        private void TestUpdate()
        {
            UpdateChallengeButton();
        }
        #endif
    }
}
