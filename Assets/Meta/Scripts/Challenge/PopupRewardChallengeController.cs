using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class PopupRewardChallengeController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard rootBB;
        private Animator rootAnim;

        private ContextElement titleTextElement;
        private ContextElement rewardTextElement;

        private ContextElement collectButtonElement;
        private ContextElement collectButtonTextElement;

        private ContextElement eventTagElement;
        private ContextElement eventTagTextElement;
        private ContextElement eventTagRemainingTimerTextElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            InitProperty();

            UpdatePopup();

            StartCoroutine(OnCloseCoroutine());
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            rootAnim = GetComponent<Animator>();

            root.UpdateContext(false);

            collectButtonElement = ContextUtils.FindElement(root, "Button Collect", FULL);
            collectButtonTextElement = ContextUtils.FindElement(collectButtonElement, "Text", FULL);
            MetaContextElementUtils.SetClickable(
                collectButtonElement,
                () => EventSender.SendEvent(gameObject, "OnClose"));

            MetaSystem.SubscribeBackButton(
                gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, "OnClose"));

            titleTextElement = ContextUtils.FindElement(root, "Title Area/Text", FULL);
            rewardTextElement = ContextUtils.FindElement(root, "Text", FULL);

            eventTagElement = ContextUtils.FindElement(root, "Event Tag", FULL);
            eventTagTextElement = ContextUtils.FindElement(eventTagElement, "Text", FULL);
            eventTagRemainingTimerTextElement = ContextUtils.FindElement(eventTagElement, "Remaining Timer/Text", FULL);
        }

        private void UpdatePopup()
        {
            MetaContextElementUtils.SetTextGlobal(collectButtonTextElement, "BUTTON_CLAIM");

            var simpleInfo = rootBB.GetVariable<Blackboard>("challengeInfo")?.value;
            var claimResponse = rootBB.GetVariable<Blackboard>("claimResponse")?.value;

            var challengeType = simpleInfo.GetValue<ChallengeType>("challengeType");

            string imageIconParentName = "Anchor/Layout/Contents Area Full/Contents Layout/Image/Anchor/Image Area";
            MetaIconUtils.MakeChallengeImageIcon(challengeType, transform, imageIconParentName);

            // Title Text
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "POPUP_CHALLENGE_REWARD_TITLE", challengeType.ToString());

            // Reward Text
            var rewardResultListBB = claimResponse.GetVariable<List<Blackboard>>("rewardResultList")?.value;
            string eachRewardText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardResultListBB, 100L, false);
            MetaContextElementUtils.SetTextGlobal(rewardTextElement, "POPUP_CHALLENGE_REWARD_TEXT", eachRewardText);

            if (rewardResultListBB != null)
            {
                for (int i = 0; i < rewardResultListBB.Count; ++i)
                {
                    BlackboardQueryUtils.ApplyRewardResult(rewardResultListBB[i]);
                }
            }

            // Event Multiplier
            eventTagElement.gameObject.SetActive(false);
            if (challengeType != ChallengeType.EVENT)
            {
                var rewardMultiplier = claimResponse.GetVariable<double>("multiplier")?.value ?? 1L;
                bool isEvent = rewardMultiplier > 1L;
                eventTagElement.gameObject.SetActive(isEvent);
                if (isEvent)
                {
                    MetaContextElementUtils.SetTextGlobal(eventTagTextElement, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", rewardMultiplier);
                    MetaContextElementUtils.SetTextGlobal(eventTagRemainingTimerTextElement, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", rewardMultiplier);
                }
            }
        }

        private IEnumerator OnCloseCoroutine()
        {
            var onCloseTrigger = new EventTrigger(this, "OnClose");
            yield return new WaitUntilTrigger(onCloseTrigger);

            EventSender.SendCalleeCallback(gameObject);

            EventSender.SendGlobalEvent("OnCreditEvent", "UpdateNaviCredit");

            rootAnim.SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
