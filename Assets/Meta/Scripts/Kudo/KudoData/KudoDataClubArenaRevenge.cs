using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubArenaRevenge : KudoData
    {
        private ContextElement kudoTextElement;

        protected override string GetKudoSceneName()
        {
            return "Kudo Club Arena In Game Scene";
        }

        protected override void InitProperty()
        {
            root = controller.GetComponent<ContextElement>();
            bb = controller.GetComponent<Blackboard>();
            anim = controller.GetComponent<Animator>();

            root.UpdateContext(false);

            ContextElement kudoTextAreaElement = ContextUtils.FindElement(root, "Kudo Text Area", CHILDREN);
            kudoTextElement = ContextUtils.FindElement(kudoTextAreaElement, "Text", CHILDREN);

            ContextElement multiBetElement = ContextUtils.FindElement(root, "Multi Bet Area", CHILDREN);
            multiBetElement.gameObject.SetActive(false);

            ContextElement buttonAreaElement = ContextUtils.FindElement(root, "Button Revenge Area", CHILDREN);
            ContextElement buttonElement = ContextUtils.FindElement(buttonAreaElement, "Button Revenge", CHILDREN);

            MetaContextElementUtils.SetClickable(buttonElement, () => EventSender.SendEvent(controller.gameObject, ON_ACCEPT));
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            string message = info.GetValue<string>("message");
            MetaContextElementUtils.SetText(kudoTextElement, message);
            // Send Bi
            SendBiKudo("trigger", "kudo_club_arena_revenge");

            // Active Animator
            anim.SetBool("IsActive", true);

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);  // Revenge Button
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);  // ClubArena Send
            yield return new WaitUntilTrigger(timerTrigger, onAcceptTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
                EventSender.SendGlobalEvent("OnClubArenaTargetRevenge");
            else if (!onSkipTrigger.IsTrigger)
                EventSender.SendGlobalEvent("OnClubArenaTargetOpponent");

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}