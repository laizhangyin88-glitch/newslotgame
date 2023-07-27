using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubArenaOneDayLeft : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Arena One Day Left Scene";
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            Blackboard clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(null, "/clubInfo")?.value ?? null;
            if (clubInfoBB == null) yield break;

            BlackboardUtils.SetOrCreateValue<string>(info, "clubSymbol", BlackboardUtils.FindValue<string>(clubInfoBB, "symbol"));

            // Send Bi
            SendBiKudo("trigger", "kudo_club_arena_last_day_alarm");

            SetFlexibleText("CLUB_ARENA_KUDO_ONE_DAY_LEFT");
            anim.SetBool("IsActive", true);

            MakeClubIcon(info);

            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}