using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubRequest : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Request Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Center Text
            SetCenterTextElement("FEED_CLUB_JOIN_REQUEST_TEXT");
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_club_join_request");

            // Show Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
