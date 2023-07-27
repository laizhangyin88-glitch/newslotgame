using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataTournamentTopRanker : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Tournament Top Ranker Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Button Text
            SetButtonTextReceiveRp();

            // Set Tournament Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Tournament Area/Text", "FEED_TOURNAMENT_RANK_TEXT", FULL, 1);
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Send Bi
            string kudoType = "kudo_tournament";
            SendBiKudo(info, "trigger", kudoType);

            // Active Anim
            ActiveAnimator();

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info, true));

            // Set Flexible Text
            long winCredit = info.GetValue<long>("winCredit");
            SetFlexibleText("FEED_TOURNAMENT_WIN_TEXT", winCredit);

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.TOURNAMENT_TOP_RANKER));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
