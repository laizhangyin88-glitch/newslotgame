using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubShare : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Share Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Accept Button Text
            SetButtonTextReceiveRp();
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Active Animator
            ActiveAnimator();

            // Send Bi
            string kudoType = "kudo_meta_game_requests";
            SendBiKudo(info, "trigger", kudoType);

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Set User Name Game
            string userName = info.GetValue<string>("name");
            var eventInfoType = info.GetValue<EventInfoType>("eventType");
            int seasonIdx = info.GetValue<int>("eventPresetId");
            string metaGameEventName = BlackboardQueryUtils.GetMetaGameEventName(eventInfoType, seasonIdx);
            SetUserNameText("FEED_RECEIVE_CLUB_SHARE_ITEM_TEXT", userName, metaGameEventName);

            // Make Club, Tier Icon
            MakeClubIcon(info);
            MakeTierIcon(info);

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.META_GAME_SHARE));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
