using System.Collections;
using NodeCanvas.Framework;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubInvite : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Invite Scene";
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Send Bi
            SendBiKudo("trigger", "kudo_club_invite");

            // Active Anim
            ActiveAnimator();

            // Make Club, Tier Icon
            MakeClubIcon(info);
            MakeTierIcon(info);

            // Set User Name
            string userName = info.GetValue<string>("name");
            string clubName = info.GetValue<string>("clubName");
            SetUserNameText("FEED_RECEIVE_CLUB_INVITE_TEXT", userName, clubName);

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
