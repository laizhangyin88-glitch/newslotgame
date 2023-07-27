using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubDeal : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Deal Scene";
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
            string kudoType = "kudo_club_deal";
            SendBiKudo(info, "trigger", kudoType);

            // Show Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Set User Name Credit
            string userName = info.GetValue<string>("name");
            long earnCredit = info.GetValue<long>("earnCredit");
            SetUserNameText("FEED_RECEIVE_SOCIAL_CLUB_DEAL_COIN_TEXT", userName, FreebieLevelUtils.GetLevelMultiplierNumeratorValue(earnCredit, FreebieLevelUtils.FreebieType.CLUB_DEAL_BONUS));

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
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.CLUB_DEAL));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
