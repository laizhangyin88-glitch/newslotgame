using System.Collections;
using NodeCanvas.Framework;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubColeaderChange : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Coleader Change Scene";
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_club_authority_change");

            // Make Club, Tier Icon
            MakeClubIcon(info);
            MakeTierIcon(info);

            // Set User Name
            bool isPromote = info.GetValue<bool>("promote");
            if(isPromote) SetUserNameText("FEED_CLUB_PROMOTE_TEXT");
            else SetUserNameText("FEED_CLUB_DEMOTE_TEXT");

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
