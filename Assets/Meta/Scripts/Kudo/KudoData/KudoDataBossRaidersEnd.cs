using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataBossRaidersEnd : KudoData
    {
        private ContextElement iconAreaElement;

        protected override string GetKudoSceneName()
        {
            return "Kudo Boss Raiders End Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            iconAreaElement = ContextUtils.FindElement(root, "Boss Raiders Icon Area", ContextSearchingType.ChildrenSearch);

            // Set Flexible Text
            SetFlexibleText("BOSS_RAIDERS_KUDO_META_GAME_END");
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            Blackboard clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(null, "/clubInfo")?.value ?? null;
            if (clubInfoBB == null) yield break;

            BlackboardUtils.SetOrCreateValue<string>(info, "clubSymbol", BlackboardUtils.FindValue<string>(clubInfoBB, "symbol"));

            MakeKudoIconPrefab(iconAreaElement, GetBossRaidersIconName(info.GetVariable<int>("themeId")?.value ?? -1));

            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_boss_raiders_end");

            // Make Club Icon
            MakeClubIcon(info);

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
