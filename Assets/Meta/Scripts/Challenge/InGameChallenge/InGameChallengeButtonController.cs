using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class InGameChallengeButtonController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard rootBB;

        private ContextElement progressTextElement;

        private Animator badgeAnim;
        private ContextElement badgeElement;
        private ContextElement badgeTextElement;

        private ContextElement buttonSpriteElement;
        private ContextElement buttonCoverElement;

        private ContextElement eventTagAreaElement;
        private ContextElement eventTagElement;
        private ContextElement eventTagParticleElement;

        private PIDButton button;

        private bool isInit = false;

        private EventInfoType eventType = EventInfoType.UNKNOWN;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const float TUTORIAL_SPEECH_BALLOON_DISPLAY_TIME = 1f;

        private Coroutine updateInteractableCoroutine = null;

        //

        private void OnEnable()
        {
            updateInteractableCoroutine = StartCoroutine(UpdateInteractableCoroutine());
        }

        private void OnDisable()
        {
            StopCoroutine(updateInteractableCoroutine);
            updateInteractableCoroutine = null;
        }

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
            if (prevEventType != eventType)
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

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            button = GetComponent<PIDButton>();

            button.interactable = false;

            root.UpdateContext(false);

            MetaContextElementUtils.SetClickable(root,
                () => EventSender.SendEvent(gameObject, ChallengeEventManager.OPEN_CHALLENGE_POPUP));

            progressTextElement = ContextUtils.FindElement(root, "Text", CHILDREN);

            badgeElement = ContextUtils.FindElement(root, "Badge", CHILDREN);
            badgeAnim = badgeElement.GetComponent<Animator>();
            badgeTextElement = ContextUtils.FindElement(badgeElement, "Text", FULL);

            buttonSpriteElement = ContextUtils.FindElement(root, "Image Challenge", CHILDREN);
            buttonCoverElement = ContextUtils.FindElement(root, "Cover", CHILDREN);

            eventTagAreaElement = ContextUtils.FindElement(root, "Event Tag Area", CHILDREN);
            eventTagElement = ContextUtils.FindElement(eventTagAreaElement, "Event Tag", CHILDREN);
            eventTagParticleElement = ContextUtils.FindElement(eventTagElement, "Particle Event", FULL);

            isInit = true;
        }

        public IEnumerator MakeTutorialSpeechBalloonCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Tutorial Speech Balloon Top";
            Transform parent = transform.Find("Tutorial Anchor");

            GameObject popupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakePrefabCoroutine(bundle, asset, parent,
                (GameObject obj) => popupObj = obj));

            popupObj.name = "Unlock Speech Balloon";

            root.UpdateContext(true);
            popupObj.SetActive(true);

            yield return new WaitForSeconds(TUTORIAL_SPEECH_BALLOON_DISPLAY_TIME);

            Destroy(popupObj);
        }

        public void UpdateChallengeButton()
        {
            Blackboard targetChallengeInfo = BlackboardQueryUtils.GetFocusedChallengeSimpleInfo(out MetaChallengeType challengeType);

            if (targetChallengeInfo == null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogWarning("InGameChallengeButtonController.UpdateChallengeButton failure. targetChallengeInfo is null.");
                return;
            }

            // Event Tag
            var eventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            bool isEventChallenge = ChallengeUtils.IsChallengeTypeEvent(challengeType);
            eventTagAreaElement.gameObject.SetActive(false);
            eventTagParticleElement.gameObject.SetActive(false);
            if (eventInfo != null)
            {
                eventTagAreaElement.gameObject.SetActive(true);
                SetEventTag(eventInfo);
            }

            bool isPersonalChallenge = ChallengeUtils.IsChallengeTypePersonal(challengeType);

            // Progress
            int maxProgress;
            if (isPersonalChallenge && !isEventChallenge)
            {
                maxProgress = BlackboardQueryUtils.GetChallengeMinCount(challengeType);
            }
            else
            {
                maxProgress = targetChallengeInfo.GetVariable<int>("maxChallengeProgress")?.value ?? 0;
            }

            bool isComplete = BlackboardQueryUtils.IsCompleteChallenge(targetChallengeInfo, challengeType);
            int progress;
            if (isComplete) progress = maxProgress;
            else progress = targetChallengeInfo.GetValue<int>("challengeProgress");

            MetaContextElementUtils.SetTextGlobal(progressTextElement, "A_PER_B", progress, maxProgress);

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

            bool hasNewChallenge = BlackboardQueryUtils.HasNewChallenge();

            // Badge
            if (isEventChallenge) // Event
            {
                if (hasNewChallenge)
                {
                    badgeAnim.SetInteger("value", 1);
                    MetaContextElementUtils.SetTextGlobal(badgeTextElement, "CHALLENGE_NEW_BADGE_TEXT");
                }
            }
            else // Daily Expert Master
            {
                if (hasNewChallenge)
                {
                    badgeAnim.SetInteger("value", 1);
                    MetaContextElementUtils.SetTextGlobal(badgeTextElement, "CHALLENGE_NEW_BADGE_TEXT");
                }
                else // Badge Count: Complete && !Claimed
                {
                    int badgeCount = BlackboardQueryUtils.GetUnclaimedCompleteChallengeCount();
                    badgeAnim.SetInteger("value", badgeCount);
                    MetaContextElementUtils.SetText(badgeTextElement, badgeCount.ToString());
                }
            }
        }

        public IEnumerator OpenChallengePopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Challenge Scene";
            Transform transform = MetaPopupUtils.PopupManagerAreaTransform;
            GameObject popupObj = null;

            // Open Popup
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, transform,
                (GameObject obj) => popupObj = obj));

            Blackboard popupBB = popupObj.GetComponent<Blackboard>();
            popupBB.AddVariable("caller", gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            // On Close Popup
            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private IEnumerator UpdateInteractableCoroutine()
        {
            var inGameDisableTrigger = new WaitWhileConditionTrigger(() => BlackboardQueryUtils.IsIngame());
            var spinButtonTrigger = new EventTrigger(this, "OnSpinButtonEvent", MetaEventDefine.ANY_EVENT_NAME);

            var endTurnTrigger = new EventTrigger(this, SendEvent.ON_CONTENT_EVENT, "EndTurn");
            var endTurnCondition = new TriggerCondition(endTurnTrigger);

            var closeShopTrigger = new EventTrigger(this, MetaEventDefine.ON_META_UI_EVENT, "CloseShop");
            var closeShopCondition = new TriggerCondition(closeShopTrigger);

            bool isFirst = true;

            Variable<bool> autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");

            while (true)
            {
                yield return new WaitUntil(() => enabled && isInit);

                if (!isFirst) yield return new WaitUntilTrigger(inGameDisableTrigger, spinButtonTrigger);

                if (isFirst || inGameDisableTrigger.IsTrigger)
                {
                    button.interactable = false;

                    // Check In Game
                    yield return new WaitUntil(() => BlackboardQueryUtils.IsIngame());

                    // Wait Content Initialized
                    yield return new WaitUntil(() => BlackboardUtils.FindVariable<bool>("./initializedContent")?.value ?? false);

                    inGameDisableTrigger.Reset();
                }
                else if (spinButtonTrigger.IsTrigger)
                {
                    button.interactable = false;

                    // Wait End Turn / Close Shop
                    while(true)
                    {
                        yield return new WaitUntilTriggers(endTurnCondition, closeShopCondition);

                        endTurnCondition.Initialize();
                        closeShopCondition.Initialize();

                        if (!(autoSpin?.value ?? false))
                            break;
                    }

                    spinButtonTrigger.Reset();
                }

                isFirst = false;
                button.interactable = true;

                yield return new WaitForEndOfFrame();
            }
        }

        private void OnChangePassiveEventType(EventInfoType type)
        {
            UpdateChallengeButton();
        }

        private void SetEventTag(EventInfo eventInfo)
        {
            if (eventInfo == null)
            {
                eventTagElement.gameObject.SetActive(false);
                return;
            }

            eventTagElement.gameObject.SetActive(true);

            string eventTagText;
            if (eventInfo.type == EventInfoType.CHALLENGE_CLAIM_MULTIPLY)
            {
                double multiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", multiplier);

                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "challengeClaimEventID", eventInfo.id);
                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "challengeClaimEventMultiplier", multiplier);
            }
            else
            {
                eventTagParticleElement.gameObject.SetActive(true);
                eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_CHALLENGE_BUTTON_EVENT_TAG_TEXT");
            }

            long endTimestamp = eventInfo.endTimestamp;
            var eventTagController = eventTagElement.GetComponent<EventTagController>();
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

#if UNITY_EDITOR
        [Button]
        private void TestTutorialSpeechBalloon()
        {
            StartCoroutine(MakeTutorialSpeechBalloonCoroutine());
        }
#endif
    }
}
