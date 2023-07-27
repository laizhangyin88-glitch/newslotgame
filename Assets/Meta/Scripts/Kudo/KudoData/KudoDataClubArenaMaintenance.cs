using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubArenaMaintenance : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Club Arena Maintenance Scene";
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            Blackboard clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(null, "/clubInfo")?.value ?? null;
            if (clubInfoBB == null) yield break;

            var symbolName = BlackboardUtils.FindVariable<string>(clubInfoBB, "symbol");
            if (symbolName != null && !string.IsNullOrEmpty(symbolName.value))
            {
                BlackboardUtils.SetOrCreateValue<string>(info, "clubSymbol", symbolName.value);
                MakeClubIcon(info);
            }
            // Send Bi
            SendBiKudo("trigger", "kudo_club_arena_start_alarm");

            SetFlexibleText("CLUB_ARENA_KUDO_START");
            anim.SetBool("IsActive", true);

            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}