using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubRequestAccepted : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Request Accepted Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set User Name
            SetUserNameText("FEED_CLUB_JOIN_ACCEPT_TEXT");
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_club_join_accept");

            // Make Club, Tier Icon
            MakeClubIcon(info);
            MakeTierIcon(info);

            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
            var userClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId");
            var clubID = info.GetValue<long>("clubId");
            meClubID.value = clubID;
            userClubID.value = clubID;

            // Club Scene Refresh Global event
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MetaEventDefine.ON_CLUB_REQUEST_ACCEPTED));

            // Club Scene Refresh
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData("OnClubSceneRefresh") );

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
