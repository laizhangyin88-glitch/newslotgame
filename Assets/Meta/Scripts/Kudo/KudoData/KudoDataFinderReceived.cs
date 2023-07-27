using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataFinderReceived : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Hidden Objects Finder Received Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Button Text
            SetButtonTextReceiveRp();
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Active Animator
            ActiveAnimator();

            // Send Bi
            string kudoType = "kudo_meta_game_requests";
            SendBiKudo(info, "trigger", kudoType);

            // Make Club Icon
            MakeClubIcon(info);
            MakeTierIcon(info);

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Set Center Text
            string userName = info.GetValue<string>("name");
            SetCenterTextElement("HIDDEN_OBJECTS_KUDO_FINDER_RECEIVED", userName);

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.HIDDEN_UNIVERSE));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
